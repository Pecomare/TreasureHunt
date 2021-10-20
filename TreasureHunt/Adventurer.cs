namespace TreasureHunt;

/// <summary>
/// Represents an adventurer.
/// </summary>
public class Adventurer
{
    /// <summary>
    /// Adventurer's name.
    /// </summary>
    private string Name { get; }
    /// <summary>
    /// Adventurer's X position on the map.
    /// </summary>
    public int PositionX { get; private set; }
    /// <summary>
    /// Adventurer's Y position on the map.
    /// </summary>
    public int PositionY { get; private set; }
    /// <summary>
    /// Adventurer's <see cref="Orientation"/>.
    /// </summary>
    private Orientation Orientation { get; set; }
    /// <summary>
    /// Adventurer's move sequence.
    /// </summary>
    /// <remarks>
    /// A move can be one of the following: Forward (A), Turn left (G) and Turn right (D).
    /// </remarks>
    private CharEnumerator Moves { get; }
    /// <summary>
    /// The amount of treasures the adventurer gathered.
    /// </summary>
    private int TreasureCount { get; set; }

    /// <summary>
    /// Creates an adventurer.
    /// </summary>
    /// <param name="name">Adventurer's name</param>
    /// <param name="x">Adventurer's x position</param>
    /// <param name="y">Adventurer's y position</param>
    /// <param name="orientation">Adventurer's <see cref="Orientation"/></param>
    /// <param name="moves">Adventurer's move sequence</param>
    public Adventurer(string name, int x, int y, Orientation orientation, CharEnumerator moves)
    {
        Name = name;
        PositionX = x;
        PositionY = y;
        Orientation = orientation;
        Moves = moves;
    }

    /// <summary>
    /// Grabs a treasure, increasing its <see cref="TreasureCount"/>.
    /// </summary>
    public void GrabTreasure()
    {
        TreasureCount++;
    }

    /// <summary>
    /// Tries to perform a move.
    /// </summary>
    /// <remarks>
    /// The move is the next one in the move sequence given at creation of the adventurer.
    /// </remarks>
    /// <param name="move">The move performed if successful, ' ' otherwise.</param>
    /// <returns><c>true</c> if the move was successfully performed, <c>else</c> otherwise.</returns>
    public bool TryGetNextMove(out char move)
    {
        bool success = Moves.MoveNext();
        if (!success)
        {
            move = ' ';
            return false;
        }
        move = Moves.Current;
        return true;
    }

    /// <summary>
    /// Rotates the adventurer 90° to the left.
    /// </summary>
    public void RotateLeft()
    {
        Orientation = Orientation switch
        {
            Orientation.N => Orientation.W,
            Orientation.W => Orientation.S,
            Orientation.S => Orientation.E,
            Orientation.E => Orientation.N,
            _ => Orientation
        };
    }

    /// <summary>
    /// Rotates the adventurer 90° to the right.
    /// </summary>
    public void RotateRight()
    {
        Orientation = Orientation switch
        {
            Orientation.N => Orientation.E,
            Orientation.E => Orientation.S,
            Orientation.S => Orientation.W,
            Orientation.W => Orientation.N,
            _ => Orientation
        };
    }

    /// <summary>
    /// Moves the adventurer one tile forward.
    /// </summary>
    public void MoveForward()
    {
        switch (Orientation)
        {
            case Orientation.N:
                PositionY--;
                break;
            
            case Orientation.E:
                PositionX++;
                break;
            
            case Orientation.S:
                PositionY++;
                break;
            
            case Orientation.W:
                PositionX--;
                break;
        }
    }

    /// <summary>
    /// Gets the coordinates of a one tile forward movement.
    /// </summary>
    /// <returns>A pair of (x, y) coordinates representing the destination of a one tile forward movement.</returns>
    public (int, int) GetForwardPosition() => Orientation switch
    {
        Orientation.N => (PositionX, PositionY - 1),
        Orientation.E => (PositionX + 1, PositionY),
        Orientation.S => (PositionX, PositionY + 1),
        Orientation.W => (PositionX - 1, PositionY),
        _ => (PositionX, PositionY)
    };

    /// <summary>
    /// Returns a string representation of the adventurer.
    /// </summary>
    /// <returns>A string representation in the following format:
    /// "A - <see cref="Name"/> - <see cref="PositionX"/> - <see cref="PositionY"/> - <see cref="Orientation"/> - <see cref="TreasureCount"/>"</returns>
    public override string ToString() => $"A - {Name} - {PositionX} - {PositionY} - {Orientation} - {TreasureCount}";
}