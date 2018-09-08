using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;

public class playerScript : MonoBehaviour {
    public enum PlayerState
    {
        Dead,
        Alive,
        Poisoned,
    }
    public enum PlayerType
    {
        Human,
        Alien,
    }
	public int ID { get; set; }
	public string PlayerName { get; set; }
	public Texture PlayerPhoto { get; set; }
	public HappyFunTimes.NetPlayer PhoneRef { get; set; }
    public gm Manager { get; set; }
    private PlayerState _state;
    private PlayerType _type;

    public PlayerState State
    {
        get
        {
            return _state;
        }

        set
        {
            _state = value;
        }
    }

    public PlayerType Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }

    public bool IsAlive { get { return State != PlayerState.Dead; } }
    public bool IsDown { get { return State != PlayerState.Alive; } }
    public bool IsAlien { get { return Type == PlayerType.Alien; } }


    // Use this for initialization

    void InitializeNetPlayer(SpawnInfo spawnInfo) {
		// Save the netplayer object so we can use it send messages to the phone
		PhoneRef = spawnInfo.netPlayer;
        Manager = GameObject.FindObjectOfType<gm>();
        Manager.addPlayer(this);
        PhoneRef.RegisterCmdHandler<messageAccuse>("accuse", onAccuse);
        PhoneRef.RegisterCmdHandler<messageSetName>("setName", onSetName);
        PhoneRef.RegisterCmdHandler<messagePoison>("poison", onPoison);
        PhoneRef.RegisterCmdHandler<messageRequestScan>("requestScan", onRequestScan);

    }
    

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private class messageAccuse
    {
        public int idToAccuse = 0;
    };

    private void onAccuse(messageAccuse data)
    {
        Manager.accusation(ID, data.idToAccuse);
    }

    private class messageSetName
    {
        public string selectedName = null;
    };

    private void onSetName(messageSetName data)
    {
        PlayerName = data.selectedName;

        //update the main screen to reflect this
    }

    private class messagePoison
    {
        public int idToPoison = 0;
    };

    private void onPoison(messagePoison data)
    {
        Manager.poison(data.idToPoison);

        //update the main screen to reflect this
    }

    private class messageRequestScan
    {
        public int idToScan = 0;
    };

    private void onRequestScan(messageRequestScan data)
    {
        Manager.requestID(ID, data.idToScan);

        //update the main screen to reflect this
    }

    public void death()
    {
        //dead = true;
        State = PlayerState.Dead;
        //PUT UP DAT RED SCREEN
    }

    public void poison()
    {
        if(State == PlayerState.Alive && !IsAlien)
        {
            State = PlayerState.Poisoned;
        }
    }
}
