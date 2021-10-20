namespace TreasureHunt.Tiles;

/// <summary>
/// Represents a mountain.
/// </summary>
/// <remarks>
/// A Mountains is non-walkable.
/// </remarks>
public class Mountains : Tile
{
    /// <summary>
    /// Creates a <see cref="Mountains"/>.
    /// </summary>
    public Mountains()
    {
        Code = 'M';
    }
}