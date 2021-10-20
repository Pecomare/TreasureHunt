namespace TreasureHunt.Tiles;

/// <summary>
/// Represents a plains.
/// </summary>
/// <remarks>
/// A plains is walkable.
/// </remarks>
public class Plains : Tile
{
    /// <summary>
    /// Gets the amount of treasures on the tile.
    /// </summary>
    public int TreasureCount { get; private set; }
    /// <summary>
    /// Gets or sets the adventurer on the tile.
    /// </summary>
    private Adventurer? Adventurer { get; set; }

    /// <summary>
    /// Gets if the tile can be walked on.
    /// </summary>
    /// <remarks>
    /// A walkable tile cannot be walked on if an <see cref="Adventurer"/> is on it.
    /// </remarks>
    public override bool CanBeWalkedOn => !IsAdventurerOn;
    /// <summary>
    /// Gets if an <see cref="Adventurer"/> is on the tile.
    /// </summary>
    public bool IsAdventurerOn => Adventurer != null;
    
    /// <summary>
    /// Creates a <see cref="Plains"/>.
    /// </summary>
    /// <param name="treasureCount">The amount of treasures on the tile.</param>
    /// <param name="adventurer">The <see cref="Adventurer"/> to place on the tile.</param>
    public Plains(int treasureCount, Adventurer? adventurer = null)
    {
        Code = '.';
        TreasureCount = treasureCount;
        Adventurer = adventurer;
    }

    /// <summary>
    /// Performs an action when an <see cref="Adventurer"/> walks on the tile.
    /// </summary>
    /// <param name="adventurer">The <see cref="Adventurer"/> who walked on the tile.</param>
    public void OnMoveOn(Adventurer adventurer)
    {
        Adventurer = adventurer;
        if (TreasureCount == 0 || Adventurer == null)
        {
            return;
        }

        TreasureCount--;
        Adventurer.GrabTreasure();
    }

    /// <summary>
    /// Performs an action when an <see cref="Adventurer"/> walks from the tile.
    /// </summary>
    public void OnWalkFrom()
    {
        Adventurer = null;
    }

    /// <summary>
    /// Returns a string representation of the plains with coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>A string with one of the following formats:<ul>
    /// <li>"T - <paramref name="x"/> - <paramref name="y"/> - <see cref="TreasureCount"/>" if there are treasures on the tile</li> 
    /// <li>". - <paramref name="x"/> - <paramref name="y"/>" otherwise</li>
    /// </ul>
    /// </returns>
    public override string ToString(int x, int y)
    {
        return TreasureCount > 0
            ? $"T - {x} - {y} - {TreasureCount}"
            : $"{base.ToString(x, y)} - {TreasureCount}";
    } 
}