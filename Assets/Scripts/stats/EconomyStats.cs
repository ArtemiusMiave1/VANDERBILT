[System.Serializable]
public class EconomyStats
{
    public int moneyEarned;
    public int moneySpent;
    public int fuelConsumed;
    public int locationsVisited;

    public int NetProfit => moneyEarned - moneySpent;
}