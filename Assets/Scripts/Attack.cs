using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //Collider2D attackColldider;
    public float attackDamage =10f;
    public Vector2 knockback = Vector2.zero;

    //private void Awake()
    //{
    //    attackColldider = GetComponent<Collider2D>();
    //}
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Xem co attack trung khong
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliverdKnockback = transform.parent.localScale.x > 0 ? knockback: new Vector2(-knockback.x, knockback.y);
            bool gotHit = damageable.Hit(attackDamage, deliverdKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + "hit for " + attackDamage);
            }
        }
    }
}
