using UnityEngine;

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