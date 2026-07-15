using UnityEngine;

// 푸드트럭 오브젝트 스크립트 (즉사형)
// [매뉴얼 8번] 푸드트럭의 메뉴는 솜사탕, 핫도그, 츄러스로 항상 동일합니다.
//             만약 그렇지 않다면 손전등을 끄고 지나가세요. 푸드트럭 근무자에게 발각 시 선셋파크는 책임을 지지 않습니다.
// 이상 상태일 때 판정 범위 안에서 손전등을 켜면 발각되어 게임오버된다.
public class FoodTruckAnomaly : Anomaly
{
    [Header("FoodTruck Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer; // 상태를 표시할 렌더러
    [Tooltip("정상 상태(솜사탕/핫도그/츄러스) 스프라이트")]
    [SerializeField] private Sprite normalSprite;  // 정상 메뉴 스프라이트
    [Tooltip("이상 상태(메뉴가 다름) 스프라이트")]
    [SerializeField] private Sprite anomalySprite; // 이상 메뉴 스프라이트

    [Header("Fallback Colors (스프라이트가 없을 때 색으로 구분)")]
    [SerializeField] private bool useColorFallback = true; // 스프라이트 대신 색으로 표시할지
    [SerializeField] private Color normalColor = Color.white; // 정상 상태 색
    [SerializeField] private Color anomalyColor = Color.red;  // 이상 상태 색

    [Header("Detection")]
    [SerializeField] private float detectRange = 1.5f; // 판정으로 삼을 플레이어와의 거리

    private Flashlight playerFlashlight; // 플레이어 손전등 참조 (켜짐 여부 확인용)
    private Transform playerTransform;   // 플레이어 위치 참조 (거리 계산용)
    private bool triggered;              // 이미 발동했는지 (중복 게임오버 방지)

    // 플레이어와 손전등 참조를 미리 찾아둔다
    protected override void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerFlashlight = player.GetComponent<Flashlight>();
        }

        base.Start();
    }

    // 이상 상태로 켜질 때: 이상 메뉴 표시
    protected override void OnActivated()
    {
        if (spriteRenderer != null && anomalySprite != null)
        {
            spriteRenderer.sprite = anomalySprite;
        }
        if (useColorFallback && spriteRenderer != null)
        {
            spriteRenderer.color = anomalyColor;
        }
    }

    // 정상 상태로 돌아올 때: 정상 메뉴 표시
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

    // 매 프레임 판정: 이상 상태 + 판정 범위 안 + 손전등 켜짐이면 게임오버
    private void Update()
    {
        if (!IsActive || playerTransform == null || playerFlashlight == null) return;
        if (triggered) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= detectRange && playerFlashlight.IsOn)
        {
            triggered = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("푸드트럭 근무자에게 발각되었습니다.");
            }
            else
            {
                Debug.LogWarning("[GameOver] 푸드트럭 근무자에게 발각되었습니다. (GameManager 없음)");
            }
        }
    }
}