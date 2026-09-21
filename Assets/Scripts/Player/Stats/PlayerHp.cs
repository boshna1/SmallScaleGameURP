using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    [Range(0f,100f)]
    public float currentHP;
    [Range(0f, 100f)]
    public float maxHP;
    // Start is called before the first frame update
    CheckpointManage cm;

    void Start()
    {
        cm = GlobalValues.cm;
    }

    public void Instakill()
    {
        Debug.Log("ToLast");
        currentHP = 0;
        cm.ToLastCheckpoint();
    }
    public float ReturnCurrentHP()
    {
        return currentHP;
    }

    public float ReturnMaxHP()
    {
        return maxHP;
    }
}
