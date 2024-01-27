namespace MonoGameClient.App;

public class Run
{
    public static void Main(string[] _)
    {
        var game = Game2048ish.Instance;
        game.Run();
    }
}
