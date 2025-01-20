using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Sensor_Bandit))]
public class BandidoEnemy_BOSS : MonoBehaviour
{
    private Rigidbody2D rb;
    private Sensor_Bandit groundSensor;
    private Animator animator;
    public HealthBar bossHealthBar;
    public UnityEvent<int, Vector2> damageableHit;

    // Variables para movimiento y salud
    public float walkSpeed = 4f;
    public float jumpForce = 7.5f;
    public Transform player; // Referencia al jugador
    public float followDistance = 5f; // Distancia a la que comienza a seguir
    public float stopDistance = 2f;    // Distancia mínima del jugador
    public float attackDistance = 1f;   // Distancia para atacar
    public int maxHealth = 100;
    public int currentHealth;

    private bool canMove = true;
    private bool isGrounded;
    private bool isDead;

    // Referencias a componentes
    [SerializeField]
    private Damageable damageable;
    public DetectionZone attackZone;

    // Nuevo: Temporizador para ataques
    private float attackCooldown = 0.7f; // Intervalo de ataque
    private float lastAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponent<Sensor_Bandit>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = Time.time; // Inicializa el tiempo del último ataque
        bossHealthBar.SetMaxHealth(maxHealth);

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    private void UpdateGroundedState()
    {
        isGrounded = groundSensor.State();
        animator.SetBool("Grounded", isGrounded);
    }

    private void Update()
    {
        UpdateGroundedState();

        if (isDead) return; // No hacer nada si está muerto

        // Comprobar la distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followDistance)
        {
            // Solo moverse si la distancia al jugador es mayor que la distancia de parada
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }

            // Verificar si el jugador está dentro de la distancia de ataque y al frente
            if (distanceToPlayer <= attackDistance && IsPlayerInFront())
            {
                AttackPlayer();
            }
        }
        else
        {
            // Si el jugador se aleja demasiado, no atacar
            animator.SetInteger("AnimState", 0); // Cambiar a estado de inactividad
        }
    }

    private bool IsPlayerInFront()
    {
        // Comprobar si el jugador está a la derecha o a la izquierda del enemigo
        return (player.position.x > transform.position.x && transform.localScale.x < 0) ||
               (player.position.x < transform.position.x && transform.localScale.x > 0);
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y);

        // Cambiar dirección del sprite
        if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
        else if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda

        animator.SetInteger("AnimState", 2); // Cambiar a caminar
    }

    private void AttackPlayer()
    {
        // Solo atacar si ha pasado el tiempo de cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack"); // Lanzar la animación de ataque
            if (damageable != null && damageable.IsAlive)
            {
                // Lógica para aplicar daño al jugador
                player.GetComponent<PlayerController>().TakeDamage(10); // Cambia 10 por el daño que deseas infligir
            }

            lastAttackTime = Time.time; // Reinicia el temporizador de ataque
        }
    }

    public void EnemyDamage(int damage, Vector2 knockback)
    {
        if (damageable != null && damageable.IsAlive)
        {
            FindObjectOfType<AudioManager>().Play("EnemigoHurt");
            currentHealth -= damage;
            bossHealthBar.SetHealth(currentHealth);
            animator.SetTrigger("Hurt");
            damageableHit?.Invoke(damage, knockback);
            if (bossHealthBar != null)
            {
                bossHealthBar.gameObject.SetActive(true); // Activar la barra de salud
            }
            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetTrigger("Death");
                rb.velocity = Vector2.zero; // Detener movimiento al morir
                // Aquí puedes añadir lógica adicional para manejar la muerte
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        EnemyDamage(damage, knockback);
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}