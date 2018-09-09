using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;
using System;

public class gm : MonoBehaviour
{
    private List<playerScript> _allPlayers;
    private Dictionary<int, playerScript> _idToPlayer;
    static readonly int[] allPossibleValues = populateAllPossible();
    List<int> allCurrentPossible;
    static int[] populateAllPossible()
    {
        int[] values = new int[1000-100];
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

    // Use this for initialization
    void Start()
    {
        AllPlayers = new List<playerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void gameStart()
    {
        //assign IDs to every player
        //idToPlayer.Add(p.ID, p);//and add the ID to the dictionary
        _idToPlayer = new Dictionary<int, playerScript>();
        //allCurrentPossible = new int[allPossibleValues.Length];
        //Array.Copy(allPossibleValues, allCurrentPossible, allPossibleValues.Length);
        allCurrentPossible = new List<int>(allPossibleValues);
        foreach (playerScript item in AllPlayers)
        {
            int index = UnityEngine.Random.Range(0, allCurrentPossible.Count);
            int value = allCurrentPossible[index];
            allCurrentPossible.Remove(index);
            item.ID = value;
            _idToPlayer.Add(value, item);
            assignID(item);
        }
    }

    public void addPlayer(playerScript p)//this is called when a player joins
    {
        AllPlayers.Add(p);//add them to the list of players
    }

    public void accusation(int source, int target)
    {
        playerScript targetPlayer = getPlayer(target);
        if (!targetPlayer.IsAlive)
            return;
        if (targetPlayer.IsAlien)//if the target is an alien
        {
            //GAME OVER WE WIN

        }
        else//if it was a false accusation
        {
            kill(source);//kill them for making the mistake
        }
    }

    public void poison(int target)
    {
        getPlayer(target).poison();

    }

    public void requestID(int requester, int target)//this gets the target ID data and returns it to the requester
    {
        //update grid to show two people interacting
        //we need to store who's interacting with who

        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.PhoneRef.SendCmd("idDelivery", new sendIDMessageTP(target, this));//THIS IS THE FUNCTION TO SEND A COMMAND TO THE PHONE, idDelivery is what the phone is listening for


    }

    public void assignID(playerScript target) {
        target.PhoneRef.SendCmd("assignID", new messageAssignID(target.ID));

    }
    


    public void sendTargets(int requester, List<int> targets)
    {
        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.PhoneRef.SendCmd("targetDelivery", new listMessageTP(targets, this));
    }

    public void sendAllTargets(int requester)
    {
        playerScript requestingPlayer = getPlayer(requester);
        requestingPlayer.PhoneRef.SendCmd("targetDelivery", new listMessageTP(new List<int>(_idToPlayer.Keys), this));
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

    private class messageAssignID
    {
        public messageAssignID(int _ID)
        {

            ID = _ID;

        }

        public int ID;
    }


    #endregion
    //END COMMANDS TO BE SENT TO THE PHONE
}
