using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAttackState : PIState
{
    public void OnEnter(Player player)
    {
        player._Attack();
    }

    public void OnExecute(Player player)
    {
        
    }

    public void OnExit(Player player)
    {
        
    }
}
