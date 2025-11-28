using UnityEngine;

public class SuitStartup : MonoBehaviour
{
    [Header("방호복 HUD (캔버스 또는 패널)")]
    public GameObject hazmatHUD;

    [Header("방호복 입었을 때 숨길 HUD들 (선택)")]
    public GameObject[] hudToHideOnEquip;

    void Start()
    {
        if (GameState.HasSuit)
        {
            // 로비에서 이미 방호복 입고 넘어온 경우
            if (hazmatHUD) hazmatHUD.SetActive(true);

            if (hudToHideOnEquip != null)
            {
                foreach (var go in hudToHideOnEquip)
                    if (go) go.SetActive(false);
            }
        }
        else
        {
            // 방호복 안 입은 상태로 시작할 땐 HUD 꺼두기
            if (hazmatHUD) hazmatHUD.SetActive(false);
        }
    }
}
