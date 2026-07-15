using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어 이동 스크립트
// 좌우 화살표 또는 A/D 키로 좌우로만 이동한다. (점프 없음, 중력/세로 이동 없음)
// 새 Input System(Keyboard.current)을 사용하며, 방향 전환 시 스프라이트와 손전등도 함께 뒤집는다.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // 이동 속도

    [Header("Sprite Flip")]
    [SerializeField] private bool flipSpriteOnDirection = true; // 방향에 따라 스프라이트를 뒤집을지
    [SerializeField] private SpriteRenderer spriteRenderer;     // 뒤집을 스프라이트 렌더러

    private Rigidbody2D rb;         // 물리 이동에 쓸 리지드바디
    private float moveInput;        // 이번 프레임의 좌우 입력값 (-1, 0, 1)
    private Flashlight flashlight;  // 방향 전환을 알려줄 손전등

    // 참조를 찾고, 중력을 끄고 세로 이동/회전을 고정한다
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flashlight = GetComponent<Flashlight>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // 좌우 입력을 읽고, 방향이 바뀌면 스프라이트와 손전등을 뒤집는다
    private void Update()
    {
        moveInput = 0f;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            {
                moveInput -= 1f;
            }
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            {
                moveInput += 1f;
            }
        }

        if (moveInput != 0f)
        {
            bool facingLeft = moveInput < 0f;

            if (flipSpriteOnDirection && spriteRenderer != null)
            {
                spriteRenderer.flipX = facingLeft;
            }

            if (flashlight != null)
            {
                flashlight.SetFacingDirection(facingLeft);
            }
        }
    }

    // 실제 이동 처리 (세로 속도는 그대로 두고 좌우 속도만 설정)
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}