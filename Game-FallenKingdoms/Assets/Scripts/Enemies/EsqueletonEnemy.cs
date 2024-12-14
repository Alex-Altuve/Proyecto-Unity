using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class EsqueletonEnemy : MonoBehaviour
{

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public UnityEvent<int, Vector2> damageableHit;

    private bool canMove = true; // Controlador de movimiento
    public float Walkspeed = 3f;
    public float malkStopRate = 0.6f;
    public float followDistance = 5f; // Distancia a la que comienza a seguir
    public float stopDistance = 2f;    // Distancia mínima para no estar en la misma posición que el jugador
    public Transform player;            // Referencia al jugador

    // Variables de salud
    public int maxHealth = 60;
    public int currentHealth;

    //Daño
    [SerializeField]
    private Damageable damageable;

    public enum WalkableDirection { Left, Right }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    public DetectionZone attackZone;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

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

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            // No permitir cambios si el enemigo no está vivo
            if (damageable != null && !damageable.IsAlive)
            {
                _hasTarget = false; // Asegurarse de que sea falso si está muerto
                animator.SetBool(AnimationStrings.hasTarget, false);
                return;
            }

            // Solo actualizar si el nuevo valor es diferente al actual
            if (_hasTarget != value)
            {
                _hasTarget = value;
                animator.SetBool(AnimationStrings.hasTarget, value);
            }
        }
    }


    private bool _hasTarget = false;

    private void FixedUpdate()
    {

        if (!LockVelocity)
        {
            rb.velocity = new Vector2(Walkspeed * walkDirectionVector.x, rb.velocity.y);
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
            FlipDirections();
        }

        // Aplicar movimiento si el enemigo puede moverse
        if (CanMove)
        {
            rb.velocity = new Vector2(Walkspeed * walkDirectionVector.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, malkStopRate), rb.velocity.y);
        }
    }


    private void FlipDirections()
    {
        // No cambiar de dirección si el enemigo no está vivo
        if (damageable != null && !damageable.IsAlive)
        {
            return;
        }

        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }


    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(PrintVelocityCoroutine());
    }

    public void EnemyDamage(int damage, Vector2 knockback)
    {
        FindObjectOfType<AudioManager>().Play("EnemigoHurt");
        currentHealth -= damage;
        Debug.Log("Le estoy haciendo daño");
        animator.SetTrigger(AnimationStrings.hitTrigger);
        damageableHit?.Invoke(damage, knockback);
        // Imprimir el valor de knockback en la consola
        Debug.Log("Knockback - X: " + knockback.x + ", Y: " + knockback.y);
        if (currentHealth <= 0)
        {
            FindObjectOfType<AudioManager>().Stop("EnemigoHurt");
            Debug.Log("Lo maté");
            currentHealth = 0;

            if (damageable != null)
            {
                FindObjectOfType<AudioManager>().Play("EnemigoDeath");
                damageable.IsAlive = false; // Cambia el estado a no vivo
            }
            animator.SetBool(AnimationStrings.canMove, false);

        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        //LockVelocity = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    IEnumerator PrintVelocityCoroutine()
    {
        while (true)
        {
            Debug.Log("Enemy velocity: " + rb.velocity);
            yield return new WaitForSeconds(2.0f);
        }
    }

}