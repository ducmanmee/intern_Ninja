using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private Image cooldownSlide;
    [SerializeField] private Image cooldownSkill;
    private bool isCooldown = false;
    private bool isCooldown1 = false;
    private int count = 0;
    private int count1 = 0;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cooldownSlide.fillAmount = 0f;
        cooldownSkill.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _cooldownSlide();     
        _cooldownSkill();
    }

    private void _cooldownSlide()
    {
        if (!Player.instance.canDash)
        {  
            isCooldown = true;
        }
        if (isCooldown)
        {
            count++;
            if (count == 1)
            {

                cooldownSlide.fillAmount = 1f;
            }
            cooldownSlide.fillAmount -= 1 / Player.instance.dashingCooldown * Time.deltaTime;

            if (cooldownSlide.fillAmount <= 0f)
            {
                cooldownSlide.fillAmount = 0f;
                isCooldown = false;
                count = 0;
                Player.instance.canDash = true;

            }
        }
    }
    private void _cooldownSkill()
    {
        Debug.Log(Player.instance.isSkill);
        time += Time.deltaTime;
        if (time >= 2.5f)
        {
            Player.instance.isSkill = false;
            Player.instance.DeActiveAttackArea();
            time = 0f;
        }
        if (!Player.instance.canSkill)
        {
            isCooldown1 = true;
        }
        if (isCooldown1)
        {
            count1++;
            if (count1 == 1)
            {
                cooldownSkill.fillAmount = 1f;
            }
            cooldownSkill.fillAmount -= 1 / Player.instance.skillCooldown * Time.deltaTime;

            if (cooldownSkill.fillAmount <= 0f)
            {
                cooldownSkill.fillAmount = 0f;
                isCooldown1 = false;
                count1 = 0;
                Player.instance.canSkill = true;
            }
        }
    }
}
