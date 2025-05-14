using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public DetectionZone biteDetectionZone;
    Animator animator;
    Rigidbody2D rb;

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    [SerializeField] private float attackCooldown = 2f; // Thời gian chờ giữa các lần tấn công
    [SerializeField] private float attackRange = 2f; // Khoảng cách tối đa để tấn công
    private float lastAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator not found on " + gameObject.name);
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D not found on " + gameObject.name);
    }

    void Update()
    {
        HasTarget = biteDetectionZone.detectedCollider.Count > 0;

        if (HasTarget && Time.time >= lastAttackTime + attackCooldown)
        {
            if (biteDetectionZone.detectedCollider.Count > 0)
            {
                Transform target = biteDetectionZone.detectedCollider[0].transform;
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (distanceToTarget <= attackRange)
                {
                    TriggerAttack();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("attackTrigger");
            Debug.Log("Attack triggered on FlyingEye!");
        }
    }
}