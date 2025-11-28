using UnityEngine;

public class OneWayLadder : MonoBehaviour
{
    public Transform topPoint;          // 올라갈 목표 지점 (환풍구 안)
    public bool requireKey = true;      // 키 입력 필요 여부 (true면 W 눌러야 이동)
    public KeyCode climbKey = KeyCode.W;

    private bool used = false;          // 한 번만 쓰이도록

    private void OnTriggerStay(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        // 키 안 쓰고 들어오기만 하면 올라가게 하고 싶으면 여기 바로 TeleportPlayer(other); 호출
        if (requireKey)
        {
            if (!Input.GetKeyDown(climbKey)) return;
        }

        TeleportPlayer(other);
    }

    void TeleportPlayer(Collider player)
    {
        if (topPoint == null) return;

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 목표 위치로 순간 이동 (살짝 위로 띄워주면 좋음)
        Vector3 targetPos = topPoint.position;
        targetPos.y += 0.1f;  // 바닥에 박히지 않게 살짝 위

        player.transform.position = targetPos;

        if (cc != null) cc.enabled = true;

        used = true;  // 한 번 쓰고 나면 다시 안 쓰이게
        // 필요하면 이 줄 대신 gameObject.SetActive(false); 해서 트리거 자체를 꺼도 됨
    }
}
