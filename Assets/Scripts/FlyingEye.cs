using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 3f;
    public DetectionZone biteDetectionZone;
    public List<Transform> wayPoints;
    public float wayPointReachedDistance = 0.1f;
    public Collider2D deathCollider;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWayPoint;
    int wayPointNum = 0;

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
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
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
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        if (wayPoints != null && wayPoints.Count > 0)
        {
            nextWayPoint = wayPoints[wayPointNum];
        }
        else
        {
            Debug.LogError("Waypoints list is empty or not assigned for FlyingEye!");
        }
    }

    private void OnEnable()
    {
        if (damageable != null && damageable.damageableDeath != null)
        {
            // Đăng ký sự kiện không tham số
            damageable.damageableDeath.AddListener(OnDeath);
        }
    }

    private void OnDisable()
    {
        if (damageable != null && damageable.damageableDeath != null)
        {
            // Hủy đăng ký sự kiện để tránh lỗi
            damageable.damageableDeath.RemoveListener(OnDeath);
        }
    }

    void Update()
    {
        HasTarget = biteDetectionZone != null && biteDetectionZone.detectedCollider.Count > 0;

        if (HasTarget && Time.time >= lastAttackTime + attackCooldown)
        {
            if (biteDetectionZone.detectedCollider.Count > 0)
            {
                Transform target = biteDetectionZone.detectedCollider[0].transform;
                float distanceToTarget = Vector2.Distance((Vector2)transform.position, (Vector2)target.position);
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

    private void FixedUpdate()
    {
        if (damageable != null && damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void Flight()
    {
        if (nextWayPoint == null) return;

        // Bay đến waypoint tiếp theo
        Vector2 directionToWayPoint = ((Vector2)nextWayPoint.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance((Vector2)nextWayPoint.position, (Vector2)transform.position);
        rb.velocity = directionToWayPoint * flightSpeed;

        UpdateDirection();
        if (distance < wayPointReachedDistance)
        {
            wayPointNum++;
            if (wayPointNum >= wayPoints.Count)
            {
                wayPointNum = 0;
            }
            nextWayPoint = wayPoints[wayPointNum];
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        if (rb != null)
        {
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (deathCollider != null)
        {
            deathCollider.enabled = true;
        }
    }
}