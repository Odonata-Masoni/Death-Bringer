using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float rollSpeed = 12f;
    public float airWalkSpeed = 3f;
    public float jumpImpusle = 10f;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirection.IsOnWall)
                {
                    if (touchingDirection.IsGrounded)
                    {
                        if (IsRolling)
                        {
                            return rollSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0; // (Idle speed)
                }
            }
            else // MOVEMENT LOCK
            {
                return 0;
            }
        }
    }

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField] private bool _isRolling = false;
    public bool IsRolling
    {
        get { return _isRolling; }
        set
        {
            _isRolling = value;
            animator.SetBool(AnimationStrings.isRolling, value);
        }
    }

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
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.isAlive); }
        set { } // Không cần gán lại, để Damageable xử lý
    }

    Rigidbody2D rb;
    Animator animator;
    private float knockbackDuration = 0.2f;
    private float knockbackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0 && damageable != null)
            {
                damageable.LockVelocity = false;
            }
        }
        else if (damageable != null && !damageable.LockVelocity && rb != null)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }
        if (animator != null && rb != null)
        {
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingRirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingRirection(Vector2 moveInput)
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

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRolling = true;
        }
        else if (context.canceled)
        {
            IsRolling = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpusle);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Attack triggered!");
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public bool GetLockVelocity()
    {
        return damageable.LockVelocity;
    }

    public void OnHitPlayer(float damage, Vector2 knockback, bool lockVelocity)
    {
        Debug.Log($"Player OnHitP called with damage: {damage}, knockback: {knockback}");
        if (damageable != null)
        {
           // damageable.Hit(damage, knockback);
            damageable.LockVelocity = lockVelocity;
        }
        if (rb != null)
        {
            Debug.Log($"Applying knockback: {knockback}");
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }
    }
}