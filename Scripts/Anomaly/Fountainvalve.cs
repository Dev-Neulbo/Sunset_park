using UnityEngine;

// 분수대 수도꼭지 오브젝트 스크립트
// 플레이어가 F키로 상호작용하면 분수대의 물을 켜고 끈다. (물을 틀어야 색을 확인할 수 있음)
[RequireComponent(typeof(Collider2D))]
public class FountainValve : Interactable
{
    [Tooltip("이 수도꼭지가 속한 분수대. 비워두면 부모에서 자동으로 찾습니다.")]
    [SerializeField] private FountainAnomaly fountain; // 물을 조작할 분수대 본체

    // 연결이 비어 있으면 부모에서 분수대를 찾아온다
    private void Awake()
    {
        if (fountain == null)
        {
            fountain = GetComponentInParent<FountainAnomaly>();
        }
    }

    // F키 상호작용 시 호출: 분수대의 물을 토글
    public override void Interact()
    {
        if (fountain != null)
        {
            fountain.ToggleWater();
        }
        else
        {
            Debug.LogWarning("[FountainValve] 연결된 FountainAnomaly가 없습니다.");
        }
    }
}