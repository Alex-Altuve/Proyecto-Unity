using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class EsqueletonEnemy : MonoBehaviour
{

    public float Walkspeed = 3f;
    // Start is called before the first frame update
    Rigidbody2D rb;
    public enum WalkableDirection { Left, Right}
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    TouchingDirections touchingDirections;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
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

    private void FixedUpdate()
    {
        if(touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirections();
        }
        rb.velocity = new Vector2(Walkspeed * walkDirectionVector.x, rb.velocity.y);

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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
