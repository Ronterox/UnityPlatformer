using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Colors")]
    public Color normalColor = Color.white;
    public Color firingColor = Color.red;
    public float colorTransitionSpeed = 10f;

    [Header("References")]
    public Image crosshairImage;

    private Color _currentColor;
    private Canvas _canvas;

    private void Awake()
    {
        if (crosshairImage == null)
        {
            CreateCrosshairUI();
        }
    }

    private void Start()
    {
        _currentColor = normalColor;
        if (crosshairImage != null)
        {
            crosshairImage.color = _currentColor;
        }
    }

    private void CreateCrosshairUI()
    {
        GameObject canvasObj = new GameObject("CrosshairCanvas");
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(canvasObj.transform);
        crosshairImage = crosshairObj.AddComponent<Image>();
        crosshairImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
        crosshairImage.rectTransform.anchoredPosition = Vector2.zero;

        RectTransform rect = crosshairObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        Texture2D crosshairTex = new Texture2D(2, 2);
        Color[] colors = new Color[4];
        for (int i = 0; i < 4; i++) colors[i] = Color.white;
        crosshairTex.SetPixels(colors);
        crosshairTex.Apply();
        crosshairImage.sprite = Sprite.Create(crosshairTex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        crosshairImage.color = normalColor;
    }

    private void Update()
    {
        StarterAssets.StarterAssetsInputs input = FindFirstObjectByType<StarterAssets.StarterAssetsInputs>();
        bool isFiring = input != null && input.attack;

        Color targetColor = isFiring ? firingColor : normalColor;
        _currentColor = Color.Lerp(_currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);

        if (crosshairImage != null)
        {
            crosshairImage.color = _currentColor;
        }
    }
}
