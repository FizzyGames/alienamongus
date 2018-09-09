using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HappyFunTimes;
using System;

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
	public Texture2D PlayerPhoto { get; set; }
	public HappyFunTimes.NetPlayer PhoneRef { get; set; }
    public gm Manager { get; set; }
    public List<int> CurrentRequesters { get; set; }
    public bool IsPoisoning { get; set; }
    private PlayerState _state;
    private PlayerType _type;
    public const int POISON_TURNS_TIMER_RESET = 2;
    public const float REQUEST_TIMER_RESET = 30;
    public int PoisonTimer { get; set; }
    public float RequestingTimer { get; set; }
    public int LastScannedID { get; set; }

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

    private bool _isRequesting;
    public bool IsRequesting
    {
        get
        {
            return _isRequesting;
        }
        set
        {
            _isRequesting = value;
            if (!_isRequesting)
                RequestingTimer = 0;
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
        CurrentRequesters = new List<int>();
        PhoneRef.RegisterCmdHandler<messageAccuse>("accuse", onAccuse);
        PhoneRef.RegisterCmdHandler<messageAccuseListRequest>("accuseListRequest", onAccuseListRequest);
        PhoneRef.RegisterCmdHandler<messageSetName>("setName", onSetName);
        PhoneRef.RegisterCmdHandler<messagePoison>("poison", onPoison);
        PhoneRef.RegisterCmdHandler<messageRequestScan>("requestScan", onRequestScan);
        PhoneRef.RegisterCmdHandler<messageReceivePhoto>("receivePhoto", onReceivePhoto);
        PhoneRef.RegisterCmdHandler<messageSetName>("_hft_setname_", onSetName);
        PoisonTimer = POISON_TURNS_TIMER_RESET;
    }

    internal void reinitialize(NetPlayer netPlayer)
    {
        PhoneRef = netPlayer;
PhoneRef.RegisterCmdHandler<messageAccuse>("accuse", onAccuse);
        PhoneRef.RegisterCmdHandler<messageAccuseListRequest>("accuseListRequest", onAccuseListRequest);
        PhoneRef.RegisterCmdHandler<messageSetName>("setName", onSetName);
        PhoneRef.RegisterCmdHandler<messagePoison>("poison", onPoison);
        PhoneRef.RegisterCmdHandler<messageRequestScan>("requestScan", onRequestScan);
        PhoneRef.RegisterCmdHandler<messageReceivePhoto>("receivePhoto", onReceivePhoto);
        PhoneRef.RegisterCmdHandler<messageSetName>("_hft_setname_", onSetName);
        Manager.reinitialize(this);
    }

    public void resetGameState()
    {
        LastScannedID = -1;
        IsPoisoning = false;
        IsRequesting = false;
        PoisonTimer = POISON_TURNS_TIMER_RESET;
        RequestingTimer = 0;
        CurrentRequesters.Clear();
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
        public messageSetName()
        {  // needed for deserialization
        }
        public messageSetName(string _name)
        {
            name = _name;
        }
        public string name = "";
    };

    private void onSetName(messageSetName data)
    {
        PlayerName = data.name;

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
        public int scanType = 0;
    };

    private void onRequestScan(messageRequestScan data)
    {
        Manager.requestID(ID, data.idToScan);
        if (IsAlien && data.scanType == 1)
            IsPoisoning = true;
        //update the main screen to reflect this
    }

    private class messageReceivePhoto
    {
        public string dataURL;
    };

    private static string s_dataUrlHeader = "data:image/png;base64,";

    private void onReceivePhoto(messageReceivePhoto data)
    {
        if (data.dataURL != null)
        {
            Debug.Log(data.dataURL);
            Byte[] temp = Convert.FromBase64String(data.dataURL.Substring(s_dataUrlHeader.Length));
            Texture2D tex = new Texture2D(4, 4);
            tex.LoadImage(temp);
            PlayerPhoto = tex;
        }
        else
            Debug.Log("dataurl is empty bitch");

        //update the main screen to reflect this
    }

    private class messageAccuseListRequest
    {
        public string a;
    }

    private void onAccuseListRequest(messageAccuseListRequest eventArgs)
    {
        Manager.sendAllTargets(ID);
    }


    public void death()
    {
        //dead = true;
        State = PlayerState.Dead;
        Manager.onDeath(this);
        //PUT UP DAT RED SCREEN
    }

    public void poison()
    {
        if(State == PlayerState.Alive && !IsAlien)
        {
            State = PlayerState.Poisoned;
        }
    }

    public void OnScan(playerScript other)
    {
        LastScannedID = other.ID;
        if(State == PlayerState.Poisoned)
        {
            PoisonTimer -= 1;
            if(PoisonTimer <= 0)
            {
                death();
            }
        }
        RequestingTimer = 0;
    }

    public void serverTick(float deltaTime)
    {
        if(IsRequesting)
        {
            RequestingTimer += Time.deltaTime;
        }
        if(RequestingTimer > REQUEST_TIMER_RESET)
        {
            RequestingTimer = 0;
            IsRequesting = false;
            Manager.cancelRequest(ID);
        }
    }
}
