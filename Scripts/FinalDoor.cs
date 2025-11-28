using UnityEngine;
using System.Collections;

public class FinalDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorPivot;        // 회전 중심(문 힌지)
    public float openAngle = 90f;      // 열릴 각도
    public float openSpeed = 2f;       // 열리는 속도

    private bool isOpen = false;
    private bool isOpening = false;

    // 플레이어가 상호작용할 경우 호출
    public void Interact()
    {
        if (!isOpen && !isOpening)
        {
            StartCoroutine(OpenDoorRoutine());
        }
    }

    IEnumerator OpenDoorRoutine()
    {
        isOpening = true;

        Quaternion startRot = doorPivot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, openAngle, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorPivot.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        // 문 열림 완료
        isOpen = true;
        isOpening = false;

        // 플레이어 충돌 방지하려면 콜라이더 꺼주기
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}
