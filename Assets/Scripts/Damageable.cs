using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;
    
    Animator animator;
    [SerializeField]private float _maxHealth = 100f;
    public float MaxHealth
    { 
    get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }
    [SerializeField]private float _health =100f;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }
    [SerializeField] 
    private bool _isAlive = true;
    private bool isInvincible = false;

    

    private float timeSinceHit = 0;
    public float isInvincibilityTime = 0.25f;

    public bool IsAlive
    {
        get
        {
        return _isAlive;
        }
        set
        {  
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive,value);
            Debug.Log("IsAlive set" + value);
        }
    }
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        { 
        animator.SetBool(AnimationStrings.lockVelocity,value);
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > isInvincibilityTime)
            {
                // remove invincibility
                isInvincible = false ;
                timeSinceHit = 0 ;
            }
            timeSinceHit += Time.deltaTime;
        }
        
    }
    //Return wether the damageable toook d
    public bool Hit(float damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            //Notify other subscribed components that the damageable was hit to handle the knockback
            //IsHit = true;
            damageableHit?.Invoke(damage,knockback);

            return true;
        }
        return false;
    }
}
