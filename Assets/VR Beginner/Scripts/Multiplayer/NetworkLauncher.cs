using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    }

    public override async void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("add player");
        // Assign authority to client for all networked objects spawned on the server
        GameObject player = Instantiate(playerPrefab); 
        await SetupWatch(conn, player);
        
        TeleportationProvider provider = player.GetComponentInChildren<TeleportationProvider>();
        GameObject[] teleports = GameObject.FindGameObjectsWithTag("Teleport0");
        if (numPlayers == 2)
        {
            teleports = GameObject.FindGameObjectsWithTag("Teleport1");
        }
        foreach (GameObject teleport in teleports)
        {
            teleport.GetComponent<TeleportationAnchor>().teleportationProvider = provider;
            teleport.GetComponent<AuthorityManager>().Authorize();
        }
        MasterController masterController = player.GetComponent<MasterController>();
        masterController.OnConnect();
        masterController.serverStarted = true;
        if (_firstAdd)
        {
            // NetworkLocomotionSystem.Instance.FindXRRig();
            // NetworkSnapTurnProvider.Instance.SetControllers();
            // find child gameobject of player named LeftUIInteractor 
            // GameObject cameraOffset = player.transform.Find("Camera Offset").gameObject;
            // GameObject leftUIInteractor = cameraOffset.transform.Find("LeftUIInteractor").gameObject;
            // GameObject rightUIInteractor = cameraOffset.transform.Find("RightUIInteractor").gameObject;
            // WitchHouseUIHook.Instance.SetRenderer(leftUIInteractor, rightUIInteractor);
            // MasterController.Instance.ServerStart();
            
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
        GameObject[] teleports = GameObject.FindGameObjectsWithTag("Teleport0");
        if (num == 2)
        {
            teleports = GameObject.FindGameObjectsWithTag("Teleport1");
        }
        foreach (GameObject teleport in teleports)
        {
            teleport.GetComponent<TeleportationAnchor>().teleportationProvider = provider;
            // teleport.GetComponent<AuthorityManager>().Authorize();
        }
        MasterController masterController = xrRig.GetComponent<MasterController>();
        masterController.OnConnect();
        masterController.serverStarted = true;
        // NetworkLocomotionSystem.Instance.FindXRRig();
        // NetworkSnapTurnProvider.Instance.SetControllers();
        // find child gameobject of player named LeftUIInteractor 
        // GameObject cameraOffset = player.transform.Find("Camera Offset").gameObject;
        // GameObject leftUIInteractor = cameraOffset.transform.Find("LeftUIInteractor").gameObject;
        // GameObject rightUIInteractor = cameraOffset.transform.Find("RightUIInteractor").gameObject;
        // MasterController.Instance.ServerStart();
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