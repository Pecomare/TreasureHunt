using System;
using System.Collections.Generic;
using System.Reflection;
using TreasureHunt;
using TreasureHunt.Tiles;
using Xunit;

namespace TreasureHuntTest;

public class AdventurerMovementTests
{
    private CharEnumerator Advance => "A".GetEnumerator();
    private CharEnumerator TurnLeft => "G".GetEnumerator();
    
    static Tile[,] CreateTileMap(int width, int height)
    {
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
    
    static Map CreateMap(Tile[,] tileMap, int width, int height, List<Adventurer> adventurers)
    {
        PropertyInfo? tileAdventurer =
            typeof(Plains).GetProperty("Adventurer", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (Adventurer adventurer in adventurers)
        {
            Tile tile = tileMap[adventurer.PositionX, adventurer.PositionY];
            if (tile is Plains walkable)
            {
                tileAdventurer.SetValue(walkable, adventurer);
            }
        }
        ConstructorInfo[] constructors = typeof(Map).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        Map map = (Map)constructors[0].Invoke(new object?[] { tileMap, width, height, adventurers });
        return map;
    }
    
    static Tile GetTile(Map map, int x, int y)
    {
        PropertyInfo? tileMapPropertyInfo = typeof(Map).GetProperty("TileMap", BindingFlags.NonPublic | BindingFlags.Instance);
        Tile[,] tileMap = (Tile[,])tileMapPropertyInfo.GetValue(map);
        return tileMap[x, y];
    }

    static bool HasAdventurer(Map map, int x, int y)
    {
        PropertyInfo? tileMapPropertyInfo = typeof(Map).GetProperty("TileMap", BindingFlags.NonPublic | BindingFlags.Instance);
        Tile[,] tileMap = (Tile[,])tileMapPropertyInfo.GetValue(map);
        Tile tile = tileMap[x, y];
        Plains walkable = (Plains)tile; 
        return walkable.IsAdventurerOn;
    }

    static bool IsAdventurerAt(Map map, Adventurer adventurer, int x, int y)
    {
        PropertyInfo? tileMapPropertyInfo = typeof(Map).GetProperty("TileMap", BindingFlags.NonPublic | BindingFlags.Instance);
        Tile[,] tileMap = (Tile[,])tileMapPropertyInfo.GetValue(map);
        Tile tile = tileMap[x, y];
        Plains walkable = (Plains)tile; 
        PropertyInfo? adventurerPropertyInfo = typeof(Plains).GetProperty("Adventurer", BindingFlags.NonPublic | BindingFlags.Instance);
        return adventurerPropertyInfo.GetValue(walkable) == adventurer;
    }
    
    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsMapLeftBoundaries()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", 0, adventurerY, Orientation.W, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, 0, adventurerY));
    }
    
    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsMapTopBoundaries()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, 0, Orientation.N, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, 0));
    }
    
    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsMapRightBoundaries()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", width - 1, adventurerY, Orientation.E, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, width - 1, adventurerY));
    }
    
    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsMapDownBoundaries()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, height - 1, Orientation.S, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, height - 1));
    }
    
    [Fact]
    public void AdventurerCanMoveIfFacingTowardsLeftPlains()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", width - 1, adventurerY, Orientation.W, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, width - 2, adventurerY));
    }
    
    [Fact]
    public void AdventurerCanMoveIfFacingTowardsTopPlains()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, height - 1, Orientation.N, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, height - 2));
    }
    
    [Fact]
    public void AdventurerCanMoveIfFacingTowardsRightPlains()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", width - 2, adventurerY, Orientation.E, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, width - 1, adventurerY));
    }
    
    [Fact]
    public void AdventurerCanMoveIfFacingTowardsDownPlains()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, height - 2, Orientation.S, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, height - 1));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsLeftMountain()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        tileMap[width - 2, adventurerY] = new Mountains();
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", width - 1, adventurerY, Orientation.W, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, width - 1, adventurerY));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsUpMountain()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        tileMap[adventurerX, height - 2] = new Mountains();
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, height - 1, Orientation.N, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, height - 1));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsRightMountain()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        tileMap[width - 1, adventurerY] = new Mountains();
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", width - 2, adventurerY, Orientation.E, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, width - 2, adventurerY));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsDownMountain()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        tileMap[adventurerX, height - 1] = new Mountains();
        Map map = CreateMap(tileMap, width, height,
            new List<Adventurer> { new("Adventurer", adventurerX, height - 2, Orientation.S, Advance) });
        map.Execute();
        Assert.True(HasAdventurer(map, adventurerX, height - 2));
    }

    [Fact]
    public void AdventurerGrabsATreasureWhenWalkingOnIt()
    {
        Tile[,] tileMap = CreateTileMap(2, 2);
        tileMap[1, 1] = new Plains(1);
        Map map = CreateMap(tileMap, 2, 2, new List<Adventurer> { new("Adventurer", 1, 0, Orientation.S, Advance) });
        map.Execute();
        Assert.True(GetTile(map, 1, 1) is Plains);
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsLeftAdventurer()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        var adventurers = new List<Adventurer>
        {
            new("Adventurer", width - 2, adventurerY, Orientation.N, TurnLeft),
            new("Adventurer2", width - 1, adventurerY, Orientation.W, Advance)
        };
        Map map = CreateMap(tileMap, width, height, adventurers);
        map.Execute();
        Assert.True(IsAdventurerAt(map, adventurers[1], width - 1, adventurerY));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsUpAdventurer()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        var adventurers = new List<Adventurer>
        {
            new("Adventurer", adventurerX, height - 2, Orientation.N, TurnLeft),
            new("Adventurer2", adventurerX, height - 1, Orientation.N, Advance)
        };
        Map map = CreateMap(tileMap, width, height, adventurers);
        map.Execute();
        Assert.True(IsAdventurerAt(map, adventurers[1], adventurerX, height - 1));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsRightAdventurer()
    {
        Random random = new();
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 1;
        int adventurerY = random.Next(height);
        Tile[,] tileMap = CreateTileMap(width, height);
        var adventurers = new List<Adventurer>
        {
            new("Adventurer", width - 1, adventurerY, Orientation.N, TurnLeft),
            new("Adventurer2", width - 2, adventurerY, Orientation.E, Advance)
        };
        Map map = CreateMap(tileMap, width, height, adventurers);
        map.Execute();
        Assert.True(IsAdventurerAt(map, adventurers[1], width - 2, adventurerY));
    }

    [Fact]
    public void AdventurerCannotMoveIfFacingTowardsDownAdventurer()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 2;
        int adventurerX = random.Next(width);
        Tile[,] tileMap = CreateTileMap(width, height);
        var adventurers = new List<Adventurer>
        {
            new("Adventurer", adventurerX, height - 1, Orientation.N, TurnLeft),
            new("Adventurer2", adventurerX, height - 2, Orientation.S, Advance)
        };
        Map map = CreateMap(tileMap, width, height, adventurers);
        map.Execute();
        Assert.True(IsAdventurerAt(map, adventurers[1], adventurerX, height - 2));
    }
}