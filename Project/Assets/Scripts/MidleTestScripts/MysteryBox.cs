using System.Collections;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    // 인스펙터 설정
    public GameObject[] itemPrefabs;
    public Transform spawnPoint;

    // 애니메이션 변수
    public float bounceSpeed = 8f;
    public float returnSpeed = 2f;
    public float bounceHeight = 0.5f;

    private bool isHit = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    // ★★★ 핵심 ★★★
    // 이 함수는 이제 'HitTrigger' 스크립트가 호출해 줄 겁니다.
    public void ActivateBox()
    {
        // 아직 맞은 적이 없다면
        if (!isHit)
        {
            Debug.Log("박스 활성화! (트리거 감지 성공)");
            isHit = true;
            StartCoroutine(BounceAndSpawnItem());
        }
    }

    // (이하 아이템 생성 및 애니메이션 코드는 동일합니다)
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