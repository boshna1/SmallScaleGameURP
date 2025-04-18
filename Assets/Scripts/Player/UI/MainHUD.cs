using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    [Header("HP Bar")]
    public Slider sliderHP;
    PlayerHp playerHp;
    public Text textHP;

    [Header("XP Bar")]
    public Slider sliderXP;
    public Text textXP;
    PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerHp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHp>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateHealthFloat();
        UpdateXP();
        UpdateXPFloat();
    }

    public void UpdateHealth()
    {
        sliderHP.value = playerHp.ReturnCurrentHP() / playerHp.ReturnMaxHP();
    }

    public void UpdateHealthFloat()
    {
        textHP.text = playerHp.ReturnCurrentHP().ToString() + " / " + playerHp.ReturnMaxHP().ToString();
    }

    public void UpdateXP()
    {
        sliderXP.value = playerStats.ReturnXP() / playerStats.ReturnXPtoLevelUp();
    }

    public void UpdateXPFloat()
    {
        textXP.text = playerStats.ReturnXP().ToString() + " / " + playerStats.ReturnXPtoLevelUp().ToString();
    }
}
