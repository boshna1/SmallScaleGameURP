using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseMovementScript : MonoBehaviour
{
    public GameObject pointL;
    public GameObject pointR;
    private Rigidbody2D rb;
    private Transform CurrentPoint;
    private float damage;
    [Header("Enemy Variables While Chasing")]
    public float speed2;

    [Header("Enemy Variables On Stationary Path")]
    public float DistanceToEngage;
    public float speed1;
    public float Distancefrompoint;
    [Header("Enemy HP")]
    public float HP;
    //Percentage (0-100%)
    public float resistance;

    private float distance;
    private float distance2;
    private Transform player;
    private int Chase;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentPoint = pointR.transform;
        player = GameObject.FindWithTag("Player").GetComponent<Transform> ();
        damage = GameObject.FindWithTag("Player").GetComponent<WeaponStats>().returnDamage();
        damage = damage - (damage * (resistance/100));
        Chase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        distance2 = Vector2.Distance(player.transform.position,transform.position);

        if (distance < DistanceToEngage|| distance2 < DistanceToEngage)
        {
            Chase = 1;
        }
        if (Chase == 0)
        {
            Vector2 point = CurrentPoint.position - transform.position;
            if (CurrentPoint == pointR.transform)
            {
                rb.velocity = new Vector2(speed1, 0);
            }
            if (CurrentPoint == pointL.transform)
            {
                rb.velocity = new Vector2(-speed1, 0);
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

        if (Chase == 1)
        {
            distance = Vector2.Distance(transform.position, player.transform.position);
            //Find Angle from player in degrees
            Vector2 Direction = player.transform.position - transform.position;
            Direction.Normalize();
            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;


            //Enemy follows player
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed2 * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
        if (damage <= 0)
        {
            Destroy(gameObject);
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
        Gizmos.DrawWireSphere(transform.position, DistanceToEngage);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HP = HP - damage;
        }
    }
}
