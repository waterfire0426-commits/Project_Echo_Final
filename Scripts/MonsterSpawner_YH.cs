using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_YH : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;        // 이 스포너가 뽑을 괴물 프리팹
    public Transform[] spawnPoints;       // 이 괴물이 나올 수 있는 위치들

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;     // 스폰 간격(초)
    public int maxAlive = 3;              // 동시에 살아 있는 최대 마리수
    public bool startOnAwake = false;     // 처음부터 스폰할지 여부

    [Header("Contam Stage 조건")]
    public int minStage = 0;              // 이 단계 이상일 때만
    public int maxStage = 99;             // 이 단계 이할 때만
    private int currentStage = 0;         // ContamHook에서 알려줄 값

    private List<GameObject> alive = new List<GameObject>();
    private float timer = 0f;
    private bool isSpawning = false;

    void Awake()
    {
        isSpawning = startOnAwake;
    }

    void Update()
    {
        if (!isSpawning) return;
        if (enemyPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // 오염도 단계 범위 밖이면 스폰 중단
        if (currentStage < minStage || currentStage > maxStage) return;

        // 죽은 애들 정리
        alive.RemoveAll(e => e == null);

        // 이미 꽉 차 있으면 더 안 뽑음
        if (alive.Count >= maxAlive) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        int idx = Random.Range(0, spawnPoints.Length);
        Transform pt = spawnPoints[idx];

        GameObject go = Instantiate(enemyPrefab, pt.position, pt.rotation);
        alive.Add(go);

        Debug.Log("[Spawner] 스폰됨: " + go.name + " at " + pt.name);
    }

    // ContamHook에서 단계 바뀔 때마다 호출해 줄 함수
    public void SetStage(int stage)
    {
        currentStage = stage;
    }

    public void StartSpawn()
    {
        isSpawning = true;
        timer = 0f;
        Debug.Log("[Spawner] 스폰 시작");
    }

    public void StopSpawn()
    {
        isSpawning = false;
        Debug.Log("[Spawner] 스폰 중지");
    }
}
