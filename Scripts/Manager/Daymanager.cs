using System.Collections.Generic;
using UnityEngine;

// 일차/순찰 흐름 관리 매니저
// 하루를 시작(플레이어 위치 리셋 + 이상현상 세팅)하고, 플레이어가 퇴장 지점에 닿으면
// 이상현상 해결 여부를 검사해서 다음 날로 넘길지 게임오버할지 결정한다.
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; } // 어디서든 접근할 수 있는 싱글턴 인스턴스

    [Header("Day Settings")]
    [SerializeField] private int currentDay = 1;  // 현재 며칠차인지
    [SerializeField] private int finalDay = 10;   // 마지막 날 (이 날을 넘기면 클리어)

    [Header("Player Reset")]
    [Tooltip("하루 시작 시 플레이어가 위치할 시작 지점")]
    [SerializeField] private Transform startPoint; // 매일 플레이어를 되돌릴 시작 위치
    [Tooltip("플레이어 (비워두면 Player 태그로 자동 탐색)")]
    [SerializeField] private Transform player;     // 플레이어 참조

    [Header("Anomalies")]
    [Tooltip("씬의 모든 이상현상. 비워두면 시작 시 자동으로 수집합니다.")]
    [SerializeField] private List<Anomaly> anomalies = new List<Anomaly>(); // 관리할 이상현상 목록

    [Header("Exit")]
    [Tooltip("퇴장 지점 (비워두면 자동 탐색)")]
    [SerializeField] private ExitZone exitZone; // 퇴장 지점 참조

    public int CurrentDay => currentDay; // 현재 날짜 읽기

    // 싱글턴 등록 (이미 있으면 자기 자신을 파괴)
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 참조 자동 연결(플레이어/이상현상/퇴장지점) 후 첫 날을 시작한다
    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (anomalies == null || anomalies.Count == 0)
        {
            anomalies = new List<Anomaly>(FindObjectsByType<Anomaly>(FindObjectsSortMode.None));
        }
        Debug.Log($"[DayManager] 이상현상 {anomalies.Count}개 수집됨");

        if (exitZone == null)
        {
            exitZone = FindFirstObjectByType<ExitZone>();
        }

        StartDay();
    }

    // 하루를 시작한다: 플레이어를 시작 지점으로 옮기고, 이상현상과 퇴장 지점을 세팅한다
    public void StartDay()
    {
        Debug.Log($"[DayManager] {currentDay}일차 시작");

        if (player != null && startPoint != null)
        {
            player.position = startPoint.position;
        }

        SetupAnomaliesForToday();

        if (exitZone != null)
        {
            exitZone.ResetZone();
        }
    }

    // 오늘 등장할 이상현상을 세팅한다 (지금은 모두 활성화, 나중에 랜덤 선택 로직으로 교체 예정)
    private void SetupAnomaliesForToday()
    {
        foreach (var anomaly in anomalies)
        {
            if (anomaly == null) continue;

            anomaly.Activate();
        }
    }

    // 플레이어가 퇴장 지점에 닿았을 때 호출된다 (ExitZone이 호출)
    // 검사를 통과하면 다음 날로 넘어간다
    public void OnReachExit()
    {
        bool allResolved = CheckAnomaliesOnExit();

        if (allResolved)
        {
            AdvanceDay();
        }
    }

    // 퇴장 시점 이상현상 검사
    // 지연형이 미해결이면 게임오버, 악화형이 미해결이면 다음 날로 넘기되 다시 등장하게 둔다
    private bool CheckAnomaliesOnExit()
    {
        Debug.Log($"[DayManager] 퇴장 체크 시작 - 이상현상 {anomalies.Count}개 검사");

        foreach (var anomaly in anomalies)
        {
            if (anomaly == null || !anomaly.IsActive) continue;

            bool resolved = anomaly.IsResolved();
            Debug.Log($"[DayManager] 검사: {anomaly.name} (Type={anomaly.Type}, Active={anomaly.IsActive}, Resolved={resolved})");

            if (resolved) continue;

            if (anomaly.Type == AnomalyType.Delayed)
            {
                Debug.LogWarning($"[DayManager] 지연형 미해결: {anomaly.name} → 게임오버");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver($"{anomaly.name} 이상 현상을 처리하지 못했습니다.");
                }
                return false;
            }
            else if (anomaly.Type == AnomalyType.Worsening)
            {
                Debug.LogWarning($"[DayManager] 악화형 미해결: {anomaly.name} → 다음날 악화되어 재등장");
            }
        }

        return true;
    }

    // 다음 날로 넘어간다. 마지막 날을 넘기면 클리어 처리한다.
    public void AdvanceDay()
    {
        if (currentDay >= finalDay)
        {
            Debug.Log("[DayManager] 모든 순찰 완료! 게임 클리어");
            return;
        }

        currentDay++;
        StartDay();
    }

    // 1일차로 되돌린다. 악화형 누적 카운트도 함께 초기화한다.
    public void ResetToDayOne()
    {
        Debug.Log("[DayManager] 1일차로 리셋");
        currentDay = 1;

        foreach (var anomaly in anomalies)
        {
            if (anomaly is FountainAnomaly fountain)
            {
                fountain.ResetCount();
            }
        }

        StartDay();
    }
}