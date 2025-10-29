using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;    // ����ü �ӵ�
    public float lifetime = 5f;  // ����ü�� 5�� �ڿ� �ڵ����� �����

    void Start()
    {
        // 5�� �ڿ� �� ���� ������Ʈ�� �ı��մϴ�.
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ��� �ڽ��� �� ����(Z��)���� ���ư��ϴ�.
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �ε������� Ȯ��
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                // �÷��̾�� �������� �ݴϴ�.
                player.TakeDamage();
            }
            // �÷��̾�� ������ ��� �ı�
            Destroy(gameObject);
        }
        // �÷��̾ �ƴ� �� ���� �Ϳ� ��Ƶ� �ı� (���鳢���� ���)
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}