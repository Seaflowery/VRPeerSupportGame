using System;
using Mirror;
using UnityEngine;

public class AuthorityManager: NetworkBehaviour
{
    public static AuthorityManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdAuthorize(GameObject hoveredObject, NetworkIdentity identity)
    { 
        identity.AssignClientAuthority(hoveredObject.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Command(requiresAuthority = false)]
    public void CmdRemoveAuthority(NetworkIdentity identity)
    {
        identity.RemoveClientAuthority();
    }

}