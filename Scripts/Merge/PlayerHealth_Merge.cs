using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth_Merge : MonoBehaviour
{
    // ===== [PlayerHealth_YH]���� �� ���� =====
    [Header("HP")]
    public float Max = 100f;
    public float Current = 100f;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDead;

    private bool isDead = false;
    private bool invincible = false;   // ���� ���� �߰�

    public bool IsDead => isDead;

    // ===== [PlayerHealth]���� �� ���� =====
    [Header("UI References")]
    public Slider hpSlider;       // ü�¹�
    public GameObject gameOverUI; // Game Over UI
    public GameObject hudUI;      // ��� HUD (��ȣ�� UI)
    public GameObject crosshair;    // 십자선 UI
    public GameObject QuestUI;      //퀘스트 UI

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // ���� �޴� �� �̸�

    void Awake()
    {
        // [PlayerHealth_YH]
        Current = Mathf.Clamp(Current, 0f, Max);
    }

    void Start()
    {
        // [PlayerHealth] �ʱ�ȭ �帧�� Max/Current ������� ����
        Current = Mathf.Clamp(Current, 0f, Max);

        // HP Slider �ʱ�ȭ
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = Max;
            hpSlider.value = Current;
        }

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // ������ �� ���콺 �����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // �׽�Ʈ��: L Ű ������ ü�� 1 ����
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(1);
            Debug.Log("ü�� ����! ���� ü�� (int �ùķ��̼�): " + Mathf.RoundToInt(Current));
        }
    }

    // ===== ������ / ȸ�� (�����ε�) =====
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || invincible) return;  // �����̸� ����

        float delta = Mathf.Abs(amount);
        float prev = Current;

        Current = Mathf.Clamp(Current - delta, 0f, Max);
        onDamaged?.Invoke();

        if (hpSlider != null)
            hpSlider.value = Current;

        if (Current <= 0f && !isDead)
        {
            isDead = true;
            onDead?.Invoke();
            GameOver(); // UI/���� ó��
        }

        Debug.Log($"[�÷��̾�HP] �ǰ�: -{delta:0.##} �� {Current:0.##}/{Max}");
    }

    public void Heal(int amount)
    {
        Heal((float)amount);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        float delta = Mathf.Abs(amount);
        float prev = Current;

        Current = Mathf.Clamp(Current + delta, 0f, Max);
        onHealed?.Invoke();

        if (hpSlider != null)
            hpSlider.value = Current;

        Debug.Log($"[�÷��̾�HP] ȸ��: +{delta:0.##} �� {Current:0.##}/{Max}");
    }

    public void SetMax(float newMax, bool refill = true)
    {
        Max = Mathf.Max(1f, newMax);
        if (refill) Current = Max;
        else Current = Mathf.Clamp(Current, 0f, Max);

        if (hpSlider != null)
        {
            hpSlider.maxValue = Max;
            hpSlider.value = Current;
        }
    }

    // �߰�: ���� ���� ON/OFF
    public void SetInvincible(bool enable)
    {
        invincible = enable;
        Debug.Log($"[�÷��̾�HP] ���� ����: {(enable ? "ON" : "OFF")}");
    }

    // ===== GameOver/��ư ó�� (PlayerHealth ���� ����) =====
    void GameOver()
    {
        // HUD ����
        if (hudUI != null)
            hudUI.SetActive(false);

        // crosshair ����
        if (crosshair != null)
            crosshair.SetActive(false);

        // QuestUI ����
        if (QuestUI != null)
            QuestUI.SetActive(false);

        // Game Over UI ǥ��
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // ���� ����
        Time.timeScale = 0f;

        // ���콺 ���̰� + UI Ŭ�� �����ϰ�
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ������ Game Over ��ų �� (��: ���� ������ 5�ܰ�)
    public void ForceGameOver()
    {
        if (!isDead)
        {
            Current = 0f; // �ߺ� ȣ�� ����
            if (hpSlider != null) hpSlider.value = Current;

            isDead = true;
            onDead?.Invoke();
            GameOver();
        }
    }

    // UI ��ư���� ȣ���� �Լ�
    public void OnClickRestart()
    {
        Time.timeScale = 1f;

        // ���� �޴��� �̵�
        var sceneToLoad = string.IsNullOrEmpty(mainMenuSceneName) ? "MainMenuScene" : mainMenuSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
