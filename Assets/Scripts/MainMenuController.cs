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
    public TMP_Dropdown pottyTripsDropdown;
    public GameObject mainMenuPanel;
    public GameObject storyPanel;
    public GameObject creditsPanel;
    public GameObject setupPanel;
    public float fadeDuration = 0.4f;

    private CanvasGroup menuCanvasGroup;
    private CanvasGroup storyCanvasGroup;
    private CanvasGroup creditsCanvasGroup;
    private CanvasGroup setupCanvasGroup;

    void Start()
    {
        menuCanvasGroup = GetOrAddCanvasGroup(mainMenuPanel);
        storyCanvasGroup = GetOrAddCanvasGroup(storyPanel);
        creditsCanvasGroup = GetOrAddCanvasGroup(creditsPanel);
        setupCanvasGroup = GetOrAddCanvasGroup(setupPanel);

        storyPanel.SetActive(false);
        creditsPanel.SetActive(false);
        setupPanel.SetActive(false);

        newGameButton.onClick.AddListener(OnNewGame);
        backToMenuButton.onClick.AddListener(OnBackToMenu);
        creditsButton.onClick.AddListener(OnCredits);
        backFromCreditsButton.onClick.AddListener(OnBackFromCredits);
        playButton.onClick.AddListener(OnPlay);

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
