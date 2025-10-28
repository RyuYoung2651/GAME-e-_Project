using UnityEngine;

public class PlayerHeadHit : MonoBehaviour
{
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // QuestionBox에 닿았는지 확인
        if (hit.gameObject.CompareTag("QuestionBox"))
        {
            // 플레이어가 아래에서 위로 부딪쳤는지 체크
            if (hit.normal.y < -0.5f)
            {
                Debug.Log("머리 박았다! 🎉");
                hit.gameObject.GetComponent<QuestionBox>()?.ActivateBox();
            }
        }
    }
}

