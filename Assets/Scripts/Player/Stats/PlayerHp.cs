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
    
    public float ReturnCurrentHP()
    {
        return currentHP;
    }

    public float ReturnMaxHP()
    {
        return maxHP;
    }
}
