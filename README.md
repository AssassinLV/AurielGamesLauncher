# AurielGamesLauncher
Downloads per version are located in https://github.com/AssassinLV/AurielGamesLauncher/tree/master/StableReleaseZips

This application requires you to have Microsoft .NET 5 runtime to be installed for application to be running.

## Features:
* Set game locations
* Launch games
* Remembers the last game you have played
* Play next button automatically gives you option to play the next eppisode
* Cleaning the OST_Game folder, you should do that if the game is constantly crashing (eg. crash on startup, crash on location change etc.)

## To be added (in future)
* Auto-scan folder for mapping the games
  * Currently have issue that multiple executables shares same MD5 checksum, have to think up an additional check (configurable via json) to definitely validate the game folders
  * In addition if there are multiple matches found, application should give user the ability to select the correct one, maybe even try to understand from the naming of the folders...
* Ability to add new Projects (groups of the game)
* Ability to add new Games
* Ability to chose the sorting of the games
* Ability to add custom sort
* Ability to update the game/project list from the file located in here...
