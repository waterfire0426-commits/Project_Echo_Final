using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("How To Play UI")]
    public GameObject howToPanel;   // 조작법 패널

    [Header("SFX")]
    public AudioSource sfxSource;   // 효과음 재생용 AudioSource
    public AudioClip sfxOpen;       // 패널 열기 사운드
    public AudioClip sfxClose;      // 패널 닫기 사운드
    public AudioClip sfxClick;      // 버튼 클릭 사운드

    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void OnClickNewGame()
    {
        PlaySFX(sfxClick);

        Debug.Log("시작하기");

        // 다음 씬은 로비
        PlayerPrefs.SetString("NextScene", "Lobby_Scene_YR");

        // 로딩씬으로 갈 때 페이드아웃 → 씬 로드
        if (ScreenFader_SH.Instance != null)
            ScreenFader_SH.Instance.FadeOutAndLoad("Lobby_LoadingScene_YR");
        else
            SceneManager.LoadScene("Lobby_LoadingScene_YR");
    }

    public void OnClickLoad()
    {
        PlaySFX(sfxClick);
        Debug.Log("불러오기");
    }

    public void OnClickHowTo()
    {
        PlaySFX(sfxOpen);

        if (howToPanel != null)
            howToPanel.SetActive(true);
    }

    // 🎮 조작법 닫기 버튼
    public void OnClickCloseHowTo()
    {
        PlaySFX(sfxClose);

        if (howToPanel != null)
            howToPanel.SetActive(false);
    }

    public void OnClickQuit()
    {
        PlaySFX(sfxClick);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
