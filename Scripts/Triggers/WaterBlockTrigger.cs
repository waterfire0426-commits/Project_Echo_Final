using UnityEngine;

public class WaterBlockTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // 중복 방지
        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("💧물이 가득 차 있어 진입이 불가능하다. 다른 길을 찾아야 한다.");
            // 필요 시 활성화:
            // QuestManager.Notify(TRG.BLOCKED_BY_WATER);
        }
    }
}
