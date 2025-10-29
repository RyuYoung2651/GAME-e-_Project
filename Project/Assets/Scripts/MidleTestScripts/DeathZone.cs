using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // �� ��ũ��Ʈ�� ���� ������Ʈ�� "Player" �±׸� ������ �ִ��� Ȯ���մϴ�.
    // Is Trigger�� üũ�� �ݶ��̴��� �ʿ��մϴ�.

    private void OnTriggerEnter(Collider other)
    {
        // ���� ������Ʈ�� "Player" �±׸� �������� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �÷��̾��� MarioController ��ũ��Ʈ�� �����ɴϴ�.
            MarioController player = other.GetComponent<MarioController>();

            if (player != null)
            {
                // �Ŀ��� ���¿� ������� ��� �׵��� Die() �Լ��� ���� ȣ���մϴ�.
                player.Die();
            }
        }
    }
}