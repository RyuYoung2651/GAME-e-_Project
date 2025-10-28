using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public float sizeMultiplier = 1.5f; // �������� ������ 1.5�� Ŀ��

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                player.GetPowerUp(sizeMultiplier); // GetPowerUp ȣ��
                Destroy(gameObject);
            }
        }
    }
}