namespace TreasureHunt.Exceptions;

public class InvalidMoveException : ApplicationException
{
    public InvalidMoveException(string message) : base(message)
    {
        
    }
}