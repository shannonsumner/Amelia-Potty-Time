using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public Button newGameButton;
    public Button backToMenuButton;
    public Button creditsButton;
    public Button backFromCreditsButton;
    public Button playButton;
    public Button nextButton;
    public Button backFromScheduleButton;
    public TMP_Dropdown pottyTripsDropdown;
    public GameObject mainMenuPanel;
    public GameObject storyPanel;
    public GameObject creditsPanel;
    public GameObject setupPanel;
    public GameObject schedulePanel;
    public TMP_Text scheduleHeading;
    public Transform inputsContainer;
    public ScheduleController scheduleController;
    public GameObject resultsPanel;
    public ResultsController resultsController;
    public Button victoryMenuButton;
    public Button defeatMenuButton;
    public Button tooMuchMenuButton;
    public float fadeDuration = 0.4f;

    private CanvasGroup menuCanvasGroup;
    private CanvasGroup storyCanvasGroup;
    private CanvasGroup creditsCanvasGroup;
    private CanvasGroup setupCanvasGroup;
    private CanvasGroup scheduleCanvasGroup;
    private CanvasGroup resultsCanvasGroup;

    void Start()
    {
        menuCanvasGroup = GetOrAddCanvasGroup(mainMenuPanel);
        storyCanvasGroup = GetOrAddCanvasGroup(storyPanel);
        creditsCanvasGroup = GetOrAddCanvasGroup(creditsPanel);
        setupCanvasGroup = GetOrAddCanvasGroup(setupPanel);
        scheduleCanvasGroup = GetOrAddCanvasGroup(schedulePanel);
        resultsCanvasGroup = GetOrAddCanvasGroup(resultsPanel);

        storyPanel.SetActive(false);
        creditsPanel.SetActive(false);
        setupPanel.SetActive(false);
        schedulePanel.SetActive(false);
        resultsPanel.SetActive(false);

        newGameButton.onClick.AddListener(OnNewGame);
        backToMenuButton.onClick.AddListener(OnBackToMenu);
        creditsButton.onClick.AddListener(OnCredits);
        backFromCreditsButton.onClick.AddListener(OnBackFromCredits);
        playButton.onClick.AddListener(OnPlay);
        nextButton.onClick.AddListener(OnNext);
        backFromScheduleButton.onClick.AddListener(OnBackFromSchedule);
        scheduleController.goButton.onClick.AddListener(OnSet);
        victoryMenuButton.onClick.AddListener(OnPopupBackToMenu);
        defeatMenuButton.onClick.AddListener(OnPopupBackToMenu);
        tooMuchMenuButton.onClick.AddListener(OnPopupBackToMenu);

        nextButton.interactable = false;
        pottyTripsDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        nextButton.interactable = index > 0;
    }

    void OnNewGame()
    {
        StartCoroutine(TransitionPanels(mainMenuPanel, menuCanvasGroup, storyPanel, storyCanvasGroup));
    }

    void OnBackToMenu()
    {
        StartCoroutine(TransitionPanels(storyPanel, storyCanvasGroup, mainMenuPanel, menuCanvasGroup));
    }

    void OnCredits()
    {
        StartCoroutine(TransitionPanels(mainMenuPanel, menuCanvasGroup, creditsPanel, creditsCanvasGroup));
    }

    void OnBackFromCredits()
    {
        StartCoroutine(TransitionPanels(creditsPanel, creditsCanvasGroup, mainMenuPanel, menuCanvasGroup));
    }

    void OnPlay()
    {
        StartCoroutine(TransitionPanels(storyPanel, storyCanvasGroup, setupPanel, setupCanvasGroup));
    }

    void OnNext()
    {
        int tripCount = pottyTripsDropdown.value;
        string tripText = pottyTripsDropdown.options[tripCount].text;

        scheduleHeading.text = $"Amelia's counting on you!\nSet the times for all {tripText} potty trips.";

        // First child is Labels header, trip rows start at index 1
        for (int i = 1; i < inputsContainer.childCount; i++)
            inputsContainer.GetChild(i).gameObject.SetActive(i - 1 < tripCount);

        scheduleController.InitializeSchedule(tripCount);
        StartCoroutine(TransitionPanels(setupPanel, setupCanvasGroup, schedulePanel, scheduleCanvasGroup));
    }

    void OnBackFromSchedule()
    {
        StartCoroutine(TransitionPanels(schedulePanel, scheduleCanvasGroup, setupPanel, setupCanvasGroup));
    }

    void OnSet()
    {
        var times = scheduleController.GetScheduledTimesInMinutes();
        resultsController.StartResults(times);
        StartCoroutine(TransitionPanels(schedulePanel, scheduleCanvasGroup, resultsPanel, resultsCanvasGroup));
    }

    void OnPopupBackToMenu()
    {
        resultsController.popup.SetActive(false);
        StartCoroutine(TransitionPanels(resultsPanel, resultsCanvasGroup, mainMenuPanel, menuCanvasGroup));
    }

    IEnumerator TransitionPanels(GameObject fromObj, CanvasGroup fromCG, GameObject toObj, CanvasGroup toCG)
    {
        toCG.alpha = 0f;
        toObj.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            fromCG.alpha = 1f - t;
            toCG.alpha = t;
            yield return null;
        }

        fromCG.alpha = 0f;
        toCG.alpha = 1f;
        fromObj.SetActive(false);
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }
}
