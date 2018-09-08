using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class gm : MonoBehaviour {
    public List<HappyFunTimes.NetPlayer> allNetPlayers;
    public List<GameObject> allPlayers;

	// Use this for initialization
	void Start () {
        allNetPlayers = new List<HappyFunTimes.NetPlayer>();
        allPlayers = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void gameStart()
    {


    }
}
