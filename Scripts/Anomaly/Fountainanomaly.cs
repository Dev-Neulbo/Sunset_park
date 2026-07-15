using UnityEngine;

// 분수대 오브젝트 스크립트 (악화형)
// [매뉴얼 4번] 선셋파크의 수질은 항상 최고 수준을 유지해야 합니다.
// 물은 기본적으로 꺼져 있고, 수도꼭지로 물을 틀어야 색을 확인할 수 있다.
// 물이 붉으면 정수 버튼을 눌러 정상화해야 하며, 정수 버튼은 물이 켜져 있을 때만 작동한다.
// 해결하지 못하고 퇴장하면 다음 날 카운트가 쌓인 채로 다시 등장하고, 카운트가 넘치면 게임오버된다.
//
// 카운트별 등장 방식
// 1회차: 물이 꺼진 채로 등장. 수도꼭지로 물을 틀어 확인 후 정수.
// 2회차: 악화되어 물이 이미 틀어진 채로 등장. 수도꼭지를 조작해도 물이 꺼지지 않고, 정수로만 해결 가능.
// 3회차: 최대 카운트를 넘겨 등장 즉시 게임오버.
public class FountainAnomaly : Anomaly
{
    [Header("Fountain Visuals")]
    [Tooltip("물 오브젝트의 SpriteRenderer (색으로 상태 표시). 비워두면 본체 것을 사용.")]
    [SerializeField] private SpriteRenderer waterRenderer; // 물 색을 바꿔서 상태를 보여줄 렌더러

    [Header("Water Colors")]
    [SerializeField] private Color waterOffColor = Color.yellow;     // 물이 꺼진 기본 색
    [SerializeField] private Color normalWaterColor = Color.yellow;  // 정수 후 정상 물 색
    [SerializeField] private Color anomalyWaterColor = Color.red;    // 이상 상태(붉은 물) 색

    [Header("Worsening Settings")]
    [SerializeField] private int maxCount = 2;          // 최대 등장 카운트, 이걸 넘기면 게임오버
    [SerializeField] private int worsenedCount = 2;     // 이 카운트부터 악화 상태(물이 틀어진 채 등장, 수도꼭지 잠금)

    private bool waterOn;    // 물이 틀어져 있는지
    private bool purified;   // 이번 등장에서 정수(해결)했는지
    private int appearCount; // 이상 상태로 등장한 누적 횟수

    public int AppearCount => appearCount; // 누적 등장 횟수 읽기
    public bool IsWaterOn => waterOn;      // 물 켜짐 여부 읽기
    public bool IsWorsened => IsActive && !purified && appearCount >= worsenedCount; // 지금 악화 상태인지

    // 유형을 악화형으로 못박는다
    private void Awake()
    {
        SetTypeWorsening();
    }

    // 이상 상태로 켜질 때: 카운트를 올리고, 넘쳤으면 게임오버.
    // 악화 카운트 이상이면 물이 이미 틀어진 채로 등장한다.
    protected override void OnActivated()
    {
        appearCount++;
        purified = false;

        Debug.Log($"[Fountain] 이상 등장 ({appearCount}회째 / 최대 {maxCount})");

        if (appearCount > maxCount)
        {
            Debug.LogWarning("[Fountain] 최대 등장 횟수 초과 → 게임오버");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("분수대의 오염이 걷잡을 수 없이 악화되었습니다.");
            }
            return;
        }

        // 악화 상태면 물이 이미 틀어진 채로, 아니면 꺼진 채로 시작
        waterOn = appearCount >= worsenedCount;

        if (waterOn)
        {
            Debug.Log("[Fountain] 악화 상태 - 물이 이미 틀어진 채로 등장 (수도꼭지 잠김)");
        }

        UpdateWaterVisual();
    }

    // 정상 상태로 돌아올 때: 상태값 초기화 후 표시 갱신
    protected override void OnDeactivated()
    {
        purified = false;
        waterOn = false;
        UpdateWaterVisual();
    }

    // 수도꼭지 상호작용: 물을 켜고 끈다. 악화 상태에서는 물을 끌 수 없다.
    public void ToggleWater()
    {
        if (!IsActive) return;

        // 악화 상태에서 물을 끄려는 경우는 막는다 (정수로만 해결 가능)
        if (IsWorsened && waterOn)
        {
            Debug.Log("[Fountain] 수도꼭지가 잠기지 않습니다. (정수 버튼을 눌러야 합니다)");
            return;
        }

        waterOn = !waterOn;
        Debug.Log($"[Fountain] 수도꼭지 조작 → 물 {(waterOn ? "ON" : "OFF")}");
        UpdateWaterVisual();
    }

    // 정수 버튼 상호작용: 물이 켜져 있을 때만 정화(해결)되고 누적 카운트를 초기화한다.
    // 정수 후에는 수도꼭지가 꺼진 정상 상태가 된다.
    public void Purify()
    {
        if (!IsActive) return;

        if (!waterOn)
        {
            Debug.Log("[Fountain] 물이 꺼져 있어 정수 버튼이 작동하지 않습니다. (먼저 물을 트세요)");
            return;
        }

        purified = true;
        appearCount = 0;
        waterOn = false; // 정상 상태가 되면 수도꼭지도 꺼진다
        Debug.Log("[Fountain] 정수 완료 - 수질 정상화 (해결)");
        UpdateWaterVisual();
    }

    // 현재 상태(정수됨 / 물 켜짐 / 물 꺼짐)에 맞춰 물 색을 바꾼다
    private void UpdateWaterVisual()
    {
        if (waterRenderer == null) return;

        if (purified || !IsActive)
        {
            waterRenderer.color = normalWaterColor;
        }
        else if (waterOn)
        {
            waterRenderer.color = anomalyWaterColor;
        }
        else
        {
            waterRenderer.color = waterOffColor;
        }
    }

    // 퇴장 시점 해결 판정: 정수했으면 해결로 본다
    public override bool IsResolved()
    {
        if (!IsActive) return true;
        return purified;
    }

    // 누적 카운트를 완전히 초기화한다 (1일차 리셋이나 새 게임 시작 시 사용)
    public void ResetCount()
    {
        appearCount = 0;
        purified = false;
        waterOn = false;
    }
}