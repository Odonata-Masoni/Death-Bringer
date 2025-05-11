using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    public UnityEvent noColliderRemain;

    public List<Collider2D> detectedCollider= new List<Collider2D>();
    Collider2D col;
        private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedCollider.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedCollider.Remove(collision);
        if(detectedCollider.Count <= 0)
        {
            noColliderRemain.Invoke();
        }
    }
}
