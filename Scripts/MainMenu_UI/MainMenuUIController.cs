using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject MainPanel;      // 메인 버튼 패널
    public GameObject OptionPanel;    // 옵션 패널

    [Header("Scene")]
    public string GameSceneName = "GameScene"; // 시작 버튼 클릭 시 이동할 게임 씬 이름                       <<<<<<<<<<<<<<<<<<<<-----------이거 건듬

    private void Start()
    {
        // 시작 시 메인 패널만 보이도록 설정하는 함수
        ShowMainPanel();
    }

    public void OnClickStart()
    {
        // 게임 시작 버튼 클릭 시 게임 씬으로 이동하는 함수
        SceneManager.LoadScene(GameSceneName);
    }

    public void OnClickOpenOption()
    {
        // 옵션 버튼 클릭 시 옵션 패널을 여는 함수
        MainPanel.SetActive(false);
        OptionPanel.SetActive(true);
    }

    public void OnClickBackFromOption()
    {
        // 옵션에서 뒤로가기 버튼 클릭 시 메인 패널로 돌아가는 함수
        ShowMainPanel();
    }

    public void OnClickQuit()
    {
        // 종료 버튼 클릭 시 애플리케이션을 종료하는 함수
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ShowMainPanel()
    {
        // 메인 패널 표시 상태를 초기화하는 함수
        MainPanel.SetActive(true);
        OptionPanel.SetActive(false);
    }
}
