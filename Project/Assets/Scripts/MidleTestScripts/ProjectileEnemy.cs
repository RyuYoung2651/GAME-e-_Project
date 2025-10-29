using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ProjectileEnemy : MonoBehaviour
{
    [Header("Shooting Logic")]
    public GameObject projectilePrefab; // 쏠 투사체 프리팹을 여기에 연결
    public Transform firePoint;         // 투사체가 발사될 위치 (빈 오브젝트)
    public float fireRate = 2f;         // 2초에 한 번씩 발사
    public float detectionRange = 20f;  // 이 거리 안에 플레이어가 들어오면 발사 시작

    private Transform playerTarget;
    private float nextFireTime = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // "Player" 태그로 플레이어를 찾아서 타겟으로 설정
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    void Update()
    {
        // 플레이어가 없으면 아무것도 안 함
        if (playerTarget == null) return;

        // 간단한 중력 적용
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }

        // 플레이어와의 거리를 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // 플레이어가 사정거리 안에 들어왔다면
        if (distanceToPlayer <= detectionRange)
        {
            // 플레이어 방향으로 부드럽게 회전 (좌우로만)
            Vector3 directionToPlayer = (playerTarget.position - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // 발사 쿨타임이 다 되었다면 발사
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // 다음 발사 시간 초기화
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // 투사체 프리팹을 firePoint의 위치와 방향으로 생성
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    // --- 밟기 & 충돌 처리 (기존 MarioEnemy와 유사) ---
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Player"))
        {
            HandlePlayerCollision(hit.collider.GetComponent<MarioController>());
        }
    }

    // ⬇️ 이 함수를 찾아서 아래 코드로 통째로 교체하세요. ⬇️
    private void HandlePlayerCollision(MarioController mario)
    {
        if (mario == null) return;

        // 밟기 판정은 StompDetector가 담당합니다.
        // 이 함수가 호출되면 무조건 옆에서 부딪힌 것이므로
        // 플레이어는 데미지를 입습니다.
        mario.TakeDamage();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}