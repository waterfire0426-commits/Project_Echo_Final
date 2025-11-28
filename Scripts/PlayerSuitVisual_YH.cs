using UnityEngine;

public class PlayerSuitVisual_YH : MonoBehaviour
{
    [Header("기본 캡슐이 그려지는 Mesh Renderer")]
    public Renderer defaultRenderer;   // Player에 붙어있는 Mesh Renderer

    [Header("방호복 모델 오브젝트")]
    public GameObject hazmatBody;      // Player의 자식으로 붙인 방호복 모델

    bool lastWearing = false;

    void Start()
    {
        lastWearing = IsWearingSuit();
        ApplyVisual(lastWearing);
    }

    void Update()
    {
        bool now = IsWearingSuit();
        if (now != lastWearing)
        {
            lastWearing = now;
            ApplyVisual(now);
        }
    }

    bool IsWearingSuit()
    {
        var suitState = GetComponent<PlayerSuitState_YH>();
        if (suitState != null)
            return suitState.isWearingSuit;

        return GameState.HasSuit;
    }

    void ApplyVisual(bool wearing)
    {
        // 캡슐 메쉬는 항상 꺼두기
        if (defaultRenderer != null)
            defaultRenderer.enabled = false;

        // 방호복 모델 on/off
        if (hazmatBody != null)
            hazmatBody.SetActive(wearing);
    }

}
