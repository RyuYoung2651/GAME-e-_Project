using UnityEngine;

public class BoxHitTrigger : MonoBehaviour
{
    // 부모 오브젝트에 있는 QuestionBox 스크립트를 담을 변수
    private QuestionBox parentBox;

    void Start()
    {
        // 시작할 때 내 부모 오브젝트에서 QuestionBox 스크립트를 찾아서 저장
        parentBox = GetComponentInParent<QuestionBox>();

        if (parentBox == null)
        {
            Debug.LogError("부모 오브젝트에서 QuestionBox 스크립트를 찾을 수 없습니다!");
        }
    }

    // 3D 트리거 감지 (Is Trigger가 체크되어 있어야 작동)
    private void OnTriggerEnter(Collider other)
    {
        // 1. 플레이어와 부딪혔는지 확인
        if (other.CompareTag("Player"))
        {
            // 2. 플레이어가 위로 점프하는 중인지 확인 (머리를 박는 중인지)
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            // 플레이어가 Rigidbody를 가지고 있고, Y축 속도(velocity.y)가 0보다 크다면 (상승 중)
            if (playerRb != null && playerRb.velocity.y > 0)
            {
                Debug.Log("플레이어가 아래에서 트리거를 쳤습니다!");

                // 3. 부모 박스의 ActivateBox() 함수를 호출!
                if (parentBox != null)
                {
                    parentBox.ActivateBox();
                }
            }
        }
    }
}