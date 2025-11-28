using UnityEngine;

public class WarningFlashLight_SH : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;          // ±Ù∫˝¿œ ∂Û¿Ã∆Æ
    public Color flashColor = Color.red;
    public float flashSpeed = 4f;      // ±Ù∫˝¿” º”µµ
    public float minIntensity = 0f;    // √÷º“ π‡±‚
    public float maxIntensity = 2f;    // √÷¥Î π‡±‚

    private Color originalColor;
    private float originalIntensity;
    private bool isFlashing = false;

    void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        originalColor = targetLight.color;
        originalIntensity = targetLight.intensity;
    }

    void Update()
    {
        if (!isFlashing) return;

        // ªÁ¿Œ∆ƒ∑Œ ∫ŒµÂ∑ØøÓ ±Ù∫˝¿”
        float t = (Mathf.Sin(Time.time * flashSpeed) + 1f) * 0.5f;
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        targetLight.color = flashColor;
    }

    public void StartFlashing()
    {
        isFlashing = true;
    }

    public void StopFlashing()
    {
        isFlashing = false;
        targetLight.intensity = originalIntensity;
        targetLight.color = originalColor;
    }
}
