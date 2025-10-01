using System.Threading.Tasks;
using UnityEngine;

public static class ScreenEffectUtils
{
    /// <summary>
    /// Fades a CanvasGroup to the target alpha over the given duration.
    /// </summary>
    public static async Task FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        if (cg == null) return;

        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            await Task.Yield();
        }

        cg.alpha = targetAlpha;
    }

    /// <summary>
    /// Fades in (to alpha 0) over duration.
    /// </summary>
    public static Task FadeIn(CanvasGroup cg, float duration)
    {
        if (cg == null) return Task.CompletedTask;
        if (cg.alpha == 0f) return Task.CompletedTask; // already invisible
        
        return FadeCanvasGroup(cg, 0f, duration);
    }

    /// <summary>
    /// Fades out (to alpha 1) over duration.
    /// </summary>
    public static Task FadeOut(CanvasGroup cg, float duration)
    {
        if (cg == null) return Task.CompletedTask;
        if (cg.alpha == 1f) return Task.CompletedTask; // already visible
        return FadeCanvasGroup(cg, 1f, duration);
    }

    /// <summary>
    /// Anchors a RectTransform (UI element) to a world position.
    /// Returns the anchored position (in local space of the parent).
    /// </summary>
    public static Vector2 AnchorToWorldPosition(
        RectTransform uiElement,
        Vector3 worldPos,
        Canvas canvas,
        Camera worldCamera)
    {
        if (uiElement == null || canvas == null || worldCamera == null)
        {
            Debug.LogWarning("AnchorToWorldPosition: Missing references.");
            return Vector2.zero;
        }

        // For Overlay canvases, the camera is ignored
        Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : worldCamera;

        // Convert world position to local point inside the parent rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiElement.parent as RectTransform,
            worldCamera.WorldToScreenPoint(worldPos),
            canvasCamera,
            out Vector2 localPos
        );

        uiElement.anchoredPosition = localPos;
        return localPos;
    }
}
