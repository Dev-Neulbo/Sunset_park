using UnityEngine;

// 자이로드롭 오브젝트 스크립트 (즉사형)
// [매뉴얼 10번] 자이로드롭은 간혹 스스로 운행하는 경우가 있습니다. 그 아래를 지나가는 것은 추천하지 않습니다.
//              자이로드롭이 보이지 않을 때까지 물러났다가 운행을 멈추면 지나가세요.
// 이상 상태로 운행 중일 때 판정 범위에 들어가면 손전등과 상관없이 게임오버된다.
// 플레이어가 자이로드롭을 화면 밖으로 벗어나게 하면 운행이 멈추고, 그 뒤엔 안전하게 지나갈 수 있다.
public class GyroDropAnomaly : Anomaly
{
    [Header("GyroDrop Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer; // 상태를 표시할 렌더러
    [Tooltip("정상 상태(시트 내려감) 스프라이트")]
    [SerializeField] private Sprite normalSprite;  // 정상(시트 내려감) 스프라이트
    [Tooltip("이상 상태(시트 올라감/운행 중) 스프라이트")]
    [SerializeField] private Sprite anomalySprite; // 이상(시트 올라감) 스프라이트

    [Header("Fallback Colors (스프라이트가 없을 때 색으로 구분)")]
    [SerializeField] private bool useColorFallback = true; // 스프라이트 대신 색으로 표시할지
    [SerializeField] private Color normalColor = Color.white;  // 정상 상태 색
    [SerializeField] private Color runningColor = Color.red;   // 운행 중(위험) 색
    [SerializeField] private Color stoppedColor = Color.yellow;// 운행 정지(안전) 색

    [Header("Detection")]
    [SerializeField] private float detectRange = 2f; // 판정으로 삼을 플레이어와의 거리

    [Header("Off-Screen Stop")]
    [Tooltip("화면 밖으로 이만큼(뷰포트 비율) 완전히 벗어나야 운행이 멈춥니다.")]
    [SerializeField] private float offScreenMargin = 0.05f; // 화면 밖 판정 여유값

    [Header("Fake Condition (확장용)")]
    [Tooltip("정기 점검(무음) 상태로 두면 이상 상태여도 진입해도 안전합니다.")]
    [SerializeField] private bool isSilentInspection = false; // 사운드 시스템 연동 전 임시 안전 플래그

    private Transform playerTransform; // 플레이어 위치 참조
    private Camera mainCamera;         // 화면 안/밖 판정에 쓸 메인 카메라
    private bool triggered;            // 이미 발동했는지 (중복 게임오버 방지)
    private bool isRunning = true;     // 운행 중이면 위험, 멈췄으면 안전
    private bool hasBeenSeen = false;  // 플레이어가 한 번이라도 화면에서 마주쳤는지

    // 플레이어와 카메라 참조를 미리 찾아둔다
    protected override void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        mainCamera = Camera.main;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        base.Start();
    }

    // 이상 상태로 켜질 때: 운행 중 상태로 시작
    protected override void OnActivated()
    {
        isRunning = true;
        UpdateVisual();
    }

    // 정상 상태로 돌아올 때: 정상 표시로 바꾼다
    protected override void OnDeactivated()
    {
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        if (useColorFallback && spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    // 운행/정지 상태에 맞춰 표시를 갱신한다
    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        if (anomalySprite != null)
        {
            spriteRenderer.sprite = anomalySprite;
        }

        if (useColorFallback)
        {
            spriteRenderer.color = isRunning ? runningColor : stoppedColor;
        }
    }

    // 이 오브젝트가 지금 카메라 화면 안에 보이는지 반환한다
    private bool IsVisibleOnScreen()
    {
        if (mainCamera == null) return true;

        Vector3 vp = mainCamera.WorldToViewportPoint(transform.position);

        if (vp.z < 0f) return false; // 카메라 뒤면 안 보이는 것으로 처리

        return vp.x >= -offScreenMargin && vp.x <= 1f + offScreenMargin
            && vp.y >= -offScreenMargin && vp.y <= 1f + offScreenMargin;
    }

    // 매 프레임 판정: 화면 밖으로 벗어나면 운행 정지, 운행 중 판정 범위에 들어오면 게임오버
    private void Update()
    {
        if (!IsActive || playerTransform == null) return;

        bool visible = IsVisibleOnScreen();

        if (visible)
        {
            hasBeenSeen = true;
        }

        // 한 번 마주친 뒤 화면 밖으로 벗어나면 운행을 멈춘다 (한 번 멈추면 계속 유지)
        if (hasBeenSeen && isRunning && !visible)
        {
            isRunning = false;
            UpdateVisual();
        }

        if (triggered) return;

        // 정기 점검(무음)이거나 운행이 멈춘 상태면 안전
        if (isSilentInspection || !isRunning) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= detectRange)
        {
            triggered = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("운행 중인 자이로드롭 아래를 지나갔습니다.");
            }
            else
            {
                Debug.LogWarning("[GameOver] 운행 중인 자이로드롭 아래를 지나갔습니다. (GameManager 없음)");
            }
        }
    }
}