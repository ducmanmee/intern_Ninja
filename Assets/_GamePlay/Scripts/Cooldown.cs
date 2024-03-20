using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    private float time;
    [SerializeField] private Image cooldown;
    private bool isCooldown = false;
    // Start is called before the first frame update
    void Start()
    {
        cooldown.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        _cooldown();
        Debug.Log(Player.instance.isDashing);

        
    }

    private void _cooldown()
    {
        if (Player.instance.isDashing)
        {
            isCooldown = true;
            cooldown.fillAmount = 1f;
            while (isCooldown)
            {
                cooldown.fillAmount -= (1/Player.instance.dashingCooldown) * Time.deltaTime;
                if (time >= Player.instance.dashingCooldown)
                {
                    isCooldown = false;
                    time = 0f;
                }
            }
        }
        
    }    
}
