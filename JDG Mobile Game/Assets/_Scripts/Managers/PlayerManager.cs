using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] protected GameObject p1;
    [SerializeField] protected GameObject p2;

    private PlayerStatus playerStatus1;
    private PlayerStatus playerStatus2;

    public PlayerStatus GetCurrentPlayerStatus()
    {
        return GameStateManager.Instance.IsP1Turn ? playerStatus1 : playerStatus2;
    }

    public PlayerStatus GetOpponentPlayerStatus()
    {
        return GameStateManager.Instance.IsP1Turn ? playerStatus2 : playerStatus1;
    }

    protected void Start()
    {
        playerStatus1 = p1.GetComponent<PlayerStatus>();
        playerStatus2 = p2.GetComponent<PlayerStatus>();
        playerStatus1.SetNumberShield(0);
        playerStatus2.SetNumberShield(0);
    }

    public void HandleAttackIfOpponentIsPlayer()
    {
        var opponentPlayerStatus = GetOpponentPlayerStatus();
        // Directly attack the player
        if (opponentPlayerStatus.NumberShield > 0)
        {
            opponentPlayerStatus.DecrementShield();
        }
        else
        {
            var diff = CardManager.Instance.ComputeDamageAttack();
            opponentPlayerStatus.ChangePv(diff);
        }
    }

    //... other player-related methods and functions ...
}