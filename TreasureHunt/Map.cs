using System.Text;
using TreasureHunt.Exceptions;
using TreasureHunt.Tiles;

namespace TreasureHunt;

/// <summary>
/// Represents a map.
/// </summary>
/// <remarks>
/// A map is a mix of <see cref="Plains"/> and <see cref="Mountains"/>
/// with <see cref="Treasure"/>s on it and <see cref="Adventurer"/>s looking for them. 
/// </remarks>
public class Map
{
    /// <summary>
    /// Gets the matrix of <see cref="Plains"/>, <see cref="Mountains"/> and <see cref="Treasure"/>s.
    /// </summary>
    private Tile[,] TileMap { get; }
    /// <summary>
    /// Gets the width of the map.
    /// </summary>
    private int Width { get;  }
    /// <summary>
    /// Gets the height of the map.
    /// </summary>
    private int Height { get; }
    /// <summary>
    /// Gets the list of <see cref="Adventurer"/>s.
    /// </summary>
    private List<Adventurer> Adventurers { get; }

    /// <summary>
    /// Creates a map.
    /// </summary>
    /// <param name="tileMap">The mix of <see cref="Plains"/>, <see cref="Mountains"/> and <see cref="Treasure"/>s.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="adventurers">The list of <see cref="Adventurer"/>s.</param>
    private Map(Tile[,] tileMap, int width, int height, List<Adventurer> adventurers)
    {
        TileMap = tileMap;
        Width = width;
        Height = height;
        Adventurers = adventurers;
    }
    
    /// <summary>
    /// Creates a <see cref="Map"/> from a file.
    /// </summary>
    /// <param name="filepath">Path to the file.</param>
    /// <returns>A <see cref="Map"/> with mountains, treasures and adventurers on it.</returns>
    /// <exception cref="ApplicationException">If map width or height is lower or equal to 0, a tile is placed outside the map boundaries or an adventurer is placed on a non-walkable tile.</exception>
    public static Map CreateMapFromFilePath(string filepath)
    {
        IEnumerator<string> lines = File.ReadLines(filepath).GetEnumerator();
        return CreateMapFromLines(lines);
    }
    
    /// <summary>
    /// Creates a <see cref="Map"/> from lines.
    /// </summary>
    /// <param name="lines">An enumeration of the map lines.</param>
    /// <returns>A <see cref="Map"/> with mountains, treasures and adventurers on it.</returns>
    /// <exception cref="ApplicationException">If map width or height is lower or equal to 0, a tile is placed outside the map boundaries or an adventurer is placed on a non-walkable tile.</exception>
    public static Map CreateMapFromLines(IEnumerator<string> lines)
    {
        List<Adventurer> adventurers = new();
        if (!lines.MoveNext())
        {
            throw new EmptyMapFileException();
        }
        string mapLine = lines.Current;
        if (!mapLine.TrimStart().StartsWith('C'))
        {
            throw new NoMapLineException();
        }

        Tile[,] tileMap = CreateTileMap(mapLine, out int width, out int height);

        // Create non-plains tiles
        while (lines.MoveNext())
        {
            string line = lines.Current;
            if (line.TrimStart().StartsWith('#'))
            {
                continue;
            }

            string[] tokens = line.Split('-');
            switch (tokens[0].Trim())
            {
                case "M":
                    CreateMountains(tileMap, width, height, tokens);
                    break;
                
                case "T":
                    CreateTreasure(tileMap, width, height, tokens);
                    break;
                
                case "A":
                    CreateAdventurer(tileMap, width, height, tokens, adventurers);
                    break;
            }
        }

        return new Map(tileMap, width, height, adventurers);
    }

    /// <summary>
    /// Creates a tile map.
    /// </summary>
    /// <param name="mapLine">The map line.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <returns>A matrix of <paramref name="width"/> x <paramref name="height"/> entirely made of <see cref="Plains"/>.</returns>
    /// <exception cref="InvalidLineFormatException">If <paramref name="mapLine"/>'s format is incorrect.</exception>
    /// <exception cref="InvalidLineValueException">If either <paramref name="width"/> or <paramref name="height"/> is not a positive integer.</exception>
    private static Tile[,] CreateTileMap(string mapLine, out int width, out int height)
    {
        string[] tokens = mapLine.Split('-');
        if (tokens.Length != 3)
        {
            throw new InvalidLineFormatException("Map declaration line should be in the following format: C - <width> - <height>");
        }
        if (!int.TryParse(tokens[1].Trim(), out width))
        {
            throw new InvalidLineValueException("Map width should be an integer");
        }
        if (width <= 0)
        {
            throw new InvalidLineValueException("Map width should be positive");
        }
        if (!int.TryParse(tokens[2].Trim(), out height))
        {
            throw new InvalidLineValueException("Map height should be an integer");
        }
        if (height <= 0)
        {
            throw new InvalidLineValueException("Map height should be positive");
        }
        
        Tile[,] tileMap = new Tile[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tileMap[i, j] = new Plains(0);
            }
        }

        return tileMap;
    }

    /// <summary>
    /// Adds a <see cref="Mountains"/> to a map.
    /// </summary>
    /// <param name="tileMap">The tile map to add the <see cref="Mountains"/> to.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="tokens">The mountains' values.</param>
    /// <exception cref="InvalidLineFormatException">If the mountains' line's format is incorrect.</exception>
    /// <exception cref="InvalidLineValueException">If either the x or y coordinate is not an integer.</exception>
    /// <exception cref="OutOfMapException">If the mountains would be placed outside the map.</exception>
    private static void CreateMountains(Tile[,] tileMap, int width, int height, string[] tokens)
    {
        if (tokens.Length != 3)
        {
            throw new InvalidLineFormatException(
                "A mountain declaration line should be in the following format: M - <x> - <y>");
        }
                    
        if (!int.TryParse(tokens[1].Trim(), out int x))
        {
            throw new InvalidLineValueException("Mountain x should be an integer");
        }
        if (x < 0 || x >= width)
        {
            throw new OutOfMapException("Mountain x should be within the dimensions of the map");
        }
                    
        if (!int.TryParse(tokens[2].Trim(), out int y))
        {
            throw new InvalidLineValueException("Mountain y should be an integer");
        }
        if (y < 0 || y >= height)
        {
            throw new OutOfMapException("Mountain y should be within the dimensions of the map");
        }

        if (tileMap[x, y] is Mountains)
        {
            // Better practice would be to log the warning. For the test's sake, we'll only write it in the console.
            Console.WriteLine($"Warning: There is already a mountain at coordinates [{x}, {y}].");
        }

        tileMap[x, y] = new Mountains();
    }

    /// <summary>
    /// Adds a <see cref="Treasure"/> to a map.
    /// </summary>
    /// <param name="tileMap">The tile map to add the <see cref="Treasure"/> to.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="tokens">The treasure's values.</param>
    /// <exception cref="InvalidLineFormatException">If the treasure's line's format is incorrect.</exception>
    /// <exception cref="InvalidLineValueException">If either the x or y coordinate is not an integer.</exception>
    /// <exception cref="OutOfMapException">If the treasure would be placed outside the map.</exception>
    /// <exception cref="TreasureCountNegative">If the treasure count is negative.</exception>
    /// <exception cref="InvalidPlacementException">If the treasure would be placed on a <see cref="Mountains"/>.</exception>
    private static void CreateTreasure(Tile[,] tileMap, int width, int height, string[] tokens)
    {
        if (tokens.Length != 4)
        {
            throw new InvalidLineFormatException(
                "A treasure declaration line should be in the following format: T - <x> - <y> - <count>");
        }
                    
        if (!int.TryParse(tokens[1].Trim(), out int x))
        {
            throw new InvalidLineValueException("Treasure x should be an integer");
        }
        if (x < 0 || x >= width)
        {
                        
            throw new OutOfMapException("Treasure x should be within the dimensions of the map");
        }
        if (!int.TryParse(tokens[2].Trim(), out int y))
        {
            throw new InvalidLineValueException("Treasure y should be an integer");
        }
        if (y < 0 || y >= height)
        {
            throw new OutOfMapException("Treasure y should be within the dimensions of the map");
        }
                    
        if (!int.TryParse(tokens[3].Trim(), out int count))
        {
            throw new InvalidLineValueException("Treasure count should be an integer");
        }
        if (count <= 0)
        {
            throw new TreasureCountNegative();
        }

        switch (tileMap[x, y])
        {
            case Mountains:
                throw new InvalidPlacementException("Cannot place a treasure on a mountain");
            
            case Plains { TreasureCount: > 0 }:
                // Better practice would be to log the warning. For the test's sake, we'll only write it in the console.
                Console.WriteLine($"There is already a treasure at coordinates [{x}, {y}]. Overriding original.");
                break;
        }

        tileMap[x, y] = new Plains(count);
    }

    /// <summary>
    /// Adds an <see cref="Adventurer"/> to a map.
    /// </summary>
    /// <param name="tileMap">The tile map to add the <see cref="Adventurer"/> to.</param>
    /// <param name="width">The width of the map.</param>
    /// <param name="height">The height of the map.</param>
    /// <param name="tokens">The treasure's values.</param>
    /// <param name="adventurers">The adventurer list to add the adventurer to.</param>
    /// <exception cref="InvalidLineFormatException">If the adventurer's line's format is incorrect.</exception>
    /// <exception cref="InvalidLineValueException">If either the x or y coordinate is not an integer.</exception>
    /// <exception cref="OutOfMapException">If the adventurer would be placed outside the map.</exception>
    /// <exception cref="InvalidPlacementException">If the adventurer would be placed on a <see cref="Mountains"/>.</exception>
    /// <exception cref="InvalidOrientationException">If the adventurer has a wrong orientation.</exception>
    /// <exception cref="InvalidMoveException">If the move list contains a move other than A (Advance), G (Turn left) or D (Turn right).</exception>
    private static void CreateAdventurer(Tile[,] tileMap, int width, int height, string[] tokens, List<Adventurer> adventurers)
    {
        if (tokens.Length != 6)
        {
            throw new InvalidLineFormatException(
                "An adventurer declaration line should be in the following format: A - <name> - <x> - <y> - <N|E|S|W> - <moves>");
        }

        string name = tokens[1];
        if (!int.TryParse(tokens[2].Trim(), out int x))
        {
            throw new InvalidLineValueException("Adventurer x should be an integer");
        }
        
        if (x < 0 || x >= width)
        {
            throw new OutOfMapException("Adventurer x should be within the dimensions of the map");
        }
        
        if (!int.TryParse(tokens[3].Trim(), out int y))
        {
            throw new InvalidLineValueException("Adventurer y should be an integer");
        }
        if (y < 0 || y >= height)
        {
            throw new OutOfMapException("Adventurer y should be within the dimensions of the map");
        }

        if (!tileMap[x, y].CanBeWalkedOn)
        {
            throw new InvalidPlacementException("Cannot put an adventurer in a non-walkable tile");
        }

        if (!Enum.TryParse(tokens[4], true, out Orientation orientation))
        {
            throw new InvalidOrientationException("An adventurer must have an orientation of either N, E, S or W");
        }

        string moves = tokens[5].Trim();
        foreach (char c in moves)
        {
            if (c != 'A' && c != 'D' && c != 'G')
            {
                throw new InvalidMoveException("An adventurer can only advance (A), turn left (G) or turn right (D)");
            }
        }

        Adventurer adventurer = new(name, x, y, orientation, moves.GetEnumerator());

        adventurers.Add(adventurer);

        tileMap[x, y] = new Plains(((Plains)tileMap[x, y]).TreasureCount, adventurer);
    }

    /// <summary>
    /// Performs all adventurers' moves.
    /// </summary>
    /// <remarks>
    /// Moves are taken by turn.<br/>
    /// First adventurer performs its first move, then second adventurer does its first, etc...<br/>
    /// Then when all adventurers did their move this turn (if they still had some to do),
    /// we go back to the first adventurer and they do their next move.<br/>
    /// The method stops when nobody has moved in a turn.
    /// </remarks>
    public void Execute()
    {
        bool hasAnAdventurerMoved = true;
        while (hasAnAdventurerMoved)
        {
            hasAnAdventurerMoved = false;
            foreach (Adventurer adventurer in Adventurers)
            {
                if (TryMoveAdventurer(adventurer))
                {
                    hasAnAdventurerMoved = true;
                }
            }
        }
    }

    /// <summary>
    /// Tries to perform an adventurer move.
    /// </summary>
    /// <param name="adventurer">The adventurer who performs the move.</param>
    /// <returns><c>true</c> if the move was successfully performed, <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidMoveException">If the move is neither A (Advance), G (Turn left) nor D (Turn right).</exception>
    private bool TryMoveAdventurer(Adventurer adventurer)
    {
        if (!adventurer.TryGetNextMove(out char move))
        {
            return false;
        }
        switch (move)
        {
            // Forward
            case 'A': 
                (int x, int y) = adventurer.GetForwardPosition();
                if (x < 0 
                    || x >= Width
                    || y < 0 
                    || y >= Height)
                {
                    break;
                }
                Tile destination = TileMap[x, y];
                if (!destination.CanBeWalkedOn)
                {
                    break;
                }
                Plains current = (Plains)TileMap[adventurer.PositionX, adventurer.PositionY];
                current.OnWalkFrom();
                adventurer.MoveForward();
                ((Plains)destination).OnMoveOn(adventurer);
                break;
            // Turn left
            case 'G':
                adventurer.RotateLeft();
                break;
            // Turn right
            case 'D':
                adventurer.RotateRight();
                break;
            
            default:
                throw new InvalidMoveException("An adventurer can only advance (A), turn left (G) or turn right (D)");
        }

        return true;
    }

    /// <summary>
    /// Exports the state of the map to a file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    public void ExportToFile(string path)
    {
        StringBuilder builder = new();
        StringBuilder mountains = new();
        StringBuilder treasures = new();
        StringBuilder adventurers = new();

        builder.AppendLine($"C - {Width} - {Height}");
        treasures.AppendLine("# {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restant}");
        adventurers.AppendLine(
            "# {A comme Aventurier} - {Nom de l'aventurier} - {Axe horizontal} - {Axe vertical} - {Orientation} - {Nb. de trésors ramassés}");

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = TileMap[x, y];
                switch (tile)
                {
                    case Mountains:
                        mountains.AppendLine(tile.ToString(x, y));
                        break;
                    
                    case Plains { TreasureCount: > 0 }:
                        treasures.AppendLine(tile.ToString(x, y));
                        break;
                }
            }
        }

        foreach (Adventurer adventurer in Adventurers)
        {
            adventurers.AppendLine(adventurer.ToString());
        }

        builder.Append(mountains).Append(treasures).Append(adventurers);
        
        File.WriteAllText(path, builder.ToString());
    }
}