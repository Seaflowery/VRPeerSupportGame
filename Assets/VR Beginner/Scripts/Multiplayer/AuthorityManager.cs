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

    

    public void OnStartAuthorize(NetworkConnectionToClient conn, GameObject prefab)
    {
        NetworkIdentity identity = prefab.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);
    }
}