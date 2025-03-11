# 구현한 기능

**기본 이동 및 점프 Input System, Rigidbo**dy ForceMode (난이도 : ★★☆☆☆)

* 플레이어의 이동(WASD), 점프(Space) 등을 설정

체력바 UI UI (난이도 : ★★☆☆☆)

* UI 캔버스에 체력바를 추가하고 플레이어의 체력을 나타내도록 설정. 플레이어의 체력이 변할 때마다 UI 갱신.

동적 환경 조사 Raycast UI (난이도: ★★★☆☆)

* Raycast를 통해 플레이어가 조사하는 오브젝트의 정보를 UI에 표시.

  예) 플레이어가 바라보는 오브젝트의 이름, 설명 등을 화면에 표시.

점프대 Rigidbody ForceMode (난이도 : ★★★☆☆)

* 캐릭터가 밟을 때 위로 높이 튀어 오르는 점프대 구현

* OnCollisionEnter 트리거를 사용해 캐릭터가 점프대에 닿았을 때 ForceMode.Impulse를 사용해 순간적인 힘을 가함.

아이템 데이터 ScriptableObject (난이도 : ★★★☆☆)

* 다양한 아이템 데이터를 ScriptableObject로 정의. 각 아이템의 이름, 설명, 속성 등을 ScriptableObject로 관리

아이템 사용 Coroutine (난이도 : ★★★☆☆)

* 특정 아이템 사용 후 효과가 일정 시간 동안 지속되는 시스템 구현

  예) 아이템 사용 후 일정 시간 동안 스피드 부스트.

추가 UI (난이도 : ★★☆☆☆)

* 점프나 대쉬 등 특정 행동 시 소모되는 스태미나를 표시하는 바 구현

  이 외에도 다양한 정보를 표시하는 UI 추가 구현

3인칭 시점 (난이도 : ★★★☆☆)

* 기존 강의의 1인칭 시점을 3인칭 시점으로 변경하는 연습

* 3인칭 카메라 시점을 설정하고 플레이어를 따라다니도록 설정

움직이는 플랫폼 구현 (난이도 : ★★★☆☆)

* 시간에 따라 정해진 구역을 움직이는 발판 구현

* 플레이어가 발판 위에서 이동할 때 자연스럽게 따라가도록 설정

# 구현하지 못한 기능

벽 타기 및 매달리기 (난이도 : ★★★★☆)

* 캐릭터가 벽에 붙어 타고 오르거나 매달릴 수 있는 시스템 구현.

* Raycast와 ForceMode를 함께 사용해 벽에 닿았을 때 적절한 물리적 반응을 구현

다양한 아이템 구현 (난이도 : ★★★★☆)

* 추가적으로 아이템을 구현해봅니다.
  예) 스피드 부스트(Speed Boost): 플레이어의 이동 속도를 일정 시간 동안 증가시킴. 더블 점프(Double Jump): 일정 시간 동안 두 번 점프할 수 있게 함. 무적(Invincibility): 일정 시간 동안 적의 공격을 받지 않도록 함.

장비 장착 (난이도 : ★★★★☆)

* 장비를 장착하여 캐릭터의 능력을 강화하는 시스템 구현
  예) 속도 증가 장비, 점프력 증가 장비 등

레이저 트랩 (난이도 : ★★★★☆)

* Raycast를 사용해 특정 구간을 레이저로 감시하고, 플레이어가 레이저를 통과하면 경고 메시지나 트랩 발동

상호작용 가능한 오브젝트 표시 (난이도 : ★★★★★)

* 상호작용 가능한 오브젝트에 마우스를 올리면 해당 오브젝트에 UI를 표시

  예) 문에 마우스를 올리면 'E키를 눌러 열기' 텍스트 표시. 레버(Lever): 'E키를 눌러 당기기' 텍스트 표시. 상자(Box): 'E키를 눌러 열기' 텍스트 표시. 버튼(Button): 'E키를 눌러 누르기' 텍스트 표시.

플랫폼 발사기 (난이도 : ★★★★★)

* 캐릭터가 플랫폼 위에 서 있을 때 특정 방향으로 힘을 가해 발사하는 시스템 구현 특정 키를 누르거나 시간이 경과하면 ForceMode를 사용해 발사

발전된 AI (난이도 : ★★★★★)

* AI Navigation 시스템을 활용하여 맵에 다양한 구조물에 대한 계산 가중치를 설정


# 기능별 구현 방법

* 플레이어의 이동(WASD), 점프(Space) 등을 설정

  * Input System을 통한 입력을 받아 PlayerController 스크립트에서 구현 후  PlayerInput에서 이벤트를 받아 움직임 구현
 
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

* UI 캔버스에 체력바를 추가하고 플레이어의 체력을 나타내도록 설정. 플레이어의 체력이 변할 때마다 UI 갱신.

  <img width="191" alt="스크린샷 2025-03-11 오후 2 57 05" src="https://github.com/user-attachments/assets/bf98b474-4735-4be9-b548-477fd205cf77" />

  * UI를 구현 위부터 Health, Energy, Water 이미지의 Fill Amount를 이용하여 원모양 테두리의 바가 줄어들도록 구현

  <img width="191" alt="스크린샷 2025-03-11 오후 3 00 14" src="https://github.com/user-attachments/assets/3d9788a0-17be-4437-a783-458b0e928715" />

  * Condition.cs를 통한 기본적인 Condition 세팅, UIConditionr과 PlayerCondition을 연결하여 UI에 반영


* Raycast를 통해 플레이어가 조사하는 오브젝트의 정보를 UI에 표시.
* 다양한 아이템 데이터를 ScriptableObject로 정의. 각 아이템의 이름, 설명, 속성 등을 ScriptableObject로 관리
* 특정 아이템 사용 후 효과가 일정 시간 동안 지속되는 시스템 구현
 
  <img width="622" alt="스크린샷 2025-03-11 오후 3 06 27" src="https://github.com/user-attachments/assets/182a7ec0-37aa-46c6-a363-82ff15394d96" />

  * 위 처럼 플레이어 중심 크로스헤어 기준으로 Ray를 사용하여 아이템과 상호작용 및 정보를 노출

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

    * 또한 물병의 경우 사용하면 Player의 UI Water가 회복 되도록 구현

          public void CheckConsumable()
          {
              Debug.Log("CheckConsumable() 실행됨");
      
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

 * CheckConsumable을 통해 ItemData에 지정해준 타입과 값을 불러와 그 수치만큼 불러오도록 구현

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

   * 아이템 사용시 인벤토리의 UseItem을 Player의 아이템데이터에서 가져와 사용 하고 CheckConsumable();을 통해 회복량 같은 것을 결정

* 점프나 대쉬 등 특정 행동 시 소모되는 스태미나를 표시하는 바 구현

 * 대쉬 사용시 UI의 Energy가 소모되고 모두 소모되면 뛰기 불가능 대쉬중이 아니라면 서서히 회복

        void Update()
        {
            RunnigEnergy();
            EnergyCheck();
            WallClimb();
        }
   * PlayerContlloer에서 업데이트로 계속 체크
  
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

         public void EnergyCheck()
          {
              if (CharacterManager.Instance.Player.condition.energy.curValue <= 0)
              {
                  moveSpeed = defaultMoveSpeed;
                  animator.SetBool("isRunning", false);
              }
          }

* 캐릭터가 밟을 때 위로 높이 튀어 오르는 점프대 구현


      public class JumpPad : MonoBehaviour
      {
          public float jumpForce = 10f;
          private Animator anim;
          private void Start()
          {
              anim = GetComponent<Animator>(); // 점프대의 애니메이터 가져오기
          }
      
          private void OnCollisionEnter(Collision collision)
          {
              if (collision.gameObject.CompareTag("Player"))
              {
                  Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                  if (rb != null)
                  {
                      rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y 속도 초기화
                      rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                      Debug.Log("JumpPad: Jump!");
                  }
                  anim.SetTrigger("Jump"); // 점프대 애니메이션 재생
              }
          }
      }

  * 간단한 코드를 통한 점프대 구현 및 인스펙터창을 통한 점프수치 변경 가능
 
* 3인칭 시점의 경우 V입력을 받아 빈오브젝트를 통해 3인칭 카메라시점을 만든후 1인칭과 3인칭 시점이 변경 가능하도록 구현

      public void OnViewChangeInput(InputAction.CallbackContext context)
      {
          if (context.phase == InputActionPhase.Started)
          {
              isThirdPerson = !isThirdPerson; // 시점 변경
              SetCameraView();
          }
      }

* 시간에 따라 정해진 구역을 움직이는 발판 구현
      
      public class MovingPlatform : MonoBehaviour
      {
          public Transform pointA;
          public Transform pointB;
          public float speed = 2f;
      
          private float t = 0f;
          private bool movingToB = true;
          private Transform player;
          private bool isJumping = false;
      
          void Update()
          {
              // 플랫폼 이동
              if (movingToB)
              {
                  t += Time.deltaTime * speed;
                  if (t >= 1f)
                  {
                      t = 1f;
                      movingToB = false;
                  }
              }
              else
              {
                  t -= Time.deltaTime * speed;
                  if (t <= 0f)
                  {
                      t = 0f;
                      movingToB = true;
                  }
              }
      
              transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
      
              // 플레이어가 점프 중이면 부모 해제
              if (player != null)
              {
                  Rigidbody playerRb = player.GetComponent<Rigidbody>();
                  if (playerRb != null)
                  {
                      // 점프입력을 감지
                      if (playerRb.velocity.y > 0.1f)
                      {
                          isJumping = true;
                          player.SetParent(null);  // 점프 중이면 부모 해제
                      }
                      // 점프가 끝났으면 부모 설정
                      else if (playerRb.velocity.y <= 0.1f && isJumping)
                      {
                          isJumping = false;
                          player.SetParent(transform);  // 점프 종료 후 부모 다시 설정
                      }
                  }
              }
          }
      
          void OnTriggerEnter(Collider other)
          {
              if (other.CompareTag("Player"))
              {
                  player = other.transform;
                  player.SetParent(transform);  // 발판을 부모로 설정
              }
          }
      
          void OnTriggerExit(Collider other)
          {
              if (other.CompareTag("Player"))
              {
                  other.transform.SetParent(null);
                  player = null;
              }
          }
      }

  * 움직이는 발판의 경우 발판의 올라갔을때 플레이어가 발판의 자식으로 들어가 미끄러지지않고 같이 움직일 수 있도록 구현 / 해당 구현으로 인해 점프가 되지않는 버그 발생하여 점프시 부모 해제를 통한 버그 해결

# 사용한 에셋 및 출처

캐릭터 에셋 https://assetstore.unity.com/packages/3d/characters/humanoids/banana-man-196830

배경 및 주변환경 https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-construction-kit-modular-159280

점프패드 https://sketchfab.com/3d-models/bounce-pad-023a85a6a63e4d39937bf8cb3e38ae21

UI  https://assetstore.unity.com/packages/2d/gui/icons/sleek-essential-ui-pack-170650

아이템 https://assetstore.unity.com/packages/3d/props/tools/survival-game-tools-139872

폰트 http://www.pfsb.co.kr/fonts/fonts_sub02.html

Animation은 Misamo를 통해 구현하였음을 알립니다.






  


