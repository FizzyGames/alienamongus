using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class playerScript : MonoBehaviour {
	public int ID;
	public string name;
	public Texture photo;
	public HappyFunTimes.netPlayer phoneRef;
	// Use this for initialization

	void InitializeNetPlayer(SpawnInfo spawnInfo) {
		// Save the netplayer object so we can use it send messages to the phone
		phoneRef = spawnInfo.netPlayer;

		// Register handler to call if the player disconnects from the game.
		m_netPlayer.OnDisconnect += Remove;

		// Track name changes
		m_playerNameManager = new HFTPlayerNameManager(m_netPlayer);
		m_playerNameManager.OnNameChange += ChangeName;

		// Setup events for the different messages.
		m_netPlayer.RegisterCmdHandler<MessageMove>("move", OnMove);
		m_netPlayer.RegisterCmdHandler<MessagePic>("pic", OnPicture);

		ExampleCameraGameSettings settings = ExampleCameraGameSettings.settings();
		m_position = new Vector3(UnityEngine.Random.Range(0, settings.areaWidth), 0, UnityEngine.Random.Range(0, settings.areaHeight));
		transform.localPosition = m_position;

		SetName(m_playerNameManager.Name);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
