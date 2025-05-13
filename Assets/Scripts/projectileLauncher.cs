using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileLauncher : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectilePrefab;
    public Transform launchPoint;
    public void FireProjectile()
    {
       GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(
            origScale.x*transform.localScale.x>0? 0.6f:-0.6f,
            origScale.y,
            origScale.z * transform.localScale.x > 0 ? 0.6f : 0.6f
            );
    }    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
