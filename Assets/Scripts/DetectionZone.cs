using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
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
    }
}
