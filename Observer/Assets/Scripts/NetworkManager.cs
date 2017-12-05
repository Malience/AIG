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
    public const byte TRAP = 0x6;
    public const byte SPLT = 0x7;
    public const byte SPWN = 0x8;
    public const byte VICT = 0x9;
    public const byte DEFE = 0x10;
    public const byte NEXT = 0x11;
    public const byte RESE = 0x12;
    public const byte KILE = 0x13;
    public const byte KILG = 0x14;

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
                
                switch (code)
                {
                    case SYNC:
                        float[] n = Unpack(buffer, 1);
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
                        n = Unpack(buffer, 1);
                        UIScript.ui.powerStep = n[0];
                        UIScript.ui.powerLoss = n[1];
                        UIScript.ui.elevatorDrain = n[2];
                        UIScript.ui.elevatorCharge = n[3];
                        UIScript.ui.genMax = n[4];
                        break;
                    case TRAP:
                        Debug.Log("Recieved Trap: " + buffer[1] + ", " + buffer[2]);
                        MazeController.mcont.lever[buffer[1]] = buffer[2] == 1;
                        break;
                    case SPLT:
                        MazeController.mcont.SplitGroup(buffer[1], buffer[2], buffer[3]);
                        break;
                    case SPWN:
                        byte[] ids = new byte[buffer[3]];
                        for(byte i = 0; i < buffer[3]; i++)
                        {
                            ids[i] = buffer[4 + i];
                        }
                        MazeController.mcont.CreateGroup(buffer[1], buffer[2], ids);
                        break;
                    case VICT:
                        break;
                    case DEFE:
                        break;
                    case NEXT:
                        int seed = BitConverter.ToInt32(buffer, 1);
                        MazeController.mcont.MapGen(seed);
                        break;
                    case RESE:
                        break;
                    case KILE:
                        MazeController.mcont.KillEnemy(buffer[1]);
                        break;
                    case KILG:
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
