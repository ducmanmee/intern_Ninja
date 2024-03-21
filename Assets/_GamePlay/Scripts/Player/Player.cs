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
    public bool isSkill = false;
    public bool canSkill = true;
    public float skillCooldown = 4f;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool rsAttack;
    private bool isAttack = false;  
    [SerializeField] private float jumpForce = 350;
    private int coin = 0;

    //Slide 
    public bool canDash = true;

    public bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = .2f;
    public float dashingCooldown = 4f;

    [SerializeField] private TrailRenderer trail;

    private Vector3 savePoint;

    [SerializeField] private Kunai kunaiPrefabs;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    public AudioClip goldClip;


    public static Player instance;
    private float lifeBullet = .4f;
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
        if(isGlide || isDashing || isSkill)
        {
            return;
        }

        if (isDead) return;

        isGrounded = _checkGrounded();

        vertical = Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }

        if(isLadder)
        {
            _changeAnim(Constant.ANIM_CLIMB);
            SetAnimationSpeed(Constant.ANIM_CLIMB, 0f);
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX 
                | RigidbodyConstraints2D.FreezeRotation
                | RigidbodyConstraints2D.FreezePositionY;
        }

        if (isClimbing)
        {
            if(vertical == 1)
            {
                SetAnimationSpeed(Constant.ANIM_CLIMB, 1f);
                Vector3 temp = this.transform.position;
                temp.y += climbSpeed * Time.deltaTime;
                this.transform.position = temp;
            }
            else if(vertical == -1)
            {
                SetAnimationSpeed(Constant.ANIM_CLIMB, 1f);
                Vector3 temp = this.transform.position;
                temp.y -= climbSpeed * Time.deltaTime;
                this.transform.position = temp;
            }
            else
            {
                SetAnimationSpeed(Constant.ANIM_CLIMB, 0f);
            }
        }

        //jump
        if (isGrounded && !Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = false;
        }

        //Glide
        if(!isGlide)
        {
            playerRb.gravityScale = initialGravityScale;
        }


        if (isGrounded)
        {
            if (isJumping || isAttack)
            {
                return;
            }

            if (Mathf.Abs(horizontal) > 0.1f)
            {
                _changeAnim(Constant.ANIM_RUN);
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
            _changeAnim(Constant.ANIM_FALL);
            isJumping = false;
        }
        if (Mathf.Abs(horizontal) < 0.1f && isGrounded && !isJumping && !isClimbing)
        {
            _changeAnim(Constant.ANIM_IDLE);
            playerRb.velocity = Vector2.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead || isDashing || isSkill)
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
        canSkill = true;
        transform.position = savePoint;
        _changeAnim(Constant.ANIM_IDLE);
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
        if (isGrounded)
        {
            if (!isJumping && !rsAttack)
            { 
                _changeAnim(Constant.ANIM_ATTACK);
                audioSource.PlayOneShot(hitClip);
                isAttack = true;
                Invoke(nameof(_resetAttack), .5f);
                ActiveAttackArea();
                Invoke(nameof(DeActiveAttackArea), .5f);
                rsAttack = true;
            }   
        }  
  

    }
    
    public void _attackSkill()
    {

        if(isGrounded && !isJumping && canDash)
        {
            if(canSkill)
            {
                _changeAnim(Constant.ANIM_ATTACKSKILL);
                playerRb.velocity = Vector2.zero;
                canSkill = false;
                isSkill = true;
                ActiveAttackArea() ;
            }
        }    
    }    

    public void _rsAttackSkill()
    {
        canSkill = true;
        isSkill = false;
        DeActiveAttackArea() ;
    }    

    private void _resetAttack()
    {
        rsAttack = false;
        isAttack = false;
        _changeAnim(Constant.ANIM_IDLE);
    }

    public void _Throw()
    {
        if(isGrounded)
        {
            if(!isJumping)
            {
                _changeAnim(Constant.ANIM_THROW);
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
            _changeAnim(Constant.ANIM_JUMP);
            playerRb.AddForce(jumpForce * Vector2.up);
            countJump = 0;
            doubleJump = true;
        }

        if (isGrounded)
        {
            doubleJump = false;
            isJumping = true;
            _changeAnim(Constant.ANIM_JUMP);
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
            _changeAnim(Constant.ANIM_DIE);

            Invoke(nameof(OnInit), 1f);
        }
        if (collision.tag == "Trap")
        {
            OnHit(100);
            _changeAnim(Constant.ANIM_DIE);

            Invoke(nameof(OnInit), 1f);
        }

        if(collision.tag == "ladder")
        {
            isLadder = true;
        }

        if(collision.tag == "lapup")
        {
            Vector3 temp = this.transform.position;
            temp.y += 4f;
            this.transform.position = temp;
            _changeAnim(Constant.ANIM_FALL);
        }

    }

    private void SetAnimationSpeed(string animationName, float speed)
    {
        AnimatorStateInfo state = getAnim().GetCurrentAnimatorStateInfo(0);
        if (state.IsName(animationName))
        {
            getAnim().SetFloat("Speed", speed); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "ladder")
        {
            isLadder = false;
            isClimbing = false; 
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;         
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

    public void DeActiveAttackArea()
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
                _changeAnim(Constant.ANIM_GLIDE);
            }
            yield return null;
        }    

    }

    private IEnumerator dash()
    {
        if(isGrounded)
        {
            if(canDash)
            {
                canDash = false;
                isDashing = true;
                float originalGravity = playerRb.gravityScale;
                playerRb.gravityScale = 0f;
                playerRb.velocity = this.transform.right * dashingPower;
                _changeAnim(Constant.ANIM_SLIDE);
                trail.emitting = true;
                yield return new WaitForSeconds(dashingTime);
                trail.emitting = false;
                playerRb.gravityScale = originalGravity;
                _changeAnim(Constant.ANIM_IDLE);
                isDashing = false;
            }    
        }
    }    
    public void DashPlayer()
    {
        StartCoroutine(dash());
    }

    
}
