

public class LabyrinthGeneratorCell
{
    public int X { get; private set; }
   
    public int Y { get; private set; }

    public bool IsVisited;

    public bool IsWallLeftActive = true;
    public bool IsWallRightActive = true;
    public bool IsWallTopActive = true;
    public bool IsWallDownActive = true;

    public LabyrinthGeneratorCell(int x, int y)
    {
        X = x;
        Y = y;
    }
}

