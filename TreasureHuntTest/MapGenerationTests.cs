using System;
using System.Collections.Generic;
using TreasureHunt;
using TreasureHunt.Exceptions;
using Xunit;

namespace TreasureHuntTest;

public class MapGenerationTests
{
    [Fact]
    public void CannotGenerateEmptyMap()
    {
        List<string> lines = new();
        Assert.ThrowsAny<EmptyMapFileException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }
    
    [Fact]
    public void CannotGenerateMapWithNegativeDimensions()
    {
        Random random = new();
        List<string> lines = new(){ $"C - {- random.Next()} - {- random.Next()}" };
        Assert.ThrowsAny<InvalidLineFormatException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }
    
    [Fact]
    public void CannotGenerateMapWithZeroDimensions()
    {
        List<string> lines = new(){ "C - 0 - 0" };
        Assert.ThrowsAny<InvalidLineValueException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }
    
    [Fact]
    public void CannotPutATileOutsideTheMap()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        List<string> lines = new()
        {
            $"C - {width} - {height}"
            , $"M - {width + 1} - {height + 1}"
        };
        Assert.ThrowsAny<OutOfMapException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }

    [Fact]
    public void CannotPutATreasureOnAMountain()
    {
        Random random = new();
        int x = random.Next(10);
        int y = random.Next(10);
        List<string> lines = new()
        {
            "C - 10 - 10"
            , $"M - {x} - {y}"
            , $"T - {x} - {y} - 1"
        };
        Assert.ThrowsAny<InvalidPlacementException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }

    [Fact]
    public void CannotPutATreasureWithNegativeCount()
    {
        Random random = new();
        int x = random.Next(10);
        int y = random.Next(10);
        List<string> lines = new()
        {
            "C - 10 - 10"
            , $"T - {x} - {y} - 0"
        };
        Assert.ThrowsAny<TreasureCountNegative>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }

    [Fact]
    public void CannotPutAnAdventurerOutsideTheMap()
    {
        Random random = new();
        int width = random.Next(10) + 1;
        int height = random.Next(10) + 1;
        List<string> lines = new()
        {
            $"C - {width} - {height}"
            , $"A - Adventurer - {width + 1} - {height + 1} - N - GDGDGDAA"
        };
        Assert.ThrowsAny<OutOfMapException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }

    [Fact]
    public void CannotPutAnAdventurerOnAMountain()
    {
        Random random = new();
        int x = random.Next(10);
        int y = random.Next(10);
        List<string> lines = new()
        {
            "C - 10 - 10"
            , $"M - {x} - {y}"
            , $"A - Adventurer - {x} - {y} - N - DGDGDGAA"
        };
        Assert.ThrowsAny<InvalidPlacementException>(() => Map.CreateMapFromLines(lines.GetEnumerator()));
    }
}