using UnityEngine;

// 카메라 추적 스크립트
// 플레이어를 부드럽게 따라간다. 딱 붙지 않고 약간 지연을 두어 은근한 무빙을 만든다.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("비워두면 Player 태그, 그다음 이름으로 자동으로 찾습니다.")]
    [SerializeField] private Transform target;             // 따라갈 대상
    [SerializeField] private string autoFindTag = "Player"; // 대상을 찾을 때 쓸 태그
    [SerializeField] private string autoFindName = "player";// 태그로 못 찾을 때 쓸 이름

    [Header("Follow Settings")]
    [Tooltip("값이 작을수록 더 빠르게, 클수록 더 느리고 부드럽게 따라갑니다.")]
    [SerializeField] private float smoothTime = 0.35f; // 추적 부드러움 정도

    [Tooltip("카메라와 대상 사이의 간격. z는 카메라가 뒤로 물러난 거리.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // 대상 기준 오프셋

    [Header("Axis Lock (횡스크롤이므로 보통 Y는 고정)")]
    [SerializeField] private bool lockY = true;   // 세로 위치를 고정할지
    [SerializeField] private float fixedY = 0f;   // 고정할 세로 위치

    [Header("Bounds (선택사항, 카메라 이동 제한)")]
    [SerializeField] private bool useBounds = false; // 좌우 이동 범위를 제한할지
    [SerializeField] private float minX = -10f;      // 카메라 왼쪽 한계
    [SerializeField] private float maxX = 10f;       // 카메라 오른쪽 한계

    private Vector3 velocity = Vector3.zero; // SmoothDamp가 내부적으로 쓰는 속도값

    // 대상이 비어 있으면 태그, 그다음 이름 순으로 찾아 연결한다
    private void Awake()
    {
        if (target != null) return;

        GameObject found = GameObject.FindGameObjectWithTag(autoFindTag);

        if (found == null && !string.IsNullOrEmpty(autoFindName))
        {
            found = GameObject.Find(autoFindName);
        }

        if (found != null)
        {
            target = found.transform;
        }
        else
        {
            Debug.LogWarning("[CameraFollow] target을 자동으로 찾지 못했습니다. 인스펙터에서 직접 연결해주세요.");
        }
    }

    // 대상 위치로 부드럽게 카메라를 이동시킨다 (축 고정/범위 제한 적용)
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (lockY)
        {
            desiredPosition.y = fixedY;
        }

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }

    // 따라갈 대상을 바꾼다
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}