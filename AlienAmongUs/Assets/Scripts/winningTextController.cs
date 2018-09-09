using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winningTextController : MonoBehaviour {
    gm manager;
    Text text;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
        manager = FindObjectOfType<gm>();
    }
	
	// Update is called once per frame
	void Update () {
        text.enabled = manager.GameIsOver;
        text.text = manager.HumansWon ?
            "HUMANS WIN" :
            "ALIENS WIN";
	}
}
