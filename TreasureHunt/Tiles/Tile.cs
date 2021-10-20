namespace TreasureHunt.Tiles;

/// <summary>
/// Represents a tile on a map.
/// </summary>
public abstract class Tile
{
    /// <summary>
    /// Gets if the tile can be walked on.
    /// </summary>
    public virtual bool CanBeWalkedOn => false;
    /// <summary>
    /// Character representation of the tile.
    /// </summary>
    protected char Code { get; init; }

    /// <summary>
    /// Returns a string representation of the <see cref="Tile"/> with coordinates.
    /// </summary>
    /// <param name="x">x coordinate.</param>
    /// <param name="y">y coordinate.</param>
    /// <returns>A string representation in the following format: "<see cref="Code"/> - <paramref name="x"/> - <paramref name="y"/>".</returns>
    public virtual string ToString(int x, int y) => $"{Code} - {x} - {y}";
}