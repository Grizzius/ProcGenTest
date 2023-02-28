using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocation : MonoBehaviour
{
    public Transform player;

    public float smoothTime = 0.1f;

    public float smoothSpeed = 10f;

    public Vector3 offset = new Vector3();

    Vector3 currentVelocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.position + offset, ref currentVelocity, smoothTime, smoothSpeed);
        
    }
}
