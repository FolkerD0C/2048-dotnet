* TODO add unit tests for all DLLs where needed in Game2048
* Update README.md
* Consider using System.Runtime.Serialization.ISerializable instead of Game2048.Shared.Models.Iserializable
* Populate Game2048.Repository.IGameRepository and Game2048.Logic.Saving.IGameSaveHandler with Guid
  * Use them at saving/serialization
  * Upon loading/creating a game store the Guid-IGameRepository and (same)Guid-IGameSaveHandler pairs in a dictionary in Game2048.Logic.GameLogic
    * Upon loading/creating a game a Guid should be generated and stored in a GameRepository and then saved in the GameSaveHandler as well when its constructor gets the GameRepository object.
    * When a game has not been saved, the Guid, the GameRepository and the GameSaveHandler should all be destroyed
    * When a game has been saved, the Guid, the GameRepository and the GameSaveHandler should all be kept and then they can be loaded from the dictionary