
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndOfDayUI : MonoBehaviour
{
    [Header("Day")]
    public TMP_Text dayText;

    [Header("Request Statistics")]
    public TMP_Text acceptedText;
    public TMP_Text completedText;
    public TMP_Text discardedText;
    public TMP_Text damagedText;
    public TMP_Text expiredText;

    [Header("Economy Statistics")]
    public TMP_Text moneyEarnedText;
    public TMP_Text moneySpentText;
    public TMP_Text netProfitText;
    public TMP_Text fuelConsumedText;
    public TMP_Text locationsVisitedText;

    [Header("Scene Settings")]
    public string gameplaySceneName = "Game";

    private void Start()
    {
        DisplayStats();
    }

    public void DisplayStats()
    {
        if (DailyStatsManager.Instance == null)
        {
            Debug.LogError("DailyStatsManager not found!");
            return;
        }

        DailyStats stats =
            DailyStatsManager.Instance.GetPreviousDay();

        if (stats == null)
        {
            Debug.LogWarning("No completed day statistics found.");
            return;
        }

        // ==================================================
        // DAY
        // ==================================================

        if (dayText != null)
            dayText.text = "Day: " + stats.dayNumber;

        // ==================================================
        // REQUEST STATISTICS
        // ==================================================

        if (acceptedText != null)
            acceptedText.text = "Accepted: " + stats.requests.accepted;

        if (completedText != null)
            completedText.text = "Completed: " + stats.requests.completed;

        if (discardedText != null)
            discardedText.text = "Discarded: " + stats.requests.discarded;

        if (damagedText != null)
            damagedText.text = "Damaged: " + stats.requests.damaged;

        if (expiredText != null)
            expiredText.text = "Expired: " + stats.requests.expired;

        // ==================================================
        // ECONOMY STATISTICS
        // ==================================================

        if (moneyEarnedText != null)
            moneyEarnedText.text = "Earned: " + stats.economy.moneyEarned;

        if (moneySpentText != null)
            moneySpentText.text = "Spent: " + stats.economy.moneySpent;

        if (netProfitText != null)
            netProfitText.text = "Profit: " + stats.economy.NetProfit;

        if (fuelConsumedText != null)
            fuelConsumedText.text = "Fuel: " + stats.economy.fuelConsumed;

        if (locationsVisitedText != null)
            locationsVisitedText.text = "Visited: " + stats.economy.locationsVisited;
    }

    // ==================================================
    // CONTINUE TO NEXT DAY
    // ==================================================

    public void ContinueToNextDay()
    {
        if (DailyStatsManager.Instance != null)
        {
            DailyStatsManager.Instance.StartNewDay();
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}
