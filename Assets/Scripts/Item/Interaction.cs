using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if(Time.time - lastCheckTime > checkRate) // 일정 시간마다 근처에 있는 상호작용 가능한 오브젝트를 체크
        {
            lastCheckTime = Time.time; 

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 카메라 중앙에서 레이 발사
            RaycastHit hit; // 레이캐스트에 맞은 오브젝트 정보를 저장할 변수

            if(Physics.Raycast(ray, out hit, maxCheckDistance, layerMask)) // 레이캐스트에 맞은 오브젝트가 있을 경우
            {
                if(hit.collider.gameObject != curInteractGameObject) // 이전에 상호작용 가능한 오브젝트와 다를 경우
                {
                    curInteractGameObject = hit.collider.gameObject; // 현재 상호작용 가능한 오브젝트로 설정
                    curInteractable = hit.collider.GetComponent<IInteractable>(); // 상호작용 가능한 오브젝트의 IInteractable 컴포넌트를 가져옴
                    SetPromptText(); // 상호작용 가능한 오브젝트의 프롬프트 텍스트 설정
                }
            }
            else
            {
                curInteractGameObject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText() // 상호작용 가능한 오브젝트의 프롬프트 텍스트 설정
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null) // 상호작용 가능한 오브젝트가 있을 경우
        {
            curInteractable.OnInteract(); // 상호작용 가능한 오브젝트의 OnInteract 메서드 호출
            Inventory.Instance.AddItem(CharacterManager.Instance.Player.itemData); // 인벤토리에 아이템 추가

            Debug.Log("아이템 획득: " + CharacterManager.Instance.Player.itemData?.displayName);
            Debug.Log("현재 인벤토리 아이템 개수: " + Inventory.Instance.GetItemCount(CharacterManager.Instance.Player.itemData));

            if (CharacterManager.Instance.Player.itemData == null)
            {
                Debug.LogError("아이템을 줍고 나서 itemData가 null입니다!");
            }

            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}