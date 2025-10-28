using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 10f;
    public float runSpeed = 15f;
    public float rotationSpeed = 30f;

    [Header("Jumping")]
    public float jumpPower = 7f;
    public float gravity = -9.81f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float bouncePower = 7f;

    [Header("Animation")]
    public Animator animator;
    [Tooltip("Animator state name to optionally crossfade to on jump")]
    public string jumpStateName = "Jump";
    public int jumpStateLayer = 0;
    public bool crossFadeOnJump = false;
    public float jumpCrossFadeDuration = 0.05f;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1.0f;

    // --- Private Variables ---
    private CharacterController controller;
    private float currentSpeed;
    private float verticalVelocity = 0f;
    private float coyoteUntil = 0f;
    private float jumpBufferedUntil = 0f;
    private Vector3 pendingHorizontalMove = Vector3.zero;
    private bool isDead = false;
    private bool hasPowerUp = false;
    private bool isInvincible = false;

    // Power-up state variables
    private Vector3 originalScale;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private Coroutine powerUpRoutine;

    // Layer variables for collision ignorance
    private int playerLayer;
    private int enemyLayer;

    // --- Unity Methods ---
    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Store original values for power-ups
        originalScale = transform.localScale;
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;

        // Get layer indices by name
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (isDead) return;
        HandleMovement();
        HandleJumpAndGravity();
        UpdateAnimation();
    }

    // --- Core Logic Methods ---
    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            Vector3 moveDirection = new Vector3(horizontal, 0f, 0f);
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            pendingHorizontalMove = moveDirection.normalized * currentSpeed;
            Vector3 faceDir = horizontal > 0 ? Vector3.right : Vector3.left;
            Quaternion targetRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0;
            pendingHorizontalMove = Vector3.zero;
        }
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpPressed)
        {
            jumpBufferedUntil = Time.time + jumpBufferTime;
        }
    }

    void HandleJumpAndGravity()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
            coyoteUntil = Time.time + coyoteTime;
        }
        if (Time.time < coyoteUntil && Time.time < jumpBufferedUntil)
        {
            verticalVelocity = jumpPower;
            jumpBufferedUntil = 0f;
            coyoteUntil = 0f;
            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger");
                if (crossFadeOnJump && !string.IsNullOrEmpty(jumpStateName))
                {
                    animator.CrossFade(jumpStateName, jumpCrossFadeDuration, jumpStateLayer);
                }
            }
        }
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 finalMove = pendingHorizontalMove + (Vector3.up * verticalVelocity);
        controller.Move(finalMove * Time.deltaTime);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("speed", animatorSpeed);
        animator.SetBool("isGrounded", controller.isGrounded);
        animator.SetFloat("yVelocity", verticalVelocity);
    }

    // --- Public Methods for Interaction ---
    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }

    public void Bounce(float power)
    {
        verticalVelocity = Mathf.Max(verticalVelocity, power > 0f ? power : bouncePower);
        jumpBufferedUntil = 0f;
    }

    public void GetPowerUp(float sizeMultiplier)
    {
        if (hasPowerUp) return;
        Debug.Log("🍄 Power Up!");
        hasPowerUp = true;
        transform.localScale = originalScale * sizeMultiplier;
    }

    public void TakeDamage()
    {
        if (isDead || isInvincible) return; // 무적이거나 죽었으면 리턴
        if (hasPowerUp)
        {
            Debug.Log("💫 Hit, but saved by power-up! Shrinking down.");
            hasPowerUp = false;
            transform.localScale = originalScale;
            walkSpeed = originalWalkSpeed;
            runSpeed = originalRunSpeed;
            StartCoroutine(InvincibilityFrames(1.5f)); // 1.5초 무적 시작
            return;
        }
        Debug.Log("💀 No power-up! Player dies.");
        Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        if (controller != null) controller.enabled = false;

        // animator.SetTrigger("dieTrigger"); // <-- 유저 요청으로 주석 처리 (죽는 애니메이션 없음)

        enabled = false;
        var gameOverUI = FindFirstObjectByType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
        else
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Destroy(gameObject);
    }

    private IEnumerator InvincibilityFrames(float duration)
    {
        Debug.Log("🛡️ Invincibility started!");
        isInvincible = true;

        // 플레이어와 적 레이어 간의 충돌을 끕니다.
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // --- 플레이어 깜빡임 효과 ---
        Renderer playerRenderer = GetComponentInChildren<Renderer>();

        if (playerRenderer != null) // 렌더러가 있는지 확인
        {
            float blinkInterval = 0.1f;
            float endTime = Time.realtimeSinceStartup + duration;

            while (Time.realtimeSinceStartup < endTime)
            {
                playerRenderer.enabled = !playerRenderer.enabled;
                yield return new WaitForSecondsRealtime(blinkInterval);
            }
            playerRenderer.enabled = true; // 깜빡임이 끝나면 반드시 켬
        }
        else // 렌더러가 없으면 그냥 시간만 기다림
        {
            Debug.LogWarning("Player Renderer not found. Waiting for duration.");
            yield return new WaitForSecondsRealtime(duration);
        }
        // -----------------------------

        // 무적이 끝나면 충돌을 다시 켭니다.
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isInvincible = false;
        Debug.Log("🛡️ Invincibility ended.");
    }


    // --- Timed Power-up (Star) ---
    public void GainPower(float sizeUp, float speedUp, float duration)
    {
        if (powerUpRoutine != null)
        {
            StopCoroutine(powerUpRoutine);
        }
        powerUpRoutine = StartCoroutine(PowerUpEffect(sizeUp, speedUp, duration));
    }

    private IEnumerator PowerUpEffect(float sizeUp, float speedUp, float duration)
    {
        transform.localScale += Vector3.one * sizeUp;
        walkSpeed += speedUp;
        runSpeed += speedUp;
        yield return new WaitForSeconds(duration);
        transform.localScale -= Vector3.one * sizeUp;
        walkSpeed -= speedUp;
        runSpeed -= speedUp;
        powerUpRoutine = null;
    }
}