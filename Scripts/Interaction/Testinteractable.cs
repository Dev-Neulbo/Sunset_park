using UnityEngine;

// 상호작용 테스트용 오브젝트 스크립트
// F키로 상호작용하면 콘솔에 메시지를 찍는다. 상호작용 시스템이 잘 도는지 확인할 때 쓴다.
[RequireComponent(typeof(Collider2D))]
public class TestInteractable : Interactable
{
    [SerializeField] private string interactMessage = "상호작용 성공!"; // 상호작용 시 찍을 메시지

    // 컴포넌트를 처음 붙일 때 콜라이더를 트리거로 맞춰준다
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    // F키 상호작용 시 호출: 콘솔에 메시지 출력
    public override void Interact()
    {
        Debug.Log($"[TestInteractable] {gameObject.name}: {interactMessage}");
    }
}