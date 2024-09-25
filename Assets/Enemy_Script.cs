using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy_Script : MonoBehaviour
{
    /*--Ground Enemy aka Bandit Script, involves moving, attacking, hurt, and death--*/
    //all public objects are public so unity can access them and store the proper objects in them

    //refernces to objects in unity
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator ani;
    public Transform player;
    public Rigidbody2D EKBody;

    //object to disable on death
    public GameObject objecttodisable;



    //making variables for enemy knight character
    public int maxHealth = 100;
    private int currentHealth = 100;



    //attacking transform
    public Transform AttackPoint;
    public float attackRange = 0.5f;
    public LayerMask playerLayer;
    public int attackDamage = 25;
    public double OOB = -10.00;
    
    private float deathtime = 2f;
    bool ishurt = false;
    public float delayTime = 1.5f;
    public float attackDelay = 2f;
    
    bool isAtk = false;
    
    bool isAlive = true;
    int viewRange = 6;
    


    //movement and direction variables
    bool isLeft = true;
    double EKDirection;
    double direction;
    double distance;
    private float speed = 3f;


    //audio variables
    public AudioClip attack;
    public AudioClip hurtSound;
    public AudioSource audiosource;



    // Start is called before the first frame update
    void Start()
    {
        
        //setting current health to max and then acquiring objects from unity
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        ani.SetBool("isDead", false);
        damage_script damageScript = GetComponent<damage_script>();
        //getting the component for the rigidbody
        EKBody = GetComponent<Rigidbody2D>();
        damageScript.ResetEkhealth(maxHealth);
        audiosource = GetComponent<AudioSource>();

        rb.constraints = RigidbodyConstraints2D.FreezePositionY;


    }

    /*-------Attacking Enemies--------*/
    void Attack()
    {
        //play attack animation
        ani.SetTrigger("attack");
        audiosource.clip = attack;
        audiosource.Play();

        //detecting if the player is in range of the attack, and stores them all in a variable

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, playerLayer);

        //damage the player
        foreach (Collider2D enemy in hitEnemies)
        {
            //has the player call their hurt function
            enemy.GetComponent<Player_Script>().TakeDamage(attackDamage);
        }

        StartCoroutine(atkDelay());
    }

    //function to see the attack point in the editor
    //cannot be seen in game
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }



/*----Changing direction---*/
    private void FixedUpdate()
    {
        if(isAlive == true)
        {
            //flipping character based on direction 
            if (EKDirection < 0)
            {
                isLeft = true;
            }
            else if (EKDirection > 0)
            {
                isLeft = false;
            }


            if (isLeft == false)
            {
                sr.flipX = true;
            }
            else if (isLeft == true)
            {
                sr.flipX = false;
            }
        }
        
    }



    /*----Taking Damage----*/
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

    /*----Death function----*/
    void Die()
    {
        EKBody.velocity = Vector2.zero;
        Debug.Log("Enemy Died");
        //die animation
        ani.SetBool("isDead", true);
        isAlive = false;
        //disables collision
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(deathDelay());
        



    }




    // Update is called once per frame
    void Update()
    {
       

        //calcualting the direction towards the player
        Vector2 direction = (Vector2)player.position - (Vector2)transform.position;
        //storing direction's x value into a temp variable to use in the facing left function
        EKDirection = direction.x;
        direction.Normalize();

        //calculating the distance
        distance = Vector2.Distance(player.position, transform.position);



        /*--------Moving and Attacking---------*/
        //if within sight and not directly in front of the object, the enemy will move towards the player
        if (distance > 1 && isAlive == true && distance < viewRange)
        {
            //move the enemy towards the player
           
            if (ishurt == false)
            {

                //if not hurt, move towards the player
                ani.SetFloat("Speed", 1);

                EKBody.velocity = direction * speed;
            }
            else
            {
                
                StartCoroutine(hurtDelay());
            }
        }
        else
        {
            //otherwise do not move
            rb.velocity = Vector2.zero;
            ani.SetFloat("Speed", 0);
        }
        //attack if the player is within range
        if (distance < 1.2 && isAlive == true && isAtk == false)
        {
            isAtk = true;
            Attack();
            

        }



        //check to see if past out of bounds range, then destroys and disables this script
        if (transform.position.y < OOB)
        {
            Destroy(objecttodisable);
            this.enabled = false;
        }


        
        
    }


 /*-------Delays-------*/
    //sets the movement speed to 0, prevents another animation from playering, and after the delay turns off the
    //var disabling other actions
    IEnumerator hurtDelay()
    {
        rb.velocity = Vector2.zero;
        ani.SetFloat("Speed", 0);
        
        yield return new WaitForSeconds(delayTime);
        ishurt = false;

    }
    //sets the movement speed to 0, prevents another animation from playering, and after the delay turns off the
    //var disabling other actions
    IEnumerator atkDelay()
    {
        rb.velocity = Vector2.zero;
        ani.SetFloat("Speed", 0);
        yield return new WaitForSeconds(attackDelay);
        isAtk = false;
    }
    //after waiting for the animation to play, destroys the object
    IEnumerator deathDelay()
    {
        
        yield return new WaitForSeconds(deathtime);
        Destroy(objecttodisable);
        
    }

}
