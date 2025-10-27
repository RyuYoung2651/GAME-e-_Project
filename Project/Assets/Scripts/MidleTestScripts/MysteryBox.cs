using System.Collections;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    // �ν����� ����
    public GameObject[] itemPrefabs;
    public Transform spawnPoint;

    // �ִϸ��̼� ����
    public float bounceSpeed = 8f;
    public float returnSpeed = 2f;
    public float bounceHeight = 0.5f;

    private bool isHit = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    // �ڡڡ� �ٽ� �ڡڡ�
    // �� �Լ��� ���� 'HitTrigger' ��ũ��Ʈ�� ȣ���� �� �̴ϴ�.
    public void ActivateBox()
    {
        // ���� ���� ���� ���ٸ�
        if (!isHit)
        {
            Debug.Log("�ڽ� Ȱ��ȭ! (Ʈ���� ���� ����)");
            isHit = true;
            StartCoroutine(BounceAndSpawnItem());
        }
    }

    // (���� ������ ���� �� �ִϸ��̼� �ڵ�� �����մϴ�)
    private IEnumerator BounceAndSpawnItem()
    {
        SpawnRandomItem();
        Vector3 targetPosition = originalPosition + Vector3.up * bounceHeight;

        while (transform.position.y < targetPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position.y > originalPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;
    }

    private void SpawnRandomItem()
    {
        if (itemPrefabs != null && itemPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject chosenItem = itemPrefabs[randomIndex];
            Instantiate(chosenItem, spawnPoint.position, Quaternion.identity);
        }
    }
}