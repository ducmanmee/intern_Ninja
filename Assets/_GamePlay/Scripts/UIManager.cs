using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] Text coinText;
    [SerializeField] Image cooldownSlide;
    private bool isSlideCooldown = false;
    private int count = 0;
    /*public static UIManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectsByType<UIManager>();
            }
            return instance;
        }
    }*/

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

    private void Update()
    {

        //Debug.Log(isSlideCooldown);
        if(!Player.instance.canDash)
        {
            Debug.Log(1);
            isSlideCooldown = true;
        }
        if(isSlideCooldown)
        {
            count++;
            if(count == 1)
            {
                
                cooldownSlide.fillAmount = 1f;
            }
            cooldownSlide.fillAmount -= 1 / Player.instance.dashingCooldown * Time.deltaTime;

            if(cooldownSlide.fillAmount <= 0f )
            {
                cooldownSlide.fillAmount = 0f;
                isSlideCooldown= false;
                count = 0;
                Player.instance.canDash = true;
            }
        }
    }


    public void setCoin(int coin)
    {
        coinText.text = coin.ToString();
    }

}
