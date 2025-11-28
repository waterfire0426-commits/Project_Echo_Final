using System;
using System.Collections;
using UnityEngine;

public class HazmatWearFX : MonoBehaviour
{
    [Header("Fade Overlay")]
    public CanvasGroup fadeGroup;     // HazmatFX_Canvas의 CanvasGroup

    [Header("Timings (seconds)")]
    public float fadeIn = 0.25f;      // 검정으로 페이드 인
    public float hold = 0.10f;        // 검정 유지(이때 HUD 교체)
    public float fadeOut = 0.35f;     // 페이드 아웃

    void Awake()
    {
        if (fadeGroup)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 블랙→(중간 호출)→복귀. onMidpoint에서 HUD 토글을 수행.
    /// </summary>
    public void Play(Action onMidpoint)
    {
        StopAllCoroutines();
        StartCoroutine(CoPlay(onMidpoint));
    }

    IEnumerator CoPlay(Action onMidpoint)
    {
        if (fadeGroup) fadeGroup.gameObject.SetActive(true);

        // 1) Fade to black
        yield return FadeAlpha(fadeGroup, 0f, 1f, fadeIn);

        // 2) Hold & swap
        yield return new WaitForSecondsRealtime(hold);
        onMidpoint?.Invoke();

        // 3) Fade back
        yield return FadeAlpha(fadeGroup, 1f, 0f, fadeOut);
        if (fadeGroup) fadeGroup.gameObject.SetActive(false);
    }

    IEnumerator FadeAlpha(CanvasGroup cg, float from, float to, float dur)
    {
        if (!cg || dur <= 0f) { if (cg) cg.alpha = to; yield break; }
        float t = 0f; cg.alpha = from;
        // Time.timeScale=0에서도 작동하도록 unscaledDeltaTime 사용
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / dur;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        cg.alpha = to;
    }
}
