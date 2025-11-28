using UnityEngine;

public class HUDGate_SH : MonoBehaviour
{
    public enum HudCheckMode
    {
        GameObjectActiveInHierarchy, // HUD 루트 GameObject activeInHierarchy
        CanvasEnabled,               // Canvas.enabled
        CanvasGroupAlpha             // CanvasGroup.alpha >= threshold
    }

    [Header("Target HUD")]
    public GameObject hudRoot;       // HUD 루트 오브젝트
    public Canvas targetCanvas;      // CanvasEnabled 모드용
    public CanvasGroup targetCanvasGroup; // CanvasGroupAlpha 모드용

    [Header("Check Mode")]
    public HudCheckMode checkMode = HudCheckMode.GameObjectActiveInHierarchy;
    [Range(0f, 1f)] public float alphaThreshold = 0.5f;

    public bool IsHudOn
    {
        get
        {
            switch (checkMode)
            {
                case HudCheckMode.GameObjectActiveInHierarchy:
                    if (hudRoot == null) AutoBindIfNeeded();
                    return hudRoot != null && hudRoot.activeInHierarchy;

                case HudCheckMode.CanvasEnabled:
                    if (targetCanvas == null) AutoBindIfNeeded();
                    return targetCanvas != null && targetCanvas.enabled && targetCanvas.gameObject.activeInHierarchy;

                case HudCheckMode.CanvasGroupAlpha:
                    if (targetCanvasGroup == null) AutoBindIfNeeded();
                    return targetCanvasGroup != null
                           && targetCanvasGroup.gameObject.activeInHierarchy
                           && targetCanvasGroup.alpha >= alphaThreshold;
            }
            return false;
        }
    }

    void AutoBindIfNeeded()
    {
        if (hudRoot == null)
        {
            var c = GetComponentInChildren<Canvas>(true);
            if (c == null) c = FindObjectOfType<Canvas>(true);
            if (c != null) hudRoot = c.gameObject;
        }
        if (targetCanvas == null && hudRoot != null)
            targetCanvas = hudRoot.GetComponentInChildren<Canvas>(true);

        if (targetCanvasGroup == null && hudRoot != null)
            targetCanvasGroup = hudRoot.GetComponentInChildren<CanvasGroup>(true);
    }
}
