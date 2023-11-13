using System;

namespace Game2048.Backend.Helpers.Exceptions;

public class GameOverException : Exception
{
    public GameOverException() : base("You have ran out of lives, game is over") {  }
}