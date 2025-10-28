using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MarioEnemy : MonoBehaviour
{
	public enum EnemyType
	{
		Goomba,
		Koopa,
		Piranha,
		HammerBro,
		Spiny
	}

	[Header("Identity")]
	public EnemyType enemyType = EnemyType.Goomba;

	[Header("Stats")]
	public int maxHealth = 2;
	public int currentHealth = 2;
	public float moveSpeed = 10f;
	public float gravity = -9.81f;
	public int contactDamage = 1;
	public float jumpPower = 13f;

	[Header("Behavior")]
	public bool patrol = true;
	public Transform leftBoundary;
	public Transform rightBoundary;
	public bool flipOnTurn = true;

	[Header("Chase (optional)")]
	public Transform target; // Mario
	public float chaseRange = 0f; // 0 = disabled

	[Header("Wall Jump")]
	public float wallCheckDistance = 7f;
	public float wallCheckRadius = 0.2f; // for sphere cast
	public LayerMask wallLayers = ~0; // default: everything
	public float jumpCooldown = 0.5f;
	private float nextJumpTime = 0f;
	public bool useSphereCast = true;
	public bool debugWallCheck = false;

	private Vector3 velocity;
	private int moveDir = 1; // 1 = right, -1 = left
	private CharacterController controller;

	void Awake()
	{
		controller = GetComponent<CharacterController>();
		if (controller == null)
		{
			controller = gameObject.AddComponent<CharacterController>();
		}
		currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
	}

	void Start()
	{
		if (target == null)
		{
			GameObject mario = GameObject.FindGameObjectWithTag("Player");
			if (mario != null) target = mario.transform;
		}
		// Ensure wallLayers is set properly (sometimes prefabs have uninitialized layer mask)
		if (wallLayers == 0)
		{
			wallLayers = ~0; // default to all layers
		}
	}

	void Update()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		// Horizontal AI: chase if in range, else patrol
		float horizontal = 0f;
		if (target != null && chaseRange > 0f)
		{
			float dx = target.position.x - transform.position.x;
			if (Mathf.Abs(dx) <= chaseRange)
			{
				horizontal = Mathf.Sign(dx);
				moveDir = horizontal >= 0 ? 1 : -1;
			}
		}
		if (horizontal == 0f && patrol)
		{
			horizontal = moveDir;
			// turn at boundaries
			if (leftBoundary != null && transform.position.x <= leftBoundary.position.x)
			{
				moveDir = 1;
				horizontal = 1;
				ApplyFacing();
			}
			else if (rightBoundary != null && transform.position.x >= rightBoundary.position.x)
			{
				moveDir = -1;
				horizontal = -1;
				ApplyFacing();
			}
		}

		// Grounding
		bool grounded = controller != null && controller.isGrounded;
		if (grounded && velocity.y < 0f)
		{
			velocity.y = -2f;
		}

		// Wall check ahead
		Vector3 facing = moveDir >= 0 ? Vector3.right : Vector3.left;
		float originHeight = controller != null ? Mathf.Max(0.5f, controller.height * 0.5f * 0.6f) : 0.5f;
		Vector3 origin = transform.position + Vector3.up * originHeight;
		bool wallAhead = false;
		RaycastHit hit;
		if (useSphereCast)
		{
			wallAhead = Physics.SphereCast(origin, wallCheckRadius, facing, out hit, wallCheckDistance, wallLayers, QueryTriggerInteraction.Ignore);
			if (debugWallCheck)
			{
				Debug.DrawRay(origin, facing * wallCheckDistance, wallAhead ? Color.red : Color.green);
			}
		}
		else
		{
			wallAhead = Physics.Raycast(origin, facing, out hit, wallCheckDistance, wallLayers, QueryTriggerInteraction.Ignore);
			if (debugWallCheck)
			{
				Debug.DrawRay(origin, facing * wallCheckDistance, wallAhead ? Color.red : Color.green);
			}
		}
		// Ignore hits against Player or other enemies
		if (wallAhead)
		{
			bool isPlayer = hit.collider != null && (hit.collider.CompareTag("Player") || hit.collider.GetComponentInParent<MarioController>() != null);
			bool isEnemy = hit.collider != null && (hit.collider.GetComponentInParent<MarioEnemy>() != null);
			if (isPlayer || isEnemy)
			{
				wallAhead = false;
			}
		}

		if (wallAhead && grounded && Time.time >= nextJumpTime)
		{
			velocity.y = jumpPower;
			nextJumpTime = Time.time + jumpCooldown;
		}

		Vector3 horizontalMove = new Vector3(horizontal, 0f, 0f) * moveSpeed;
		velocity.y += gravity * Time.deltaTime;
		Vector3 verticalMove = new Vector3(0f, velocity.y, 0f);

		if (controller != null)
		{
			controller.Move((horizontalMove + verticalMove) * Time.deltaTime);
		}

		// flip facing gradually
		ApplyFacing();
	}

	private void ApplyFacing()
	{
		if (!flipOnTurn) return;
		Vector3 faceDir = moveDir >= 0 ? Vector3.right : Vector3.left;
		Quaternion targetRot = Quaternion.LookRotation(faceDir, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
	}

	public void TakeDamage(int amount)
	{
		if (amount <= 0) return;
		currentHealth = Mathf.Max(0, currentHealth - amount);
		if (currentHealth == 0)
		{
			Die();
		}
	}

	private void Die()
	{
		Destroy(gameObject);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// CharacterController 충돌에서는 stomp 판정이 어렵기 때문에 머리 위에서 내려온 경우를 간단히 추정
		if (hit.collider == null) return;
		var mario = hit.collider.GetComponentInParent<MarioController>();
		if (mario == null)
		{
			// 플레이어가 아닌 것에 부딪힘 - 경계 확인용
			return;
		}
		HandlePlayerCollision(mario);
	}

    private void HandlePlayerCollision(MarioController mario)
    {
        float playerY = mario.transform.position.y;
        float enemyHeight = controller != null ? controller.height : 1f;

        // ❌ [수정됨] enemyTop 계산은 밟기 판정 로직 안으로 이동
        // float enemyTop = transform.position.y + enemyHeight * 0.5f;

        float enemyBottom = transform.position.y - enemyHeight * 0.5f;
        float playerVelocity = mario.GetVerticalVelocity();

        // 
        // ✅ [수정된 밟기 판정 로직]
        // 1. 플레이어가 아래로 떨어지고 있는가? (playerVelocity < -0.1f)
        // 2. 플레이어의 발(playerY)이 몬스터의 중심(transform.position.y)보다 위에 있는가?
        //
        bool fromAbove = playerVelocity < -0.1f && playerY > transform.position.y;

        Debug.Log($"Collision: playerY={playerY:F2}, enemyCenterY={transform.position.y:F2}, playerVel={playerVelocity:F2}, fromAbove={fromAbove}");

        if (fromAbove)
        {
            Debug.Log("Stomp! Enemy dies.");
            mario.Bounce(mario.bouncePower);
            Die(); // 몬스터(자신)가 죽음
        }
        else
        {
            Debug.Log("Player hit from side or bottom.");
            mario.TakeDamage(); // <- 이렇게 바꿔야 합니다!
        }
    }

    private void OnTriggerEnter(Collider other)
	{
		var mario = other.GetComponentInParent<MarioController>();
		if (mario == null) return;
		HandlePlayerCollision(mario);
	}

	void OnDrawGizmosSelected()
	{
		if (!debugWallCheck) return;
		Vector3 facing = moveDir >= 0 ? Vector3.right : Vector3.left;
		float originHeight = controller != null ? Mathf.Max(0.5f, controller.height * 0.5f * 0.6f) : 0.5f;
		Vector3 origin = transform.position + Vector3.up * originHeight;
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(origin, origin + facing * wallCheckDistance);
		Gizmos.DrawWireSphere(origin + facing * wallCheckDistance, wallCheckRadius);
	}
}
