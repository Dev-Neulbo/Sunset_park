using UnityEngine;

// 트램 선로 오브젝트 스크립트 (지연형)
// [매뉴얼 5번] 트램 선로는 안전을 위해 항상 비어 있는 상태로 유지해야 합니다.
// 이상 상태 = 선로 위에 장애물(가방/안구)이 놓여 있음. 장애물에 F키로 상호작용해 치우면 해결된다.
// 퇴장할 때까지 치우지 못하면 게임오버.
public class TramTrackAnomaly : Anomaly
{
    [Header("Tram Obstacle")]
    [Tooltip("선로 위에 놓이는 장애물 오브젝트. 이상 상태일 때 나타나고 상호작용으로 사라집니다.")]
    [SerializeField] private GameObject obstacle; // 선로 위 장애물 오브젝트

    private bool cleared; // 장애물을 치웠는지(해결 여부)

    // 이상 상태로 켜질 때: 장애물을 나타나게 한다
    protected override void OnActivated()
    {
        cleared = false;
        if (obstacle != null)
        {
            obstacle.SetActive(true);
        }
    }

    // 정상 상태로 돌아올 때: 장애물을 숨긴다
    protected override void OnDeactivated()
    {
        cleared = false;
        if (obstacle != null)
        {
            obstacle.SetActive(false);
        }
    }

    // 장애물을 치운다 (TramObstacle이 F키 상호작용 때 호출)
    public void ClearObstacle()
    {
        if (!IsActive) return;

        cleared = true;
        if (obstacle != null)
        {
            obstacle.SetActive(false);
        }

        Debug.Log("[TramTrack] 선로 위 오브젝트를 제거했습니다.");
    }

    // 퇴장 시점 해결 판정: 장애물을 치웠으면 해결로 본다
    public override bool IsResolved()
    {
        if (!IsActive) return true;
        return cleared;
    }
}