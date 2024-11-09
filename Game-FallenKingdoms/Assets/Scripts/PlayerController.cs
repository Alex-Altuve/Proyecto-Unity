using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    Vector2 moveInput;

    public bool _isMoving = false;

    public float leftLimit = -15f;  // Límite izquierdo
    public float rightLimit = 15f;  // Límite derecho

    // Variables de salud
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;


    // Variables de stamina
    public int maxStamina = 100;
    public int currentStamina;
    public StaminaBar staminaBar;
    public int staminaCostPerAttack = 10; // Costo de stamina por cada ataque
    public float staminaRegenRate = 5f; // Cantidad de stamina que se regenera por segundo
    //private bool isAttacking = false; // Controla si el jugador está atacando
    private float lastAttackTime; // Tiempo del último ataque

    ///Mensaje de perdida
    public Text TextGameOver;


    void Start()
    {
        // Inicialización de salud
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Inicialización de stamina
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

        // Inicializar el tiempo del último ataque
        lastAttackTime = -1f;
    }

    public float CurrentMoventSpeed
    {
        get
        {
            if (CanMove)
            {
                return IsMoving ? walkSpeed : 0;
            }
            return 0;
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

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }

        // Verifica si han pasado 2 segundos desde el último ataque antes de regenerar stamina
        if (Time.time - lastAttackTime >= 0.75f && currentStamina < maxStamina)
        {
            GetStamina(5); // Recupera 5 de stamina
            lastAttackTime = Time.time; // Reinicia el tiempo del último ataque para el siguiente intervalo de 2 segundos
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
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
        //Borrar para arreglar el error de los limites 
        //rb.velocity = new Vector2(moveInput.x * CurrentMoventSpeed, 0);
        // Calcular la nueva posición x
        float newXPosition = rb.position.x + moveInput.x * CurrentMoventSpeed * Time.fixedDeltaTime;

        // Aplicar límites
        newXPosition = Mathf.Clamp(newXPosition, leftLimit, rightLimit);

        // Actualizar la posición del Rigidbody2D con los límites aplicados
        rb.MovePosition(new Vector2(newXPosition, rb.position.y));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        SetFacingDirection(moveInput);
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
        if (context.started && currentStamina >= staminaCostPerAttack)
        {
            currentStamina -= staminaCostPerAttack; // Reduce la stamina por el costo del ataque
            staminaBar.SetStamina(currentStamina); // Actualiza la barra de stamina
            animator.SetTrigger(AnimationStrings.attack);

            lastAttackTime = Time.time; // Actualiza el tiempo del último ataque
        }
    }

    ///Mensajes en caso que gane o pierda
    public void GameOver()
    {
        if (currentHealth == 0)
        {
            TextGameOver.text = "GAME OVER";
            StartCoroutine(PausarJuego());
        }
    }

    /// Esto es para detener el juego
    IEnumerator PausarJuego()
    {
        yield return new WaitForSeconds(3F);

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
}