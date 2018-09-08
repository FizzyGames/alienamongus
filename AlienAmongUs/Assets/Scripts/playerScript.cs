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
	public int ID;
	public string playerName;
	public Texture playerPhoto;
	public HappyFunTimes.NetPlayer phoneRef;
    public gm gms;
    //public bool isAlien, dead, poisoned;
    private PlayerState state;
    private PlayerType _type;

    public PlayerState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
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
        public int idToAccuse = 0;
    };

    private void onAccuse(messageAccuse data)
    {
        gms.accusation(ID, data.idToAccuse);
    }

    private class messageSetName
    {
        public string selectedName = null;
    };

    private void onSetName(messageSetName data)
    {
        playerName = data.selectedName;

        //update the main screen to reflect this
    }

    private class messagePoison
    {
        public int idToPoison = 0;
    };

    private void onPoison(messagePoison data)
    {
        gms.poison(data.idToPoison);

        //update the main screen to reflect this
    }

    private class messageRequestScan
    {
        public int idToScan = 0;
    };

    private void onRequestScan(messageRequestScan data)
    {
        gms.requestID(ID, data.idToScan);

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
