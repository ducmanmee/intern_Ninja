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

        if (collision.tag == "Player" || collision.tag == "Enemy")
        {
            if (canAttack)
            {
                if(!Player.instance.canSkill)
                {
                    collision.GetComponent<Character>().OnHit(30f);
                    canAttack = false;
                }   
            }
        }
        if(collision.tag == "Enemy")
        {
            if(canAttack)
            {
                if(Player.instance.canSkill)
                {
                    StartCoroutine(dameSkill(collision));
                    Debug.Log("dame");
                    canAttack = false;
                }
            }
        }
    }  

    IEnumerator dameSkill(Collider2D collision)
    {
        yield return new WaitForSeconds(.7f);
        collision.GetComponent<Character>().OnHit(100f);
    }
}
