using UnityEngine;

public class StompDetector : MonoBehaviour
{
    // 몬스터 본체에 있는 메인 스크립트를 저장할 변수
    private MarioEnemy marioEnemy;
    private ProjectileEnemy projectileEnemy;

    void Start()
    {
        // 부모 오브젝트에서 몬스터 스크립트를 찾아옵니다.
        marioEnemy = GetComponentInParent<MarioEnemy>();
        projectileEnemy = GetComponentInParent<ProjectileEnemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player == null) return;

            // 플레이어가 아래로 떨어지고 있는지 (즉, 밟고 있는지) 확인
            if (player.GetVerticalVelocity() < -0.1f)
            {
                // 1. 플레이어를 튕겨 올립니다.
                player.Bounce(player.bouncePower);

                // 2. 몬스터(부모)를 죽입니다.
                if (marioEnemy != null)
                {
                    marioEnemy.Die();
                }
                else if (projectileEnemy != null)
                {
                    projectileEnemy.Die();
                }
            }
        }
    }
}