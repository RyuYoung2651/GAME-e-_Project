using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public float sizeMultiplier = 1.5f; // 아이템을 먹으면 1.5배 커짐

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                player.GetPowerUp(sizeMultiplier); // GetPowerUp 호출
                Destroy(gameObject);
            }
        }
    }
}