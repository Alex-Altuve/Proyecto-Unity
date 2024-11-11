using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class EsqueletonEnemy : MonoBehaviour
{

   
    // Start is called before the first frame update
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public float Walkspeed = 3f;
    public float malkStopRate = 0.6f;
    public enum WalkableDirection { Left, Right}
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    public DetectionZone attackZone;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }
   
    // Update is called once per frame
    void Update()
    {
        HasTarget= attackZone.detectedColliders.Count > 0;
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
                }else if (_walkDirection == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    public bool HasTarget {
        get { return _hasTarget; } private set
        {
           _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        } }

    public bool _hasTarget = false;

    private void FixedUpdate()
    {
        if(touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirections();
        }
        if (CanMove)
        {
            rb.velocity = new Vector2(Walkspeed * walkDirectionVector.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x,0, malkStopRate), rb.velocity.y);
        }
       

    }

    private void FlipDirections()
    {
        if (WalkDirection == WalkableDirection.Right )
        {
            WalkDirection = WalkableDirection.Left;
        }else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current walkable directions is not set to legal values of right o left");
        }
    }

    public bool CanMove
    {
        get{ 
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    void Start()
    {
        
    }

   
}
