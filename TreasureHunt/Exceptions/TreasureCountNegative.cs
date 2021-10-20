namespace TreasureHunt.Exceptions;

public class TreasureCountNegative : ApplicationException
{
    public override string Message => "Treasure count cannot be 0 or less";
}