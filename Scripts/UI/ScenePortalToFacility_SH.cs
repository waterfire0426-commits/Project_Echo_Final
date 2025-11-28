using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class ScenePortalToFacility_SH : MonoBehaviour
{
    [Header("Who can trigger")]
    public string requiredTag = "Player";
    public LayerMask allowedLayers = ~0;
    public bool requireCharacterController = false;
    public bool oneShot = true;

    [Header("Suit condition")]
    public bool requireSuit = true;              // 방호복 필수 여부

    [Header("Routing")]
    public string loadingSceneName = "LoadingScene_YR";   // 로딩씬 이름
    public string targetSceneName  = "Facility_Scene_YH"; // 네 맵 이름

    bool triggered;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (oneShot && triggered) return;
        if (!PassesFilter(other)) return;

        triggered = true;

        // 로딩씬에 최종 목적지 알려주기
        PlayerPrefs.SetString("NextScene", targetSceneName);

        // 페이드 아웃 + 로딩씬 로드
        if (ScreenFader_SH.Instance != null)
            ScreenFader_SH.Instance.FadeOutAndLoad(loadingSceneName);
        else
            SceneManager.LoadScene(loadingSceneName);
    }

    bool PassesFilter(Collider other)
    {
        // 태그 체크
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return false;

        // 레이어 체크
        if (((1 << other.gameObject.layer) & allowedLayers) == 0)
            return false;

        // 캐릭터컨트롤러 필요할 때
        if (requireCharacterController && !other.GetComponent<CharacterController>())
            return false;

        // 🔥 방호복 조건
        if (requireSuit)
        {
            // Player 또는 부모에서 PlayerSuitState_YH 찾기
            var suitState = other.GetComponentInParent<PlayerSuitState_YH>();

            // 스크립트 없거나 아직 안 입었으면 통과 X
            if (suitState == null) return false;
            if (!suitState.isWearingSuit) return false;
        }

        return true;
    }
}
