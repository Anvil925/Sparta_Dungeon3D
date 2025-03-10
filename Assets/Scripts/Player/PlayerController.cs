using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float runSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;
    private float defaultMoveSpeed; // 기본 이동 속도 저장용 변수
    private Animator animator;

    
    [Header("Use Item")]
    private PlayerCondition condition;
    private Inventory inventory;
    public ItemDataConsumable[] consumables;


    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    private Vector2 mouseDelta;

    [HideInInspector]
    public bool canLook = true;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        condition = GetComponent<PlayerCondition>();
        inventory = GetComponent<Inventory>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultMoveSpeed = moveSpeed; // 시작 시 기본 속도 저장
    }

    void Update()
    {
        RunnigEnergy();
        EnergyCheck();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            animator.SetBool("isWalking", true);
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            // Vector2.up을 Vector3.up으로 수정
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            
            // 디버그로 점프 확인 (문제 해결 후 제거)
            Debug.Log("Jump! IsGrounded: " + IsGrounded() + ", Jump Power: " + jumpPower);
        }
    }

    public void OnRunInput (InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            moveSpeed = runSpeed;
            animator.SetBool("isRunning", true);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            moveSpeed = defaultMoveSpeed; // 저장해둔 기본 속도로 복원
            animator.SetBool("isRunning", false);
        }
    }
    public void OnUseItemInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (CharacterManager.Instance.Player.itemData != null)
            {
                int itemCount = Inventory.Instance.GetItemCount(CharacterManager.Instance.Player.itemData);
                Debug.Log("현재 아이템 개수: " + itemCount);

                if (itemCount > 0) // 아이템이 1개 이상일 때만 사용
                {
                    Debug.Log("아이템 사용: " + CharacterManager.Instance.Player.itemData.displayName);
                    CheckConsumable();
                    Inventory.Instance.UseItem(CharacterManager.Instance.Player.itemData);
                    Debug.Log("사용 후 남은 아이템 개수: " + Inventory.Instance.GetItemCount(CharacterManager.Instance.Player.itemData));
                }
                else
                {
                    Debug.LogWarning("아이템이 없습니다! 사용 불가");
                }
            }
            else
            {
                Debug.LogError("아이템 데이터가 null입니다!");
            }
        }
    }

    public void CheckConsumable()
    {
        Debug.Log("✅ CheckConsumable() 실행됨");

        if (condition == null)
        {
            Debug.LogError("condition이 null입니다! 올바르게 할당되지 않았을 가능성이 큼.");
            return;
        }

        for (int i = 0; i < CharacterManager.Instance.Player.itemData.consumables.Length; i++)
        {
            Debug.Log("아이템 효과: " + CharacterManager.Instance.Player.itemData.consumables[i].type + ", 값: " + CharacterManager.Instance.Player.itemData.consumables[i].value);

            if (CharacterManager.Instance.Player.itemData.consumables[i].type == ConsumableType.Health)
            {
                Debug.Log("Health 회복: " + CharacterManager.Instance.Player.itemData.consumables[i].value);
                condition.Heal(CharacterManager.Instance.Player.itemData.consumables[i].value);
            }
            else if (CharacterManager.Instance.Player.itemData.consumables[i].type == ConsumableType.Energy)
            {
                Debug.Log("Energy 추가: " + CharacterManager.Instance.Player.itemData.consumables[i].value);
                condition.energy.Add(CharacterManager.Instance.Player.itemData.consumables[i].value);
            }
            else if (CharacterManager.Instance.Player.itemData.consumables[i].type == ConsumableType.Water)
            {
                Debug.Log("Water 사용: " + CharacterManager.Instance.Player.itemData.consumables[i].value);
                condition.Drink(CharacterManager.Instance.Player.itemData.consumables[i].value);
            }
        }
    }
    public void EnergyCheck()
    {
        if (CharacterManager.Instance.Player.condition.energy.curValue <= 0)
        {
            moveSpeed = defaultMoveSpeed;
            animator.SetBool("isRunning", false);
        }
    }
    public void RunnigEnergy()
    {
        if (CharacterManager.Instance.Player.condition.energy.curValue > 0 && animator.GetBool("isRunning"))
        {
            CharacterManager.Instance.Player.condition.energy.Subtract(10 * Time.deltaTime);
        }
        else if (!animator.GetBool("isRunning"))
        {
            CharacterManager.Instance.Player.condition.energy.Add(CharacterManager.Instance.Player.condition.energy.passiveValue * Time.deltaTime);
        }
    }
    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        // 마우스 Y 입력값을 이용해 X축 회전 (위/아래 보기)
        camCurXRot -= mouseDelta.y * lookSensitivity; // 마우스 위아래 반전
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook); // 최대/최소 각도 제한
        cameraContainer.localRotation = Quaternion.Euler(camCurXRot, 0, 0); // X축만 회전

        // 마우스 X 입력값을 이용해 Y축 회전 (좌우 회전)
        transform.Rotate(Vector3.up * mouseDelta.x * lookSensitivity);
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}