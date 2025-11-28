using UnityEngine;

public class DebugEndingHotkey_YH : MonoBehaviour
{
    [Header("디버그용 엔딩 해금 키")]
    public KeyCode debugKey = KeyCode.K;

    [Header("선택: 길도 같이 스위치할지 여부")]
    public PathSwitcher pathSwitcher;   // 안 넣어도 됨
    public bool alsoSwitchPaths = false; // 기본은 false

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            // 엔딩 조건 강제 해금
            GameState.EndUnlocked = true;
            Debug.Log("[DebugEndingHotkey] K 눌림 → EndUnlocked = true");

            // 원하면 길 스위치도 같이 테스트
            if (alsoSwitchPaths && pathSwitcher != null)
            {
                pathSwitcher.SwitchPaths();
                Debug.Log("[DebugEndingHotkey] PathSwitcher.SwitchPaths() 강제 호출");
            }
        }
    }
}
