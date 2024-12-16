using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class BossController : MonoBehaviour
{
    private bool isBattleMusicPlaying = false;

    public UnityEvent<int, Vector2> damageableHit;

    public float walkspeed = 3f;

    public float enragedSpeed = 4.5f; // Velocidad de fase 2 del boss
    private bool isEnraged = false;  // Flag para verificar si ya se activó el modo "enfurecido"

    public float walkStopRate = 0.6f;

    public float followDistance = 5f;

    public float stopDistance = 2f;

    private bool canMove = true;

    public int maxHealth = 200;
    public int currentHealth;

    public HealthBar bossHealthBar;

    public Transform player;

    Rigidbody2D rb;

    TouchingDirections touchingDirections;

    Animator animator;

    public DetectionZone attackZone;

    //Daño
    [SerializeField]
    private Damageable damageable;

    public enum WalkableDirection { Left, Right }

    private WalkableDirection _walkDirection;

    private Vector2 walkDirectionVector = Vector2.right;

    public bool _hasTarget = false;

    public bool HasTarget 
    { 
        get 
        { 
            return _hasTarget; 
        } 
        private set 
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);    
        } 
    
    
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public WalkableDirection WalkDirection
    {                 
        get { return _walkDirection; }
        set 
        {
            if (_walkDirection != value) 
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (_walkDirection == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (_walkDirection == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;  
        }
    }

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {

        if (!LockVelocity)
        {
            rb.velocity = new Vector2(walkspeed * walkDirectionVector.x, rb.velocity.y);
        }

        // Detener el movimiento si no puede moverse o si está muerto
        if (!canMove || !CanMove || (damageable != null && !damageable.IsAlive))
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Mantener la velocidad en cero
            return; // No ejecutar más lógica si no puede moverse o está muerto
        }

        // Cambiar dirección si está en el suelo y tocando una pared, solo si está vivo
        if (damageable != null && damageable.IsAlive && touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }



        if (CanMove)
        {
            rb.velocity = new Vector2(walkspeed * walkDirectionVector.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }

    }

    private void FlipDirection()
    {
        // No cambiar de dirección si el enemigo no está vivo
        if (damageable != null && !damageable.IsAlive)
        {
            return;
        }

        
    }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        bossHealthBar.SetMaxHealth(maxHealth);

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (damageable != null && damageable.IsAlive)
        {
            HasTarget = attackZone.detectedColliders.Count > 0;

            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer < followDistance && distanceToPlayer > stopDistance)
                {
                    // Moverse hacia el jugador
                    if (transform.position.x > player.position.x)
                    {
                        WalkDirection = WalkableDirection.Right;
                    }
                    else
                    {
                        WalkDirection = WalkableDirection.Left;
                    }
                }
                else
                {
                    // Detenerse si está dentro de la distancia mínima
                    WalkDirection = WalkDirection; // Mantiene la dirección actual
                }

            }



            }

            
    }

    public void BossDamage(int damage, Vector2 knockback)
    {

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true); // Activar la barra de salud
        }

        if (!isBattleMusicPlaying)
        {
            FindObjectOfType<AudioManager>().Stop("Theme");
            FindObjectOfType<AudioManager>().Play("Batalla");
            isBattleMusicPlaying = true; // Marcar que la música ya está sonando
        }

        currentHealth -= damage;
        bossHealthBar.SetHealth(currentHealth);
        animator.SetTrigger(AnimationStrings.hitTrigger);
        damageableHit?.Invoke(damage, knockback);

        // Verificar si la vida está al 30% o menos y no está ya enfurecido
        if (currentHealth <= (maxHealth * 0.3f) && !isEnraged)
        {
            isEnraged = true; // Activar flag para evitar múltiples activaciones
            walkspeed = enragedSpeed; // Aumentar la velocidad
            Debug.Log("El jefe está enfurecido y su velocidad aumenta!");
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Maté al Boss");
            currentHealth = 0;

            if (damageable != null)
            {
                Debug.Log("entré aquí");
                FindObjectOfType<AudioManager>().Stop("Batalla");
                FindObjectOfType<AudioManager>().Stop("Theme");
                SceneManager.LoadScene("WinChangeWorld");
                damageable.IsAlive = false;
            }
            animator.SetBool(AnimationStrings.canMove, false);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        //LockVelocity = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

}
