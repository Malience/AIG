using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using System.Text;
using System;

public class NetworkManager : MonoBehaviour {

    [SerializeField]
    GameObject ui;

    int connectionID = -1;
    int hostID;
    int tcp;
    int udp;
    int port = 28889;
    int serverPort = 28888;
    byte error;

    // Use this for initialization
    void Start()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        tcp = config.AddChannel(QosType.Reliable);
        udp = config.AddChannel(QosType.Unreliable);

        HostTopology topology = new HostTopology(config, 2);
        hostID = NetworkTransport.AddHost(topology, port, null);
        Debug.Log("Host ID: " + hostID);
    }

    const byte SYNC = 0x0;
    const byte COAL = 0x1;
    const byte BURN = 0x2;
    const byte LEVE = 0x3;
    const byte EVEL = 0x4;
    const byte SETU = 0x5;

    float[] Unpack(byte[] buffer, int start)
    {
        int length = (buffer.Length - start) / sizeof(float);
        float[] n = new float[length];

        for(int i = 0; i < length; i++)
        {
            n[i] = BitConverter.ToSingle(buffer, start + sizeof(float) * i);
        }

        return n;
    }

    // Update is called once per frame
    void Update () {
        byte[] buffer = new byte[sizeof(float) * 5 + 1];
        int hostid, conid, channelid, datasize;
        NetworkEventType netEvent = NetworkTransport.Receive(out hostid, out conid, out channelid, buffer, sizeof(float) * 5 + 1, out datasize, out error);

        switch (netEvent)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection from: " + hostid);
                Message("Houston, we have lift off");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(hostid + " disconnected");
                Connect();
                break;
            case NetworkEventType.DataEvent:
                byte code = buffer[0];
                float[] n = Unpack(buffer, 1);
                switch (code)
                {
                    case SYNC:
                        UIScript.ui.power = n[0];
                        UIScript.ui.elevatorPower = n[1];
                        UIScript.ui.powerGen = n[2];
                        UIScript.ui.coalBurning = (int)n[3];
                        UIScript.ui.lever = n[4] == 1;
                        break;
                    case COAL:
                        UIScript.ui.coalBurning++;
                        break;
                    case BURN:
                        UIScript.ui.coalBurning--;
                        break;
                    case LEVE:
                        UIScript.ui.lever = false;
                        break;
                    case EVEL:
                        UIScript.ui.lever = true;
                        break;
                    case SETU:
                        UIScript.ui.powerStep = n[0];
                        UIScript.ui.powerLoss = n[1];
                        UIScript.ui.elevatorDrain = n[2];
                        UIScript.ui.elevatorCharge = n[3];
                        UIScript.ui.genMax = n[4];
                        break;
                }
                
                break;
        }
    }

    void Connect()
    {
        connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverPort, 0, out error);

        if (error > 0) Debug.Log("Error: " + error);
    }

    void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    void Message(string message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, udp, buffer, message.Length * sizeof(char), out error);
    }

    void OnGUI()
    {
        if (connectionID < 0)
        {
            ui.SetActive(false);
            if (GUI.Button(new Rect(250, 250, 250, 250), "Connect"))
            {
                Connect();
                
            }

        }
        else { 
            ui.SetActive(true);
        }
    }
}
