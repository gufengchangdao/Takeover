using UnityEngine;
using UnityEngine.UI;

public class UIMatAnimationControl : MonoBehaviour
{
    public float outlineAlpha = 0;
    private float oldOutlineAlpha;
    private static int _OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");

    public Color outlineColor = Color.white;
    private Color oldOutlineColor;
    private static int _OutlineColor = Shader.PropertyToID("_OutlineColor");

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        image.material = new Material(image.material);

        oldOutlineAlpha = outlineAlpha;
        image.material.SetFloat(_OutlineAlpha, outlineAlpha);

        oldOutlineColor = outlineColor;
        image.material.SetColor(_OutlineColor, outlineColor);
    }

    void Update()
    {
        if (outlineAlpha != oldOutlineAlpha)
        {
            oldOutlineAlpha = outlineAlpha;
            image.material.SetFloat(_OutlineAlpha, outlineAlpha);
        }
        if (outlineColor != oldOutlineColor)
        {
            oldOutlineColor = outlineColor;
            image.material.SetColor(_OutlineColor, outlineColor);
        }
    }
}
