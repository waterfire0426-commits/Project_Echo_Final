using UnityEngine;

public class HazmatEquip : MonoBehaviour
{
    [Header("Meshes / Objects")]
    public GameObject normalBody;   // 평상복 바디
    public GameObject hazmatBody;   // 방호복 입은 바디
    public GameObject flashlight;   // 후레쉬(손전등) 있으면 여기

    public bool isEquipped = false; // 현재 방호복 착용 여부

    void Start()
    {
        ApplyState();
    }

    public void EquipHazmat(bool equip)
    {
        isEquipped = equip;
        ApplyState();
    }

    void ApplyState()
    {
        if (normalBody != null) normalBody.SetActive(!isEquipped);
        if (hazmatBody != null) hazmatBody.SetActive(isEquipped);

        // 방호복 입었을 때만 후레쉬 켜고 싶으면:
        if (flashlight != null) flashlight.SetActive(isEquipped);
    }
}
