using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArrow : MonoBehaviour
{
    Rigidbody2D rb;
    BowAttack bowAttack;
    public float arrowForce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bowAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<BowAttack>();
        Invoke("StartDropOff", 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(arrowForce,rb.velocity.y);
    }

    public void StartDropOff()
    {
        rb.gravityScale = 5f;
    }
}
