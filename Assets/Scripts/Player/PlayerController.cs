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