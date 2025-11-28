using UnityEngine;

public class Act3Trigger : MonoBehaviour
{
    [Header("References")]
    public Contamination contamination;       // Player의 Contamination 스크립트
    public GameObject mainCreaturePrefab;     // 메인 괴물 프리팹
    public Transform monsterSpawnPoint;       // 메인 괴물 스폰 위치
    public Transform playerTransform;         // 플레이어 Transform

    [Header("Path Control")]
    public PathSwitcher pathSwitcher;         // 길 막기 / 열기 제어 스크립트

    private bool triggered = false;

    [Header("누리 대사")]
    public DialogueData nuri_act3;  // Act3 회상 영상 이후 누리 대사

    [Header("Spawn Event")]
    public GameObject[] activateOnMonsterSpawn;    // 메인 괴물 등장 시 활성화할 오브젝트들


    public void OnDownloadComplete()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log("[Act3Trigger] 다운로드 완료! Act3 시작");

        // 1) 길 스위치
        if (pathSwitcher != null)
        {
            Debug.Log("[Act3Trigger] PathSwitcher 호출");
            pathSwitcher.SwitchPaths();
        }
        else
        {
            Debug.LogWarning("[Act3Trigger] pathSwitcher 연결 안 되어 있음!");
        }

        // 2) 오염 4단계 진입
        if (contamination != null)
        {
            contamination.RaiseToStage(4);
            Debug.Log("[Act3Trigger] 영구 오염 4단계 진입");
        }

        // 3) Act3 회상 영상 재생 시도
        var video = FindObjectOfType<FacilityVideoController_SH>();
        if (video != null)
        {
            Debug.Log("[Act3Trigger] Act3 영상 재생 요청");
            video.PlayAct3();

            // 영상이 끝났을 때 호출될 콜백 등록
            video.onVideoEnd.AddListener(OnAct3VideoEnd);
        }
        else
        {
            Debug.LogWarning("[Act3Trigger] FacilityVideoController_SH를 찾지 못했습니다. 영상 없이 바로 진행합니다.");
            // 영상 시스템이 없으면 바로 후속 처리
            AfterAct3Cinematic();
        }
    }

    /// <summary>
    /// FacilityVideoController_SH.onVideoEnd에 붙는 콜백
    /// </summary>
    void OnAct3VideoEnd()
    {
        Debug.Log("[Act3Trigger] Act3 영상 종료 콜백 도착");

        // 더 이상 중복 호출되지 않도록 리스너 제거
        var video = FindObjectOfType<FacilityVideoController_SH>();
        if (video != null)
            video.onVideoEnd.RemoveListener(OnAct3VideoEnd);

        // 영상 이후 처리
        AfterAct3Cinematic();
    }

    /// <summary>
    /// Act3 회상 영상이 끝난 뒤에 할 일:
    /// 1) 누리 대사 재생
    /// 2) 메인 괴물 스폰
    /// </summary>
    void AfterAct3Cinematic()
    {
        // 1) 누리 대사
        var dm = FindObjectOfType<DialogueManager>();
        if (dm != null && nuri_act3 != null)
        {
            dm.PlayDialogue(nuri_act3);
            Debug.Log("[Act3Trigger] nuri_act3 대사 재생");
        }
        else
        {
            Debug.LogWarning("[Act3Trigger] DialogueManager 또는 nuri_act3가 없습니다.");
        }

        // 2) 메인 괴물 스폰
        SpawnMainMonster();
    }

    private void SpawnMainMonster()
    {
        if (mainCreaturePrefab == null)
        {
            Debug.LogWarning("[Act3Trigger] mainCreaturePrefab 비어 있음!");
            return;
        }

        Vector3 pos = monsterSpawnPoint ? monsterSpawnPoint.position : transform.position;
        Quaternion rot = monsterSpawnPoint ? monsterSpawnPoint.rotation : Quaternion.identity;

        GameObject monster = Instantiate(mainCreaturePrefab, pos, rot);
        Debug.Log("[Act3Trigger] 메인 괴물 스폰됨!");

        // 🔥 추가한 부분: 오브젝트 활성화
        if (activateOnMonsterSpawn != null)
        {
            foreach (var obj in activateOnMonsterSpawn)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"[Act3Trigger] 활성화됨: {obj.name}");
                }
            }
        }

        // 괴물 AI 초기화
        MainMonsterChase chase = monster.GetComponent<MainMonsterChase>();
        if (chase != null)
        {
            chase.Init(playerTransform);
            Debug.Log("[Act3Trigger] 메인 괴물 추격 시작!");
        }
        else
        {
            Debug.LogWarning("[Act3Trigger] 스폰된 괴물에 MainMonsterChase 없음!");
        }
    }

}
