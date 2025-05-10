using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirection), typeof(Damageable ))] 
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float rollSpeed = 12f;
    public float airWalkSpeed = 3f;
    public float jumpImpusle = 10f;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
   

    public float currentMoveSPeed 
    { get 
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
            }else //mOVEMENTlOCK
            {  return 0; }
           
        }
    }
    [SerializeField] private bool _isMoving = false;
    
    public bool IsMoving { get 
        { 
            return _isMoving;
        } private set 
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving,value);
        }
        
    }
    [SerializeField] private bool _isRolling = false;
    public bool IsRolling
    {
        get
        {
            return _isRolling;  
        }
        set
        {
            _isRolling = value;
            animator.SetBool(AnimationStrings.isRolling, value);
        }
    }
    public bool _isFacingRight = true;
    public bool IsFacingRight { get { return _isFacingRight; }
        private set {
            if (_isFacingRight != value)
            {
                //Flip scale sang huong doi dien
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;

        } }
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
        set
        {

        }
    }

    

    Rigidbody2D rb;
    Animator animator;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();

        
    }
  
    private void FixedUpdate()
    {
        if(!damageable.LockVelocity)
        rb.velocity = new Vector2(moveInput.x * currentMoveSPeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingRirection(moveInput);
        }else
        {
            IsMoving=false;
        }
        
    }

    private void SetFacingRirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            //Face Right
            IsFacingRight = true;   
        }
        else if (moveInput.x < 0 && IsFacingRight)
        { 
            //Face left;
            IsFacingRight= false;
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
        //Todo: Check xem con song khong nua nhung ma lam sau
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

    public void OnHit(float damage, Vector2 knockback, bool lockVelocity)
    {
        
        rb.velocity = new Vector2(knockback.x,rb.velocity.y + knockback.y);
    }

}

