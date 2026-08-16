using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ScheduleController : MonoBehaviour
{
    public Transform inputsContainer;
    public Button goButton;
    public Sprite disabledButtonSprite;

    private const int MIN_HOUR = 6;   // 6 AM
    private const int MAX_HOUR = 22;  // 10 PM

    private struct TripRow
    {
        public GameObject root;
        public TMP_Dropdown hours;
        public TMP_Dropdown minutes;
        public TMP_Dropdown meridiem;
    }

    private List<TripRow> tripRows = new List<TripRow>();

    void Start()
    {
        if (disabledButtonSprite != null)
        {
            goButton.transition = Selectable.Transition.SpriteSwap;
            SpriteState ss = goButton.spriteState;
            ss.disabledSprite = disabledButtonSprite;
            goButton.spriteState = ss;
        }
        goButton.interactable = false;
    }

    public void InitializeSchedule(int tripCount)
    {
        tripRows.Clear();

        for (int i = 0; i < inputsContainer.childCount; i++)
        {
            Transform child = inputsContainer.GetChild(i);
            if (!child.gameObject.activeSelf)
                continue;

            TripRow row;
            row.root = child.gameObject;
            row.hours = child.Find("Hours")?.GetComponent<TMP_Dropdown>();
            row.minutes = child.Find("Minutes")?.GetComponent<TMP_Dropdown>();
            row.meridiem = child.Find("Meridiem")?.GetComponent<TMP_Dropdown>();

            if (row.hours == null || row.minutes == null || row.meridiem == null)
                continue;

            int index = tripRows.Count;
            row.hours.onValueChanged.AddListener(_ => OnTimeChanged());
            row.minutes.onValueChanged.AddListener(_ => OnTimeChanged());
            row.meridiem.onValueChanged.AddListener(_ => OnTimeChanged());

            tripRows.Add(row);
        }

        // Set Trip 1 to 6 AM
        if (tripRows.Count > 0)
        {
            SetTripTime(tripRows[0], MIN_HOUR);
        }

        OnTimeChanged();
    }

    void SetTripTime(TripRow row, int hour24)
    {
        bool isPM = hour24 >= 12;
        int hour12 = hour24 % 12;
        if (hour12 == 0) hour12 = 12;

        SetDropdownByText(row.hours, hour12.ToString("D2"));
        SetDropdownByText(row.minutes, ":00");
        row.meridiem.value = isPM ? 1 : 0;
    }

    void SetDropdownByText(TMP_Dropdown dropdown, string text)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text.Trim() == text.Trim())
            {
                dropdown.value = i;
                return;
            }
        }
    }

    void OnTimeChanged()
    {
        bool valid = ValidateSchedule();
        goButton.interactable = valid;
    }

    bool ValidateSchedule()
    {
        int previousMinutes = -1;

        for (int i = 0; i < tripRows.Count; i++)
        {
            int totalMinutes = GetTotalMinutes(tripRows[i]);

            if (totalMinutes < MIN_HOUR * 60 || totalMinutes > MAX_HOUR * 60)
                return false;

            if (totalMinutes <= previousMinutes)
                return false;

            previousMinutes = totalMinutes;
        }

        return true;
    }

    int GetTotalMinutes(TripRow row)
    {
        string hourText = row.hours.options[row.hours.value].text.Trim();
        string minText = row.minutes.options[row.minutes.value].text.Trim().TrimStart(':');
        string meridiemText = row.meridiem.options[row.meridiem.value].text.Trim();

        int hour = int.Parse(hourText);
        int min = int.Parse(minText);

        // Convert 12-hour to 24-hour
        if (meridiemText == "AM")
        {
            if (hour == 12) hour = 0;
        }
        else
        {
            if (hour != 12) hour += 12;
        }

        return hour * 60 + min;
    }

    public List<int> GetScheduledTimesInMinutes()
    {
        List<int> times = new List<int>();
        for (int i = 0; i < tripRows.Count; i++)
            times.Add(GetTotalMinutes(tripRows[i]));
        return times;
    }
}
