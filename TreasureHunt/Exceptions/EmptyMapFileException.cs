namespace TreasureHunt.Exceptions;

public class EmptyMapFileException : ApplicationException
{
    public override string Message => "Cannot generate a map from en empty file";
}