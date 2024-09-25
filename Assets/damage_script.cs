using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class damage_script : MonoBehaviour
{

    //this script is for the damage function, and so all enemy types can use it, including the player
    

    //variables for storing the health of enemy objects
    private int batHealth = 0;
    private int EKnightHealth = 0;
    private int BossHealth = 0;
    void Start()
    {
        //allows for use with other scripts
        Bat_Script batSc = GetComponent<Bat_Script>();
        Enemy_Script enSc = GetComponent<Enemy_Script>();
        Boss_Script bossSc = GetComponent<Boss_Script>();
    }

    /*----new takedamage, replaced individual functions----*/
    public void TakeDamage(int damage, Collider2D caller)
    {
        if (caller.GetComponent<Bat_Script>() != null)
        {
            batHealth -= damage;
            caller.GetComponent<Bat_Script>().hurt(batHealth);
        }
        else if (caller.GetComponent<Enemy_Script>() != null)
        {
            EKnightHealth -= damage;
            caller.GetComponent<Enemy_Script>().hurt(EKnightHealth);
        }
        else if (caller.GetComponent<Boss_Script>() != null)
        {
            BossHealth -= damage;
            caller.GetComponent<Boss_Script>().hurt(BossHealth);
        }



    }

    /*----resetting health functions, called by each object----*/
    public void ResetBathealth(int batInput)
    {
        batHealth = batInput;
    }
    public void ResetEkhealth(int EKInput)
    {
        EKnightHealth = EKInput;
    }

    public void ResetBosshealth(int bossInput)
    {
        BossHealth = bossInput;
    }



}
