using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [Header("Power-Up Stats")]
    public float sizeMultiplier = 1.5f; // 1.5�� Ŀ��
    public float speedBoost = 5f;       // �ȱ�/�ٱ� �ӵ� 5 ����
    public float jumpBoost = 2f;        // ������ 2 ����

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                // [������] GetPowerUp �Լ��� ��� �ɷ�ġ�� �����մϴ�.
                player.GetPowerUp(sizeMultiplier, speedBoost, jumpBoost);
                Destroy(gameObject);
            }
        }
    }
}