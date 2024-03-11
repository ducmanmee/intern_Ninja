using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5f;

    private float horizontal;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isDead = false;
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
    }

    private void Start()
    {
        SavePoint();
        OnInit();
    }

    private void Update()
    {
        if(isDead) return;

        isGrounded = _checkGrounded();
        //jump
        if (isGrounded)
        {
            if(isJumping)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _Jump();
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

        //horizontal = Input.GetAxisRaw("Horizontal");

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
        Debug.Log(savePoint);
        isDead = false;
        isAttack = false;

        transform.position = savePoint;
        _changeAnim("idle");
        DeActiveAttackArea();
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
            Destroy(collision.gameObject);
        }
        if(collision.tag == "deathZone")
        {
            isDead = true;
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