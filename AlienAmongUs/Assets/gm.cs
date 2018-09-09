using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;
using System;
using System.Linq;

public class gm : MonoBehaviour
{
    private List<playerScript> _allPlayers;
    private Dictionary<int, playerScript> _idToPlayer;
    static readonly int[] allPossibleValues = populateAllPossible();
    List<int> allCurrentPossible;
    Dictionary<int, int> _matchesInProgress;
    static int[] populateAllPossible()
    {
        int[] values = new int[1000 - 100];
        for (int i = 100; i < 1000; i++)
        {
            values[i - 100] = i;
        }
        return values;
    }

    public List<playerScript> AllPlayers
    {
        get
        {
            return _allPlayers;
        }
        private set
        {
            _allPlayers = value;
        }
    }

    public playerScript getPlayer(int id)
    {
        playerScript returnVal = null;
        Debug.Assert(_idToPlayer.TryGetValue(id, out returnVal));
        return returnVal;
    }

    public bool tryGetPlayer(int id, out playerScript playerScript)
    {
        return _idToPlayer.TryGetValue(id, out playerScript);
    }

    // Use this for initialization
    void Start()
    {
        AllPlayers = new List<playerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        tick();
    }

    private void tick()
    {
        for (int i = 0; i < AllPlayers.Count; i++)
        {
            AllPlayers[i].serverTick(Time.deltaTime);
        }
    }

    private void checkGameState()
    {
        int aliveHumanPlayers = 0;
        int aliveAlienPlayers = 0;
        foreach (var idToPlayer in _idToPlayer)
        {
            if (!idToPlayer.Value.IsDown && !idToPlayer.Value.IsAlien)
                ++aliveHumanPlayers;
            else if (!idToPlayer.Value.IsDown && idToPlayer.Value.IsAlien)
                ++aliveAlienPlayers;
        }
        if (aliveHumanPlayers == 1 && aliveAlienPlayers == 1)
            gameOver(false);
        else if (aliveAlienPlayers == 0)
            gameOver(true);
    }

    public void gameStart()
    {
        //assign IDs to every player
        //idToPlayer.Add(p.ID, p);//and add the ID to the dictionary
        _idToPlayer = new Dictionary<int, playerScript>();
        //allCurrentPossible = new int[allPossibleValues.Length];
        //Array.Copy(allPossibleValues, allCurrentPossible, allPossibleValues.Length);
        allCurrentPossible = new List<int>(allPossibleValues);
        _matchesInProgress = new Dictionary<int, int>();
        foreach (playerScript item in AllPlayers)
        {
            int index = UnityEngine.Random.Range(0, allCurrentPossible.Count);
            int value = allCurrentPossible[index];
            allCurrentPossible.Remove(index);
            item.ID = value;
            _idToPlayer.Add(value, item);
            assignID(item);
            item.State = playerScript.PlayerState.Alive;
            assignState(item);
            item.LastScannedID = -1;
        }
        int randomIndex = UnityEngine.Random.Range(0, _idToPlayer.Count);
        _idToPlayer.ElementAt(randomIndex).Value.Type = playerScript.PlayerType.Alien;
        foreach (var idToPlayer in _idToPlayer)
        {
            assignType(idToPlayer.Value);
        }
    }

    public void addPlayer(playerScript p)//this is called when a player joins
    {
        AllPlayers.Add(p);//add them to the list of players
    }

    public void reinitialize(playerScript player)
    {
        assignID(player);
        assignType(player);
        assignState(player);
    }

    public void accusation(int source, int target)
    {
        playerScript targetPlayer = getPlayer(target);
        if (!targetPlayer.IsAlive)
            return;
        if (targetPlayer.IsAlien)//if the target is an alien
        {
            //GAME OVER WE WIN
            kill(targetPlayer.ID);
        }
        else//if it was a false accusation
        {
            kill(source);//kill them for making the mistake
        }
        checkGameState();
    }

    public void poison(int target)
    {
        getPlayer(target).poison();
    }

    private void sendFailuresToWaitingParties(playerScript player, int validTarget)
    {
        int requestedCount = player.CurrentRequesters.Count;
        if (requestedCount != 0)
        {
            for (int i = 0; i < requestedCount; i++)
            {
                int currID = player.CurrentRequesters[i];
                if (currID != validTarget)
                {
                    playerScript previousRequester = getPlayer(currID);
                    sendIDCallback(previousRequester, false);
                }
            }
            player.CurrentRequesters.Clear();
        }
    }
    private void sendIDCallback(playerScript receiver, bool success, playerScript target = null)
    {
        idRequestMessageTP.MessageSuccessState state = success ? idRequestMessageTP.MessageSuccessState.Success : idRequestMessageTP.MessageSuccessState.Failure;
        Debug.Assert(!success || target != null);
        receiver.PhoneRef.SendCmd("idRequestCallback", new idRequestMessageTP(state, target != null ? target.ID : -1, this));
    }

    public void onDeath(playerScript playerScript)
    {
        int recipient;
        GameObject.FindObjectOfType<databaseDisplayScript>().killPlayer(playerScript);
        if (_matchesInProgress.TryGetValue(playerScript.ID, out recipient))
        {
            _matchesInProgress.Remove(playerScript.ID);
        }
        else
        {
            KeyValuePair<int, int> match = _matchesInProgress.FirstOrDefault(x => x.Value == playerScript.ID);
            if (match.Key != 0 && match.Value != 0)
            {
                _matchesInProgress.Remove(match.Key);
            }
        }
        assignState(playerScript);
        checkGameState();
    }

    private void gameOver(bool humansWin)
    {

    }

    public void onSuccessfulMatch(int player1, int player2)
    {
        onSuccessfulMatch(getPlayer(player1), getPlayer(player2));

    }

    public void onSuccessfulMatch(playerScript player1, playerScript player2)
    {
        GameObject.FindObjectOfType<databaseDisplayScript>().glowPlayer(player1);
        GameObject.FindObjectOfType<databaseDisplayScript>().glowPlayer(player2);
        player1.OnScan(player2);
        player2.OnScan(player1);
        if (player1.IsPoisoning && player1.IsAlien)
        {
            poison(player2.ID);
            player1.IsPoisoning = false;
        }
        if (player2.IsPoisoning && player2.IsAlien)
        {
            poison(player1.ID);
            player2.IsPoisoning = false;
        }
        checkGameState();
    }

    public void requestID(int requester, int target)//this gets the target ID data and returns it to the requester
    {
        //update grid to show two people interacting
        //we need to store who's interacting with who
        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.IsRequesting = false;
        if (requester == target)
        {
            sendIDCallback(requestingPlayer, false);
            return;
        }
        if (requestingPlayer.LastScannedID == target)
        {
            sendIDCallback(requestingPlayer, false);
            return;
        }
        if (target != -1)
        {
            playerScript targetPlayer = null;
            if (tryGetPlayer(target, out targetPlayer))
            {
                sendFailuresToWaitingParties(requestingPlayer, target);
                if (_matchesInProgress.ContainsKey(target))
                {
                    sendIDCallback(requestingPlayer, true, targetPlayer);
                    sendIDCallback(targetPlayer, true, requestingPlayer);
                    targetPlayer.IsRequesting = false;
                    onSuccessfulMatch(requestingPlayer, targetPlayer);
                    _matchesInProgress.Remove(target);
                }
                else
                {
                    _matchesInProgress.Add(requestingPlayer.ID, targetPlayer.ID);
                    requestingPlayer.IsRequesting = true;
                    targetPlayer.CurrentRequesters.Add(requestingPlayer.ID);
                }
            }
            else
            {
                sendIDCallback(requestingPlayer, false);
            }
        }
        else
        {
            if (_matchesInProgress.ContainsKey(requester))
            {
                _matchesInProgress.Remove(requester);
            }
        }


        //requestingPlayer.PhoneRef.SendCmd("idDelivery", new sendIDMessageTP(target, this));//THIS IS THE FUNCTION TO SEND A COMMAND TO THE PHONE, idDelivery is what the phone is listening for
    }

    public void cancelRequest(int ID)
    {
        if (_matchesInProgress.ContainsKey(ID))
        {
            _matchesInProgress.Remove(ID);
        }
        playerScript player = getPlayer(ID);
        player.IsRequesting = false;
        sendIDCallback(player, false);
    }

    public void assignID(playerScript target)
    {
        Debug.Log(string.Format("assigning id to target " + target.PlayerName + " id + " + target.ID.ToString()));
        target.PhoneRef.SendCmd("assignID", new messageAssignIDTP(target.ID));

    }

    public void assignType(playerScript target)
    {
        Debug.Log(string.Format("assigning type to target " + target.PlayerName + " type + " + target.Type.ToString()));
        target.PhoneRef.SendCmd("assignType", new messageAssignTypeTP(target.Type));
    }

    public void assignState(playerScript target)
    {
        Debug.Log(string.Format("assigning state to target " + target.PlayerName + " state + " + target.State.ToString()));
        target.PhoneRef.SendCmd("assignState", new messageAssignStateTP(target.State));
    }


    public void sendTargets(int requester, List<int> targets)
    {
        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.PhoneRef.SendCmd("targetDelivery", new listMessageTP(targets, this));
    }

    public void sendAllTargets(int requester)
    {
        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.PhoneRef.SendCmd("targetDelivery", new listMessageTP(new List<int>(_idToPlayer
            .Where(x => { return x.Value.IsAlive; })
            .Select(x => { return x.Key; })),
            this));
    }


    public void kill(int target)
    {
        //update the grid
        //play the noise
        getPlayer(target).death();

    }

    //COMMANDS TO BE SENT TO THE PHONE
    #region toPhone
    [Serializable]
    public class sendIDMessageTP
    {
        public sendIDMessageTP(int target, gm manager)
        {
            if (target == -1)
            {
                playerID = -1;
                return;
            }
            playerScript player = manager.getPlayer(target);
            Debug.Assert(player != null);
            //player.PlayerName = playerName;
            //player.PlayerPhoto = playerPhoto;
            playerID = target;
            playerName = player.PlayerName != null ? player.PlayerName : "unnamed";
            playerPhoto = player.PlayerPhoto != null ? Utility.ConvertTexture2DToString(player.PlayerPhoto) : "null";
        }

        public sendIDMessageTP()
        {

        }

#if UNITY_EDITOR && DEBUG
        public sendIDMessageTP(string name, Texture2D texture)
        {
            if (Application.isPlaying)
                Debug.LogError("Critical Error: Test Constructor called in build!");
            playerName = name;
            playerPhoto = texture != null ? Utility.ConvertTexture2DToString(texture) : null;
        }
#endif
        public readonly int playerID = 0;
        public readonly string playerName = null;
        public readonly string playerPhoto = null;
        public readonly playerScript.PlayerState playerStatus = playerScript.PlayerState.Alive;

        //on the phone, the handler will look like
        //client.addEventListener('idDelivery', function(data){idName = data.playerName}

    }

    [Serializable]
    public class listMessageTP
    {
        public readonly List<sendIDMessageTP> _messages = null;

        static List<sendIDMessageTP> convertToMessages(List<int> targets, gm manager)
        {
            Debug.Assert(targets != null && targets.Count > 0);
            List<sendIDMessageTP> returnVal = new List<sendIDMessageTP>();
            foreach (int item in targets)
            {
                returnVal.Add(new sendIDMessageTP(item, manager));
            }
            return returnVal;
        }
        public listMessageTP()
        {

        }

        public listMessageTP(List<int> targets, gm manager)
            : this(convertToMessages(targets, manager))
        {

        }

        public listMessageTP(List<sendIDMessageTP> messages)
        {
            _messages = new List<sendIDMessageTP>(messages);
        }

    }

    [Serializable]
    public class idRequestMessageTP : sendIDMessageTP
    {
        public enum MessageSuccessState
        {
            Failure,
            Success,
        }
        public readonly MessageSuccessState successState;
        public idRequestMessageTP(MessageSuccessState state, int targetID, gm manager)
            : base(targetID, manager)
        {
            successState = state;
        }


    }
    private class messageAssignIDTP
    {
        public messageAssignIDTP(int _ID)
        {

            ID = _ID;

        }

        public int ID;
    }

    private class messageAssignTypeTP
    {
        public playerScript.PlayerType type;
        public messageAssignTypeTP(playerScript.PlayerType _type)
        {
            type = _type;
        }
    }

    private class messageAssignStateTP
    {
        public playerScript.PlayerState state;
        public messageAssignStateTP(playerScript.PlayerState _state)
        {
            state = _state;
        }
    }
    #endregion
    //END COMMANDS TO BE SENT TO THE PHONE
}
