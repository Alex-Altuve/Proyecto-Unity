using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f; // Velocidad al correr
    public float jumpImpulse = 17f;
    Vector2 moveInput;
    
    Collider2D playerCollider;
    Rigidbody2D playerRigidBody;
    Rigidbody2D rb;
    Animator animator;

    TouchingDirections touchingDirections;

    public bool _isMoving = false;
    private bool canMove = true; // Controlador de movimiento
    private bool isRunning = false; // Controla si est� corriendo
    private bool isRolling = false;
    private float rollDuration = 2f; // Duraci�n de la acci�n de rodar
    private float slideDuration = 1f; // Duraci�n de la acci�n de deslizar
    private Coroutine rollCoroutine;
    private Coroutine slideCoroutine;
    // Variables de salud
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    // Variables de stamina
    public int maxStamina = 100;
    public int currentStamina;
    public StaminaBar staminaBar;
    
    ///Reduccion de estamina por accion
    public int staminaCostPerRoll = 15;
    public int staminaCostPerSlide = 10;
    public int staminaCostPerAttack = 10;
    public int staminaCostPerRun = 5;
    private float staminaReductionTimer = 0.5f;
    public float staminaRegenRate = 5f;
   
    // Ataque
    private float lastAttackTime;
   
    //Da�o
    [SerializeField]
    private Damageable damageable;
    
    //Mensaje
    public Text TextGameOver;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();

    }
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

    void Update()
    {
        Die();

        if (!isRunning && Time.time - lastAttackTime >= 0.75f && currentStamina < maxStamina)
        {
            GetStamina(5);
            lastAttackTime = Time.time;
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isRolling = Input.GetKey(KeyCode.J);
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

        ReduccionEstamina_Mecanica();

    }

    //Da�o y Estamina
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
            animator.SetBool(AnimationStrings.canMove, false);
            GameOver();
        }
    }

    void GetStamina(int stamina)
    {
        currentStamina += stamina;
        staminaBar.SetStamina(currentStamina);
    }

    //Reduccion de estamina 
    public void ReduccionEstamina_Mecanica()
    {
        // Aseg�rate de que el jugador est� en movimiento y corriendo para reducir la estamina
        if (isRunning && IsMoving && currentStamina >= staminaCostPerRun)
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
            staminaReductionTimer = 0.5f; // Reiniciar el temporizador si no est� corriendo
        }
    }

    ///Logica movimiento racing and ismoving
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

    //Mecanicas ----------------------------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // Si el jugador est� rodando o desliz�ndose, no permitir cambiar de direcci�n
        if (animator.GetBool(AnimationStrings.isRolling) || animator.GetBool(AnimationStrings.isSliding))
        {
            // Solo permitir el movimiento hacia la direcci�n actual
            if (moveInput.x != 0 && (IsFacingRight && moveInput.x < 0 || !IsFacingRight && moveInput.x > 0))
            {
                // Cancelar la acci�n de rodar o deslizar
                if (rollCoroutine != null)
                {
                    StopCoroutine(rollCoroutine);
                    rollCoroutine = null;
                    animator.SetBool(AnimationStrings.isRolling, false);
                }

                if (slideCoroutine != null)
                {
                    StopCoroutine(slideCoroutine);
                    slideCoroutine = null;
                    animator.SetBool(AnimationStrings.isSliding, false);
                }
                return; // No permitir movimiento en direcci�n opuesta
            }
        }

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
        // No permitir saltar si el jugador est� muerto o no puede moverse
        if (!canMove || !damageable.IsAlive) return;

        // Verificar si el jugador est� rodando, desliz�ndose o agachado
        if (animator.GetBool(AnimationStrings.isRolling) || animator.GetBool(AnimationStrings.isSliding) || animator.GetBool(AnimationStrings.isCrouching))
        {
            return; // No permitir saltar en estos estados
        }

        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }


    public void OnRoll(InputAction.CallbackContext context)
    {
        // No permitir rodar si el jugador est� muerto o no puede moverse
        if (!canMove || !damageable.IsAlive) return;

        if (context.started && rollCoroutine == null)
        {
            // Verificar si hay suficiente stamina para rodar
            if (currentStamina >= staminaCostPerRoll)
            {
                currentStamina -= staminaCostPerRoll; // Reducir stamina
                staminaBar.SetStamina(currentStamina); // Actualizar la barra de stamina
                rollCoroutine = StartCoroutine(Roll());
            }
        }
    }

    private IEnumerator Roll()
    {
        animator.SetBool(AnimationStrings.isRolling, true);
        animator.SetTrigger(AnimationStrings.roll);

        yield return new WaitForSeconds(rollDuration);

        animator.SetBool(AnimationStrings.isRolling, false);
        rollCoroutine = null; // Reiniciar la coroutine
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        // No permitir deslizar si el jugador est� muerto o no puede moverse
        if (!canMove || !damageable.IsAlive) return;

        if (context.started && slideCoroutine == null)
        {
            // Verificar si hay suficiente stamina para deslizar
            if (currentStamina >= staminaCostPerSlide) // Usar el mismo coste que para rodar
            {
                currentStamina -= staminaCostPerSlide; // Reducir stamina
                staminaBar.SetStamina(currentStamina); // Actualizar la barra de stamina
                slideCoroutine = StartCoroutine(Slide());
            }
        }
    }

    private IEnumerator Slide()
    {
        animator.SetBool(AnimationStrings.isSliding, true);
        animator.SetTrigger(AnimationStrings.slide);

        yield return new WaitForSeconds(slideDuration);

        animator.SetBool(AnimationStrings.isSliding, false);
        slideCoroutine = null; // Reiniciar la coroutine
    }

    public void OnCruch(InputAction.CallbackContext context)
    {
        // No permitir rodar si el jugador est� muerto o no puede moverse
        if (!canMove || !damageable.IsAlive) return;

        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.crouch);
            animator.SetBool(AnimationStrings.isCrouching, true);
            if (IsMoving)
            {
                animator.SetTrigger(AnimationStrings.crouchWalk);
            }
            
        }
        else if (context.canceled)
        {
            // Si el contexto se cancela, restablece el estado
            animator.SetBool(AnimationStrings.isCrouching, false);

        }
    }

    /// Mensajes
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
 
    }

    
}