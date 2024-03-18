using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5f;


    private bool doubleJump;

    private float horizontal;
    private bool glideInput;
    private float vertical;
    private float climbSpeed = 4f;
    private bool isLadder;
    private bool isClimbing = false;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool rsAttack;
    private bool isAttack = false;
    private bool canJumpAtk = false;
    [SerializeField] private float jumpForce = 350;
    private int coin = 0;

    //Slide 
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = .2f;
    public float dashingCooldown = 1f;

    [SerializeField] private TrailRenderer trail;

    private Vector3 savePoint;

    [SerializeField] private Kunai kunaiPrefabs;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    public AudioClip goldClip;


    public static Player instance;
    private float lifeBullet = 4f;
    private int countJump = 0;

    [SerializeField] private float glidingSpeed;
    private float initialGravityScale;
    private bool isGlide;

    public void makeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Awake()
    {
        makeInstance();
        SavePoint();
        OnInit();
        //Save coin sau khi play again
        coin = PlayerPrefs.GetInt("coin", 0);
        initialGravityScale = playerRb.gravityScale;
    }

    private void Update()
    {
        if(isGlide || isDashing)
        {
            return;
        }   
        
        vertical = Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }

        if (isClimbing)
        {
            playerRb.gravityScale = 0f;
            playerRb.velocity = new Vector2(playerRb.velocity.x, vertical * speed * Time.fixedDeltaTime);
            _changeAnim("climb");
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        }
        else
        {
            playerRb.gravityScale = 1f;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (isDead) return;

        isGrounded = _checkGrounded();
        //jump
        if (isGrounded && !Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = false;
        }

        if (isGrounded)
        {
            if (isJumping || isAttack)
            {
                return;
            }

            if (Mathf.Abs(horizontal) > 0.1f)
            {
                _changeAnim("run");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (!isAttack)
                {
                    _Attack();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _Throw();
            }

        }

        //check falling 
        if (!isGrounded && playerRb.velocity.y < 0)
        {
            _changeAnim("fall");
            isJumping = false;
        }
        if (Mathf.Abs(horizontal) < 0.1f && isGrounded && !isJumping && !isClimbing)
        {
            _changeAnim("idle");
            playerRb.velocity = Vector2.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead || isDashing)
        {
            return;
        }
        //Climbing
        
        if (!isClimbing)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
  

        if(isAttack)
        {
            playerRb.velocity = Vector2.zero;
            return;
        }

        if(Mathf.Abs(horizontal) > 0.1f)
        {
            playerRb.velocity = new Vector2(horizontal * Time.fixedDeltaTime * speed, playerRb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }      
    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;

        transform.position = savePoint;
        _changeAnim("idle");
        DeActiveAttackArea();
        if(UIManager.instance  != null)
        {
            UIManager.instance.setCoin(coin);

        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private bool _checkGrounded()
    {
        Debug.DrawRay(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        
        return ( hit.collider != null);
    }    

    public void _Attack()
    {
        //Debug.Log(canJumpAtk + ", " + isJumping);
        if (isGrounded)
        {
            if (!isJumping && !rsAttack)
            { 
                _changeAnim("attack");
                audioSource.PlayOneShot(hitClip);
                isAttack = true;
                Invoke(nameof(_resetAttack), .5f);
                ActiveAttackArea();
                Invoke(nameof(DeActiveAttackArea), .5f);
                rsAttack = true;
            }   
        }
  

    }  

    private void _resetAttack()
    {
        rsAttack = false;
        isAttack = false;
        _changeAnim("idle");
    }

    public void _Throw()
    {
        if(isGrounded)
        {
            if(!isJumping)
            {
                _changeAnim("throw");
                audioSource.PlayOneShot(throwClip);
                isAttack = true;
                Invoke(nameof(_resetAttack), .5f);

                Instantiate(kunaiPrefabs, throwPoint.position, throwPoint.rotation);
            }
        } 
    }

    public void _Jump()
    {
        if (!isGrounded && !doubleJump)
        {
            countJump++;
        }
        if(countJump == 1)
        {
            _changeAnim("jump");
            playerRb.AddForce(jumpForce * Vector2.up);
            countJump = 0;
            doubleJump = true;
        }

        if (isGrounded)
        {
            doubleJump = false;
            isJumping = true;
            _changeAnim("jump");
            playerRb.AddForce(jumpForce * Vector2.up);
        }
    }  
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            audioSource.PlayOneShot(goldClip);
            UIManager.instance.setCoin(coin);
            Destroy(collision.gameObject);
        }
        if(collision.tag == "deathZone")
        {
            OnHit(100);
            _changeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }
        if (collision.tag == "Trap")
        {
            OnHit(100);
            _changeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }

        if(collision.tag == "ladder")
        {
            isLadder = true;

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "ladder")
        {
            isLadder = false;
            isClimbing = false;
            Vector2 temp = this.transform.position;
            temp.y = collision.transform.position.y + 4.5f;
            this.transform.position = temp;
            _changeAnim("fall");
        }
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttackArea()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttackArea()
    {
        attackArea.SetActive(false);

    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    public void onPointerDown()
    {
        isGlide = true;
        StartCoroutine(glidePlayer());
    }
    public void onPointerUp()
    {
        isGlide = false;
    }

    IEnumerator glidePlayer()
    {
        yield return new WaitForSeconds(1f);
        while (isGlide)
        {
            if (playerRb.velocity.y <= 0 && !isGrounded)
            {
                playerRb.gravityScale = 0;
                playerRb.velocity = new Vector2(playerRb.velocity.x, -glidingSpeed);
                Debug.Log("bay");  
                _changeAnim("glide");
                Debug.Log(1);

            }
            else
            {
                playerRb.gravityScale = initialGravityScale;
                _changeAnim("fall");
            }
            yield return null;
        }    

    }

    private IEnumerator dash()
    {
        if(isGrounded)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = playerRb.gravityScale;
            playerRb.gravityScale = 0f;
            playerRb.velocity = this.transform.right * dashingPower;
            _changeAnim("slide");
            trail.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            trail.emitting = false;
            playerRb.gravityScale = originalGravity;
            _changeAnim("idle");
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }    
    public void DashPlayer()
    {
        StartCoroutine(dash());
    }    
}
