using UnityEngine;

public class PathSwitcher : MonoBehaviour
{
    public GameObject blockVent;     // 환풍구 앞 벽 (초기 ON)
    public GameObject blockOldPath;  // 기존 길 막는 벽 (초기 OFF)

    bool switched = false;

    public void SwitchPaths()
    {
        if (switched) return;
        switched = true;

        if (blockVent) blockVent.SetActive(false);   // 환풍구 열기
        if (blockOldPath) blockOldPath.SetActive(true); // 기존 길 막기

        Debug.Log("[PathSwitcher] 길 스위치 완료!");

        // 🔥 여기서 엔딩 해금
        GameState.EndUnlocked = true;
        Debug.Log("[GameState] EndUnlocked = true (Act3 시작, 엔딩 해금)");
    }
}
