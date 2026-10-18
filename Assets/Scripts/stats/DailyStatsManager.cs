
using System.Collections.Generic;
using UnityEngine;

public class DailyStatsManager : MonoBehaviour
{
    public static DailyStatsManager Instance;

    [Header("Current Day")]
    public int currentDay = 1;

    public DailyStats currentStats = new DailyStats();

    [Header("Completed Days")]
    public List<DailyStats> dailyHistory =
        new List<DailyStats>();

    private bool dayFinished = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the manager when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentStats.dayNumber = currentDay;
    }

    // ==========================================
    // REQUEST STATISTICS
    // ==========================================

    public void RecordRequestAccepted()
    {
        currentStats.requests.accepted++;
    }

    public void RecordRequestCompleted()
    {
        currentStats.requests.completed++;
    }

    public void RecordRequestDiscarded()
    {
        currentStats.requests.discarded++;
    }

    public void RecordRequestDamaged()
    {
        currentStats.requests.damaged++;
    }

    public void RecordRequestExpired()
    {
        currentStats.requests.expired++;
    }

    // ==========================================
    // ECONOMY STATISTICS
    // ==========================================

    public void RecordMoneyEarned(int amount)
    {
        currentStats.economy.moneyEarned +=
            Mathf.Max(0, amount);
    }

    public void RecordMoneySpent(int amount)
    {
        currentStats.economy.moneySpent +=
            Mathf.Max(0, amount);
    }

    public void RecordFuelConsumed(int amount)
    {
        currentStats.economy.fuelConsumed +=
            Mathf.Max(0, amount);
    }

    public void RecordLocationVisited()
    {
        currentStats.economy.locationsVisited++;
    }

    // ==========================================
    // END OF DAY
    // ==========================================

    public void EndDay()
    {
        if (dayFinished)
            return;

        dayFinished = true;

        // Save a snapshot of today's statistics.
        DailyStats completedDay =
            JsonUtility.FromJson<DailyStats>(
                JsonUtility.ToJson(currentStats)
            );

        dailyHistory.Add(completedDay);

        Debug.Log(
            "Day " + currentDay +
            " finished. Completed requests: " +
            completedDay.requests.completed
        );
    }

    // ==========================================
    // START NEXT DAY
    // ==========================================

    public void StartNewDay()
    {
        if (!dayFinished)
            return;

        currentDay++;

        currentStats = new DailyStats
        {
            dayNumber = currentDay
        };

        dayFinished = false;

        if (GameClock.Instance != null)
        {
            GameClock.Instance.StartNewDay();
        }
    }

    // ==========================================
    // GET COMPLETED DAY
    // ==========================================

    public DailyStats GetPreviousDay()
    {
        if (dailyHistory.Count == 0)
            return null;

        return dailyHistory[dailyHistory.Count - 1];
    }
}
