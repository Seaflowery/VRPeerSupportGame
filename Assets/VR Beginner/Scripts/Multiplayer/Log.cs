
using Mirror;
using UnityEngine;

public class Log: NetworkBehaviour
{
    public static Log Instance;
    [SyncVar]
    public int playerNum = 0;
    
    public void Awake()
    {
        Instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdLog(string s)
    {
        Debug.Log(s);
    }
}