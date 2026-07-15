using UnityEngine;

// 퇴장 지점 오브젝트 스크립트
// 플레이어의 x 위치가 이 지점을 넘어서면 DayManager에 순찰 완료를 알린다.
// 트리거 충돌 대신 위치 비교로 판정해서 놓치는 경우가 없다.
public class ExitZone : MonoBehaviour
{
    [Tooltip("플레이어 (비워두면 Player 태그로 자동 탐색)")]
    [SerializeField] private Transform player; // 위치를 비교할 플레이어

    [Tooltip("true면 플레이어가 오른쪽으로 이 지점을 넘을 때 발동합니다.")]
    [SerializeField] private bool triggerWhenRight = true; // 판정 방향

    private bool reached; // 이미 도달 처리했는지 (하루에 한 번만 발동)

    // 플레이어 참조가 비어 있으면 태그로 찾아온다
    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    // 매 프레임 플레이어가 퇴장선을 넘었는지 확인하고, 넘었으면 DayManager에 알린다
    private void Update()
    {
        if (reached || player == null) return;

        bool crossed = triggerWhenRight
            ? player.position.x >= transform.position.x
            : player.position.x <= transform.position.x;

        if (crossed)
        {
            reached = true;
            Debug.Log("[ExitZone] 퇴장 지점 도달 - 순찰 완료");

            if (DayManager.Instance != null)
            {
                DayManager.Instance.OnReachExit();
            }
        }
    }

    // 다음 날 다시 통과할 수 있도록 도달 상태를 초기화한다 (DayManager가 호출)
    public void ResetZone()
    {
        reached = false;
    }
}