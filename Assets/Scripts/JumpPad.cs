using UnityEngine;

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