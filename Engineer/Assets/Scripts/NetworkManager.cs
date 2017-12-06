using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    public static NetworkManager manager;

    int connectionID = -1;
    int hostID;
    int tcp, udp;
    int port = 28888;
    byte error;

    [SerializeField]
    string host_address = "127.0.0.1";

    // Use this for initialization
    void Start () {
        manager = this;

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        tcp = config.AddChannel(QosType.Reliable);
        udp = config.AddChannel(QosType.Unreliable);

        HostTopology topology = new HostTopology(config, 2);
        hostID = NetworkTransport.AddHost(topology, port, host_address);

        Debug.Log("Host ID: " + hostID);
    }

    byte[] Pack(params float[] n)
    {
        byte[] buffer = new byte[sizeof(float) * n.Length];
        for(int i = 0; i < n.Length; i++)
        {
            byte[] bn = BitConverter.GetBytes(n[i]);
            for (int j = 0; j < sizeof(float); j++)
            {
                buffer[j + sizeof(float) * i] = bn[j];
            }
        }
        return buffer;
    }

    byte[] Pack(byte code, int n)
    {
        byte[] buffer = new byte[1 + sizeof(int)];
        buffer[0] = code;
        byte[] bn = BitConverter.GetBytes(n);
        for (int j = 0; j < sizeof(int); j++)
        {
            buffer[j + 1] = bn[j];
        }
        return buffer;
    }

    byte[] Pack(byte code, params byte[] n)
    {
        byte[] buffer = new byte[n.Length + 1];
        buffer[0] = code;
        for (int i = 0; i < n.Length; i++)
        {
            buffer[i + 1] = n[i];
        }
        return buffer;
    }

    byte[] Pack(byte code, byte[] bn, params byte[] n)
    {
        byte[] buffer = new byte[bn.Length + n.Length + 1];
        buffer[0] = code;
        for (int i = 0; i < bn.Length; i++)
        {
            buffer[i + 1] = bn[i];
        }
        for (int i = 0; i < n.Length; i++)
        {
            buffer[i + 1 + bn.Length] = n[i];
        }
        return buffer;
    }

    byte[] Pack(byte code, params float[] n)
    {
        byte[] buffer = new byte[sizeof(float) * n.Length + 1];
        buffer[0] = code;
        for (int i = 0; i < n.Length; i++)
        {
            byte[] bn = BitConverter.GetBytes(n[i]);
            for (int j = 0; j < sizeof(float); j++)
            {
                buffer[j + sizeof(float) * i + 1] = bn[j];
            }
        }
        return buffer;
    }
    public const byte SYNC = 0x0;
    public const byte COAL = 0x1;
    public const byte BURN = 0x2;
    public const byte LEVE = 0x3;
    public const byte EVEL = 0x4;
    public const byte SETU = 0x5;
    public const byte TRAP = 0x6;
    public const byte SPLT = 0x7;
    public const byte SPWN = 0x8;
    public const byte VICT = 0x9;
    public const byte DEFE = 0x10;
    public const byte NEXT = 0x11;
    public const byte RESE = 0x12;
    public const byte KILE = 0x13;
    public const byte KILG = 0x14;


    public void SendByteCode(byte code)
    {
        if (connectionID < 0) return;
        byte[] buffer = new byte[]{ code };
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);
        if (error == 2) connectionID = -1;
    }

    public void SendSetup()
    {
        FurnaceController f = FurnaceController.furnace;
        SendSetup(f.powerStep, f.powerLoss, f.elevatorDrain, f.elevatorCharge, f.genMax);
    }
    public void SendSetup(float powerStep, float powerLoss, float elevatorDrain, float elevatorCharge, float genMax)
    {
        if (connectionID < 0) return;
        byte[] buffer = Pack(SYNC, powerStep, powerLoss, elevatorDrain, elevatorCharge, genMax);
        NetworkTransport.Send(hostID, connectionID, tcp, buffer, buffer.Length, out error);
        if (error == 2) connectionID = -1;
    }

    public void SendSync()
    {
        FurnaceController f = FurnaceController.furnace;
        SendSync(f.power, f.elevatorPower, f.powerGen, f.coalBurning, f.chargelever.active ? 1 : 0);
    }
    public void SendSync(float power, float elevatorPower, float gen, float coal, float lever)
    {
        if (connectionID < 0) return;
        byte[] buffer = Pack(SYNC, power, elevatorPower, gen, coal, lever);
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendTrap(byte id, bool active)
    {
        if (connectionID < 0) return;
        byte[] buffer = { TRAP, id, (byte)(active ? 1 : 0) };
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendSplit(byte id, byte nid, byte len)
    {
        if (connectionID < 0) return;
        byte[] buffer = { SPLT, id, nid, len};
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendSpawn(byte id, byte loc, byte[] ids)
    {
        if (connectionID < 0) return;
        byte[] buffer = Pack(SPWN, new byte[]{ id, loc, (byte)ids.Length}, ids);
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendNext(int seed)
    {
        if (connectionID < 0) return;
        byte[] buffer = Pack(NEXT, seed);
        NetworkTransport.Send(hostID, connectionID, tcp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendKillEnemy(byte id)
    {
        if (connectionID < 0) return;
        byte[] buffer = { KILE, id};
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    public void SendKillGroup(byte id)
    {
        if (connectionID < 0) return;
        byte[] buffer = { KILG, id };
        NetworkTransport.Send(hostID, connectionID, udp, buffer, buffer.Length, out error);

        if (error == 2) connectionID = -1;
    }

    // Update is called once per frame
    void Update()
    {
        byte[] buffer = new byte[1024];
        int hostid, conid, channelid, datasize;
        NetworkEventType netEvent = NetworkTransport.Receive(out hostid, out conid, out channelid, buffer, 1024, out datasize, out error);

        switch (netEvent)
        {
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection from: " + hostid);
                connectionID = conid;
                NetworkManager.manager.SendSetup();
                NetworkManager.manager.SendSync();
                NetworkManager.manager.SendNext(MazeController.mcont.maze.seed);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(hostid + " disconnected");
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(buffer);
                Debug.Log("Message Recieved: " + msg);
                break;
        }
    }
}
