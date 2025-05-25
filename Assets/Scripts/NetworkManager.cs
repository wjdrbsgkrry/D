using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession serverSession = new ServerSession();

    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry iPHost = Dns.GetHostEntry(host);
        IPAddress iPAddress = iPHost.AddressList[0];
        IPEndPoint iPEnd = new IPEndPoint(iPAddress, 7777);

        Connector connector = new Connector();
        connector.Connect(iPEnd, () => { return serverSession; }, 1);
    }


    void Update()
    {
        
    }
}
