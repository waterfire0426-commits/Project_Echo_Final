using UnityEngine;
using System.Collections;

public class VentEscapeLadder : MonoBehaviour
{
    [Header("시작 / 끝 지점")]
    public Transform startPoint;   // 아래 통로 바닥
    public Transform endPoint;     // 위 환풍구 바닥

    [Header("입구에서 누를 키")]
    public KeyCode climbKey = KeyCode.W;

    [Header("올라가는 연출 시간 (초)")]
    public float climbDuration = 0.8f;

    private bool used = false;

    private void OnTriggerStay(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        // W 눌렀을 때만 발동
        if (Input.GetKeyDown(climbKey))
        {
            StartCoroutine(ClimbRoutine(other));
        }
    }

    IEnumerator ClimbRoutine(Collider player)
    {
        used = true;

        // 이동/중력 담당 컴포넌트 잠깐 끄기
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 만약 플레이어 이동 스크립트가 따로 있다면 여기서 꺼줘도 됨
        // 예: var move = player.GetComponent<PlayerMove>(); if (move) move.enabled = false;

        Vector3 from = startPoint ? startPoint.position : player.transform.position;
        Vector3 to   = endPoint   ? endPoint.position   : player.transform.position;

        from.y += 0.1f;
        to.y   += 0.1f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / climbDuration;
            player.transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        player.transform.position = to;

        // 다시 컨트롤 켜주기
        if (cc != null) cc.enabled = true;
        // if (move) move.enabled = true;

        // 필요하면 이 트리거 자체 꺼도 됨
        // gameObject.SetActive(false);
    }
}
