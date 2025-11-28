using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VentHandKill : MonoBehaviour
{
    private void Reset()
    {
        // 자동으로 트리거로 만들기
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 플레이어 체력 스크립트 찾기
        var hp = other.GetComponent<PlayerHealth_YH>();
        if (hp == null)
            hp = other.GetComponentInParent<PlayerHealth_YH>();

        if (hp != null)
        {
            hp.TakeDamage(9999f);  // 사실상 즉사 데미지
            Debug.Log("[VentHandKill] 손에 닿음 → 플레이어 즉사");
        }
        else
        {
            Debug.LogWarning("[VentHandKill] PlayerHealth_YH를 찾지 못했습니다.");
        }
    }
}
