using UnityEngine;

public class Act2Trigger : MonoBehaviour
{
    [Header("퍼즐 완료 여부")]
    public bool puzzle1Done;
    public bool puzzle2Done;
    public bool puzzle3Done;

    [Header("연결")]
    public DialogueData puzzleClearDialogue;     // 퍼즐 전부 클리어 시 나오는 누리 대사
    public DialogueData nuri_act2;              // Act2 회상 영상 후 누리 대사
    public WaterBlockController waterBlock;     //  물 배수 컨트롤러

    // 각 퍼즐이 한 번만 카운트되도록 내부 플래그
    private bool _p1Counted = false;
    private bool _p2Counted = false;
    private bool _p3Counted = false;

    public Contamination contamination;       // Player의 Contamination 스크립트

    void Update()
    {
        // 모든 퍼즐이 완료되면 회상 트리거 실행
        if (puzzle1Done && puzzle2Done && puzzle3Done)
        {
            // 1) 퍼즐 클리어 직후 누리 대사 (퍼즐 요약/언급)
            var dm = FindObjectOfType<DialogueManager>();
            if (dm != null && puzzleClearDialogue != null)
            {
                dm.PlayDialogue(puzzleClearDialogue);
            }

            // 2) Act2 시작 트리거
            QuestManager.Notify(TRG.ACT2_START);

            // 3) 오염 3단계 진입
            if (contamination != null)
            {
                contamination.RaiseToStage(3);
                Debug.Log("[Act2Trigger] 영구 오염 3단계 진입");
            }

            Debug.Log("⚡ 모든 장치가 작동! 2액트 회상(Act2 영상) 트리거 발동!");

            // 4) Act2 회상 영상 재생
            var video = FindObjectOfType<FacilityVideoController_SH>();
            if (video != null)
            {
                // Act2 영상 재생
                video.PlayAct2();

                // 영상이 끝났을 때 호출될 콜백 등록
                // (나중에 RemoveListener로 지워줄 것)
                video.onVideoEnd.AddListener(OnAct2VideoEnd);
            }
            else
            {
                Debug.LogWarning("[Act2Trigger] FacilityVideoController_SH를 찾지 못했습니다. Act2 영상을 재생할 수 없습니다.");
            }

            // 중복 실행 방지
            enabled = false;
        }
    }

    // 🔹 Act2 영상 종료 후 호출될 콜백
    void OnAct2VideoEnd()
    {
        // 1) 영상 끝나고 누리 대사 실행
        var dm = FindObjectOfType<DialogueManager>();
        if (dm != null && nuri_act2 != null)
        {
            dm.PlayDialogue(nuri_act2);
        }

        Debug.Log("[Act2Trigger] Act2 영상 종료 → nuri_act2 대사 재생");

        // 2) 콜백 해제 (다음에 다른 데서 영상을 써도 중복 호출 방지)
        var video = FindObjectOfType<FacilityVideoController_SH>();
        if (video != null)
        {
            video.onVideoEnd.RemoveListener(OnAct2VideoEnd);
        }
    }

    // 퍼즐 트리거에서 호출할 함수 (퍼즐 1,2,3 각각)
    public void ActivatePuzzle(int id)
    {
        if (id == 1)
        {
            puzzle1Done = true;
            Debug.Log("퍼즐 1 활성화됨");

            // ✅ 퍼즐 1 처음 완료될 때만 물 배수 카운트
            if (!_p1Counted)
            {
                _p1Counted = true;
                waterBlock?.OnMiniGameFinished();
            }
        }
        else if (id == 2)
        {
            puzzle2Done = true;
            Debug.Log("퍼즐 2 활성화됨");

            //  퍼즐 2 처음 완료될 때만
            if (!_p2Counted)
            {
                _p2Counted = true;
                waterBlock?.OnMiniGameFinished();
            }
        }
        else if (id == 3)
        {
            puzzle3Done = true;
            Debug.Log("퍼즐 3 활성화됨");

            //  퍼즐 3 처음 완료될 때만
            if (!_p3Counted)
            {
                _p3Counted = true;
                waterBlock?.OnMiniGameFinished();
            }
        }
        else
        {
            Debug.LogWarning($"알 수 없는 퍼즐 id: {id}");
        }
    }
}
