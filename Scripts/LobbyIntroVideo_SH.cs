using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class LobbyIntroVideo_SH : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;      // 인스펙터에서 할당 (없으면 GetComponent)
    public GameObject videoCanvas;      // 전체 화면 영상용 캔버스 or 패널

    [Header("Game Objects Control")]
    [Tooltip("영상이 끝난 뒤에 켤 오브젝트들 (플레이어, HUD, 몬스터 스폰 등)")]
    public GameObject[] enableAfterVideo;   // 여기엔 GameplayRoot 하나만 넣어도 됨

    [Tooltip("영상 동안 꺼둘 오브젝트들 (있으면)")]
    public GameObject[] disableDuringVideo;

    [Header("Cameras")]
    [Tooltip("인트로 영상 전용 카메라 (영상 재생 중에만 ON)")]
    public Camera introCamera;

    [Tooltip("게임에서 사용할 메인 카메라 (플레이어 카메라)")]
    public Camera mainCamera;           // 선택 사항, 없어도 됨

    [Header("Events")]
    public UnityEvent onVideoStart;
    public UnityEvent onVideoEnd;

    void Start()
    {
        // VideoPlayer 자동 할당
        if (!videoPlayer)
            videoPlayer = GetComponent<VideoPlayer>();

        if (!videoPlayer)
        {
            Debug.LogError("[LobbyIntroVideo_SH] VideoPlayer가 없습니다.");
            return;
        }

        // 🔹 인트로 카메라 켜기, 메인 카메라 끄기
        if (introCamera)
            introCamera.enabled = true;

        if (mainCamera)
            mainCamera.enabled = false;   // 메인 카메라를 명시적으로 꺼두고 시작

        // 🔹 영상 캔버스 켜기
        if (videoCanvas)
            videoCanvas.SetActive(true);

        // 🔹 영상 동안 끌 것들 미리 끄기
        if (disableDuringVideo != null)
        {
            foreach (var go in disableDuringVideo)
                if (go) go.SetActive(false);
        }

        // 🔹 나중에 켤 것들 미리 꺼두기
        if (enableAfterVideo != null)
        {
            foreach (var go in enableAfterVideo)
                if (go) go.SetActive(false);
        }

        // 🔹 콜백 등록 + 재생 시작
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
        onVideoStart?.Invoke();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 🔹 영상 UI 끄기
        if (videoCanvas)
            videoCanvas.SetActive(false);

        // 🔹 인트로 카메라 끄기
        if (introCamera)
            introCamera.enabled = false;

        // 🔹 메인 카메라 켜기 (선택)
        if (mainCamera)
            mainCamera.enabled = true;

        // 🔹 게임 오브젝트들 켜기 (플레이어, HUD, 적 등)
        if (enableAfterVideo != null)
        {
            foreach (var go in enableAfterVideo)
                if (go) go.SetActive(true);
        }

        // 🔹 필요하면 다시 켜줄 것도 여기에서
        if (disableDuringVideo != null)
        {
            foreach (var go in disableDuringVideo)
                if (go) go.SetActive(true);
        }

        onVideoEnd?.Invoke();

        // 더 이상 필요 없으면 자기 자신 제거해도 됨
        // Destroy(gameObject);
    }
}
