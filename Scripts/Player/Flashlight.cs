using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal; // Light2D

// 손전등 스크립트
// 스페이스바로 손전등을 켜고 끈다. 
// 플레이어가 방향을 바꾸면 손전등 각도와 위치도 좌우로 뒤집어 정면을 계속 비추게 한다.
public class Flashlight : MonoBehaviour
{
    [Header("Light Source (둘 중 하나만 사용해도 됩니다)")]
    [Tooltip("URP 2D Light를 쓰는 경우 연결하세요.")]
    [SerializeField] private Light2D light2D;              // 실제 빛을 내는 2D 라이트

    [Tooltip("스프라이트로 손전등 빛을 표현하는 경우 연결하세요.")]
    [SerializeField] private GameObject flashlightVisual;  // 라이트 대신 스프라이트로 표현할 때 쓸 오브젝트

    [Header("Settings")]
    [SerializeField] private bool startsOn = false; // 시작할 때 켜진 상태로 둘지

    public bool IsOn { get; private set; } // 지금 손전등이 켜져 있는지

    private float baseLocalZRotation;  // 오른쪽을 볼 때의 기준 회전값 (반전 계산용)
    private Vector3 baseLocalPosition; // 오른쪽을 볼 때의 기준 위치 (반전 계산용)
    private bool isFacingLeft;         // 현재 왼쪽을 보고 있는지

    // 라이트 참조를 찾고 기준 회전/위치를 저장한 뒤 초기 상태를 적용한다
    private void Awake()
    {
        if (light2D == null)
        {
            light2D = GetComponentInChildren<Light2D>(true);
        }

        if (light2D != null)
        {
            baseLocalZRotation = light2D.transform.localEulerAngles.z;
            baseLocalPosition = light2D.transform.localPosition;
        }

        IsOn = startsOn;
        ApplyState();
    }

    // 스페이스바 입력을 받아 손전등을 토글한다
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    // 손전등을 켜고 끈다
    public void Toggle()
    {
        IsOn = !IsOn;
        ApplyState();
    }

    // 손전등 상태를 직접 지정한다
    public void SetOn(bool value)
    {
        IsOn = value;
        ApplyState();
    }

    // 바라보는 방향에 맞춰 손전등 각도와 위치를 좌우로 뒤집는다 (PlayerMovement가 호출)
    public void SetFacingDirection(bool facingLeft)
    {
        if (light2D == null || isFacingLeft == facingLeft) return;

        isFacingLeft = facingLeft;

        float z = facingLeft ? -baseLocalZRotation : baseLocalZRotation;
        Vector3 euler = light2D.transform.localEulerAngles;
        euler.z = z;
        light2D.transform.localEulerAngles = euler;

        Vector3 pos = baseLocalPosition;
        pos.x = facingLeft ? -baseLocalPosition.x : baseLocalPosition.x;
        light2D.transform.localPosition = pos;
    }

    // 현재 켜짐 상태를 실제 라이트/비주얼에 반영한다
    private void ApplyState()
    {
        if (light2D != null)
        {
            light2D.enabled = IsOn;
        }

        if (flashlightVisual != null)
        {
            flashlightVisual.SetActive(IsOn);
        }
    }
}