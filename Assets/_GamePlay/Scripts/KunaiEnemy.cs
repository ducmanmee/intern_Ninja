using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KunaiEnemy : MonoBehaviour
{
    public GameObject hitVFX;
    public Rigidbody2D rb;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        if(Enemy.instance != null)
        {
            rb.velocity = Enemy.instance.transform.right * speed;
            Invoke(nameof(OnDespawn), 4f);

        }
    }

    public void OnDespawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().OnHit(30f);
            Instantiate(hitVFX, transform.position, transform.rotation);
            OnDespawn();
        }
    }
}
