using UnityEngine;

// 시계탑 태엽 오브젝트 스크립트
// 플레이어가 F키로 상호작용할 때마다 연결된 시계탑에게 태엽을 감으라고 알린다.
[RequireComponent(typeof(Collider2D))]
public class ClockWinder : Interactable
{
    [Tooltip("이 태엽이 속한 시계탑. 비워두면 부모에서 자동으로 찾습니다.")]
    [SerializeField] private ClockTowerAnomaly clockTower; // 감기를 전달할 시계탑 본체

    // 연결이 비어 있으면 부모 오브젝트에서 시계탑을 찾아온다
    private void Awake()
    {
        if (clockTower == null)
        {
            clockTower = GetComponentInParent<ClockTowerAnomaly>();
        }
    }

    // F키 상호작용 시 호출: 시계탑에게 태엽 감기를 전달
    public override void Interact()
    {
        if (clockTower != null)
        {
            clockTower.Wind();
        }
        else
        {
            Debug.LogWarning("[ClockWinder] 연결된 ClockTowerAnomaly가 없습니다.");
        }
    }
}