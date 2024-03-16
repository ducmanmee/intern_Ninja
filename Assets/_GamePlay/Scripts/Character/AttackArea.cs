using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    bool canAttack = true;

    private void OnEnable()
    {
        canAttack = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" ||  collision.tag == "Enemy")
        {
            if (canAttack)
            {
                collision.GetComponent<Character>().OnHit(30f);
                canAttack = false;
            }
        }
    }
}
