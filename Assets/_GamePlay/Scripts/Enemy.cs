using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;

    private Character target;
    public Character Target => target;

    private IState currentState;
    private bool isRight = true;

    private void Update()
    {
        if(currentState != null)
        {
            currentState.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();

        ChangeState(new IdleState());
        DeActiveAttackArea();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }


    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }    

        currentState = newState;

        if(currentState != null)
        {
            currentState.OnEnter(this);
        }
    }    

    public void Moving()
    {
        _changeAnim("run");
        rb.velocity = transform.right * moveSpeed;
    }   
    
    public void StopMoving()
    {
        _changeAnim("idle");
        rb.velocity = Vector2.zero;
    }   
    
    public void Attack()
    {
        _changeAnim("attack");
        ActiveAttackArea();
        Invoke(nameof(DeActiveAttackArea), .5f);
    }   
    
    public bool IsTargetInRange()
    {
        if(target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyWall")
        {
            ChangeDirection(!isRight);
        }    
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation  = isRight ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
    }

    internal void SetTarget(Character character)
    {
        this.target = character;
        if(IsTargetInRange())
        {
            ChangeState(new AttackState());
        }
        else if(Target != null)
        {
            ChangeState(new PatrolState());
        }
        else
        {
            ChangeState(new IdleState());
        }
    }

    private void ActiveAttackArea()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttackArea()
    {
        attackArea.SetActive(false);

    }
}
