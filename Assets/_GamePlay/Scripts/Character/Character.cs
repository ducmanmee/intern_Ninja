using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float hp;
    private string currentAnim;
    [SerializeField] private Animator anim;
    public AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip throwClip;

    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText combatTextPrb;




    public bool isDead => hp <= 0;

    private void Start()
    {
        OnInit();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100, transform);
    }

    public virtual void OnDespawn()
    {

    }

    protected virtual void OnDeath()
    {
        _changeAnim("die");
        Invoke(nameof(OnDespawn), 1f);
    }

    protected void _changeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(animName);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    public void OnHit(float damage)
    {
        if (!isDead)
        {
            hp -= damage;
            if (isDead)
            {
                hp = 0;
                OnDeath();
            }
            healthBar.setNewHp(hp);
            Instantiate(combatTextPrb, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);
        }

    } 

}
