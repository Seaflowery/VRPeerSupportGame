using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkLauncher: NetworkManager
{
    // public GameObject syncObject;
    public AudioSource qwq;
    public AudioSource qaq;
    private string _IPAddress = "172.25.99.89";
    
    public void OnPressStartClient()
    {
        networkAddress = _IPAddress;
        singleton.StartClient();
    }
    public override void OnStartServer()
    {
        Debug.Log("get started server");
        NetworkServer.SpawnObjects();
        Debug.Log("succeed!!!");
        
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("add player");
        // Assign authority to client for all networked objects spawned on the server
        XRGrabInteractable[] grabInteractables = FindObjectsOfType<XRGrabInteractable>();
        foreach (XRGrabInteractable grabInteractable in grabInteractables)
        {
            if (grabInteractable.GetComponent<NetworkIdentity>() != null && grabInteractable.GetComponent<NetworkIdentity>().isServer)
            {
                // Assign authority to the client
                NetworkIdentity identity = grabInteractable.GetComponent<NetworkIdentity>();
                identity.AssignClientAuthority(conn);
                Debug.Log("Authority assigned to object " + identity.netId);
            }
        }
        Debug.Log("authority added!");
    }


    public override void OnStopServer()
    {
        Debug.Log(("server stop"));
    }

    public override void OnClientConnect()
    {
        Debug.Log("Client connect");
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }
        }
    }
    
  

    public override void OnClientDisconnect()
    {
        Debug.Log("Client disconnect");
    }
}