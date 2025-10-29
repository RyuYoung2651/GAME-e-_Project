using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;    // 투사체 속도
    public float lifetime = 5f;  // 투사체는 5초 뒤에 자동으로 사라짐

    void Start()
    {
        // 5초 뒤에 이 게임 오브젝트를 파괴합니다.
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 계속 자신의 앞 방향(Z축)으로 날아갑니다.
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 부딪혔는지 확인
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                // 플레이어에게 데미지를 줍니다.
                player.TakeDamage();
            }
            // 플레이어에게 닿으면 즉시 파괴
            Destroy(gameObject);
        }
        // 플레이어가 아닌 벽 같은 것에 닿아도 파괴 (적들끼리는 통과)
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}