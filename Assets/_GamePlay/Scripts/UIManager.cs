using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
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

    [SerializeField] Text coinText; 

    public void setCoin(int coin)
    {
        coinText.text = coin.ToString();
    }

}
