using UnityEngine;

public class SuitHUDInitializer : MonoBehaviour
{
    [Header("방호복 착용 시 켤 HUD")]
    public GameObject hazmatHUD;          // 방호복 HUD 캔버스 or 패널

    [Header("방호복 착용 시 숨길 HUD들 (선택)")]
    public GameObject[] hudToHideOnEquip; // 평소 HUD, 조준점 같은 것들

    void Start()
    {
        Debug.Log($"[SuitHUDInitializer] Start, HasSuit={GameState.HasSuit}");

        if (GameState.HasSuit)
        {
            // 로비에서 이미 방호복 입고 넘어온 경우
            if (hazmatHUD != null)
            {
                hazmatHUD.SetActive(true);
                Debug.Log("[SuitHUDInitializer] hazmatHUD ON");
            }

            if (hudToHideOnEquip != null)
            {
                foreach (var go in hudToHideOnEquip)
                {
                    if (go != null)
                    {
                        go.SetActive(false);
                        Debug.Log($"[SuitHUDInitializer] hide {go.name}");
                    }
                }
            }
        }
        else
        {
            // 방호복 안 입고 시작하는 경우엔 HUD 꺼둠
            if (hazmatHUD != null)
            {
                hazmatHUD.SetActive(false);
                Debug.Log("[SuitHUDInitializer] hazmatHUD OFF (no suit)");
            }
        }
    }
}
