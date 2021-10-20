namespace TreasureHunt.Exceptions;

public class NoMapLineException : ApplicationException
{
    public override string Message => "Cannot create a map without its dimensions";
}