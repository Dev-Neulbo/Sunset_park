using UnityEngine;

// 이상 현상 유형 구분
// Instant   즉사형   판정 범위 안에서 위반 행위를 하면 바로 게임오버
// Delayed   지연형   퇴장할 때 해결 안 됐으면 게임오버
// Worsening 악화형   퇴장할 때 해결 안 됐으면 다음 날 다시 등장, 카운트가 쌓이다 넘치면 게임오버
public enum AnomalyType
{
    Instant,
    Delayed,
    Worsening
}

// 모든 이상 현상(러버니, 트램 선로, 시계탑, 분수대 등)의 부모 클래스
// 활성/비활성 상태와 해결 판정 같은 공통 기능을 여기서 담당한다
public abstract class Anomaly : MonoBehaviour
{
    [Header("Anomaly Base")]
    [SerializeField] private AnomalyType anomalyType = AnomalyType.Instant; // 이 이상 현상의 유형
    [SerializeField] private bool startActive = false;                      // 시작하자마자 켜진 상태로 둘지 (DayManager 없이 단독 테스트할 때 사용)

    public AnomalyType Type => anomalyType; // 유형 읽기 전용 접근
    public bool IsActive { get; private set; } // 지금 이 이상 현상이 발생 중인지

    // 파생 클래스에서 유형을 코드로 못박아두고 싶을 때 쓴다 (인스펙터 설정 실수 방지)
    protected void SetTypeInstant() => anomalyType = AnomalyType.Instant;
    protected void SetTypeDelayed() => anomalyType = AnomalyType.Delayed;
    protected void SetTypeWorsening() => anomalyType = AnomalyType.Worsening;

    // 씬에 DayManager가 있으면 켜고 끄는 건 그쪽에 맡기고, 없을 때만 startActive대로 처리한다
    protected virtual void Start()
    {
        if (DayManager.Instance != null)
        {
            return;
        }

        if (startActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    // 이상 현상을 발생시킨다
    public void Activate()
    {
        IsActive = true;
        OnActivated();
    }

    // 이상 현상을 정상 상태로 되돌린다
    public void Deactivate()
    {
        IsActive = false;
        OnDeactivated();
    }

    // 퇴장 시점에 해결됐는지 판정한다 (즉사형은 사용하지 않음)
    public virtual bool IsResolved()
    {
        return !IsActive;
    }

    // 켜질 때의 처리 (스프라이트 교체, 색 변경 등) - 자식이 구현
    protected abstract void OnActivated();

    // 꺼질 때의 처리 - 자식이 구현
    protected abstract void OnDeactivated();
}