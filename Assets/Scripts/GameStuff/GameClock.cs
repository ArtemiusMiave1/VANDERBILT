using UnityEngine.SceneManagement;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    [Header("Starting Time")]
    [Range(0, 23)]
    public int startingHour = 8;

    [Range(0, 59)]
    public int startingMinute = 0;

    [Header("End of Day")]
    [Range(0, 23)]
    public int endOfDayHour = 16;

    [Range(0, 59)]
    public int endOfDayMinute = 0;

    [Header("Time Speed")]
    [Tooltip("How many real seconds pass before 1 in-game minute passes.")]
    public float realSecondsPerGameMinute = 1f;

    [Header("Current Time")]
    [SerializeField]
    private int currentHour;

    [SerializeField]
    private int currentMinute;

    [Header("Clock State")]
    public bool clockRunning = true;

    [Tooltip("If enabled, the clock automatically starts when the game begins.")]
    public bool startAutomatically = true;

    [SerializeField]
    private bool dayEnded = false;

    private float minuteTimer = 0f;

    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        currentHour = startingHour;
        currentMinute = startingMinute;

        minuteTimer = 0f;
        dayEnded = false;

        clockRunning = startAutomatically;
        DailyStatsManager.Instance.StartNewDay();
        CheckEndOfDay();
    }

    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        // Runs independently of ship movement.
        if (!clockRunning || dayEnded)
            return;

        if (realSecondsPerGameMinute <= 0f)
            return;

        minuteTimer += Time.deltaTime;

        // Handles multiple game minutes if a frame takes longer.
        while (minuteTimer >= realSecondsPerGameMinute &&
               clockRunning &&
               !dayEnded)
        {
            minuteTimer -= realSecondsPerGameMinute;

            AdvanceMinute();
        }
    }

    // ==================================================
    // ADVANCE MINUTE
    // ==================================================

    private void AdvanceMinute()
    {
        currentMinute++;

        if (currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;

            if (currentHour >= 24)
            {
                currentHour = 0;
            }
        }

        CheckEndOfDay();
    }

    // ==================================================
    // END OF DAY
    // ==================================================

    private void CheckEndOfDay()
    {
        int currentTotalMinutes = GetTotalMinutes();

        int endTotalMinutes =
            (endOfDayHour * 60) + endOfDayMinute;

        if (currentTotalMinutes >= endTotalMinutes)
        {
            currentHour = endOfDayHour;
            currentMinute = endOfDayMinute;

            EndDay();
        }
    }

    private void EndDay()
    {
        if (dayEnded)
            return;

        dayEnded = true;
        clockRunning = false;

        Debug.Log("End of day reached: " + GetTime12Hour());

        if (DailyStatsManager.Instance != null)
        {
            DailyStatsManager.Instance.EndDay();
            print("stats");
        }

        SceneManager.LoadScene("GameEnd");
    }

    // ==================================================
    // TIME CONTROL
    // ==================================================

    public void StartClock()
    {
        if (dayEnded)
            return;

        clockRunning = true;
    }

    public void StopClock()
    {
        clockRunning = false;
    }

    public void ResetClock()
    {
        currentHour = startingHour;
        currentMinute = startingMinute;

        minuteTimer = 0f;
        dayEnded = false;
    }

    // ==================================================
    // NEW DAY
    // ==================================================

    public void StartNewDay()
    {
        ResetClock();

        clockRunning = true;

        Debug.Log("New day started: " + GetTime12Hour());
    }

    // ==================================================
    // SET TIME
    // ==================================================

    public void SetTime(int hour, int minute)
    {
        currentHour = Mathf.Clamp(hour, 0, 23);
        currentMinute = Mathf.Clamp(minute, 0, 59);

        minuteTimer = 0f;
        dayEnded = false;

        CheckEndOfDay();
    }

    // ==================================================
    // ADVANCE TIME
    // ==================================================

    public void AddMinutes(int minutes)
    {
        int totalMinutes =
            (currentHour * 60) +
            currentMinute +
            minutes;

        totalMinutes %= 1440;

        if (totalMinutes < 0)
            totalMinutes += 1440;

        currentHour =
            totalMinutes / 60;

        currentMinute =
            totalMinutes % 60;

        CheckEndOfDay();
    }

    // ==================================================
    // GET TIME
    // ==================================================

    public int GetHour()
    {
        return currentHour;
    }

    public int GetMinute()
    {
        return currentMinute;
    }

    public int GetTotalMinutes()
    {
        return (currentHour * 60) + currentMinute;
    }

    public bool HasDayEnded()
    {
        return dayEnded;
    }

    // ==================================================
    // FORMATTED TIME
    // ==================================================

    public string GetTime24Hour()
    {
        return string.Format(
            "{0:00}:{1:00}",
            currentHour,
            currentMinute
        );
    }

    public string GetTime12Hour()
    {
        int displayHour = currentHour % 12;

        if (displayHour == 0)
            displayHour = 12;

        string period =
            currentHour >= 12 ? "PM" : "AM";

        return string.Format(
            "{0}:{1:00} {2}",
            displayHour,
            currentMinute,
            period
        );
    }
}
