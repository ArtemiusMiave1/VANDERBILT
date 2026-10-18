[System.Serializable]
public class DailyStats
{
    public int dayNumber = 1;

    public RequestStats requests = new RequestStats();
    public EconomyStats economy = new EconomyStats();
}