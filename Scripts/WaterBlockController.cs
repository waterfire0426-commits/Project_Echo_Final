using UnityEngine;

public class WaterBlockController : MonoBehaviour
{
    [Header("설정")]
    public int requiredMiniGames = 3;   // 퍼즐 개수와 맞추기
    public float drainDistance = 20f;
    public float drainDuration = 2.5f;

    [Header("Debug")]
    public bool enableDebugKey = true;   // M키 디버그 on/off

    private int clearedCount = 0;
    private bool drained = false;
    private Collider[] _colliders;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();
    }

    // Act2Trigger에서 퍼즐 하나 완료될 때마다 호출
    public void OnMiniGameFinished()
    {
        if (drained) return;

        clearedCount++;
        Debug.Log($"[WaterBlock] 미니게임 완료 {clearedCount}/{requiredMiniGames}");

        if (clearedCount >= requiredMiniGames)
        {
            DrainWater();
        }
    }

    private void DrainWater()
    {
        if (drained) return;
        drained = true;

        StartCoroutine(DrainRoutine());
    }

    private System.Collections.IEnumerator DrainRoutine()
    {
        Vector3 startPos  = transform.position;
        Vector3 targetPos = startPos + Vector3.down * drainDistance;

        float t = 0f;
        while (t < drainDuration)
        {
            t += Time.deltaTime;
            float lerp = t / drainDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, lerp);
            yield return null;
        }

        // 내려간 뒤 모든 콜라이더 꺼버리기
        foreach (var col in _colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        Debug.Log("[WaterBlock] 배수 완료 및 콜라이더 비활성화");
    }

    private void Update()
    {
        if (!enableDebugKey) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("[WaterBlock] DEBUG: M key pressed → 강제로 물 빼기");
            ForceDrainNow();
        }
    }

    // 디버그용 강제 배수
    public void ForceDrainNow()
    {
        if (!drained)
        {
            clearedCount = requiredMiniGames;
            DrainWater();
        }
    }
}
