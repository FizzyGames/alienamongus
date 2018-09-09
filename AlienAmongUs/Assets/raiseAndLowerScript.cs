using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class raiseAndLowerScript : MonoBehaviour {
    public float distance, lerpVal, lerpSpeed, cooldown, counter;
    public bool rising, sinking, killerRise, holding;
    public Vector3 startPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (holding)
        {
            counter += Time.deltaTime;
            if (counter >= cooldown)
            {
                counter = 0;
                beginSink();
                holding = false;

            }


        }
        if (rising)
        {
            lerpVal += Time.deltaTime * lerpSpeed;
            
            if (killerRise)
            {
                GetComponent<RectTransform>().localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2, lerpVal);
                GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, new Vector3(0, 0, 0), lerpVal);
            }
            else
                GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, startPos + new Vector3(0, distance, 0), lerpVal);
            if (lerpVal >= 1)
            {
                lerpVal = 0;
                rising = false;
                if(!killerRise)
                    holding = true;
            }
        }
        if (sinking)
        {
            lerpVal += Time.deltaTime * lerpSpeed;
            GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos + new Vector3(0, distance, 0), startPos, lerpVal);
            if (lerpVal >= 1)
            {
                lerpVal = 0;
                sinking = false;

            }
        }
    }

    [ContextMenu("Start rise")]
    public void beginRise()
    {
        rising = true;
        transform.GetChild(1).GetComponent<RawImage>().color = Color.green;
    }

    public void beginSink()
    {
        sinking = true;
        transform.GetChild(1).GetComponent<RawImage>().color = Color.white;

    }

    [ContextMenu("Enemy reveal rise")]
    public void enemyVictoryRise()
    {
        killerRise = true;
        rising = true;
        transform.GetChild(1).GetComponent<RawImage>().color = Color.magenta;
    }
}
