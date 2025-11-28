using UnityEngine;
using System.Collections;

public class SimpleMonsterSpawnOnTrigger : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject monsterPrefab;   // 스폰할 괴물 프리팹
    public Transform spawnPoint;       // 스폰 위치
    public Transform playerTransform;  // 플레이어 Transform
    public float spawnDelay = 0.5f;    // 트리거 밟은 후 딜레이

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[SimpleSpawn] OnTriggerEnter: {other.name}", this);

        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        Debug.Log($"[SimpleSpawn] {spawnDelay}초 뒤 스폰 시작", this);
        yield return new WaitForSeconds(spawnDelay);

        if (monsterPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning($"[SimpleSpawn] monsterPrefab({monsterPrefab}) 또는 spawnPoint({spawnPoint}) 비어 있음!", this);
            yield break;
        }

        // 혹시 남아 있는 괴물 있으면 한 마리만 정리
        var old = FindObjectOfType<MainMonsterChase>();
        if (old != null)
        {
            Debug.Log($"[SimpleSpawn] 기존 괴물 제거: {old.name}", this);
            Destroy(old.gameObject);
        }

        GameObject m = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"[SimpleSpawn] 새 괴물 스폰: {m.name}", this);

        var chase = m.GetComponent<MainMonsterChase>();
        if (chase != null && playerTransform != null)
        {
            chase.Init(playerTransform);
            Debug.Log("[SimpleSpawn] 추격 Init 완료", this);
        }
        else
        {
            Debug.LogWarning($"[SimpleSpawn] MainMonsterChase({chase}) 혹은 playerTransform({playerTransform}) null", this);
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }
    }

}
