// ScreenFader_SH.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader_SH : MonoBehaviour
{
    public static ScreenFader_SH Instance { get; private set; }

    [Header("Refs")]
    public Canvas rootCanvas;      // Overlay
    public Image blackImage;      // 화면 전체를 덮는 검정 이미지

    [Header("Defaults")]
    public float defaultFadeIn = 0.35f;
    public float defaultFadeOut = 0.35f;

    // 중복 전환/중복 클릭 방지 용
    bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!rootCanvas) rootCanvas = GetComponentInChildren<Canvas>(true);
        if (!blackImage) blackImage = GetComponentInChildren<Image>(true);

        // 항상 최상단
        if (rootCanvas)
        {
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rootCanvas.sortingOrder = 32767;
        }

        // 기본적으로 클릭을 가로막지 않음 (버튼 눌리게)
        if (blackImage) blackImage.raycastTarget = false;

        // 첫 씬 진입 시 화면은 일단 검정 → 바로 페이드인 시작해 버튼 가로막힘 방지
        SetBlack(1f);
        StartCoroutine(Fade(1f, 0f, defaultFadeIn));

        // 이후 씬 로드될 때마다 자동 페이드인
        SceneManager.sceneLoaded += OnSceneLoaded_AutoFadeIn;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded_AutoFadeIn;
    }

    void OnSceneLoaded_AutoFadeIn(Scene s, LoadSceneMode m)
    {
        // 새 씬에서는 입력을 막지 않도록 보장
        if (blackImage) blackImage.raycastTarget = false;
        isTransitioning = false;
        FadeIn();
    }

    public void SetBlack(float a)
    {
        if (!blackImage) return;
        var c = blackImage.color;
        c.a = Mathf.Clamp01(a);
        blackImage.color = c;
    }

    public Coroutine FadeIn(float duration = -1f)
        => StartCoroutine(Fade(1f, 0f, duration < 0 ? defaultFadeIn : duration));

    public Coroutine FadeOut(float duration = -1f)
        => StartCoroutine(Fade(0f, 1f, duration < 0 ? defaultFadeOut : duration));

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        SetBlack(from);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            SetBlack(a);
            yield return null;
        }
        SetBlack(to);
    }

    // 페이드아웃 → 씬 로드 (로딩 전에 어두워짐을 확실히 보여줌)
    public void FadeOutAndLoad(string sceneName, float fadeOut = -1f)
    {
        if (isTransitioning) return; // 중복 호출 방지
        isTransitioning = true;
        StartCoroutine(FadeOutAndLoadRoutine(sceneName, fadeOut < 0 ? defaultFadeOut : fadeOut));
    }

    IEnumerator FadeOutAndLoadRoutine(string sceneName, float fadeOut)
    {
        // 페이드아웃 동안만 클릭 막기(중복 클릭/전환 방지)
        if (blackImage) blackImage.raycastTarget = true;

        yield return Fade(0f, 1f, fadeOut);  // 어두워짐
        SceneManager.LoadScene(sceneName);   // 새 씬 로드 → OnSceneLoaded에서 자동 FadeIn 및 raycastTarget 해제
    }
}
