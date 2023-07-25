using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationAnchors: NetworkBehaviour
{
    public GameObject[] _teleports0;
    public GameObject[] _teleports1;
    public static TeleportationAnchors Instance;
    [SyncVar]
    public int playerNum = 0;
    
    public void Awake()
    {
        Instance = this;
    }
}