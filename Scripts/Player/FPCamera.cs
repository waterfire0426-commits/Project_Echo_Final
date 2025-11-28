using UnityEngine;

public class FPCamera : MonoBehaviour
{
    // 기준점(XZ) + 현재 시야 높이
    float baseX, baseZ;
    float viewY;

    [Header("References")]
    public Transform playerRoot;        // 플레이어 본체(좌우 회전)
    public CharacterController cc;      // 속도/지상 상태
    public PlayerMove motor;            // 스프린트 상태(선택)

    [Header("Mouse Look")]
    public float mouseXSens = 500f;
    public float mouseYSens = 500f;
    public float pitchMin = -80f;
    public float pitchMax = 80f;

    [Header("Crouch View")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float standHeight = 1.6f;
    public float crouchHeight = 1.1f;
    public float crouchLerp = 12f;

    [Header("Head Bob (stride-based)")]
    public float walkBobAmp = 0.03f;
    public float sprintBobAmp = 0.05f;
    public float crouchBobAmp = 0.02f;
    public float bobLerp = 10f;

    [Header("Cursor Lock")]
    public bool autoLockCursor = true;
    public bool isPaused = false;

    public PlayerHealth_Merge playerHealth;

    void OnApplicationFocus(bool hasFocus)
    {
        // 🔒 게임오버면 자동 커서 락 금지
        if (playerHealth.IsDead)
            return;

        if (hasFocus && autoLockCursor && !isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // stride info
    public float walkStride = 1.8f;
    public float sprintStride = 2.4f;
    public float crouchStride = 1.4f;

    float yaw, pitch;
    float stepAccum = 0f;
    Vector3 bobOffset = Vector3.zero;

    void Awake()
    {
        if (!playerHealth)
        playerHealth = FindAnyObjectByType<PlayerHealth_Merge>();

        if (!playerRoot) playerRoot = transform.root;
        if (playerRoot && !cc) cc = playerRoot.GetComponent<CharacterController>();
        if (playerRoot && !motor) motor = playerRoot.GetComponent<PlayerMove>();

        baseX = transform.localPosition.x;
        baseZ = transform.localPosition.z;

        if (Mathf.Approximately(standHeight, 0f))
            standHeight = transform.localPosition.y;

        viewY = standHeight;

        if (playerRoot) yaw = playerRoot.eulerAngles.y;

        pitch = 0f;
        if (playerRoot) playerRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // 🔒 시작 시 커서 잠금 (게임오버가 아닐 때만)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ⭐⭐⭐ 게임오버 시 카메라 완전 정지 ⭐⭐⭐
        if (playerHealth.IsDead)
            return;

        // UI 열림 중이라면 정지
        if (isPaused) return;

        if (!playerRoot || !cc) return;

        // ----- Mouse look -----
        float mx = Input.GetAxisRaw("Mouse X") * mouseXSens;
        float my = Input.GetAxisRaw("Mouse Y") * mouseYSens;

        yaw += mx;
        pitch = Mathf.Clamp(pitch - my, pitchMin, pitchMax);

        playerRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // ----- Crouch view -----
        bool crouching = Input.GetKey(crouchKey);
        float targetY = crouching ? crouchHeight : standHeight;
        viewY = Mathf.Lerp(viewY, targetY, Time.deltaTime * crouchLerp);

        // ----- Head bob -----
        Vector3 planarVel = cc.velocity; 
        planarVel.y = 0f;

        float speed = planarVel.magnitude;
        bool grounded = cc.isGrounded;
        bool moving = speed > 0.05f && grounded;

        float amp, stride;
        if (crouching) { amp = crouchBobAmp; stride = crouchStride; }
        else if (motor && motor.isSprinting) { amp = sprintBobAmp; stride = sprintStride; }
        else { amp = walkBobAmp; stride = walkStride; }

        if (moving)
        {
            stepAccum += speed * Time.deltaTime;
            float phase = (stepAccum / stride) * Mathf.PI * 2;

            float s = Mathf.Sin(phase);
            float c = Mathf.Cos(phase * 0.5f);

            Vector3 targetBob = new Vector3(c * amp * 0.5f, s * amp, 0f);
            bobOffset = Vector3.Lerp(bobOffset, targetBob, Time.deltaTime * bobLerp);
        }
        else
        {
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * bobLerp);
        }

        transform.localPosition = new Vector3(
            baseX + bobOffset.x,
            viewY + bobOffset.y,
            baseZ + bobOffset.z
        );

        // 자동 커서 락 유지 — 단, 게임오버는 절대 잠그지 않음
        if (autoLockCursor && !isPaused && !playerHealth.IsDead)
        {
            if (Application.isFocused && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}


