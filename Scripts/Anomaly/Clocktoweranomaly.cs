using UnityEngine;

// 시계탑 오브젝트 스크립트 (지연형)
// [매뉴얼 6번] 시계탑의 바늘은 움직이지 않습니다. 만약 움직인다면 태엽을 정확히 3번 감아주시기 바랍니다.
// 태엽을 정확히 3번 감아야 해결되고, 4번 이상 감으면 미해결로 처리한다.
public class ClockTowerAnomaly : Anomaly
{
    [Header("ClockTower Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer; // 상태를 표시할 렌더러
    [SerializeField] private Sprite normalSprite;           // 정상 상태 스프라이트
    [SerializeField] private Sprite anomalySprite;          // 이상 상태 스프라이트

    [Header("Fallback Colors (스프라이트가 없을 때 색으로 구분)")]
    [SerializeField] private bool useColorFallback = true; // 스프라이트 대신 색으로 상태를 표시할지
    [SerializeField] private Color normalColor = Color.white; // 정상 상태 색
    [SerializeField] private Color anomalyColor = Color.red;  // 이상 상태 색

    [Header("Wind Settings")]
    [SerializeField] private int requiredWinds = 3; // 해결에 필요한 정확한 태엽 감기 횟수

    private int windCount; // 지금까지 태엽을 감은 횟수

    public int WindCount => windCount;       // 감은 횟수 읽기
    public int RequiredWinds => requiredWinds; // 필요 횟수 읽기

    // 이상 상태로 켜질 때: 횟수를 초기화하고 이상 표시로 바꾼다
    protected override void OnActivated()
    {
        windCount = 0;

        if (spriteRenderer != null && anomalySprite != null)
        {
            spriteRenderer.sprite = anomalySprite;
        }
        if (useColorFallback && spriteRenderer != null)
        {
            spriteRenderer.color = anomalyColor;
        }
    }

    // 정상 상태로 돌아올 때: 횟수를 초기화하고 정상 표시로 바꾼다
    protected override void OnDeactivated()
    {
        windCount = 0;

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
        if (useColorFallback && spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    // 태엽을 한 번 감는다 (ClockWinder가 F키 상호작용 때 호출)
    public void Wind()
    {
        if (!IsActive) return;

        windCount++;
        Debug.Log($"[ClockTower] 태엽을 감았습니다. ({windCount}회)");

        if (windCount == requiredWinds)
        {
            Debug.Log("[ClockTower] 정확히 3번 감았습니다. (현재 해결 상태)");
        }
        else if (windCount > requiredWinds)
        {
            Debug.Log("[ClockTower] 너무 많이 감았습니다! (미해결 상태로 전환)");
        }
    }

    // 퇴장 시점 해결 판정: 정확히 필요 횟수만큼 감았을 때만 해결로 본다
    public override bool IsResolved()
    {
        if (!IsActive) return true;
        return windCount == requiredWinds;
    }
}