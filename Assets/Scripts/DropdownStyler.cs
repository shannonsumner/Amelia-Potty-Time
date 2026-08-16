using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DropdownTheme { Dark, Light }

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownStyler : MonoBehaviour
{
    public DropdownTheme theme = DropdownTheme.Dark;

    [Header("Layout")]
    public float itemHeight = 0f;
    public float padding = 10f;
    public float cornerRadius = 20f;
    public float maxHeight = 400f;

    private TMP_Dropdown dropdown;
    private RectTransform templateRect;

    private Color backgroundColor;
    private Color textColor;
    private Color highlightColor;
    private Color checkmarkColor;

    static readonly Color DarkBg = new Color(0.588f, 0.047f, 0.180f, 1f);
    static readonly Color CreamText = new Color(0.961f, 0.922f, 0.816f, 1f);
    static readonly Color DarkHighlight = new Color(0.72f, 0.11f, 0.27f, 1f);

    static readonly Color LightBg = new Color(0.980f, 0.937f, 0.867f, 1f);
    static readonly Color DarkText = new Color(0.588f, 0.047f, 0.180f, 1f);
    static readonly Color LightHighlight = new Color(0.765f, 0.624f, 0.612f, 1f);

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        templateRect = dropdown.template;

        ApplyThemeColors();
        DetectItemHeight();
        ResizeTemplate();
        StyleTemplate();
        ApplyRoundedCorners();
        HideScrollbar();
    }

    void ApplyThemeColors()
    {
        if (theme == DropdownTheme.Dark)
        {
            backgroundColor = DarkBg;
            textColor = CreamText;
            highlightColor = DarkHighlight;
            checkmarkColor = CreamText;
        }
        else
        {
            backgroundColor = LightBg;
            textColor = DarkText;
            highlightColor = LightHighlight;
            checkmarkColor = DarkText;
        }
    }

    void DetectItemHeight()
    {
        if (itemHeight > 0f)
            return;

        Toggle itemToggle = templateRect.GetComponentInChildren<Toggle>(true);
        if (itemToggle != null)
        {
            float detected = ((RectTransform)itemToggle.transform).sizeDelta.y;
            if (detected > 10f)
            {
                itemHeight = detected;
                return;
            }
        }

        float buttonHeight = ((RectTransform)transform).sizeDelta.y;
        itemHeight = buttonHeight > 0f ? buttonHeight : 70f;
    }

    void ResizeTemplate()
    {
        int itemCount = dropdown.options.Count;
        float totalHeight = Mathf.Min(itemCount * itemHeight + padding * 2, maxHeight);

        templateRect.sizeDelta = new Vector2(templateRect.sizeDelta.x, totalHeight);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.anchoredPosition = Vector2.zero;
    }

    void StyleTemplate()
    {
        // Template background
        Image templateImage = templateRect.GetComponent<Image>();
        if (templateImage != null)
        {
            templateImage.enabled = true;
            templateImage.color = backgroundColor;
        }

        // Item toggle colors — normalColor is white so it doesn't multiply with the bg
        Toggle itemToggle = templateRect.GetComponentInChildren<Toggle>(true);
        if (itemToggle != null)
        {
            ColorBlock cb = itemToggle.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = highlightColor;
            cb.pressedColor = highlightColor;
            cb.selectedColor = highlightColor;
            cb.colorMultiplier = 1f;
            itemToggle.colors = cb;

            Image itemBg = itemToggle.targetGraphic as Image;
            if (itemBg != null)
                itemBg.color = backgroundColor;
        }

        // Item text
        TMP_Text itemLabel = dropdown.itemText;
        if (itemLabel != null)
            itemLabel.color = textColor;

        // Checkmark
        Toggle toggle = templateRect.GetComponentInChildren<Toggle>(true);
        if (toggle != null && toggle.graphic != null)
        {
            Image checkImg = toggle.graphic as Image;
            if (checkImg != null)
                checkImg.color = checkmarkColor;
        }
    }

    void HideScrollbar()
    {
        ScrollRect scrollRect = templateRect.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;
        }

        Scrollbar scrollbar = templateRect.GetComponentInChildren<Scrollbar>(true);
        if (scrollbar != null)
            scrollbar.gameObject.SetActive(false);
    }

    void ApplyRoundedCorners()
    {
        float radius = theme == DropdownTheme.Light ? cornerRadius * 0.5f : cornerRadius;
        Sprite roundedSprite = CreateRoundedRectSprite(128, 128, radius);

        Image templateImage = templateRect.GetComponent<Image>();
        if (templateImage != null)
        {
            templateImage.sprite = roundedSprite;
            templateImage.type = Image.Type.Sliced;
        }

        // Add border for light theme
        if (theme == DropdownTheme.Light)
        {
            Outline border = templateRect.gameObject.GetComponent<Outline>();
            if (border == null)
                border = templateRect.gameObject.AddComponent<Outline>();
            border.effectColor = new Color(0.420f, 0.122f, 0.165f, 1f);
            border.effectDistance = new Vector2(2f, -2f);
        }

        Mask viewportMask = templateRect.GetComponentInChildren<Mask>(true);
        if (viewportMask != null)
        {
            Image viewportImage = viewportMask.GetComponent<Image>();
            if (viewportImage != null)
            {
                viewportImage.sprite = roundedSprite;
                viewportImage.type = Image.Type.Sliced;
            }
        }
    }

    Sprite CreateRoundedRectSprite(int width, int height, float radius)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Bilinear;

        Color clear = Color.clear;
        Color fill = Color.white;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = 0f, dy = 0f;

                if (x < radius) dx = radius - x;
                else if (x > width - radius - 1) dx = x - (width - radius - 1);

                if (y < radius) dy = radius - y;
                else if (y > height - radius - 1) dy = y - (height - radius - 1);

                if (dx > 0 && dy > 0)
                {
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > radius)
                        tex.SetPixel(x, y, clear);
                    else if (dist > radius - 1.5f)
                        tex.SetPixel(x, y, new Color(1, 1, 1, (radius - dist) / 1.5f));
                    else
                        tex.SetPixel(x, y, fill);
                }
                else
                {
                    tex.SetPixel(x, y, fill);
                }
            }
        }

        tex.Apply();

        int border = Mathf.CeilToInt(radius);
        return Sprite.Create(
            tex,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(border, border, border, border)
        );
    }
}
