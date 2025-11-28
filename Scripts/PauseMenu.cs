using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;   // ESC 눌렀을 때 열리는 일시정지 메뉴
    public GameObject crosshair;    // 십자선 UI
    public GameObject hudimage;     // HUD UI

    [Header("How To Play UI")]
    public GameObject howToPanel;   // 조작법 패널 (Pause 메뉴용)

    [Header("Player References")]
    public FPCamera cameraScript;   // 인스펙터에서 플레이어 카메라 드래그
    public PlayerMove playerMove;   // 인스펙터에서 플레이어 이동 스크립트 드래그

    public PlayerHealth_Merge playerHealth; // 게임 오버 때문에 추가

    [Header("SFX")]
    public AudioSource sfxSource;   // 효과음 재생용 AudioSource
    public AudioClip sfxOpen;       // 메뉴 열기 효과음
    public AudioClip sfxClose;      // 메뉴 닫기 효과음
    public AudioClip sfxClick;      // 버튼 클릭 효과음

    private bool isPaused = false;

    void Update()
    {
        // 게임오버면 ESC 작동 금지
        if (playerHealth != null && playerHealth.IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 만약 HowTo 패널이 열려있으면 ESC로 먼저 HowTo만 닫기
            if (howToPanel != null && howToPanel.activeSelf)
            {
                howToPanel.SetActive(false);
                PlaySFX(sfxClose); // HowTo 닫기 사운드

                if (pausePanel != null)
                    pausePanel.SetActive(true);   // 일시정지 메뉴로 복귀
                return;
            }

            // 일반 ESC 토글 (일시정지 켜고 끄기)
            isPaused = !isPaused;
            if (pausePanel != null)
                pausePanel.SetActive(isPaused);

            // 게임 일시정지/재개
            Time.timeScale = isPaused ? 0f : 1f;

            // 카메라 & 이동 스크립트 비활성화
            if (cameraScript != null)
            {
                cameraScript.isPaused = isPaused;
                cameraScript.enabled = !isPaused;    // 회전 중지
            }

            if (playerMove != null)
                playerMove.enabled = !isPaused;       // 이동 중지

            // crosshair, HUD 표시/숨김
            if (crosshair != null)
                crosshair.SetActive(!isPaused);

            if (hudimage != null)
                hudimage.SetActive(!isPaused);

            // 커서 표시/고정
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

            // 메뉴 열기/닫기 효과음
            if (isPaused)
                PlaySFX(sfxOpen);
            else
                PlaySFX(sfxClose);
        }
    }

    // 계속하기 버튼 (Resume)
    public void OnClickResume()
    {
        PlaySFX(sfxClick);

        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        // HowTo가 떠 있었다면 같이 닫아두기
        if (howToPanel != null)
            howToPanel.SetActive(false);

        Time.timeScale = 1f;

        // 카메라 복구
        if (cameraScript != null)
        {
            cameraScript.isPaused = false;
            cameraScript.enabled = true;
        }

        // 이동 복구
        if (playerMove != null)
            playerMove.enabled = true;

        // UI 복구
        if (crosshair != null)
            crosshair.SetActive(true);
        if (hudimage != null)
            hudimage.SetActive(true);

        // 커서 다시 숨김 + 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // HowTo 버튼 (Pause 메뉴 안의 "조작법" 버튼에 연결)
    public void OnClickHowTo()
    {
        PlaySFX(sfxClick);

        if (howToPanel != null)
            howToPanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);   // 일시정지 메뉴는 숨기고 조작법 패널만 보여주기
    }

    // HowTo 닫기 버튼 (조작법 패널 안의 "닫기" 버튼에 연결)
    public void OnClickCloseHowTo()
    {
        PlaySFX(sfxClick);

        if (howToPanel != null)
            howToPanel.SetActive(false);

        // 아직 일시정지 상태라면 일시정지 메뉴 복귀
        if (pausePanel != null && isPaused)
            pausePanel.SetActive(true);
    }

    // 종료 버튼
    public void OnClickQuit()
    {
        PlaySFX(sfxClick);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("게임 종료");
    }

    // 공통 SFX 재생 함수
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}
