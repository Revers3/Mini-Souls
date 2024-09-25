using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bat_Script : MonoBehaviour
{
    /*--Script for flying bat enemy, includes pathing, attacking, hurt, and death--*/

    //movement and direction variables
    private float fallspeed = 6f;
    private float delayTime = 0.75f;
    private float attackDelay = 4f;
    private float speed = 2f;
    double distance;
    double batDirection;
    bool facingLeft = true;

    //objects variables
    //public for the purpose of allowing unity to access this location
    public Transform player;
    public Rigidbody2D batBody;
    public Animator ani;
    public GameObject objToDie;
    public SpriteRenderer sr;
    private AudioSource audiosource;
    //coordinate that if the object falls below it will be destoryed
    double OOB = -10.00;

    /*--Attacking variables--*/
    public Transform AttackPoint;
    private float attackRange = 0.5f;
    public LayerMask playerLayer;
    private int attackDamage = 25;

    //health variables
    public int maxHealth = 50;
    private int currentHealth = 50;
    bool isAlive = true;
    bool ishurt = false;
    bool isAtk = false;
    int viewRange = 6;

    //audio variables
    public AudioClip attack;
    public AudioClip hurtSound;

    

    void Start()
    {
        //getting the component for the rigidbody
        batBody = GetComponent<Rigidbody2D>();

        //getting animator object
        ani = GetComponent<Animator>();
        //sprite component
        sr = GetComponent<SpriteRenderer>();
        //getting audio componenet
        audiosource = GetComponent<AudioSource>();
        //creating reference to the health manager
        damage_script damageScript = GetComponent<damage_script>();
        //restting the health in the health manager 
        damageScript.ResetBathealth(maxHealth);

    }


    void Update()
    {

        //calcualting the direction towards the player
        Vector2 direction = (Vector2)player.position - (Vector2)transform.position;
        //storing direction's x value into a temp variable to use in the facing right function
        batDirection = direction.x;
        direction.Normalize();

        
         distance = Vector2.Distance(player.position, transform.position);

        //if the player is in sight but is not in front of the bat, it will move towards the player, otherwise it will attack the player
        if (distance > 1 && isAlive == true && distance < viewRange)
        {
            //move the enemy towards the player
            if (ishurt == false) {
                batBody.velocity = direction * speed;
            } else
            {
                StartCoroutine(hurtDelay());
            }
            
            

        } 
        if (distance < 1 && isAlive == true && isAtk == false)
        {
            isAtk = true;
            Attack();
            
        }


        /*---checks to see if the object has gone out of bounds---*/
        //check to see if past out of bounds range, then destroys and disables this script
        if (transform.position.y < OOB)
        {
            Destroy(objToDie);
            this.enabled = false;
        }



    }


    /*----flipping direction based on movement towards the player----*/
    private void FixedUpdate()
    {

        //flipping character based on direction 
        if (batDirection < 0)
        {
            facingLeft = true;
        }
        else if (batDirection > 0)
        {
            facingLeft = false;
        }


        if (facingLeft == false)
        {
            sr.flipX = true;
        }
        else if (facingLeft == true)
        {
            sr.flipX = false;
        }
    }


    
    //function to see the attack point in the editor
    //not usable in game, purely for editing sake
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }


    

    /*----Death function----*/
    void Die()
    {
        isAlive = false;
        Debug.Log("Enemy Died");
        //die animation
        ani.SetBool("isDead", true);


        GetComponent<Collider2D>().enabled = false;
        
        batBody.velocity = Vector2.down * fallspeed;

    }  


    /*----Taking damage, replaced old TakeDamage function----*/
    public void hurt(int newhealth)
    {
           
        ani.SetTrigger("hurt");
        audiosource.clip = hurtSound;
        audiosource.Play();
        currentHealth = newhealth;

        if (currentHealth > 0)
        {
            ishurt = true;
            
        }
        else
        {
            Die();
            
        }
        
    }


    /*-------Attacking player--*/
    void Attack()
    {
        //play attack animation
        ani.SetTrigger("attack");
        audiosource.clip = attack;
        audiosource.Play();

        //detecting if the player is in range of the attack, and stores them all in a variable

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, playerLayer);

        //damage the player
        foreach (Collider2D enemy in hitPlayer)
        {
            enemy.GetComponent<Player_Script>().TakeDamage(attackDamage);
            Debug.Log("Enemy hit " + enemy.name);
            
        }
        StartCoroutine(atkDelay());

    }


/*----Delay functions----*/

    //stops speed and waits before changing the hurt variable, allowing it to move again
    IEnumerator hurtDelay()
    {
        batBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(delayTime);
        ishurt = false;
    }
    //simialar to hurt function, but for the var that allows it to do other things
    IEnumerator atkDelay()
    {
        batBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(attackDelay);
        isAtk = false;
    }


}


    



