using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{

    Rigidbody2D rb;

    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;

    CapsuleCollider2D touchinCol;

    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded; 
    

    public bool IsGrounded { get {
            return _isGrounded;
        } private set { 
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        } }

    private void Awake()
    {
        touchinCol = GetComponent<CapsuleCollider2D>();  
        animator = GetComponent<Animator>();
    }

    

    void FixedUpdate()
    {
        IsGrounded = touchinCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
    }
}
