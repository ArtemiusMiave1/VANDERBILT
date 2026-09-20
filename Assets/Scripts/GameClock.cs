using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    [Header("Starting Time")]
    [Range(0, 23)]
    public int startingHour = 8;

    [Range(0, 59)]
    public int startingMinute = 0;

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

    private float minuteTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentHour = startingHour;
        currentMinute = startingMinute;

        minuteTimer = 0f;
    }

    private void Update()
    {
        if (!clockRunning)
            return;

        if (realSecondsPerGameMinute <= 0f)
            return;

        minuteTimer += Time.deltaTime;

        if (minuteTimer >= realSecondsPerGameMinute)
        {
            minuteTimer -= realSecondsPerGameMinute;

            AdvanceMinute();
        }
    }

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
    }

    // --------------------------------------------------
    // TIME CONTROL
    // --------------------------------------------------

    public void StartClock()
    {
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
    }

    // --------------------------------------------------
    // SET TIME
    // --------------------------------------------------

    public void SetTime(int hour, int minute)
    {
        currentHour = Mathf.Clamp(hour, 0, 23);
        currentMinute = Mathf.Clamp(minute, 0, 59);

        minuteTimer = 0f;
    }

    // --------------------------------------------------
    // ADVANCE TIME
    // --------------------------------------------------

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
    }

    // --------------------------------------------------
    // GET TIME
    // --------------------------------------------------

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

    // --------------------------------------------------
    // FORMATTED TIME
    // --------------------------------------------------

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