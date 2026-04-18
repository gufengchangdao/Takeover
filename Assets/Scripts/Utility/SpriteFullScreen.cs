using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFullScreen : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private SpriteRenderer spriteRenderer;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private float lastOrthographicSize;
    private Sprite lastSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        RefreshScale(true);
    }

    private void LateUpdate()
    {
        RefreshScale(false);
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        RefreshScale(true);
    }

    private void RefreshScale(bool force)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return;
            }
        }

        if (!targetCamera.orthographic)
        {
            return;
        }

        var sprite = spriteRenderer.sprite;
        if (sprite == null)
        {
            return;
        }

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var orthographicSize = targetCamera.orthographicSize;

        if (!force &&
            screenWidth == lastScreenWidth &&
            screenHeight == lastScreenHeight &&
            Mathf.Approximately(orthographicSize, lastOrthographicSize) &&
            sprite == lastSprite)
        {
            return;
        }

        var spriteExtents = sprite.bounds.extents;
        if (spriteExtents.x <= 0f || spriteExtents.y <= 0f || screenHeight <= 0)
        {
            return;
        }

        var verticalHalfSize = orthographicSize;
        var horizontalHalfSize = verticalHalfSize * screenWidth / screenHeight;

        transform.localScale = new Vector3(
            horizontalHalfSize / spriteExtents.x,
            verticalHalfSize / spriteExtents.y,
            1f);

        lastScreenWidth = screenWidth;
        lastScreenHeight = screenHeight;
        lastOrthographicSize = orthographicSize;
        lastSprite = sprite;
    }
}
