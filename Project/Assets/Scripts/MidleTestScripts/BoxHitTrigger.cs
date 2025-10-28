using UnityEngine;

public class HeadHitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("QuestionBox"))
        {
            Debug.Log("머리 박았다! 🎉");
            other.GetComponent<QuestionBox>()?.ActivateBox();
        }
    }
}
