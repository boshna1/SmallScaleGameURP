using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public float playerXP;
    public float XPtoLevelUp;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValues.SetPlayer(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ReturnXP()
    {
        return playerXP;
    }

    public float ReturnXPtoLevelUp()
    {
        return XPtoLevelUp;
    }
}
