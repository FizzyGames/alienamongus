using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class databaseDisplayScript : MonoBehaviour {
    public gm gms;
    public List<GameObject> playerAvatars;
    public List<GameObject> players;
    public GameObject playerAvatarPrefab;
	// Use this for initialization
	void Start () {
        playerAvatars = new List<GameObject>();
        gms = GameObject.FindObjectOfType<gm>();
  
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("Start Game Display")]
    public void startGameDisplay()
    {
        foreach (playerScript p in gms.AllPlayers)
        {
            players.Add(p.gameObject);
            playerAvatars.Add(Instantiate(playerAvatarPrefab));
            GameObject currentAvatar = playerAvatars[playerAvatars.Count - 1];
            currentAvatar.transform.SetParent(GameObject.Find("Canvas").transform);
            currentAvatar.transform.GetChild(0).GetComponent<RawImage>().texture = p.PlayerPhoto;
            currentAvatar.transform.GetChild(2).GetComponent<Text>().text = p.PlayerName;
        }
        arrangeAvatars();
    }

    public void arrangeAvatars()
    {
        if (playerAvatars.Count <= 7)
        {
            for (int i = 0; i < playerAvatars.Count; i++)
            {
                playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 3), 125, 0);


            }
        }
        else if (playerAvatars.Count <= 14)
        {
            for (int i = 0; i < playerAvatars.Count; i++)
            {
                if(i>=7)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 10), -125, 0);
                else
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 3), 125, 0);


            }
        }
        else if (playerAvatars.Count <= 21)
        {
            for (int i = 0; i < playerAvatars.Count; i++)
            {       
                if (i < 7)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 3), 375, 0);
                else if (i < 14)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 10), 125, 0);
                else if (i < 21)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 10), -125, 0);

            }
        }
        else if (playerAvatars.Count <= 28)
        {
            for (int i = 0; i < playerAvatars.Count; i++)
            {
                if (i < 7)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 3), 375, 0);
                else if (i < 14)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 10), 125, 0);
                else if (i < 21)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 17), -125, 0);
                else if (i < 28)
                    playerAvatars[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(250 * (i - 21), -375, 0);

            }
        }

        foreach (GameObject p in playerAvatars)
        {
            p.GetComponent<raiseAndLowerScript>().startPos = p.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void glowPlayer(playerScript p)
    {
        //int index = players.FindIndex(x=>x.Equals(p.gameObject));
        int index = players.IndexOf(p.gameObject);
        playerAvatars[index].GetComponent<raiseAndLowerScript>().beginRise();
    }

    public void killPlayer(playerScript p)
    {
        int index = players.IndexOf(p.gameObject);
        playerAvatars[index].transform.GetChild(1).GetComponent<RawImage>().color = Color.red;

    }

    public void revealKiller(playerScript p)
    {
        int index = players.IndexOf(p.gameObject);
        //playerAvatars[index].transform.GetChild(1).GetComponent<RawImage>().color = Color.magenta;
    }

    
}
