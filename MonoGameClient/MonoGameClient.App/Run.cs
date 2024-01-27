namespace MonoGameClient.App;

public class Run
{
    public static void Main(string[] _)
    {
        using Game2048ish game = Game2048ish.Instance;
        game.Run();
    }
}