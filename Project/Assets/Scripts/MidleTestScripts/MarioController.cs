using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioController : MonoBehaviour
{
	[Header("̵")]
	public float walkSpeed = 10f;
	public float runSpeed = 15f;
	public float rotationSpeed = 30f;

	[Header(" ")]
	public float attackDuration = 0.8f;              // ð
	public bool canMoveWhileAttacking = false;       //  ̵  

	[Header("")]
	public float jumpPower = 7f;
	public float gravity = -9.81f;
	public float coyoteTime = 0.1f;                  // allow jump shortly after leaving ground
	public float jumpBufferTime = 0.1f;              // buffer jump input before landing

	[Header("Ʈ")]
	public Animator animator;
	[Tooltip("Animator state name to optionally crossfade to on jump")]
	public string jumpStateName = "Jump";
	public int jumpStateLayer = 0;
	public bool crossFadeOnJump = false;
	public float jumpCrossFadeDuration = 0.05f;

	private CharacterController controller;
	private Camera playerCamera;

	// 
	private float currentSpeed;
	private bool isAttacking = false;   //  üũ
	private float verticalVelocity = 0f;
	private bool wasGrounded = true;
	private float coyoteUntil = 0f;
	private float jumpBufferedUntil = 0f;
	private Vector3 pendingHorizontalMove = Vector3.zero;

	// Start is called before the first frame update
	void Start()
	{
		controller = GetComponent<CharacterController>();
		playerCamera = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		HandleMovement();
		HandleJumpAndGravity();
		UpdateAnimation();
	}

	void HandleMovement()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");

		// 2D 좌우 이동만 허용 (월드 X축)
		if (horizontal != 0)
		{
			Vector3 moveDirection = new Vector3(horizontal, 0f, 0f);

			if (Input.GetKey(KeyCode.LeftShift))
			{
				currentSpeed = runSpeed;
			}
			else
			{
				currentSpeed = walkSpeed;
			}

			pendingHorizontalMove = moveDirection.normalized * currentSpeed;  // defer, combine later

			// 좌우로만 바라보게 회전
			Vector3 faceDir = horizontal > 0 ? Vector3.right : Vector3.left;
			Quaternion targetRotion = Quaternion.LookRotation(faceDir, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotion, rotationSpeed * Time.deltaTime);
		}
		else
		{
			currentSpeed = 0;
			pendingHorizontalMove = Vector3.zero;
		}

		// Space/W/UpArrow 모두 동일하게 점프 입력 버퍼링
		bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
		if (jumpPressed)
		{
			jumpBufferedUntil = Time.time + jumpBufferTime;
		}
	}

	void HandleJumpAndGravity()
	{
		bool isGrounded = controller.isGrounded;
		if (isGrounded)
		{
			coyoteUntil = Time.time + coyoteTime;
			if (verticalVelocity < 0f)
			{
				verticalVelocity = -2f; // ensures we stay grounded
			}
		}

		// allow jump if within coyote and buffered
		if (Time.time <= coyoteUntil && Time.time <= jumpBufferedUntil)
		{
			verticalVelocity = jumpPower; // identical jump power regardless of input source
			jumpBufferedUntil = 0f; // consume buffer
			if (animator != null)
			{
				animator.SetTrigger("jumpTrigger");
				if (crossFadeOnJump && !string.IsNullOrEmpty(jumpStateName))
				{
					int stateHash = Animator.StringToHash(jumpStateName);
					if (animator.HasState(jumpStateLayer, stateHash))
					{
						animator.CrossFade(stateHash, jumpCrossFadeDuration, jumpStateLayer);
					}
				}
			}
		}

		verticalVelocity += gravity * Time.deltaTime;
		Vector3 verticalMove = new Vector3(0f, verticalVelocity, 0f);
		// single Move combining horizontal and vertical
		controller.Move((pendingHorizontalMove + verticalMove) * Time.deltaTime);

		// cache for animation
		wasGrounded = isGrounded;
	}

	void UpdateAnimation()
	{
		//ü ִ ӵ(runSpeed)  0 ~ 1 
		float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
		if (animator != null)
		{
			animator.SetFloat("speed", animatorSpeed);
			animator.SetBool("isGrounded", controller.isGrounded);
			animator.SetFloat("yVelocity", verticalVelocity);
		}
	}
}
