using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public GameObject DamageText;
    private float damage;
    [Header("Enemy HP")]
    public float HP;
    public float resistance;//Percentage (0-100%)

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
            DisplayDamage();
            gameObject.GetComponentInParent<DestroyObject>().Destroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            DisplayDamage();
            HP -= damage;
        }
    }
    private void DisplayDamage()
    {
        GameObject TMP = Instantiate(DamageText, transform.position, Quaternion.identity, transform.parent);
        TMP.GetComponentInChildren<TMP_Text>().text = damage.ToString();
    }
}
