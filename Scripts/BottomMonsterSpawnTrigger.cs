using UnityEngine;
using System.Collections;

public class BottomMonsterSpawn : MonoBehaviour
{
    [Header("References")]
    public GameObject monsterPrefab;      // 괴물 프리팹
    public Transform spawnPoint;          // 스폰 위치
    public Transform playerTransform;     // 플레이어 Transform

    [Header("Settings")]
    public float spawnDelay = 2f;         // 몇 초 뒤에 스폰할지

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[BottomMonsterSpawn] OnTriggerEnter: {other.name}", this);

        if (triggered)
        {
            Debug.Log("[BottomMonsterSpawn] 이미 발동됨, 무시", this);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[BottomMonsterSpawn] Player가 아니라서 무시", this);
            return;
        }

        triggered = true;
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        Debug.Log($"[BottomMonsterSpawn] {spawnDelay}초 뒤 스폰 예정", this);
        yield return new WaitForSeconds(spawnDelay);

        // 여기서 남아있는 괴물 먼저 정리
        var old = Object.FindObjectOfType<MainMonsterChase>();
        if (old != null)
        {
            Debug.Log($"[BottomMonsterSpawn] 기존 괴물 제거: {old.name}", this);
            Destroy(old.gameObject);
        }

        // 이제 새 괴물 스폰
        if (monsterPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning(
                $"[BottomMonsterSpawn] monsterPrefab({monsterPrefab}) 혹은 spawnPoint({spawnPoint}) 비어 있음!",
                this
            );
            yield break;
        }

        GameObject m = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"[BottomMonsterSpawn] 괴물 스폰 완료: {m.name}", this);

        var chase = m.GetComponent<MainMonsterChase>();
        if (chase != null && playerTransform != null)
        {
            chase.Init(playerTransform);
            Debug.Log("[BottomMonsterSpawn] 괴물 추격 시작", this);
        }
        else
        {
            Debug.LogWarning($"[BottomMonsterSpawn] MainMonsterChase({chase}) 또는 playerTransform({playerTransform}) null", this);
        }
    }
}
