using UnityEngine;

// 분수대 정수 버튼 오브젝트 스크립트
// 플레이어가 F키로 상호작용하면 분수대에게 정수(정화)를 요청한다.
[RequireComponent(typeof(Collider2D))]
public class WaterPurifier : Interactable
{
    [Tooltip("이 정수 시스템이 속한 분수대. 비워두면 부모에서 자동으로 찾습니다.")]
    [SerializeField] private FountainAnomaly fountain; // 정수를 요청할 분수대 본체

    // 연결이 비어 있으면 부모에서 분수대를 찾아온다
    private void Awake()
    {
        if (fountain == null)
        {
            fountain = GetComponentInParent<FountainAnomaly>();
        }
    }

    // F키 상호작용 시 호출: 분수대에게 정수를 요청
    public override void Interact()
    {
        if (fountain != null)
        {
            fountain.Purify();
        }
        else
        {
            Debug.LogWarning("[WaterPurifier] 연결된 FountainAnomaly가 없습니다.");
        }
    }
}