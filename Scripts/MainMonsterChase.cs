using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MainMonsterChase : MonoBehaviour
{
    public enum State
    {
        Idle,
        GroundChase,   // 방 안에서 추격
        ClimbLadder,   // 사다리/환풍구 입구로 올라가는 중
        VentChase,     // 환풍구 안에서 추격
        Stopped        // 마지막 문 앞에서 멈춘 상태
    }

    [Header("공통")]
    public Transform target;                 // 플레이어 Transform
    public float killDistance = 1.0f;        // 닿으면 죽는 거리
    public PlayerHealth_Merge playerHealth;  // 머지 버전 사용

    [Header("바닥 추격 설정")]
    public float groundSpeed = 3.5f;

    [Header("사다리 / 환풍구 입구")]
    public Collider ladderCollider;          // LadderWallSimple이 붙어있는 콜라이더
    public Transform ladderTransform;        // 사다리(벽) Transform
    public float climbSpeed = 3f;            // 위로 올라가는 속도
    public float forwardSpeed = 2f;          // 꼭대기에서 환풍구 안쪽으로 밀어주는 속도
    public float topMargin = 0.2f;           // 꼭대기 인식 여유

    [Header("환풍구 안 추격 설정")]
    public float ventSpeed = 3f;

    [Header("마지막 정지 위치(선택)")]
    public Transform ventEndStopPoint;       // 문 연타 구간 직전 위치(없으면 현재 자리에서 멈춤)

    private NavMeshAgent agent;
    private State state = State.Idle;
    private bool initialized = false;

    // -------------------------------------------------------
    // 🔊 발소리 시스템
    // -------------------------------------------------------
    [Header("발소리 SFX")]
    public AudioSource footstepSource;       // 괴물 발소리 재생용 AudioSource

    public AudioClip[] groundFootsteps;      // 방 안 추격 발소리
    public AudioClip[] ventFootsteps;        // 환풍구 추격 발소리
    public AudioClip[] climbSFX;             // 사다리/통로 기어오르는 소리

    public float groundStepInterval = 0.6f;
    public float ventStepInterval = 0.5f;
    public float climbStepInterval = 0.45f;

    private float footstepTimer = 0f;


    // =======================================================
    // Init (Act3Trigger에서 호출)
    // =======================================================
    public void Init(Transform player)
    {
        target = player;

        if (!playerHealth)
            playerHealth = player.GetComponent<PlayerHealth_Merge>();

        if (!agent) agent = GetComponent<NavMeshAgent>();

        agent.enabled = true;

        // 스폰 위치가 NavMesh에서 약간 벗어나 있어도 가까운 NavMesh로 스냅
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log("[MainMonster] NavMesh에 스냅: " + hit.position);
            }
            else
            {
                Debug.LogError("[MainMonster] 주변에서 NavMesh를 찾지 못했습니다. 스폰 위치를 NavMesh 위로 옮겨주세요.");
                initialized = false;
                return;
            }
        }

        agent.speed = groundSpeed;
        agent.stoppingDistance = 0f;

        initialized = true;
        state = State.GroundChase;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    // =======================================================
    // Update
    // =======================================================
    private void Update()
    {
        if (!initialized || !target) return;

        switch (state)
        {
            case State.GroundChase:
                UpdateGroundChase();
                break;
            case State.ClimbLadder:
                UpdateClimbLadder();
                break;
            case State.VentChase:
                UpdateVentChase();
                break;
        }

        // 🔊 발소리 처리
        UpdateFootsteps();

        // 공통: 즉사 판정
        if (state == State.GroundChase || state == State.VentChase || state == State.ClimbLadder)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= killDistance)
            {
                KillPlayer();
            }
        }
    }

    // =======================================================
    // 바닥 추격
    // =======================================================
    void UpdateGroundChase()
    {
        if (agent == null) return;
        if (!agent.enabled) return;
        if (!agent.isOnNavMesh) return;

        agent.speed = groundSpeed;
        agent.SetDestination(target.position);
    }

    // =======================================================
    // 사다리 시작 (외부 트리거에서 호출)
    // =======================================================
    public void BeginClimb()
    {
        if (state != State.GroundChase) return;
        if (!ladderCollider || !ladderTransform)
        {
            Debug.LogWarning("[MainMonster] 사다리 정보가 비어 있습니다.");
            return;
        }

        Debug.Log("[MainMonster] 사다리 오르기 시작");
        state = State.ClimbLadder;
        agent.enabled = false; // 직접 움직일 것
    }

    // =======================================================
    // 사다리 + 꼭대기 밀어주기
    // =======================================================
    void UpdateClimbLadder()
    {
        Vector3 pos = transform.position;

        float bottomY = ladderCollider.bounds.min.y + 0.1f;
        float topY = ladderCollider.bounds.max.y - 0.1f;

        // 아직 꼭대기보다 아래라면 → 위로 올라가기
        if (pos.y < topY - topMargin)
        {
            float targetY = Mathf.Clamp(pos.y + climbSpeed * Time.deltaTime, bottomY, topY);
            pos.y = targetY;
            transform.position = pos;

            // 사다리 쪽으로 몸 방향 맞춰주기
            Vector3 lookDir = ladderTransform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    Time.deltaTime * 5f
                );
            }
        }
        else
        {
            // 꼭대기 근처면 → 환풍구 안쪽으로 밀어주기
            Vector3 forward = ladderTransform.forward;
            transform.position += forward * forwardSpeed * Time.deltaTime;

            // 어느 정도 앞으로 나갔으면 환풍구 추격 상태로 전환
            float forwardDist =
                Vector3.Dot(transform.position - ladderTransform.position, forward.normalized);

            if (forwardDist > 0.6f)
            {
                StartVentChase();
            }
        }
    }

    // =======================================================
    // 환풍구 안 추격
    // =======================================================
    void StartVentChase()
    {
        Debug.Log("[MainMonster] 환풍구 안 추격 시작");
        state = State.VentChase;

        if (!agent) agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        // 환풍구 들어가기 직전 위치로 에이전트 동기화
        if (agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
        }

        agent.speed = ventSpeed;
    }

    void UpdateVentChase()
    {
        if (agent == null) return;
        if (!agent.enabled) return;
        if (!agent.isOnNavMesh) return;

        agent.speed = ventSpeed;
        agent.SetDestination(target.position);
    }

    // =======================================================
    // 발소리 시스템
    // =======================================================
    void UpdateFootsteps()
    {
        if (!footstepSource) return;

        float speedMag = 0f;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            speedMag = agent.velocity.magnitude;

        bool shouldPlay = false;
        AudioClip[] clips = null;
        float interval = groundStepInterval;

        switch (state)
        {
            case State.GroundChase:
                shouldPlay = speedMag > 0.1f;
                clips = groundFootsteps;
                interval = groundStepInterval;
                break;

            case State.VentChase:
                shouldPlay = speedMag > 0.1f;
                clips = (ventFootsteps != null && ventFootsteps.Length > 0)
                        ? ventFootsteps
                        : groundFootsteps;
                interval = ventStepInterval;
                break;

            case State.ClimbLadder:
                // 사다리 타는 중엔 계속 기어가는 느낌
                shouldPlay = true;
                clips = (climbSFX != null && climbSFX.Length > 0)
                        ? climbSFX
                        : groundFootsteps;
                interval = climbStepInterval;
                break;

            default:
                shouldPlay = false;
                break;
        }

        if (shouldPlay && clips != null && clips.Length > 0)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = interval;
                var clip = clips[Random.Range(0, clips.Length)];
                footstepSource.PlayOneShot(clip);
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    // =======================================================
    // 환풍구 끝에서 정지 (외부 트리거)
    // =======================================================
    public void StopAtVentEnd()
    {
        if (state != State.VentChase && state != State.ClimbLadder) return;

        Debug.Log("[MainMonster] 환풍구 끝에서 정지");
        state = State.Stopped;
        agent.enabled = false;

        if (ventEndStopPoint)
        {
            transform.position = ventEndStopPoint.position;
            transform.rotation = ventEndStopPoint.rotation;
        }
    }

    // =======================================================
    // 플레이어 즉사 처리
    // =======================================================
    void KillPlayer()
    {
        Debug.Log("[MainMonster] 플레이어 즉사!");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(9999);
        }
        // 여기서 애니메이션/사운드/게임오버 UI 호출 등 추가 가능
    }

    // 필요하면 외부에서 강제 제거
    public void KillMonster()
    {
        Destroy(gameObject);
    }
}
