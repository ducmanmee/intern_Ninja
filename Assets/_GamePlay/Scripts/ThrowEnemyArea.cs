using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEnemyArea : MonoBehaviour
{
    public Enemy enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            enemy.throwEnemy(collision.GetComponent<Character>());
        }    
    }

}
