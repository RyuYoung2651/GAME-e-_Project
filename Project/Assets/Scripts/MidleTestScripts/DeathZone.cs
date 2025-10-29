using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // 이 스크립트는 닿은 오브젝트가 "Player" 태그를 가지고 있는지 확인합니다.
    // Is Trigger가 체크된 콜라이더가 필요합니다.

    private void OnTriggerEnter(Collider other)
    {
        // 들어온 오브젝트가 "Player" 태그를 가졌는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어의 MarioController 스크립트를 가져옵니다.
            MarioController player = other.GetComponent<MarioController>();

            if (player != null)
            {
                // 파워업 상태와 상관없이 즉시 죽도록 Die() 함수를 직접 호출합니다.
                player.Die();
            }
        }
    }
}