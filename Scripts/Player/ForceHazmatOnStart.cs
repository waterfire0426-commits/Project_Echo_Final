using UnityEngine;
using UnityEngine.SceneManagement;  

public class ForceHazmatOnStart : MonoBehaviour
{
    [SerializeField]
    private string onlyInScene = "Facility_Scene_SH"; 

    void Start()
    {

        // 0. 이 스크립트가 동작할 씬이 아니면 바로 종료
        if (SceneManager.GetActiveScene().name != onlyInScene)
        {
            return;
        }
        
        // 1. 방호복 시스템 쪽에 "입었다"라고 알려주기
        var suit = GetComponentInChildren<ISuitReceiver>();
        if (suit != null && !suit.IsSuited)
        {
            suit.ApplySuit(true);  // 애니/메쉬 쪽은 기존 코드가 알아서 처리
        }

        // 2. 네가 이미 쓰고 있는 상태 플래그들 맞춰주기
        var suitState = GetComponent<PlayerSuitState_YH>();
        if (suitState != null)
        {
            suitState.isWearingSuit = true;
        }

        GameState.HasSuit = true;

        // 3. 필요하면 여기서 플래시라이트도 켜주기
        // (플래시가 Player 자식이면 이렇게)
        /*
        var flash = GetComponentInChildren<FlashlightType스크립트 or GameObject>();
        if (flash != null) flash.SetActive(true);
        */
    }
}
