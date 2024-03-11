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
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;
    [SerializeField] private float jumpForce = 350;
    private int coin = 0;

    private Vector3 savePoint;

    [SerializeField] private Kunai kunaiPrefabs;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    

    public static Player instance;
    private float lifeBullet = 4f;

    public void makeInstance()
    {
        if(instance == null)
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
    }

    private void Update()
    {
        if(isDead) return;

        isGrounded = _checkGrounded();
        //jump
        if(isGrounded && !Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if( isGrounded || doubleJump)
            {
                _Jump();
                doubleJump = !doubleJump;
            }
        }

        if (isGrounded)
        {

            if(isJumping || doubleJump)
            {
                return;
            }

            if (Mathf.Abs(horizontal) > 0.1f)
            {
                _changeAnim("run");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if(!isAttack)
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
        if(Mathf.Abs(horizontal) < 0.1f && isGrounded && !isJumping)
        {
            _changeAnim("idle");
            playerRb.velocity = Vector2.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDead) return;
        horizontal = Input.GetAxisRaw("Horizontal");

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
        /*else if (isGrounded && !isJumping) 
        {
            _changeAnim("idle");
            playerRb.velocity = Vector2.zero;
        } */   
    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;

        transform.position = savePoint;
        _changeAnim("idle");
        DeActiveAttackArea();

        UIManager.instance.setCoin(coin);
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
        _changeAnim("attack");
        isAttack = true;
        Invoke(nameof(_resetAttack), .5f);
        ActiveAttackArea();
        Invoke(nameof(DeActiveAttackArea), .5f);

    }

    private void _resetAttack()
    {
        isAttack = false;
        _changeAnim("idle");
    }

    public void _Throw()
    {
        _changeAnim("throw");
        isAttack = true;
        Invoke(nameof(_resetAttack), .5f);

        Instantiate(kunaiPrefabs, throwPoint.position, throwPoint.rotation);
    }

    public void _Jump()
    {
        isJumping = true;
        _changeAnim("jump");
        playerRb.AddForce(jumpForce * Vector2.up);
    }  
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
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
}
