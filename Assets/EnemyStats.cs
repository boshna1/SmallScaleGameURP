using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private float damage;
    [Header("Enemy HP")]
    public float HP;
    public float resistance;//Percentage (0-100%)
    // Start is called before the first frame update
    void Start()
    {
        damage = GameObject.FindWithTag("Player").GetComponent<WeaponStats>().returnDamage();
        damage = damage - (damage * (resistance / 100));
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HP = HP - damage;
        }
    }
}
