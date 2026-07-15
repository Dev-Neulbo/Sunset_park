using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public GameObject ManualPanel; // 메뉴얼 패널 변수
    public GameObject PauseDimPanel; // 일시정지 딤 패널 변수
    public GameObject PauseSettingPanel; // 일시정지 설정 패널 변수
    public MonoBehaviour PlayerController; // 플레이어 조작 스크립트 변수
    public string MainMenuSceneName = "MainMenuScene"; // 메인 메뉴 씬 이름 변수

    private bool _isManualOpen; // 메뉴얼 열림 상태 변수
    private bool _isPauseOpen; // 일시정지 열림 상태 변수

    private void Start()
    {
        _isManualOpen = false;
        _isPauseOpen = false;
        if (ManualPanel != null) ManualPanel.SetActive(false);
        if (PauseDimPanel != null) PauseDimPanel.SetActive(false);
        if (PauseSettingPanel != null) PauseSettingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.tabKey.wasPressedThisFrame) ToggleManualPanel();
        if (keyboard.escapeKey.wasPressedThisFrame) TogglePausePanel();
    }

    public void ToggleManualPanel()
    {
        if (_isPauseOpen) return;
        _isManualOpen = !_isManualOpen;
        if (ManualPanel != null) ManualPanel.SetActive(_isManualOpen);
    }

    public void TogglePausePanel()
    {
        _isPauseOpen = !_isPauseOpen;

        if (_isPauseOpen && _isManualOpen)
        {
            _isManualOpen = false;
            if (ManualPanel != null) ManualPanel.SetActive(false);
        }

        if (PauseDimPanel != null) PauseDimPanel.SetActive(_isPauseOpen);
        if (PauseSettingPanel != null) PauseSettingPanel.SetActive(_isPauseOpen);

        Time.timeScale = _isPauseOpen ? 0f : 1f;
        AudioListener.pause = _isPauseOpen;
        if (PlayerController != null) PlayerController.enabled = !_isPauseOpen;
    }

    public void OnClickResume()
    {
        _isPauseOpen = false;
        if (PauseDimPanel != null) PauseDimPanel.SetActive(false);
        if (PauseSettingPanel != null) PauseSettingPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (PlayerController != null) PlayerController.enabled = true;
    }

    public void OnClickBackToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(MainMenuSceneName);
    }
}