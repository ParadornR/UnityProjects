using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections),typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float runSpeed = 10f;
    public float airWalkSpeed = 8f;
    public float CrouchSpeed = 5f;
    public float jumpImpulse = 10f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsRunning && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        if (IsCrouchWalk)
                        {
                            return CrouchSpeed;
                        }
                        else
                        {
                            return runSpeed;
                        }
                    }
                    else
                    {
                        // Air Move
                        return airWalkSpeed;
                    }
                }
                else
                {
                    // Idle speed is 0
                    return 0;
                }
            }
            else
            {
                // Movement locked
                return 0;
            }

        }
    }

    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    [SerializeField]
    private bool _isCrouchWalk = false;
    public bool IsCrouchWalk
    {
        get
        {
            return _isCrouchWalk;
        }
        private set
        {
            _isCrouchWalk = value;
            animator.SetBool(AnimationStrings.isCrouchWalk, value);
        }
    }

    [SerializeField]
    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {

            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }


    [SerializeField]
    private bool _isAttacking = false;
    public bool IsAttacking
    {
        get { return _isAttacking; }
        private set
        {
            _isAttacking = value;
            animator.SetBool(AnimationStrings.attackTrigger, value);
        }
    }

    public bool LockVelosity { get {
            return animator.GetBool(AnimationStrings.lockVelocity);
        } }

 

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive && CanMove)
        {
            IsRunning = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
        else
        {
            IsRunning = false;
        }
        
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x < 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x > 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnCrouchWalk(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsCrouchWalk = true;
        }
        else if (context.canceled)
        {
            IsCrouchWalk = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }

    }
 
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback) {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
