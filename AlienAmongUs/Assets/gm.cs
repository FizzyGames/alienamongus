using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class gm : MonoBehaviour {
    public List<playerScript> allPlayers;
    public Dictionary<int, playerScript> idToPlayer;

	// Use this for initialization
	void Start () {
        allPlayers = new List<playerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void gameStart()
    {
        //assign IDs to every player
    }

    public void addPlayer(playerScript p)//this is called when a player joins
    {
        allPlayers.Add(p);//add them to the list of players
        idToPlayer.Add(p.ID, p);//and add the ID to the dictionary
    }

    public void accusation(int source, int target)
    {
        if (idToPlayer[target].isAlien)//if the target is an alien
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
        idToPlayer[target].poisoned = true;

    }

    public void requestID(int requester, int target)//this gets the target ID data and returns it to the requester
    {
        //update grid to show two people interacting
        //we need to store who's interacting with who
        idToPlayer[requester].phoneRef.SendCmd("idDelivery", new messageSendID(target));//THIS IS THE FUNCTION TO SEND A COMMAND TO THE PHONE, idDelivery is what the phone is listening for


    }

    public void kill(int target)
    {
        //update the grid
        //play the noise
        idToPlayer[target].death();

    }

    //COMMANDS TO BE SENT TO THE PHONE

    private class messageSendID
    {
        public messageSendID(int target)
        {
            gm gms = GameObject.FindObjectOfType<gm>();
            gms.idToPlayer[target].playerName = playerName;
            gms.idToPlayer[target].playerPhoto = playerPhoto;

        }
        public string playerName;
        public Texture playerPhoto;

        //on the phone, the handler will look like
        //client.addEventListener('idDelivery', function(data){idName = data.playerName}

    }

    //END COMMANDS TO BE SENT TO THE PHONE
}
