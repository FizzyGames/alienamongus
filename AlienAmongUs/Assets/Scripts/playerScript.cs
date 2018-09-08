using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class playerScript : MonoBehaviour {
	public int ID;
	public string playerName;
	public Texture playerPhoto;
	public HappyFunTimes.NetPlayer phoneRef;
    public gm gms;
    public bool isAlien, dead, poisoned;
	// Use this for initialization

	void InitializeNetPlayer(SpawnInfo spawnInfo) {
		// Save the netplayer object so we can use it send messages to the phone
		phoneRef = spawnInfo.netPlayer;
        gms = GameObject.FindObjectOfType<gm>();
        gms.addPlayer(this);
        phoneRef.RegisterCmdHandler<messageAccuse>("accuse", onAccuse);
        phoneRef.RegisterCmdHandler<messageSetName>("setName", onSetName);
        phoneRef.RegisterCmdHandler<messagePoison>("poison", onPoison);
        phoneRef.RegisterCmdHandler<messageRequestScan>("requestScan", onRequestScan);

    }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private class messageAccuse
    {
        public int idToAccuse;
    };

    private void onAccuse(messageAccuse data)
    {
        gms.accusation(ID, data.idToAccuse);
    }

    private class messageSetName
    {
        public string selectedName;
    };

    private void onSetName(messageSetName data)
    {
        playerName = data.selectedName;

        //update the main screen to reflect this
    }

    private class messagePoison
    {
        public int idToPoison;
    };

    private void onPoison(messagePoison data)
    {
        gms.poison(data.idToPoison);

        //update the main screen to reflect this
    }

    private class messageRequestScan
    {
        public int idToScan;
    };

    private void onRequestScan(messageRequestScan data)
    {
        gms.requestID(ID, data.idToScan);

        //update the main screen to reflect this
    }

    public void death()
    {
        dead = true;
        //PUT UP DAT RED SCREEN
    }

}
