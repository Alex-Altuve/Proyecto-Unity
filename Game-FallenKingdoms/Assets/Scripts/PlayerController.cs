using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    Vector2 moveInput;

    public bool _isMoving = false;

    public float leftLimit = -15f;  // Límite izquierdo
    public float rightLimit = 15f;  // Límite derecho

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

    private void FixedUpdate()
    {
        //Borrar para arreglar el error de los limites 
        //rb.velocity = new Vector2(moveInput.x * CurrentMoventSpeed, 0);
        // Calcular la nueva posición x
        float newXPosition = rb.position.x + moveInput.x * CurrentMoventSpeed * Time.fixedDeltaTime;

        // Aplicar límites
        newXPosition = Mathf.Clamp(newXPosition, leftLimit, rightLimit);

        // Actualizar la posición del Rigidbody2D
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
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attack);
        }
    }
}