using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam; //Ti nua nhet BG vao day
    public Transform followTarget; // Nhet player vao day
    // Vi tri bat dau cho cai parallax GO;
    Vector2 startingPosition;
    //Doan nay can vi tri Z khoi dau, noi chung chieu sau lien quan den Z value, thu tu truoc sau nen la phai khai bao cai nay
    float startingZ;

    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;
    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane: cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;
    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;
        transform.position = new Vector3(newPosition.x,newPosition.y, startingZ);
    }
}
