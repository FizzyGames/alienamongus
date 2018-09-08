using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class playerScript : MonoBehaviour {
	public int ID;
	public string playerName;
	public Texture playerPhoto;
	public HappyFunTimes.NetPlayer phoneRef;
	// Use this for initialization

	void InitializeNetPlayer(SpawnInfo spawnInfo) {
		// Save the netplayer object so we can use it send messages to the phone
		phoneRef = spawnInfo.netPlayer;
        GameObject.FindObjectOfType<gm>().allNetPlayers.Add(phoneRef);

	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
