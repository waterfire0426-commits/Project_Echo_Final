using UnityEngine;

public class VentHandMove : MonoBehaviour
{
    public Transform endPoint;        // 손이 멈출 위치 (막힌 길 입구)
    public Transform player;          // 플레이어 Transform

    [Header("Settings")]
    public float triggerDistance = 6f; // 손 기준, 플레이어가 이 거리 안으로 오면 시작
    public float moveSpeed = 0.5f;     // 손이 앞으로 기어 나오는 속도
    public bool onlyOnce = true;       // 한 번만 움직일지

    private bool started = false;
    private bool finished = false;

    void Start()
    {
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!endPoint)
            Debug.LogWarning("[VentHandMove] endPoint가 비어 있습니다!");
    }

    void Update()
    {
        if (finished || !player || !endPoint) return;

        // 아직 시작 안했고, 플레이어가 충분히 가까워지면 시작
        if (!started)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist <= triggerDistance)
            {
                started = true;
                Debug.Log("[VentHandMove] 플레이어 접근 → 손 전진 시작");
            }
            else
            {
                return;
            }
        }

        // 시작했으면 endPoint 쪽으로 천천히 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        float remain = Vector3.Distance(transform.position, endPoint.position);
        if (remain < 0.02f)
        {
            finished = true;
            Debug.Log("[VentHandMove] 손 도착, 이동 종료");
        }
    }
}
