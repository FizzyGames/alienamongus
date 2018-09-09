using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winningParticleController : MonoBehaviour {
    gm manager;
    ParticleSystem ps;
    ParticleSystemRenderer psRenderer;
    private const float ALPHA = 0.5f;
    public Color humanWinColor = new Color(192, 187, 0, ALPHA);
    public Color alienWinColor = new Color(192, 0, 187, ALPHA);
    // Use this for initialization
    void Start () {
        manager = FindObjectOfType<gm>();
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        var emitter = ps.emission;
        emitter.enabled = manager.GameIsOver;
        if (manager)
        {
            var main = ps.main;
            if (manager.HumansWon)
                main.startColor = humanWinColor;
            else
                main.startColor = alienWinColor;
            
        }
	}
}
