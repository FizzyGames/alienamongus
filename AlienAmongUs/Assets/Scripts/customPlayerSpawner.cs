using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using UnityEngine;

public class customPlayerSpawner : HappyFunTimes.PlayerSpawner
{
    private Dictionary<string, playerScript> _existingSessions;

    protected override void Awake()
    {
        base.Awake();
        _existingSessions = new Dictionary<string, playerScript>();
    }

    protected override GameObject StartPlayer(NetPlayer netPlayer, object data)
    {
        playerScript player = null;
        if (_existingSessions.TryGetValue(netPlayer.GetSessionId(), out player))
        {
            player.reinitialize(netPlayer);
            return player.gameObject;
        }
        else
        {
            GameObject newPlayerGO = base.StartPlayer(netPlayer, data);
            _existingSessions.Add(netPlayer.GetSessionId(), newPlayerGO.GetComponent<playerScript>());
            return newPlayerGO;
        }
    }
}
