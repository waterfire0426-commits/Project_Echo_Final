using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalEndingTrigger_YH : MonoBehaviour
{
    public FacilityVideoController_SH videoController;
    public bool playOnce = true;
    private bool played = false;

    [Header("영상 재생 시 비활성화할 오브젝트들")]
    public GameObject[] objectsToDisable;

    private void OnTriggerEnter(Collider other)
    {
        TryTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryTrigger(other);
    }

    private void TryTrigger(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playOnce && played) return;

        if (!GameState.EndUnlocked)
        {
            Debug.Log("[FinalEndingTrigger] 아직 엔딩 조건(EndUnlocked)이 충족되지 않음");
            return;
        }

        played = true;

        if (!videoController)
            videoController = FindObjectOfType<FacilityVideoController_SH>();

        if (videoController != null)
        {
            Debug.Log("[FinalEndingTrigger] 엔딩 영상 재생!");

            // 🎬 엔딩 영상 시작
            videoController.PlayEnding();

            // 🔥 영상 재생과 동시에 지정된 오브젝트 비활성화
            DisableObjects();

            // 🎯 영상 끝나면 메인메뉴로 이동
            videoController.onVideoEnd.AddListener(OnEndingVideoFinished);
        }
        else
        {
            Debug.LogWarning("[FinalEndingTrigger] FacilityVideoController_SH를 찾지 못함");
        }
    }

    private void DisableObjects()
    {
        if (objectsToDisable == null) return;

        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"[FinalEndingTrigger] {obj.name} 비활성화됨");
            }
        }
    }

    private void OnEndingVideoFinished()
    {
        Debug.Log("[FinalEndingTrigger] 엔딩 영상 종료 → 메인메뉴로 돌아갑니다.");

        if (videoController != null)
            videoController.onVideoEnd.RemoveListener(OnEndingVideoFinished);

        SceneManager.LoadScene("MainMenuScene");
    }
}
