using UnityEngine;

// 트램 선로 위 장애물 오브젝트 스크립트
// 플레이어가 F키로 상호작용하면 연결된 트램 선로에게 장애물을 치우라고 알린다.
[RequireComponent(typeof(Collider2D))]
public class TramObstacle : Interactable
{
    [Tooltip("이 장애물이 속한 트램 선로. 비워두면 부모에서 자동으로 찾습니다.")]
    [SerializeField] private TramTrackAnomaly tramTrack; // 제거를 전달할 트램 선로 본체

    // 연결이 비어 있으면 부모에서 트램 선로를 찾아온다
    private void Awake()
    {
        if (tramTrack == null)
        {
            tramTrack = GetComponentInParent<TramTrackAnomaly>();
        }
    }

    // F키 상호작용 시 호출: 트램 선로에게 장애물 제거를 전달
    public override void Interact()
    {
        if (tramTrack != null)
        {
            tramTrack.ClearObstacle();
        }
        else
        {
            Debug.LogWarning("[TramObstacle] 연결된 TramTrackAnomaly가 없습니다.");
        }
    }
}