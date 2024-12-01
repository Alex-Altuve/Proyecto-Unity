using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class EsqueletonEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public float Walkspeed = 3f;
    public float malkStopRate = 0.6f;
    public float followDistance = 5f; // Distancia a la que comienza a seguir
    public float stopDistance = 2f;    // Distancia mínima para no estar en la misma posición que el jugador
    public Transform player;            // Referencia al jugador

    public enum WalkableDirection { Left, Right }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    public DetectionZone attackZone;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    void Update()
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
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private bool _hasTarget = false;

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirections();
        }
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
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    void Start()
    {

    }
}