using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TreasureHunt;
using TreasureHunt.Tiles;
using Xunit;

namespace TreasureHuntTest;

public class MapExportTests
{
    [Fact]
    public void MapExport()
    {
        Random random = new Random();

        // Generate and initialise TileMap
        int width = random.Next(10) + 2;
        int height = random.Next(10) + 2;
        Tile[,] tileMap = new Tile[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tileMap[i, j] = new Plains(0);
            }
        }
        
        // Generate non-plains tiles and adventurers
        int mountainX = random.Next(width / 2);
        int mountainY = random.Next(height);
        tileMap[mountainX, mountainY] = new Mountains();
        
        int treasureX = random.Next(width / 2) + (width / 2);
        int treasureY = random.Next(height);
        int treasureCount = random.Next(1, 10);
        tileMap[treasureX, treasureY] = new Plains(treasureCount);

        int adventurerX = random.Next(width / 2) + (width / 2);
        int adventurerY = random.Next(height);
        Orientation adventurerOrientation = Enum.GetValues<Orientation>()[random.Next(4)];
        
        var adventurers = new List<Adventurer>
        {
            new("Adventurer",adventurerX,adventurerY,adventurerOrientation,"".GetEnumerator())
        };

        // Generate map object
        ConstructorInfo[] constructors = typeof(Map).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        Map map = (Map)constructors[0].Invoke(new object?[] { tileMap, width, height, adventurers });
        
        // Export map
        map.ExportToFile("export_test.txt");
        IEnumerator<string> lines = File.ReadLines("export_test.txt").GetEnumerator();
        
        // Compare file lines to map lines
        lines.MoveNext();
        Assert.Equal($"C - {width} - {height}", lines.Current);

        lines.MoveNext();
        Assert.Equal($"M - {mountainX} - {mountainY}", lines.Current);

        lines.MoveNext();
        Assert.Equal("# {T comme Trésor} - {Axe horizontal} - {Axe vertical} - {Nb. de trésors restant}",
            lines.Current);
        
        lines.MoveNext();
        Assert.Equal($"T - {treasureX} - {treasureY} - {treasureCount}", lines.Current);

        lines.MoveNext();
        Assert.Equal(
            "# {A comme Aventurier} - {Nom de l'aventurier} - {Axe horizontal} - {Axe vertical} - {Orientation} - {Nb. de trésors ramassés}",
            lines.Current);
        
        lines.MoveNext();
        Assert.Equal($"A - Adventurer - {adventurerX} - {adventurerY} - {adventurerOrientation} - 0", lines.Current);
        
        lines.Dispose();
    }
}