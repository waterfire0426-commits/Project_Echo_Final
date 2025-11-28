using UnityEngine;

public class LadderClimbZone : MonoBehaviour
{
    public Transform bottomPoint;    // 사다리 시작 높이
    public Transform topPoint;       // 사다리 끝 높이
    public float climbSpeed = 3f;    // 올라가는 속도

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        float input = 0f;

        // W = 위로, S = 아래로
        if (Input.GetKey(KeyCode.W)) input = 1f;
        else if (Input.GetKey(KeyCode.S)) input = -1f;

        if (Mathf.Abs(input) < 0.01f) return; // 아무 입력 없으면 패스

        // 현재 위치
        Vector3 pos = other.transform.position;

        // Y값만 조절 (위/아래)
        pos.y += input * climbSpeed * Time.deltaTime;

        // Y를 bottom~top 사이로만 클램프
        float minY = bottomPoint ? bottomPoint.position.y : pos.y;
        float maxY = topPoint ? topPoint.position.y : pos.y;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // 위치 적용 (XZ는 건들지 않음)
        other.transform.position = pos;
    }
}
