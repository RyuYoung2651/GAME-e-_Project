using UnityEngine;

public class BoxHitTrigger : MonoBehaviour
{
    // �θ� ������Ʈ�� �ִ� QuestionBox ��ũ��Ʈ�� ���� ����
    private QuestionBox parentBox;

    void Start()
    {
        // ������ �� �� �θ� ������Ʈ���� QuestionBox ��ũ��Ʈ�� ã�Ƽ� ����
        parentBox = GetComponentInParent<QuestionBox>();

        if (parentBox == null)
        {
            Debug.LogError("�θ� ������Ʈ���� QuestionBox ��ũ��Ʈ�� ã�� �� �����ϴ�!");
        }
    }

    // 3D Ʈ���� ���� (Is Trigger�� üũ�Ǿ� �־�� �۵�)
    private void OnTriggerEnter(Collider other)
    {
        // 1. �÷��̾�� �ε������� Ȯ��
        if (other.CompareTag("Player"))
        {
            // 2. �÷��̾ ���� �����ϴ� ������ Ȯ�� (�Ӹ��� �ڴ� ������)
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            // �÷��̾ Rigidbody�� ������ �ְ�, Y�� �ӵ�(velocity.y)�� 0���� ũ�ٸ� (��� ��)
            if (playerRb != null && playerRb.velocity.y > 0)
            {
                Debug.Log("�÷��̾ �Ʒ����� Ʈ���Ÿ� �ƽ��ϴ�!");

                // 3. �θ� �ڽ��� ActivateBox() �Լ��� ȣ��!
                if (parentBox != null)
                {
                    parentBox.ActivateBox();
                }
            }
        }
    }
}