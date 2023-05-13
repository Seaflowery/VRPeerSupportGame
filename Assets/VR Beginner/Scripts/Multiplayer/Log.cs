
using Mirror;
using UnityEngine;

public class Log: NetworkBehaviour
{
    public static Log Instance;
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