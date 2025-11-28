using UnityEngine;
using System.Collections;

public class VentEnterTrigger : MonoBehaviour
{
    [Header("Monster Settings")]
    public GameObject monsterPrefab;      // 새로 스폰할 괴물 프리팹
    public Transform respawnPoint;        // 스폰 위치
    public Transform playerTransform;     // 플레이어 Transform

    [Header("Timing")]
    public float spawnDelay = 1.0f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[VentEnterTrigger] OnTriggerEnter: {other.name}", this);

        if (triggered)
        {
            Debug.Log("[VentEnterTrigger] 이미 발동됨, 무시", this);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[VentEnterTrigger] Player가 아님, 무시", this);
            return;
        }

        triggered = true;

        // 위에서 쫓아오던 괴물 한 마리 찾아서 제거
        var old = FindObjectOfType<MainMonsterChase>();
        if (old != null)
        {
            Debug.Log($"[VentEnterTrigger] 기존 괴물 제거: {old.name}", this);
            Destroy(old.gameObject);
        }
        else
        {
            Debug.Log("[VentEnterTrigger] 기존 괴물을 찾지 못함 (없을 수도 있음)", this);
        }

        StartCoroutine(SpawnMonsterAfterDelay());
    }

    private IEnumerator SpawnMonsterAfterDelay()
    {
        Debug.Log($"[VentEnterTrigger] {spawnDelay}초 뒤 스폰 예정", this);
        yield return new WaitForSeconds(spawnDelay);

        if (monsterPrefab == null || respawnPoint == null)
        {
            Debug.LogWarning("[VentEnterTrigger] monsterPrefab 또는 respawnPoint가 비어 있음!", this);
            yield break;
        }

        GameObject m = Instantiate(monsterPrefab, respawnPoint.position, respawnPoint.rotation);
        Debug.Log($"[VentEnterTrigger] 새 괴물 스폰: {m.name}", this);

        var chase = m.GetComponent<MainMonsterChase>();
        if (chase != null && playerTransform != null)
        {
            chase.Init(playerTransform);
            Debug.Log("[VentEnterTrigger] 새 괴물 추격 시작", this);
        }
        else
        {
            Debug.LogWarning("[VentEnterTrigger] MainMonsterChase 또는 playerTransform이 null", this);
        }
    }
}
