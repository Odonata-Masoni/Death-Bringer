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
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float airControlMultiplier = 0.8f;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
    private bool isJumping = false;

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
                        return IsRolling ? rollSpeed : walkSpeed;
                    }
                    else
                    {
                        return airWalkSpeed * airControlMultiplier;
                    }
                }
                return 0;
            }
            return 0;
        }
    }

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            if (animator != null) animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField] private bool _isRolling = false;
    public bool IsRolling
    {
        get { return _isRolling; }
        set
        {
            _isRolling = value;
            if (animator != null) animator.SetBool(AnimationStrings.isRolling, value);
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
        get { return animator != null && animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get { return damageable != null && damageable.IsAlive; }
        set { if (damageable != null) damageable.IsAlive = value; }
    }

    Rigidbody2D rb;
    Animator animator;
    private float knockbackDuration = 0.2f;
    private float knockbackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D not found on " + gameObject.name);

        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator not found on " + gameObject.name);

        touchingDirection = GetComponent<TouchingDirection>();
        if (touchingDirection == null) Debug.LogError("TouchingDirection not found on " + gameObject.name);

        damageable = GetComponent<Damageable>();
        if (damageable == null) Debug.LogError("Damageable not found on " + gameObject.name);
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

            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0 && !isJumping)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
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
        if (IsAlive)
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
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsAlive && touchingDirection.IsGrounded && CanMove)
        {
            if (context.started)
            {
                isJumping = true;
                if (animator != null) animator.SetTrigger(AnimationStrings.jumpTrigger);
                if (rb != null) rb.velocity = new Vector2(rb.velocity.x, jumpImpusle);
            }
            else if (context.canceled)
            {
                isJumping = false;
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started  && !IsRolling)
        {
            Debug.Log("Attack triggered!");
            if (animator != null) animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Range Attack triggered!");
            if (animator != null) animator.SetTrigger(AnimationStrings.rangeAttackTrigger);
        }
    }

    public void HandleAttackHit()
    {
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach (var hit in hits)
            {
                Skeleton skeleton = hit.GetComponent<Skeleton>();
                if (skeleton != null)
                {
                    skeleton.OnHit(10f, new Vector2(IsFacingRight ? 5f : -5f, 2f));
                }
            }
        }
    }

    public void OnHitPlayer(float damage, Vector2 knockback, bool lockVelocity)
    {
        Debug.Log($"Player OnHitP called with damage: {damage}, knockback: {knockback}");
        if (damageable != null)
        {
            damageable.LockVelocity = lockVelocity;
            knockbackTimer = knockbackDuration;
        }
        if (rb != null)
        {
            Debug.Log($"Applying knockback: {knockback}");
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        }
    }
}