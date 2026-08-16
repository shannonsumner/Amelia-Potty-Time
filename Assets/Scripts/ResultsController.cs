using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ResultsController : MonoBehaviour
{
    [Header("Clock")]
    public RectTransform hourHand;
    public RectTransform minuteHand;

    [Header("Times Display")]
    public Transform timesContainer;
    public GameObject timeEntryPrefab;

    [Header("Colors")]
    public Color defaultColor = new Color(0.588f, 0.047f, 0.180f, 1f);
    public Color greenColor = new Color(0.2f, 0.7f, 0.2f, 1f);
    public Color redColor = new Color(0.85f, 0.15f, 0.15f, 1f);

    // Correct schedule in minutes from midnight
    private static readonly int[] correctTimesMinutes = {
        360,  // 06:00
        570,  // 09:30
        810,  // 13:30
        1050, // 17:30
        1230  // 20:30
    };

    private struct TimelineEntry
    {
        public int minutes;
        public string source; // "input", "correct", "both"
        public TMP_Text textObject;
    }

    private List<TimelineEntry> timeline = new List<TimelineEntry>();
    private Coroutine clockRoutine;

    public void StartResults(List<int> playerTimes)
    {
        BuildTimeline(playerTimes);
        PopulateTimesDisplay();
        if (clockRoutine != null)
            StopCoroutine(clockRoutine);
        clockRoutine = StartCoroutine(RunClock());
    }

    void BuildTimeline(List<int> playerTimes)
    {
        timeline.Clear();
        Dictionary<int, string> map = new Dictionary<int, string>();

        foreach (int t in correctTimesMinutes)
            map[t] = "correct";

        foreach (int t in playerTimes)
        {
            if (map.ContainsKey(t))
                map[t] = "both";
            else
                map[t] = "input";
        }

        foreach (var kvp in map)
        {
            TimelineEntry entry;
            entry.minutes = kvp.Key;
            entry.source = kvp.Value;
            entry.textObject = null;
            timeline.Add(entry);
        }

        timeline.Sort((a, b) => a.minutes.CompareTo(b.minutes));
    }

    void PopulateTimesDisplay()
    {
        // Clear existing entries
        for (int i = timesContainer.childCount - 1; i >= 0; i--)
            Destroy(timesContainer.GetChild(i).gameObject);

        for (int i = 0; i < timeline.Count; i++)
        {
            GameObject obj = Instantiate(timeEntryPrefab, timesContainer);
            obj.SetActive(false);
            TMP_Text txt = obj.GetComponent<TMP_Text>();
            txt.text = FormatTime(timeline[i].minutes);
            txt.color = defaultColor;

            TimelineEntry entry = timeline[i];
            entry.textObject = txt;
            timeline[i] = entry;
        }
    }

    IEnumerator RunClock()
    {
        float currentMinutes = 360f; // 6:00 AM
        int nextEntryIndex = 0;
        float stopAt = 22 * 60f; // default: 10 PM

        // Find if there's a missed time — stop 30 min later and show that time as red
        int missedIndex = -1;
        for (int i = 0; i < timeline.Count; i++)
        {
            if (timeline[i].source == "correct")
            {
                missedIndex = i;
                stopAt = timeline[i].minutes + 30f;

                // Replace the missed time text with +30 min
                TimelineEntry entry = timeline[i];
                entry.minutes = timeline[i].minutes + 30;
                if (entry.textObject != null)
                    entry.textObject.text = FormatTime(entry.minutes);
                timeline[i] = entry;
                break;
            }
        }

        SetClockRotation(currentMinutes);

        // 60 minutes per second of real time
        float minutesPerSecond = 60f;

        while (currentMinutes < stopAt)
        {
            // Check if we've passed any timeline entries
            while (nextEntryIndex < timeline.Count && timeline[nextEntryIndex].minutes <= currentMinutes)
            {
                TimelineEntry entry = timeline[nextEntryIndex];
                entry.textObject.gameObject.SetActive(true);
                if (entry.source == "correct")
                {
                    entry.textObject.color = redColor;
                }
                else
                {
                    entry.textObject.color = greenColor;
                }
                nextEntryIndex++;
            }

            currentMinutes += minutesPerSecond * Time.deltaTime;
            currentMinutes = Mathf.Min(currentMinutes, stopAt);

            SetClockRotation(currentMinutes);
            yield return null;
        }

        // Process any remaining entries at stop time
        while (nextEntryIndex < timeline.Count && timeline[nextEntryIndex].minutes <= stopAt)
        {
            TimelineEntry entry = timeline[nextEntryIndex];
            entry.textObject.gameObject.SetActive(true);
            if (entry.source == "correct")
                entry.textObject.color = redColor;
            else
                entry.textObject.color = greenColor;
            nextEntryIndex++;
        }

        // TODO: handle too_many_inputs case and show win/lose UI
    }

    void SetClockRotation(float totalMinutes)
    {
        float minutes = totalMinutes % 60f;
        float hours = totalMinutes / 60f;

        // Minute hand: full rotation per 60 minutes
        float minuteAngle = -(minutes / 60f) * 360f;
        minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle);

        // Hour hand: full rotation per 12 hours
        float hourAngle = -((hours % 12f) / 12f) * 360f;
        hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
    }

    string FormatTime(int totalMinutes)
    {
        int hour24 = totalMinutes / 60;
        int min = totalMinutes % 60;
        string meridiem = hour24 >= 12 ? "PM" : "AM";
        int hour12 = hour24 % 12;
        if (hour12 == 0) hour12 = 12;
        return $"{hour12:D2}:{min:D2} {meridiem}";
    }
}
