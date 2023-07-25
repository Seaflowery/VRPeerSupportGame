using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AuthorityManager: NetworkBehaviour
{
    public List<GameObject> authorizeObjects;

    public void Authorize()
    {
        if (!isServer && !isOwned)
            CmdAuthorize(netIdentity);
    }
    
    
    public void RemoveAuthority()
    {
        if (!isServer && isOwned)
            CmdRemoveAuthority(netIdentity);
        foreach (GameObject authorizeObject in authorizeObjects)
        {
            if (authorizeObject != null && authorizeObject.GetComponent<NetworkIdentity>() != null)
            {
                NetworkIdentity id = authorizeObject.GetComponent<NetworkIdentity>();
                if (!id.isServer && id.isOwned)
                    CmdRemoveAuthority(id);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAuthorize(NetworkIdentity identity, NetworkConnectionToClient sender = null)
    { 
        // Debug.Log(identity.name + " is being authorized");
        identity.RemoveClientAuthority();
        identity.AssignClientAuthority(sender);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveAuthority(NetworkIdentity identity, NetworkConnectionToClient sender = null)
    {
        identity.RemoveClientAuthority();
    }

}