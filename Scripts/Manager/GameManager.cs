using UnityEngine;

// 게임 전역 매니저 (임시)
// 지금은 게임오버가 불리면 로그를 찍고 시간을 멈추는 정도만 한다.
// 나중에 사망 연출, 블랙아웃, 1일차 리셋 등으로 확장할 예정.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 어디서든 접근할 수 있는 싱글턴 인스턴스

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

    // 게임오버 처리. reason은 사망 원인(디버그/연출용).
    public void GameOver(string reason)
    {
        Debug.LogWarning($"[GameOver] {reason}");
        Time.timeScale = 0f;
    }
}