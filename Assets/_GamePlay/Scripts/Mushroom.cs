using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private GameObject mushroomVFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player")
        {
            collision.GetComponent<Character>().OnHit(30f);
            Instantiate(mushroomVFX, transform.position + Vector3.right * 1.5f, Quaternion.identity);
            OnDespawn();
        }

    }

    private void OnDespawn()
    {
        Destroy(gameObject);
    }
}
