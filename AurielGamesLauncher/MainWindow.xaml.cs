namespace AurielGamesLauncher
{
    using AurielGamesLauncher.Helpers;
    using ByteDev.Crypto;
    using ByteDev.Crypto.Hashing;
    using ByteDev.Crypto.Hashing.Algorithms;
    using Microsoft.Win32;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string OST_Folder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\OST_Game";
        public readonly IFileChecksumService _fileChecksumService;

        public Data Data;
        public MainWindow()
        {
            Data = JsonHelper.LoadData();
            _fileChecksumService = new FileChecksumService(new Md5Algorithm(), EncodingType.Hex);
            InitializeComponent();
            ContentControl buttons = new ContentControl();
            for (int i = 0; i < Data.Projects.Count; i++)
            {
                Projects.Children.Add(new Button
                {
                    Name = $"Project{i}",
                    Content = Data.Projects[i].Name,
                    Margin = new Thickness
                    {
                        Top = 5,
                        Left = 40,
                    },
                    Width = 100,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            };

            foreach (UIElement button in Projects.Children)
            {
                button.AddHandler(Button.ClickEvent, new RoutedEventHandler(Project_Click));
            }

            SetUndersideButtons();
        }

        private void SetUndersideButtons()
        {
            if (string.IsNullOrEmpty(Data.LastPlayedGameName))
            {
                LaunchLast.IsEnabled = false;
                LaunchNext.IsEnabled = false;
            }
            else
            {
                LaunchLast.Content = $"Launch {Data.LastPlayedGameName}";
                LaunchLast.Tag = new GameButtonsTag(Data.LastPlayedProject, Data.LastPlayedGameName);
                LaunchLast.AddHandler(Button.ClickEvent, new RoutedEventHandler(LaunchGame_Click));
                LaunchLast.IsEnabled = true;

                Project project = Data.Projects.First(o => o.Name == Data.LastPlayedProject);
                Game lastPlayedGame = project.Games.First(w => w.Name == Data.LastPlayedGameName);
                Game nextGame = project.CurrentOrderType == 1
                    ? project.Games.FirstOrDefault(o => o.PrimaryOrder == lastPlayedGame.PrimaryOrder + 1)
                    : project.Games.FirstOrDefault(o => o.SecundaryOrder == lastPlayedGame.SecundaryOrder + 1);

                if (nextGame != null)
                {
                    LaunchNext.Content = $"Launch next: {nextGame.Name}";
                    LaunchNext.Tag = new GameButtonsTag(Data.LastPlayedProject, nextGame.Name);
                    LaunchNext.AddHandler(Button.ClickEvent, new RoutedEventHandler(LaunchGame_Click));
                    LaunchNext.IsEnabled = true;
                }
                else
                {
                    LaunchNext.IsEnabled = false;
                }
            }

            if (!Directory.Exists(OST_Folder))
            {
                cleanOst.IsEnabled = false;
            }
            else
            {
                cleanOst.IsEnabled = true;
            }
        }

        public void Project_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            string projectName = ((Button)sender).Content.ToString();
            Project project = Data.Projects.First(o => o.Name == projectName);
            List<Game> games = project.CurrentOrderType == 1
                ? project.Games.OrderBy(o => o.PrimaryOrder).ToList()
                : project.Games.OrderBy(o => o.SecundaryOrder).ToList();
            Games.Children.Clear();
            for (int i = 0; i < games.Count; i++)
            {
                Games.Children.Add(GameInfo(games[i], i, projectName));
            };
        }

        private DockPanel GameInfo(Game game, int element, string projectName)
        {
            DockPanel result = new();
            GameButtonsTag gameButtonsTag = new GameButtonsTag
            {
                ProjectName = projectName,
                GameName = game.Name,
            };

            result.Children.Add(new Label
            {
                Content = game.Name,
                Margin = new Thickness
                {
                    Top = 5,
                    Left = 40,
                },
                Height = 30,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
            });

            Button launchGameButton = new Button
            {
                Tag = gameButtonsTag,
                Name = $"LaunchGame{element}",
                Content = "Launch Game",
                Width = 150,
            };
            launchGameButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(LaunchGame_Click));
            result.Children.Add(launchGameButton);
            Button changeGameLocationButton = new Button
            {
                Tag = gameButtonsTag,
                Name = $"ChangeGameLocation{element}",
                Content = "Change Game Location",
                Width = 150,
            };
            changeGameLocationButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(ChangeGameLocation_Click));
            result.Children.Add(changeGameLocationButton);
            result.Children.Add(new Label());
            return result;
        }

        public void LaunchGame_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            GameButtonsTag gameButtonsTag = (GameButtonsTag)((Button)sender).Tag;
            Game game = Data.Projects
                .First(o => o.Name == gameButtonsTag.ProjectName)
                .Games.First(o => o.Name == gameButtonsTag.GameName);

            if (string.IsNullOrEmpty(game.Location) || !File.Exists(game.FullPath))
            {
                this.ChangeGameLocation(game, gameButtonsTag.ProjectName);
                if (!File.Exists(game.FullPath))
                {
                    return;
                }
            }

            Process.Start(game.FullPath);
            Thread.Sleep(2000);
            SetUndersideButtons();
        }

        public void ChangeGameLocation_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            GameButtonsTag gameButtonsTag = (GameButtonsTag)((Button)sender).Tag;
            Game game = Data.Projects
                .First(o => o.Name == gameButtonsTag.ProjectName)
                .Games.First(o => o.Name == gameButtonsTag.GameName);
            
            this.ChangeGameLocation(game, gameButtonsTag.ProjectName, false);
        }

        public void ChangeGameLocation(Game game, string projectName, bool gameLaunch = true)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = $"Select exe for: {projectName} - {game.Name}",
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                game.Location = fileDialog.FileName;
                if (gameLaunch)
                {
                    Data.LastPlayedGameName = game.Name;
                    Data.LastPlayedProject = projectName;
                }
                game.CheckSum = _fileChecksumService.Calculate(game.FullPath);

                Data.SaveData();
            }
        }

        private void cleanOst_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(OST_Folder, true);
            if (!Directory.Exists(OST_Folder))
            {
                cleanOst.IsEnabled = false;
            }
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            Data exportable = (Data)Data.Clone();
            CommonSaveFileDialog commonSaveFileDialog = new CommonSaveFileDialog("GamesDefinition.json");
            commonSaveFileDialog.Filters.Add(new CommonFileDialogFilter("Json File", "json"));

            if (commonSaveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string saveLocation = commonSaveFileDialog.FileName;
                exportable.SaveData(saveLocation);
            }
        }

        private void ScanForGames_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = $"Select folder for scanning...",
            };

            if (fileDialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            List<string> executaleNames = new List<string>();
            Data.Projects.ForEach(o =>
            {
                executaleNames.AddRange(o.Games.Select(w => w.ExecutableName).ToList());
            });
            executaleNames = executaleNames.Distinct().ToList();

            List<string> foundExecutables = 
                Directory.GetFiles(fileDialog.FileName, "*.exe", SearchOption.AllDirectories)
                .Where(o => executaleNames.Any(p => o.EndsWith(p))).ToList();

            foundExecutables.ForEach(o =>
            {
                string checkSum = _fileChecksumService.Calculate(o);
               // Data.Projects.ForEach(p =>
                foreach (var projects in Data.Projects)
                {
                    Game game = projects.Games.FirstOrDefault(l => l.ExecutableName == Path.GetFileName(o) && l.CheckSum == checkSum);
                    if (game != null)
                    {
                        game.Location = o.Replace(game.PathFromLocationToExe == null ? game.ExecutableName : $"{game.PathFromLocationToExe}//{executaleNames}", string.Empty);
                    }
                };
            });

            Data.SaveData();
        }
    }
}