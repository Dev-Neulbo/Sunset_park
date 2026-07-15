using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionAudioController : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer MainMixer;             // 볼륨을 제어할 오디오 믹서
    public string MasterVolumeParam = "MasterVolume"; // 오디오 믹서에 노출된 파라미터 이름

    [Header("UI")]
    public Slider MasterVolumeSlider;        // 마스터 볼륨 슬라이더 (0.0001 ~ 1 권장)

    [Header("Save Key")]
    public string MasterVolumeSaveKey = "MASTER_VOLUME"; // PlayerPrefs 저장 키

    private void Awake()
    {
        // 슬라이더 최소값이 0이면 Log 계산 문제가 생겨서 작은 값으로 보정하는 함수
        if (MasterVolumeSlider != null && MasterVolumeSlider.minValue <= 0f)
        {
            MasterVolumeSlider.minValue = 0.0001f;
        }
    }

    private void Start()
    {
        // 저장된 볼륨을 불러와 슬라이더/믹서에 반영하는 함수
        float savedVolume = PlayerPrefs.GetFloat(MasterVolumeSaveKey, 1f);

        if (MasterVolumeSlider != null)
        {
            MasterVolumeSlider.value = savedVolume;
        }

        ApplyMasterVolume(savedVolume);
    }

    public void OnChangedMasterVolume(float sliderValue)
    {
        // 슬라이더 변경 시 오디오 믹서와 저장값을 갱신하는 함수
        ApplyMasterVolume(sliderValue);
        PlayerPrefs.SetFloat(MasterVolumeSaveKey, sliderValue);
        PlayerPrefs.Save();
    }

    private void ApplyMasterVolume(float linearValue)
    {
        // 0~1 선형값을 dB로 변환해 오디오 믹서에 적용하는 함수
        float clampedValue = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float decibel = Mathf.Log10(clampedValue) * 20f;
        MainMixer.SetFloat(MasterVolumeParam, decibel);
    }
}
