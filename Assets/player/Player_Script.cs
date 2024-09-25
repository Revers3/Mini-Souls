using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player_Script : MonoBehaviour
{
    /*--This is the script for the player object, it has animations, movement, attacking, death, and calls the try again and exit buttons on death--*/

    //everything with the public tag is assigned a value in unity, or used by other scripts

    // Start is called before the first frame update

    //movement variables
    private float speed = 6f;
    private float jumpForce = 8f;
    private bool isGrounded;
    private bool isDash = false;
    public Vector2 movement;
    private bool isJump;
    //private float wallCollisionDetection = 0;
    double OOB = -10.00;
    private float dashDelayTime = 0.3f;

    //facing variables 
    
    bool facingRight = true;
    private float dashSpeed = 12;
    
    
    //attacking variables
    private float attackRange = 1.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 25;
    private float hurtDelayTime = 0.4f;
    private float atkDelayTime = 0.2f;

    //health variables
    public int maxHealth = 100;
    private int currentHealth;
    private float deathDelayTime = 0.9f;
    private int playerLayernum;
    private int rangeLayernum;

    //object variables - these are assigned in unity
    public Animator ani;
    public Transform AttackPoint;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public GameObject objToDie;
    public Button retryButton;
    public Button exitButton;
    //sound variables
    private AudioSource audiosource;
    public AudioClip jump;
    public AudioClip attack;
    public AudioClip dash;
    public AudioClip hurt;


    //setting values and getting definitions from unity
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log(currentHealth);
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        ani.SetBool("isdead", false);
        damage_script damageScript = GetComponent<damage_script>();

        rb.freezeRotation = true;


        playerLayernum = LayerMask.NameToLayer("Player");
        rangeLayernum = LayerMask.NameToLayer("range");

        Physics2D.IgnoreLayerCollision(playerLayernum, rangeLayernum, false);
    }

   

    // Update is called once per frame
    //this checks for keyboard inputs from the player for movement, attacking, and dashing
    void Update()
    {
        
        
        /*----movment and animation----*/
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        
        movement = new Vector2(horizontal, 0f);
        

        
        //setting float for animation
        ani.SetFloat("Speed", Mathf.Abs(horizontal));
       // transform.Translate(movement * speed * Time.deltaTime);
       /*
        if (horizontal != 0f && isDash == false) {
            rb.velocity = new Vector2((movement.x * speed), 0f);
        }
        else if (horizontal == 0f && isDash == false)
        {
            rb.velocity = new 
        }
       */
       if (isGrounded == true && isDash == false)
        {
            rb.velocity = new Vector2((movement.x * speed), movement.y);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            audiosource.Stop();
            audiosource.clip = jump;
            audiosource.Play();
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jumpForce);
        } // This checks if the jump key is pressed, and if the player is on the ground with the isGrounded bool



        //check to see if attack key is pressed, if it does it follows functions otherwise no.
        if (Input.GetKeyDown(KeyCode.J) == true && isGrounded == true && isDash == false)
        {
            Attack();
            StartCoroutine(atkDelay());


        }


        if (Input.GetKeyDown(KeyCode.H) == true && isDash == false)
        {
            isDash = true;
            Dash(movement);
            
        }

        

        //after jumping, if the isgrounded variable is true, then it sets the bool to false
        if (isGrounded == true)
        {
            ani.SetBool("isJumping", false);
            ani.SetBool("isFalling", false);
        }

        if (isGrounded == false && rb.velocity.y > 0 && isDash == false)
        {
            ani.SetBool("isJumping", true);
            ani.SetBool("isFalling", false);
        }
        else if (isGrounded == false && rb.velocity.y < 0 && isDash == false)
        {
            ani.SetBool("isJumping", false);
            ani.SetBool("isFalling", true);
        }
        



        /*---checks to see if the player object has gone out of bounds---*/
        //check to see if past out of bounds range, then destroys and disables this script
        if (transform.position.y < OOB)
        {
            Die();
            Destroy(objToDie);

            //run game over script here
            this.enabled = false;
        }
    }


    /*------flipping directions for player character------*/
    private void FixedUpdate()
    {
        if (isDash == true)
        {
            //freeze z rotation - for some reason unity enables it here
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        //facing directions statements
        // Get horizontal input
        float horizontal = Input.GetAxisRaw("Horizontal");

        //flipping character based on horizontal
        if (horizontal < 0)
        {
            facingRight = false;
        }
        else if (horizontal > 0)
        {
            facingRight = true;
        }


        if (facingRight == false)
        {
            sr.flipX = true;
        }
        else if (facingRight == true)
        {
            sr.flipX = false;
        }


    }


    /*-------Attacking Enemies--------*/
    void Attack()
    {
        audiosource.clip = attack;
        audiosource.Play();
        //play attack animation
        ani.SetTrigger("Attack");

        //detecting if enemies are in range of the attack, and stores them all in a variable
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);

        //damage the enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            //calls the hurt function from the health manager script
            enemy.GetComponent<damage_script>().TakeDamage(attackDamage, enemy);
            
            Debug.Log("We hit " + enemy.name);
        }
    }


    /*-----Taking damage------*/
    public void TakeDamage(int damage)
    {
        if (isDash == false)
        {
            currentHealth -= damage;

            ani.SetTrigger("hurt");
            StartCoroutine(hurtDelay());

            if (currentHealth <= 0)
            {
                Die();

            }
        }
        
    }


    /*------Death function------*/
    void Die()
    {
        Debug.Log("you are dead!");
        //die animation
        ani.SetBool("isdead", true);
        ani.SetTrigger("die");

        GetComponent<Collider2D>().enabled = false;

        retryButton.GetComponent<tryAgainMovementScript>().EnableRetryButton(retryButton);
        exitButton.GetComponent<tryAgainMovementScript>().EnableExitButton(exitButton);
        StartCoroutine(deathDelay());
        

        

    }

    
    //function to see the attack point in the editor
    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
    

    /*----dash function----*/
    //sets the speed and moves in the proper direction based on the player direction
    private void Dash(Vector2 movement)
    {
        audiosource.clip = dash;
        audiosource.Play();
        if (isDash == true)
        {
            ani.SetBool("isJumping", false);
            ani.SetBool("isDash", true);
            ani.SetTrigger("dash");
            
            dashSpeed = speed * 2;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            

            //ignores collision with enemies attacks while dashing, but still can interact with environment
            Physics2D.IgnoreLayerCollision(playerLayernum, rangeLayernum, true);
           

            if (facingRight == true)
            {
                rb.velocity = new Vector2(dashSpeed, movement.y);
                
            }
            else if (facingRight == false)
            {

                dashSpeed = dashSpeed * -1;
                rb.velocity = new Vector2(dashSpeed, movement.y);
                

            }
            //called to allow the dash to player for a certain amount of time
            StartCoroutine(DashDelay());
        }   
    }
    
    /*-----------Ground checks for jumping------*/
    //ground check function, if the player object is on the ground, this boolean will set it to true
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            

        }

        
    }
    //opposite of the OnCollisionEnter2d function
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


/*---delays----*/
    //after the time stops, the function reenables everything and sets the speed back to normal
    IEnumerator DashDelay()
    {
        
        yield return new WaitForSeconds(dashDelayTime);
        
        //reactivates collision with enemy attacks and player layer
        Physics2D.IgnoreLayerCollision(playerLayernum, rangeLayernum, false);
        rb.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        ani.SetBool("isDash", false);
        //rb.velocity = Vector2.zero;
        isDash = false;
    }
    //pauses player movement and collision while hurt
    IEnumerator hurtDelay()
    {
        GetComponent<Collider2D>().enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        yield return new WaitForSeconds(hurtDelayTime);
        rb.constraints = ~RigidbodyConstraints2D.FreezePositionY;
        GetComponent<Collider2D>().enabled = true;
        
    }
    //sets a timer for animation, and then destroys the object
    IEnumerator deathDelay()
    {
        yield return new WaitForSeconds(deathDelayTime);
        Destroy(objToDie);
    }

    IEnumerator atkDelay()
    {
        yield return new WaitForSeconds(atkDelayTime);
    }

}

