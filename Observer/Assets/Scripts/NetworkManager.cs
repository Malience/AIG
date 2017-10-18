using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class NetworkManager : NetworkBehaviour {

    [SerializeField]
    GameObject ui;

    [SyncVar]
    public float power;

    HostData[] hosts;

	// Use this for initialization
	void Start () {
        //MasterServer.ipAddress = "127.0.0.1";
        ui.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (UIScript.ui != null)
        {
            UIScript.ui.power = power;
        }
	}

    void Refresh()
    {
        MasterServer.RequestHostList("Elevator Game");
        
    }

    void OnServerReadyToBeginMessage(NetworkMessage netMsg)
    {
        var beginMessage = netMsg.ReadMessage<IntegerMessage>();
        Debug.Log("received OnServerReadyToBeginMessage " + beginMessage.value);
    }

        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (!stream.isWriting)
        {
            float newpower = 0;
            stream.Serialize(ref newpower);
            UIScript.ui.power = newpower;
        }
    }

    void OnMasterServerEvent(MasterServerEvent e)
    {
        if(e == MasterServerEvent.HostListReceived)
        {
            hosts = MasterServer.PollHostList();
        }
    }

    void OnConnectedToServer()
    {
        Debug.Log("Holy crap we're actually connected!");
        ui.SetActive(true);
    }

    void OnGUI()
    {
        if (!Network.isClient)
        {
            ui.SetActive(false);
            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts")) Refresh();

            if (hosts != null)
            {
                for (int i = 0; i < hosts.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hosts[i].gameName))
                    {
                        Network.Connect(hosts[i]);
                    }
                }
            }
        } else
        {
            ui.SetActive(true);
        }
    }
}
