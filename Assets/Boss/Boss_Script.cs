using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Script : MonoBehaviour
{
    /*--This script is for the final enemy object aka the boss, it has the boss' attack patterns, taking damage, dying, and upon
     death calls the victory button script*/



    //boss will be able to make a certain number of actions before having a cooldown, this will be the punish period for the player
    private int maxAction = 3;
    private int actionCount = 0;
    private int actionChoice;

    //movement variables
    private float dashSpeedLeft = -15f;
    private float dashSpeedRight = 15f;
    private float dashDelayTime = 0.5f;
    private float preTPDelay = 0.6f;
    private float coolDownDelay = 3f;
    private float actionDelayTime = 1f;
    private float postTPDelay = 1.2f;
    public Vector3 movement;


    //object variables, all public so that unity can assign them
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator ani;
    public Collider2D col;
    public Transform dashPoint;
    public Transform player;
    public GameObject objToDie;
    public Button winButton;
    //health variables
    private int maxHealth = 300;
    private int currentHealth;
    private bool facingLeft = true;
    private float deathDelayTime = 1.5f;
    private bool isAlive = true;


    //combat variables
    private int dashDamage = 20;
    private bool isDash = false;
    public Player_Script playerScr;
    public LayerMask playerLayer;
    public float dashRange = 1.4f;
    private float playerDirection;
    private bool ishurt = false;
    private bool isAtk = false;
    private float atkDelayTime = 0.6f;
    private Vector2 direction;

    /*---Teleport variables---*/
    private bool isTeleport = false;
    private Vector2 b_location;
    private Vector2 p_location;
    private Vector2 og_location;
    public Transform AttackPoint;
    private float attackRange = 3f;
    public int attackDamage = 25;



    //teleport variables
    private bool tpToPlayer = false;
    private bool toFromPlayer = false;

    //sound variables
    public AudioClip attackSound;
    public AudioClip dashSound;
    public AudioClip hurtSound;
    public AudioClip tpSound;
    public AudioClip deathSound;
    private AudioSource audiosource;

    void Start()
    {
        //setting health variables and objects variables
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

        Player_Script playerScr = GetComponent<Player_Script>();
        audiosource = GetComponent<AudioSource>();
        damage_script damageScript = GetComponent<damage_script>();
        damageScript.ResetBosshealth(maxHealth);
    }

    /*--Update delegates the boss taking actions--*/
    void Update()
    {
        if(isAtk == false)
        {
            //calcualting the direction towards the player
            direction = (Vector2)player.position - (Vector2)transform.position;
            playerDirection = direction.x;
        }
        
        
        //storing location before teleport in order to return to this position after the attack
        if (isTeleport == false) 
        {
            b_location = transform.position;
            Vector2 p_location = (Vector2)player.position;
            
        }
       
        //changes on update
        if (playerDirection < 0)
        {
            facingLeft = true;
            sr.flipX = true;
        }
        else if (playerDirection > 0)
        {
            facingLeft = false;
            sr.flipX = false;
        }
        if (isAlive == true & actionCount < 3 && isDash == false && isTeleport == false)
        {
           
            StartCoroutine(actionDelay());

            //if action less than 3, the boss will teleport otherwise he will dash
            if (actionChoice < 3)
            {
                Teleport(transform.position, player.position);
            }
            if (actionChoice > 3)
            {
                dash(movement);
                actionCount++;
                
            }
        }

        //only calls while the boss is actively dashing
        //considered a constant attack around the boss
        if (isDash == true)
        {
            attackCollide(col);
        }


    }
    //generate random number function
    int generateRandomAction()
    {
        //generating a random number from 1 to 6 so it has more chances to switch between the actions
        int randomNumber = UnityEngine.Random.Range(1, 6);
       
        return randomNumber;
    }

    //hurt function, if health is less than zero will call the death function
    public void hurt(int newhealth)
    {
        audiosource.clip = hurtSound;
        audiosource.Play();
        ani.SetTrigger("hurt");
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

    private void dash(Vector3 speed)
    {
       
        //if left, set the speed to negative so the boss moves left
        
        //animation parameter
        ani.SetTrigger("dash");
        
        //freezing the y coordinate
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        //disabling collider to move past player
        GetComponent<Collider2D>().enabled = false;
        //move direction based on boolean
        if (facingLeft == true)
        {
            rb.velocity = new Vector2(dashSpeedLeft, speed.y);
            audiosource.Stop();
            audiosource.clip = dashSound;
            audiosource.Play();
        } else
        {
            rb.velocity = new Vector2(dashSpeedRight, speed.y);
            audiosource.Stop();
            audiosource.clip = dashSound;
            audiosource.Play();
        }
        //prevents other actions from happening
        isDash = true;
        //delay function called
        StartCoroutine(DashDelay());
    }

    //1st half of the teleport pattern this is purely the movement 
    private void Teleport(Vector2 bossPosition, Vector2 playerPosition)
    {
        isTeleport = true;
        isAtk = true;
       
        //creates new xy coordinate of the target location
        Vector2 bossTemp = new Vector2(playerPosition.x - 1.5f, bossPosition.y);

        //calls first of 3 sections of teleport pattern
        StartCoroutine(TPtoPlayerDelay(bossTemp));

       

        
        



    }

    /*-------Attacking player--*/
    void Attack()
    {
        isAtk = true;
        //play attack animation
        ani.SetTrigger("attack");
        StartCoroutine(atkDelay());

        //attackiong is ran in delay

    }


    //function from dash that allows it to attack while dashing towards the player
    private void attackCollide(Collider2D collider)
    {
        
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(dashPoint.position, dashRange, playerLayer);

        //damage the player
        foreach (Collider2D enemy in hitPlayer)
        {
            
            enemy.GetComponent<Player_Script>().TakeDamage(dashDamage);
            Debug.Log("Enemy hit " + enemy.name);

        }


    }
    //death function,calls the delay in order to allow the full animation to play before running rest of script
    void Die()
    {
        
        //die animation
        ani.SetBool("isDead", true);
        isAlive = false;

        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        GetComponent<Collider2D>().enabled = false;
        audiosource.Stop(); 
        audiosource.clip = deathSound;
        audiosource.Play();
        StartCoroutine(deathDelay());




    }

    
    //function to see the attack point in the editor
    //can be changed in unity to see the ranges of the normal attack and the dash attack range
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }

    

    /*---delays----*/

    //this delay allows for it to move, and stop the dash movement after a certain amount of time
    //from there, it checks to see if it has hit the maximum amount of actions before it needs to call the cooldown delay
    IEnumerator DashDelay()
    {
       
        yield return new WaitForSeconds(dashDelayTime);
        rb.velocity = Vector2.zero;
        isDash = false;
        GetComponent<Collider2D>().enabled = true;
        rb.constraints = ~RigidbodyConstraints2D.FreezePositionY;

        actionCount++;



        
        if (actionCount >= 3)
        {
            Debug.Log("on cooldown!");
            StartCoroutine(CDDelay());
            

        }

        
    }

    //this causes a delay for the object to play it's death animation, and then summon the win, retry, and exit buttons before destroying itself
    IEnumerator deathDelay()
    {

        yield return new WaitForSeconds(deathDelayTime);
        winButton.GetComponent<winShowScript>().winShow(winButton);
        Destroy(objToDie);
        


    }

    //this is the part of the telport function that allows the boss to move towards to player, with animations, and then attacks when it is next 
    //to the player
    IEnumerator TPtoPlayerDelay(Vector2 tpLocation)
    {
        
        audiosource.Stop();
        audiosource.clip = tpSound;
        audiosource.Play();

        ani.SetTrigger("disappear");
        yield return new WaitForSeconds(preTPDelay);
        objToDie.GetComponent<Renderer>().enabled = false;
        transform.position = tpLocation;
        objToDie.GetComponent<Renderer>().enabled = true;
        ani.SetTrigger("reappear");
        yield return new WaitForSeconds(preTPDelay);
        Attack();



    }

    //this part of the telelport function has the boss return to it's original position
    //includes animations and checks to make sure the boss does or does not need to go on cooldown
    IEnumerator ReturnDelay(Vector2 Location)
    {
        
        //play telport animations and play sounds
        ani.SetTrigger("disappear");
        audiosource.Stop();
        audiosource.clip = tpSound;
        audiosource.Play();
        yield return new WaitForSeconds(preTPDelay);
        objToDie.GetComponent<Renderer>().enabled = false;
        //move to new location
        transform.position = Location;
        objToDie.GetComponent<Renderer>().enabled = true;
        ani.SetTrigger("reappear");
        yield return new WaitForSeconds(preTPDelay);

        //add to the action count
        actionCount++;

        yield return new WaitForSeconds(actionDelayTime);
        //cool down delay check
        if (actionCount >= 3)
        {
            Debug.Log("on cooldown!");
            StartCoroutine(CDDelay());
            actionCount = 0;

        }

        isTeleport = false;


    }


    //this delay is for the attack that the boss does during the middle of the telport pattern
    //it changes direction thanks to the update function, and determines the location of the attack based on where it is facing
    IEnumerator atkDelay()
    {

        yield return new WaitForSeconds(atkDelayTime);
        audiosource.clip = attackSound;
        audiosource.Play();
        if (facingLeft == true && direction.x < 1 || facingLeft == false && direction.x > 1)
        {
            Collider2D[] hitplayer = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, playerLayer);

            //damage the player
            foreach (Collider2D enemy in hitplayer)
            {
                //run player hurt function
                enemy.GetComponent<Player_Script>().TakeDamage(attackDamage);
                

            }
        }
        //wait a bit after attack 
        yield return new WaitForSeconds(atkDelayTime);
        isAtk = false;
        //runs final delay for teleport with og location as parameter
        StartCoroutine(ReturnDelay(b_location));


    }

    //delay for action cool down
    //resets the counter so that the action pattern can happen repeatadly
    private IEnumerator CDDelay()
    {

        yield return new WaitForSeconds(coolDownDelay);
        actionCount = 0;



    }

    //delay for action cool down
    //only pauses for a moment so actions do not play instantly 
    public IEnumerator actionDelay()
    {
        yield return new WaitForSeconds(actionDelayTime);
        
        

    }

}
