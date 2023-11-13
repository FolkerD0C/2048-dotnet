using System;

namespace Game2048.Repository.Exceptions;

public class GameOverException : Exception
{
    public GameOverException() : base("You have ran out of lives, game is over") { }
}