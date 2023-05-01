namespace Game2048;

class Display
{
    readonly string[] borderUntil2049 = new string[]
    {
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+",
        "|    |    |    |    |",
        "+----+----+----+----+"
    };

    readonly string[] borderAfter2048 = new string[]
    {
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+",
        "|      |      |      |      |",
        "+------+------+------+------+"
    };

    readonly string help1 = "Use arrow keys";
    readonly string help2 = "or WASD to move";

    readonly string points = "Points:";

    int maxSpaceForTiles = 4;
    
    int maxSpaceForScore = 9;

    int gridWidth = 21;

    //Constants for display positions
    readonly (int Vertical, int Horizontal) gridPosition = (0, 0);
    readonly (int Vertical, int Horizontal) help1Position = (3, 35);
    readonly (int Vertical, int Horizontal) help2Position = (5, 35);
    readonly (int Vertical, int Horizontal) pointsPosition = (10, 10);
    readonly (int Vertical, int Horizontal) scorePosition = (10, 18);

    string NumberToDisplayWidth(int number, int maxSpace)
    {
        int actualWidth = $"{number}".Length;
        string result = new string(' ', maxSpace - actualWidth) + $"{number}";
        return result;
    }

    public void InitializeDisplay()
    {

    }

    public void ScaleUp()
    {

    }

    public void PrintConsts((int Vertical, int Horizontal) position, string displayText)
    {

    }

    public void PrintTile(int vertical, int horizontal, int value)
    {

    }

    (int Vertical, int Horizontal) ParsePosition(int vertical, int horizontal)
    {
        return (-1, -1);
    }
}
