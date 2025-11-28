using UnityEngine;

public class AutoStartMiniGame : MonoBehaviour
{
    void Start()
    {
        // 이 오브젝트에 붙은 컴포넌트 중
        // 이름이 "StartMiniGame"인 메서드가 있으면 호출
        SendMessage("StartMiniGame", SendMessageOptions.DontRequireReceiver);
    }
}
