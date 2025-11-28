using System.Collections.Generic;
using UnityEngine;

public class FacilityLightController : MonoBehaviour
{
    [Header("시작할 때 맵 조명 끌지 여부")]
    public bool startOff = true;

    // 씬 안에서 자동으로 찾은 조명들
    private List<Light> cachedLights = new List<Light>();
    private List<float> originalIntensity = new List<float>();

    void Awake()
    {
        // 이 로그가 안 뜨면: 오브젝트가 비활성 / 스크립트가 안 붙어 있음
        Debug.Log("[FacilityLightController] Awake 호출됨");

        // 씬 안의 모든 Light 컴포넌트 찾기
#if UNITY_2022_1_OR_NEWER
        var allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
#else
        var allLights = FindObjectsOfType<Light>();
#endif

        foreach (var l in allLights)
        {
            if (l == null) continue;

            // 1) 플레이어 안에 붙은 라이트(손전등 등)는 제외
            if (l.GetComponentInParent<PlayerMove>() != null)
                continue;

            cachedLights.Add(l);
            originalIntensity.Add(l.intensity);
        }

        Debug.Log($"[FacilityLightController] 찾은 라이트 개수: {cachedLights.Count}");

        // 시작할 때 전체 OFF 설정이면 바로 끔
        if (startOff)
        {
            SetLights(false);
        }
    }

    public void TurnOn()
    {
        SetLights(true);
    }

    public void TurnOff()
    {
        SetLights(false);
    }

    private void SetLights(bool on)
    {
        for (int i = 0; i < cachedLights.Count; i++)
        {
            var l = cachedLights[i];
            if (l == null) continue;

            // enabled 플래그
            l.enabled = on;

            // 혹시 Baked/혼합이라도 강제로 더 어둡게 / 밝게
            if (on)
                l.intensity = originalIntensity[i];
            else
                l.intensity = 0f;
        }

        Debug.Log($"[FacilityLightController] 조명 {(on ? "ON" : "OFF")} (총 {cachedLights.Count}개)");
    }
}
