using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f; // Velocidad al correr
    Vector2 moveInput;
    Collider2D playerCollider;
    Rigidbody2D playerRigidBody;

    TouchingDirections touchingDirections;

    public bool _isMoving = false;
    private bool canMove = true; // Controlador de movimiento

    public float leftLimit = -15f;  // L�mite izquierdo
    public float rightLimit = 15f;  // L�mite derecho

    // Variables de salud
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    // Variables de stamina
    public int maxStamina = 100;
    public int currentStamina;
    public StaminaBar staminaBar;
    public int staminaCostPerAttack = 10;
    public int staminaCostPerRun = 5;
    public float staminaRegenRate = 5f;
    private float lastAttackTime;
    private float staminaReductionTimer = 0.5f;

    [SerializeField]
    private Damageable damageable;

    public Text TextGameOver;

    Rigidbody2D rb;
    Animator animator;
    public float jumpImpulse = 17f;

    private bool isRunning = false; // Controla si est� corriendo

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

        lastAttackTime = -1f;

        playerCollider = gameObject.GetComponent<Collider2D>();
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();

        if (damageable == null)
        {
            damageable = GetComponent<Damageable>();
        }
    }

    public float CurrentMoventSpeed
    {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall && canMove) // Verifica si puede moverse
            {
                if (isRunning && currentStamina >= staminaCostPerRun)
                {
                    return runSpeed;
                }
                else
                {
                    return walkSpeed;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    [SerializeField]
    public bool IsMoving
    {
        get => _isMoving;
        set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    void Update()
    {
        Die();

        if (!isRunning && Time.time - lastAttackTime >= 0.75f && currentStamina < maxStamina)
        {
            GetStamina(5);
            lastAttackTime = Time.time;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            if (damageable != null)
            {
                damageable.IsAlive = false; // Cambia el estado a no vivo
            }

            Debug.Log("Player has died.");
            animator.SetBool(AnimationStrings.canMove, false);
            GameOver();
        }
    }

    void GetStamina(int stamina)
    {
        currentStamina += stamina;
        staminaBar.SetStamina(currentStamina);
    }

    private void FixedUpdate()
    {
        // Detener el movimiento si no puede moverse
        if (!canMove || !CanMove)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(moveInput.x * CurrentMoventSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        if (isRunning && currentStamina >= staminaCostPerRun)
        {
            staminaReductionTimer -= Time.fixedDeltaTime;

            if (staminaReductionTimer <= 0)
            {
                currentStamina -= staminaCostPerRun;
                staminaBar.SetStamina(currentStamina);
                staminaReductionTimer = 0.5f; // Reiniciar el temporizador
            }
        }
        else
        {
            staminaReductionTimer = 0.5f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (damageable.IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && currentStamina >= staminaCostPerAttack)
        {
            currentStamina -= staminaCostPerAttack;
            staminaBar.SetStamina(currentStamina);
            animator.SetTrigger(AnimationStrings.attack);
            lastAttackTime = Time.time;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // No permitir saltar si el jugador est� tocando el layer "Vacio"
        if (!canMove) return;

        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void GameOver()
    {
        TextGameOver.text = "GAME OVER";
        StartCoroutine(PausarJuego());
    }

    IEnumerator PausarJuego()
    {
        yield return new WaitForSeconds(4F);
        QuitGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Die()
    {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Vacio")))
        {
            canMove = false; // Desactivar el movimiento al tocar el layer "Vacio"
            GameOver();
        }
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            // Puedes agregar aqu� la l�gica para manejar el da�o o muerte por enemigos
            // animator.SetTrigger("dead");
        }
    }

    
}