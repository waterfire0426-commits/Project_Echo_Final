using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class FacilityVideoController_SH : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;

    [Header("Control Targets")]
    public GameObject[] disableDuringVideo;
    public GameObject[] enableAfterVideo;

    [Header("Clips by Progress")]
    public VideoClip act1Clip;
    public VideoClip act2Clip;
    public VideoClip act3Clip;   // 🔥 추가된 Act3 영상
    public VideoClip endingClip;

    [Header("Events")]
    public UnityEvent onVideoStart;
    public UnityEvent onVideoEnd;

    // 🔹 외부에서 영상 재생중인지 확인 가능
    public bool IsPlaying { get; private set; } = false;

    void Awake()
    {
        if (!videoPlayer)
            videoPlayer = GetComponent<VideoPlayer>();

        if (!videoPlayer)
        {
            Debug.LogError("[FacilityVideo] VideoPlayer 없음");
            enabled = false;
            return;
        }

        videoPlayer.loopPointReached += OnVideoFinished;

        if (videoCanvas)
            videoCanvas.SetActive(false);
    }

    // ----------- 개별 영상 재생 함수 -----------
    public void PlayAct1() => PlayClip(act1Clip);
    public void PlayAct2() => PlayClip(act2Clip);
    public void PlayAct3() => PlayClip(act3Clip);     // 🔥 추가된 함수
    public void PlayEnding() => PlayClip(endingClip);

    // ----------- 공통 재생 함수 -----------
    void PlayClip(VideoClip clip)
    {
        if (!clip)
        {
            Debug.LogWarning("[FacilityVideo] 재생할 클립이 없습니다.");
            return;
        }

        if (videoCanvas)
            videoCanvas.SetActive(true);

        if (disableDuringVideo != null)
            foreach (var go in disableDuringVideo)
                if (go) go.SetActive(false);

        if (enableAfterVideo != null)
            foreach (var go in enableAfterVideo)
                if (go) go.SetActive(false);

        videoPlayer.clip = clip;
        videoPlayer.Play();

        IsPlaying = true;
        onVideoStart?.Invoke();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        IsPlaying = false;

        if (videoCanvas)
            videoCanvas.SetActive(false);

        if (disableDuringVideo != null)
            foreach (var go in disableDuringVideo)
                if (go) go.SetActive(true);

        if (enableAfterVideo != null)
            foreach (var go in enableAfterVideo)
                if (go) go.SetActive(true);

        onVideoEnd?.Invoke();
    }
}
