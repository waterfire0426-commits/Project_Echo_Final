// SceneRoute_SH.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRoute_SH : MonoBehaviour
{
    public static SceneRoute_SH Instance { get; private set; }
    public string NextSceneName { get; private set; }
    public float MinLoadingShowTime = 0.5f; // 필요 시 로딩 최소표시시간

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 어디서든 호출: "로딩씬 -> 목적지씬" 흐름 시작
    public void LoadViaLoading(string loadingSceneName, string targetSceneName, float minShowTime = 0.5f)
    {
        NextSceneName = targetSceneName;
        MinLoadingShowTime = Mathf.Max(0f, minShowTime);
        if (string.IsNullOrEmpty(loadingSceneName) || string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[SceneRoute_SH] scene names missing.");
            return;
        }
        Time.timeScale = 1f; // 혹시 멈춰있을 수 있으니 안전장치
        SceneManager.LoadScene(loadingSceneName);
    }
}
