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
	public float moveSpeed = 2f;
	public float gravity = -9.81f;
	public int contactDamage = 1;
	public float jumpPower = 5f;

	[Header("Behavior")]
	public bool patrol = true;
	public Transform leftBoundary;
	public Transform rightBoundary;
	public bool flipOnTurn = true;

	[Header("Chase (optional)")]
	public Transform target; // Mario
	public float chaseRange = 0f; // 0 = disabled

	[Header("Wall Jump")]
	public float wallCheckDistance = 0.6f;
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
		else
		{
			// Fallback: transform-based motion if controller is missing
			transform.position += (horizontalMove + verticalMove) * Time.deltaTime;
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

	private bool TryHandleStomp(Collision collision)
	{
		var mario = collision.collider.GetComponentInParent<MarioController>();
		if (mario == null) return false;
		// stomp 판정: 플레이어의 발 위치가 적의 상단보다 위, 그리고 플레이어가 하강 중이었는지
		float playerY = mario.transform.position.y;
		float enemyTop = transform.position.y + (controller != null ? controller.height * 0.5f : 0.5f);
		bool fromAbove = playerY > enemyTop - 0.1f && mario.GetVerticalVelocity() <= 0f;
		if (!fromAbove) return false;
		// 스톰프 처리: 적 사망 + 플레이어 바운스
		mario.Bounce(mario.bouncePower);
		Die();
		return true;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (TryHandleStomp(collision)) return;
		var mario = collision.collider.GetComponentInParent<MarioController>();
		if (mario != null)
		{
			mario.Die();
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// CharacterController 충돌에서는 stomp 판정이 어렵기 때문에 머리 위에서 내려온 경우를 간단히 추정
		var mario = hit.collider != null ? hit.collider.GetComponentInParent<MarioController>() : null;
		if (mario != null)
		{
			float playerY = mario.transform.position.y;
			float enemyTop = transform.position.y + (controller != null ? controller.height * 0.5f : 0.5f);
			bool fromAbove = playerY > enemyTop - 0.1f && mario.GetVerticalVelocity() <= 0f;
			if (fromAbove)
			{
				mario.Bounce(mario.bouncePower);
				Die();
			}
			else
			{
				mario.Die();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var mario = other.GetComponentInParent<MarioController>();
		if (mario != null)
		{
			// 트리거에서는 상대적 위치로 간단히 판정
			float playerY = mario.transform.position.y;
			float enemyTop = transform.position.y + (controller != null ? controller.height * 0.5f : 0.5f);
			bool fromAbove = playerY > enemyTop - 0.1f && mario.GetVerticalVelocity() <= 0f;
			if (fromAbove)
			{
				mario.Bounce(mario.bouncePower);
				Die();
			}
			else
			{
				mario.Die();
			}
		}
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
