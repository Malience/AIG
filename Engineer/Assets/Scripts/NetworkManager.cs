using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetworkManager : NetworkBehaviour {
    static NetworkManager manager;

    [SyncVar]
    public float power;

	// Use this for initialization
	void Start () {
        //MasterServer.ipAddress = "127.0.0.1";
        Network.InitializeServer(2, 28888, !Network.HavePublicAddress());
        MasterServer.RegisterHost("Elevator Game", "Room 13");
        manager = this;
    }

    // Update is called once per frame
    void Update()
    {
        power = FurnaceController.furnace.power;
    }

    void OnServerInitialized()
    {
        Debug.Log("Server");
    }

    void Sync()
    {

    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.Serialize(ref FurnaceController.furnace.power);
        }
    }
}
