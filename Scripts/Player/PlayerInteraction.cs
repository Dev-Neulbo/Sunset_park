using UnityEngine;
using UnityEngine.InputSystem;

// 플레이어 상호작용 스크립트
// 주변에서 가장 가까운 Interactable 오브젝트를 찾아, 근접하면 F 프롬프트를 띄우고
// F키를 누르면 그 대상의 Interact()를 실행한다.
public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("상호작용 오브젝트를 감지할 반경")]
    [SerializeField] private float interactRange = 1.2f; // 감지 반경

    [Tooltip("감지할 레이어. 비워두면 모든 레이어에서 감지합니다.")]
    [SerializeField] private LayerMask interactableLayer = ~0; // 감지 대상 레이어

    [Header("UI Prompt")]
    [Tooltip("근접 시 띄울 F 프롬프트. 비워두면 이름이 'InteractPrompt'인 오브젝트를 자동으로 찾습니다.")]
    [SerializeField] private GameObject promptObject;              // F 프롬프트 오브젝트
    [SerializeField] private string autoFindPromptName = "InteractPrompt"; // 자동 탐색에 쓸 이름

    private Interactable currentTarget;                       // 지금 상호작용 대상
    private readonly Collider2D[] overlapResults = new Collider2D[8]; // 감지 결과를 담을 버퍼
    private ContactFilter2D contactFilter;                    // 감지에 쓸 필터(레이어/트리거 설정)

    // 필터를 설정하고, 프롬프트가 비어 있으면 이름으로 찾아둔다
    private void Awake()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(interactableLayer);
        contactFilter.useTriggers = true;

        if (promptObject == null && !string.IsNullOrEmpty(autoFindPromptName))
        {
            promptObject = FindInactiveObjectByName(autoFindPromptName);
        }
    }

    // 비활성 오브젝트는 GameObject.Find로 못 찾으므로, 씬 전체를 뒤져서 이름으로 찾는다
    private GameObject FindInactiveObjectByName(string targetName)
    {
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in rootObjects)
        {
            if (root.name == targetName) return root;

            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (t.name == targetName) return t.gameObject;
            }
        }
        return null;
    }

    // 매 프레임 대상 감지 + 프롬프트 갱신 + F키 입력 처리
    private void Update()
    {
        DetectNearestInteractable();
        UpdatePrompt();

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    // 감지 반경 안에서 가장 가까운 상호작용 대상을 찾는다
    private void DetectNearestInteractable()
    {
        int count = Physics2D.OverlapCircle(transform.position, interactRange, contactFilter, overlapResults);

        Interactable nearest = null;
        float nearestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D col = overlapResults[i];
            if (col == null) continue;

            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable == null || !interactable.IsInteractable) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = interactable;
            }
        }

        currentTarget = nearest;
    }

    // 대상이 있으면 프롬프트를 켜서 대상 위에 띄우고, 없으면 끈다
    private void UpdatePrompt()
    {
        if (promptObject == null) return;

        bool shouldShow = currentTarget != null;
        if (promptObject.activeSelf != shouldShow)
        {
            promptObject.SetActive(shouldShow);
        }

        if (shouldShow)
        {
            promptObject.transform.position = currentTarget.transform.position + Vector3.up * 0.8f;
        }
    }

    // 에디터에서 선택했을 때 감지 반경을 원으로 보여준다
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}