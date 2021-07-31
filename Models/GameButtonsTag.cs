namespace Models
{
    public class GameButtonsTag
    {
        public GameButtonsTag()
        {
            // No logic needed here!
        }

        public GameButtonsTag(string projectName, string gameName)
        {
            ProjectName = projectName;
            GameName = gameName;
        }

        public string ProjectName { get; set; }
        public string GameName { get; set; }
    }
}
