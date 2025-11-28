using UnityEngine;
using System.Collections;

public class VentExitTeleport : MonoBehaviour
{
    public Transform exitPoint;       // 플레이어를 내보낼 위치
    public Transform playerTransform; // 플레이어 Transform
    public float delay = 0.2f;        // 살짝 딜레이 (연출용)

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(TeleportAfterDelay());
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (playerTransform != null && exitPoint != null)
        {
            // 위치 순간이동
            playerTransform.position = exitPoint.position;
            playerTransform.rotation = exitPoint.rotation;
        }
        else
        {
            Debug.LogWarning("[VentExitTeleport] exitPoint 또는 playerTransform이 비어 있습니다!");
        }
    }
}
