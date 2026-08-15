using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownStyler : MonoBehaviour
{
    [Header("Colors")]
    public Color backgroundColor = new Color(0.588f, 0.047f, 0.180f, 1f);
    public Color textColor = new Color(0.961f, 0.922f, 0.816f, 1f);
    public Color highlightColor = new Color(0.72f, 0.11f, 0.27f, 1f);
    public Color arrowColor = new Color(0.961f, 0.922f, 0.816f, 1f);
    public Color checkmarkColor = new Color(0.961f, 0.922f, 0.816f, 1f);

    [Header("Layout")]
    public float itemHeight = 70f;
    public float padding = 10f;
    public float cornerRadius = 20f;

    private TMP_Dropdown dropdown;
    private RectTransform templateRect;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        templateRect = dropdown.template;

        ResizeTemplateToFitItems();
        StyleDropdownColors();
        ApplyRoundedCorners();
        RemoveScrollbar();
    }

    void ResizeTemplateToFitItems()
    {
        int itemCount = dropdown.options.Count;
        float totalHeight = itemCount * itemHeight + padding * 2;

        // Cap height so the list doesn't overflow the screen
        float maxHeight = 400f;
        totalHeight = Mathf.Min(totalHeight, maxHeight);

        templateRect.sizeDelta = new Vector2(templateRect.sizeDelta.x, totalHeight);

        // Anchor below the dropdown button (pivot at top)
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.anchoredPosition = new Vector2(0f, 0f);
    }

    void StyleDropdownColors()
    {
        // Template background
        Image templateImage = templateRect.GetComponent<Image>();
        if (templateImage != null)
        {
            templateImage.enabled = true;
            templateImage.color = backgroundColor;
        }

        // Item hover/selected colors via the Toggle's ColorBlock
        Toggle itemToggle = templateRect.GetComponentInChildren<Toggle>(true);
        if (itemToggle != null)
        {
            ColorBlock cb = itemToggle.colors;
            cb.normalColor = backgroundColor;
            cb.highlightedColor = highlightColor;
            cb.pressedColor = highlightColor;
            cb.selectedColor = highlightColor;
            cb.colorMultiplier = 1f;
            itemToggle.colors = cb;

            // Item background image
            Image itemBg = itemToggle.targetGraphic as Image;
            if (itemBg != null)
                itemBg.color = backgroundColor;
        }

        // Item text color
        TMP_Text itemLabel = dropdown.itemText;
        if (itemLabel != null)
            itemLabel.color = textColor;

        // Caption (selected value) text color
        TMP_Text captionLabel = dropdown.captionText;
        if (captionLabel != null)
            captionLabel.color = textColor;

        // Arrow color
        Transform arrowTransform = transform.Find("Arrow");
        if (arrowTransform != null)
        {
            Image arrowImage = arrowTransform.GetComponent<Image>();
            if (arrowImage != null)
                arrowImage.color = arrowColor;
        }

        // Checkmark color
        Transform checkmark = templateRect.GetComponentInChildren<Toggle>(true)?.graphic?.transform;
        if (checkmark != null)
        {
            Image checkmarkImage = checkmark.GetComponent<Image>();
            if (checkmarkImage != null)
                checkmarkImage.color = checkmarkColor;
        }

        // Main dropdown button highlight colors
        ColorBlock dropdownColors = dropdown.colors;
        dropdownColors.normalColor = Color.white;
        dropdownColors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        dropdownColors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        dropdownColors.selectedColor = Color.white;
        dropdown.colors = dropdownColors;
    }

    void RemoveScrollbar()
    {
        ScrollRect scrollRect = templateRect.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.inertia = true;
        }

        // Hide the scrollbar visually but keep scroll functionality
        Scrollbar scrollbar = templateRect.GetComponentInChildren<Scrollbar>(true);
        if (scrollbar != null)
            scrollbar.gameObject.SetActive(false);
    }

    void ApplyRoundedCorners()
    {
        Sprite roundedSprite = CreateRoundedRectSprite(128, 128, cornerRadius);

        // Template background
        Image templateImage = templateRect.GetComponent<Image>();
        if (templateImage != null)
        {
            templateImage.sprite = roundedSprite;
            templateImage.type = Image.Type.Sliced;
        }

        // Viewport mask — this is what actually clips the content
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
                // Find distance to nearest corner circle center
                float dx = 0f, dy = 0f;

                if (x < radius) dx = radius - x;
                else if (x > width - radius - 1) dx = x - (width - radius - 1);

                if (y < radius) dy = radius - y;
                else if (y > height - radius - 1) dy = y - (height - radius - 1);

                if (dx > 0 && dy > 0)
                {
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    // Anti-alias the edge
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

        // 9-slice borders so the corners don't stretch
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
