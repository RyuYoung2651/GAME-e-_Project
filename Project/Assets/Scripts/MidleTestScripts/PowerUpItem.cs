using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [Header("Power-Up Stats")]
    public float sizeMultiplier = 1.5f; // 1.5배 커짐
    public float speedBoost = 5f;       // 걷기/뛰기 속도 5 증가
    public float jumpBoost = 2f;        // 점프력 2 증가

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                // [수정됨] GetPowerUp 함수에 모든 능력치를 전달합니다.
                player.GetPowerUp(sizeMultiplier, speedBoost, jumpBoost);
                Destroy(gameObject);
            }
        }
    }
}