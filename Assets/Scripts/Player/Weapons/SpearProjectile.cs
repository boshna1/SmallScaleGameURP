using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    public float forceY;
    public float forceX;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Debug.Log(transform.rotation.eulerAngles.z);
        if (transform.rotation.eulerAngles.z == 0)
        {
            rb.velocity = new Vector2(forceX,0);
        }
        if (transform.rotation.eulerAngles.z == -180 || transform.rotation.eulerAngles.z == 180)
        {
            rb.velocity = new Vector2(-forceX, 0);
        }
        if (transform.rotation.eulerAngles.z == 90)
        {
            rb.velocity = new Vector2(0, forceY);
        }
        if (transform.rotation.eulerAngles.z == 270)
        {
            rb.velocity = new Vector2(0, -forceY);
        }
        if (IsAngleApproximately(transform.rotation.eulerAngles.z, 45))
        {
            rb.velocity = new Vector2(forceX, forceY);
        }
        if (transform.rotation.eulerAngles.z == 225)
        {
            rb.velocity = new Vector2(-forceX, -forceY);
        }
        if (transform.rotation.eulerAngles.z == 135)
        {
            rb.velocity = new Vector2(-forceX, forceY);
        }
        if (transform.rotation.eulerAngles.z == 315)
        {
            rb.velocity = new Vector2(forceX, -forceY);
        }
    }

    bool IsAngleApproximately(float angle, float target, float tolerance = 1f)
    {
        return Mathf.Abs(Mathf.DeltaAngle(angle, target)) <= tolerance;
    }
}
