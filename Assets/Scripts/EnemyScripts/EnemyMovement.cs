using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class EnemyMovement : MonoBehaviour
{
    public GameObject pointL;
    public GameObject pointR;
    private Rigidbody2D rb;
    private Animator anima;
    private Transform CurrentPoint;
    private float damage;
    private Vector2 Locations;
    [Header("Enemy Variables")]
    //
    public float speed;

    [Header("Path Variables")]
    public float Distancefrompoint;

    // Start is called before the first frame update
    void Start()
    {
        Locations = transform.position;
        rb = GetComponent<Rigidbody2D>();
        anima = GetComponent<Animator>();
        CurrentPoint = pointR.transform;
        damage = GameObject.FindWithTag("Player").GetComponent<WeaponStats>().returnDamage();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 point = CurrentPoint.position - transform.position;
        if (CurrentPoint == pointR.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        if (CurrentPoint == pointL.transform)
        {
            rb.velocity = new Vector2(-speed, 0);
        }
        if (Vector2.Distance(transform.position, CurrentPoint.position) < Distancefrompoint && CurrentPoint == pointR.transform)
        {
            CurrentPoint = pointL.transform;
            flip();
        }
        if (Vector2.Distance(transform.position, CurrentPoint.position) < Distancefrompoint && CurrentPoint == pointL.transform)
        {
            CurrentPoint = pointR.transform;
            flip();
        }

    }
    private void flip()
    {
        Vector3 localscale = transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointL.transform.position, Distancefrompoint);
        Gizmos.DrawWireSphere(pointR.transform.position, Distancefrompoint);
        Gizmos.DrawLine(pointL.transform.position, pointR.transform.position);
    }
}
