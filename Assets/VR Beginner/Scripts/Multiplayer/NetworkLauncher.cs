using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine.XR.Interaction.Toolkit;

using Debug = UnityEngine.Debug;


public class NetworkLauncher: NetworkManager
{
    // public GameObject syncObject;
    public static NetworkLauncher Instance;
    private bool _firstAdd = true;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void OnStartServer()
    {
        Debug.Log("get started server");
        NetworkServer.SpawnObjects();
        

    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        GameObject key = GameObject.Find("KeyForDoor");
        key.SetActive(false); 
        GameObject dancingInteriors = GameObject.Find("Interior/dancingInteriors");
        GameObject pumpkin = dancingInteriors.transform.Find("Pumpkin").gameObject;
        GameObject pumpkin1 = dancingInteriors.transform.Find("Pumpkin1").gameObject;
        pumpkin.SetActive(false);
        pumpkin1.SetActive(false);
        GameObject fireboy = GameObject.Find("Interior/FireBoy");
        fireboy.SetActive(false);
    }

    public override async void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("add player");
        // Assign authority to client for all networked objects spawned on the server
        GameObject player = Instantiate(playerPrefab); 
        await SetupWatch(conn, player);
        
        TeleportationProvider provider = player.GetComponentInChildren<TeleportationProvider>();
        GameObject[] teleports = TeleportationAnchors.Instance._teleports0;
        if (numPlayers == 2)
        {
            teleports = TeleportationAnchors.Instance._teleports1;
        }
        foreach (GameObject teleport in teleports)
        {
            teleport.GetComponent<TeleportationAnchor>().teleportationProvider = provider;
            // teleport.GetComponent<AuthorityManager>().Authorize();
        }
        MasterController masterController = player.GetComponent<MasterController>();
        masterController.OnConnect();
        masterController.serverStarted = true;
        if (_firstAdd)
        {
            
            WatchScript.Instance.ServerStart();
            WatchScript.Instance.serverStart = true;
            // MasterController.Instance.serverStarted = true;
            AlignmentTrigger.Instance.serverStarted = true;
            CCManager.Instance.ServerStart();
            CCManager.Instance.serverStart = true;
            _firstAdd = false;
        }
    }
    

    private async Task SetupWatch(NetworkConnectionToClient conn, GameObject player)
    {
        // Add the player to the game world
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("add xr rig");

        await Task.Yield(); // Wait for next frame
    }

    public override void OnStartClient()
    {
        StartCoroutine(FindXRRig());
    }
    
    IEnumerator FindXRRig()
    {
        int num = numPlayers + 1;
        GameObject[] players = null;
        while (players == null || players.Length < num)
        {
            players = GameObject.FindGameObjectsWithTag("XRRig");
            yield return null;
        }
        Debug.Log("out");
        GameObject xrRig = null;
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                xrRig = player;
                break;
            }
        }

        TeleportationProvider provider = xrRig.GetComponentInChildren<TeleportationProvider>();
        Log.Instance.CmdLog("provider: " + provider);
        GameObject[] teleports = TeleportationAnchors.Instance._teleports0;
        if (num == 2)
        {
            teleports = TeleportationAnchors.Instance._teleports1;
        }
        foreach (GameObject teleport in teleports)
        {
            teleport.SetActive(true);
            teleport.GetComponent<TeleportationAnchor>().teleportationProvider = provider;
            // teleport.GetComponent<AuthorityManager>().Authorize();
        }
        MasterController masterController = xrRig.GetComponent<MasterController>();
        masterController.OnConnect();
        masterController.serverStarted = true;
        
        WatchScript.Instance.ServerStart();
        WatchScript.Instance.serverStart = true;
        // MasterController.Instance.serverStarted = true;
        AlignmentTrigger.Instance.serverStarted = true;
        CCManager.Instance.ServerStart();
        CCManager.Instance.serverStart = true; 
    }

    public override void OnStopServer()
    {
        Debug.Log(("server stop"));
    }

    public override void OnClientConnect()
    {
        Debug.Log("Client connect");
        NetworkClient.Ready();
    }
    
  

    public override void OnClientDisconnect()
    {
        Debug.Log("Client disconnect");
    }
}