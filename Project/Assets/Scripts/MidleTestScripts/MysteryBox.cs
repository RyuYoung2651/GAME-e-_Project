using System.Collections;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public Transform spawnPoint;
    public float bounceSpeed = 8f;
    public float returnSpeed = 2f;
    public float bounceHeight = 0.5f;

    private bool isHit = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Player 태그 오브젝트인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // contact.normal.y < -0.5f == 플레이어가 아래에서 위로 부딪침
                if (contact.normal.y < -0.5f)
                {
                    Debug.Log("✅ 머리 박았다!");
                    ActivateBox();
                    break;
                }
            }
        }
    }

    public void ActivateBox()
    {
        if (isHit) return;

        Debug.Log("🎁 박스 활성화!");
        isHit = true;
        StartCoroutine(BounceAndSpawnItem());
    }

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
