using UnityEngine;

public class VentEndTrigger : MonoBehaviour
{
    public MainMonsterChase topMonster;      // 위에서 쫓아오던 괴물
    public bool destroyAfterStop = true;     // 멈춘 뒤 없앨지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (topMonster != null)
        {
            topMonster.StopAtVentEnd();  // 환풍구 끝에서 멈추게
            if (destroyAfterStop)
            {
                Destroy(topMonster.gameObject, 1.0f); // 살짝 있다가 제거
            }
        }
    }
}
