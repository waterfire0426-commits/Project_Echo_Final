using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public string name;                  // 먼지진드기 / 일반몹 / 콜라 이런 이름
    public GameObject prefab;            // 이 타입 괴물 프리팹
    public Transform[] spawnPoints;      // 이 타입이 나올 수 있는 위치들

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;    // 이 타입 스폰 간격
    public int maxAlive = 3;             // 이 타입 동시에 살아있는 최대 마리 수

    [Header("Contam Stage 조건")]
    public int minStage = 0;             // 이 단계 이상일 때만 스폰
    public int maxStage = 99;            // 이 단계 이할 때만 스폰

    [Header("확률 (가중치)")]
    [Range(0f, 1f)] public float weight = 1f; // 다른 타입과 섞어서 쓸 때 확률 비율

    [HideInInspector] public float timer;            // 내부용 타이머
    [HideInInspector] public List<GameObject> alive; // 살아있는 개체들
}

public class MultiMonsterSpawner_YH : MonoBehaviour
{
    [Header("전체 스포너 설정")]
    public bool startOnAwake = false;   // 처음부터 스폰 켜둘지
    public int currentStage = 0;        // ContamHook에서 알려줄 오염도 단계

    [Header("스폰 타입들")]
    public SpawnEntry[] entries;        // 먼지진드기 / 일반몹 / 콜라 등

    private bool isSpawning = false;

    void Awake()
    {
        isSpawning = startOnAwake;

        // 각 엔트리 alive 리스트 초기화
        if (entries != null)
        {
            foreach (var e in entries)
            {
                e.timer = 0f;
                if (e.alive == null) e.alive = new List<GameObject>();
            }
        }
    }

    void Update()
    {
        if (!isSpawning) return;
        if (entries == null || entries.Length == 0) return;

        foreach (var e in entries)
        {
            if (e.prefab == null) continue;
            if (e.spawnPoints == null || e.spawnPoints.Length == 0) continue;

            // 오염도 단계 조건에 안 맞으면 스킵
            if (currentStage < e.minStage || currentStage > e.maxStage) continue;

            // 살아있는 애들 정리
            if (e.alive == null) e.alive = new List<GameObject>();
            e.alive.RemoveAll(go => go == null);

            // 이미 최대 마리 수면 스킵
            if (e.alive.Count >= e.maxAlive) continue;

            // 타이머 + 간격 체크
            e.timer += Time.deltaTime;
            if (e.timer < e.spawnInterval) continue;
            e.timer = 0f;

            // 스폰 포인트 중 랜덤
            int idx = Random.Range(0, e.spawnPoints.Length);
            Transform pt = e.spawnPoints[idx];

            GameObject go = Instantiate(e.prefab, pt.position, pt.rotation);
            e.alive.Add(go);

            Debug.Log($"[MultiSpawner] {e.name} 스폰됨 at {pt.name}");
        }
    }

    // ContamHook에서 단계 알려줄 때 호출
    public void SetStage(int stage)
    {
        currentStage = stage;
        // Debug.Log("[MultiSpawner] currentStage = " + currentStage);
    }

    public void StartSpawn()
    {
        isSpawning = true;
        Debug.Log("[MultiSpawner] 스폰 시작");
    }

    public void StopSpawn()
    {
        isSpawning = false;
        Debug.Log("[MultiSpawner] 스폰 중지");
    }
}
