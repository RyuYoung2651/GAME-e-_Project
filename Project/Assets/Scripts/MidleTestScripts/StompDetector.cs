using UnityEngine;

public class StompDetector : MonoBehaviour
{
    // ���� ��ü�� �ִ� ���� ��ũ��Ʈ�� ������ ����
    private MarioEnemy marioEnemy;
    private ProjectileEnemy projectileEnemy;

    void Start()
    {
        // �θ� ������Ʈ���� ���� ��ũ��Ʈ�� ã�ƿɴϴ�.
        marioEnemy = GetComponentInParent<MarioEnemy>();
        projectileEnemy = GetComponentInParent<ProjectileEnemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player == null) return;

            // �÷��̾ �Ʒ��� �������� �ִ��� (��, ��� �ִ���) Ȯ��
            if (player.GetVerticalVelocity() < -0.1f)
            {
                // 1. �÷��̾ ƨ�� �ø��ϴ�.
                player.Bounce(player.bouncePower);

                // 2. ����(�θ�)�� ���Դϴ�.
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