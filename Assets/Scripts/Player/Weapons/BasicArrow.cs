using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArrow : MonoBehaviour
{
    Rigidbody2D rb;
    BowAttack bowAttack;
    public Vector2 arrowForce;

    public bool dropOff = false;
    public float arrowXTowards;
    public float arrowYTowards;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bowAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<BowAttack>();
        Invoke("StartDropOff", 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        if (dropOff)
        {
            arrowForce = Vector2.MoveTowards(arrowForce, new Vector2(arrowXTowards, arrowYTowards),0.5f);
            arrowYTowards = Mathf.MoveTowards(arrowYTowards, -10, 0.5f);
            arrowXTowards = Mathf.MoveTowards(arrowXTowards, 5, 0.5f);
        }
        Debug.Log(rb.angularVelocity);
        rb.velocity = arrowForce;
    }

    public void StartDropOff()
    {
        dropOff = true;
    }
}
