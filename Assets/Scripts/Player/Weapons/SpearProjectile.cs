using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();       
    }

    void Update()
    {
        rb.velocity = new Vector2(0,-1 * speed);
    }
}
