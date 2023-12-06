* Remove Game2048.Shared.ISerializable and use System.Text.Json.JsonSerializer instead
  * Create Game2048.Repository.Helpers.SaveDataObjects.GameSaveData and HighscoreSaveData classes for that
* Populate Game2048.Repository.IGameRepository and Game2048.Logic.Saving.IGameSaveHandler with Guid
  * Use them at saving/serialization
  * Upon loading/creating a game store the Guid-IGameRepository and (same)Guid-IGameSaveHandler pairs in a dictionary in Game2048.Logic.GameLogic
    * Upon loading/creating a game a Guid should be generated and stored in a GameRepository and then saved in the GameSaveHandler as well when its constructor gets the GameRepository object.
    * When a game has not been saved, the Guid, the GameRepository and the GameSaveHandler should all be destroyed
    * When a game has been saved, the Guid, the GameRepository and the GameSaveHandler should all be kept and then they can be loaded from the dictionary
* Move play logic events to Game2048.Logic if possible
* Rename namespaces and classes
  * Repository to Processors
  * Logic to Managers
* Add unit tests for all DLLs where needed in Game2048
* Rename repository to 2048ish
* Update README.md