using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(PlayClick);
        }

        TMP_Dropdown[] dropdowns = FindObjectsByType<TMP_Dropdown>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Dropdown dd in dropdowns)
        {
            dd.onValueChanged.AddListener(_ => PlayClick());
        }
    }

    void PlayClick()
    {
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, volume);
    }
}
