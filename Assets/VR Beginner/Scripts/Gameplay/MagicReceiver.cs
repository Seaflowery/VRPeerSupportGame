using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Will Trigger the OnMagicCollision when a MagicBallProjectile collide with the collider on which that script is
/// </summary>
public class MagicReceiver : NetworkBehaviour
{
    public UnityEvent OnMagicCollision;
    public bool DestroyedOnTriggered;
    
    void OnCollisionEnter(Collision other)
    {
        var proj = other.rigidbody.GetComponent<MagicBallProjectile>();

        if (proj != null)
        {
            Destroy(proj);
            OnMagicCollision.Invoke();
            if (!isServer)
                CmdInvoke();
            if(DestroyedOnTriggered)
                Destroy(this);
        }
    }
    
    [Command(requiresAuthority = false)]
    void CmdInvoke()
    {
        OnMagicCollision.Invoke();
        RpcInvoke();
    }
    
    [ClientRpc]
    void RpcInvoke()
    {
        OnMagicCollision.Invoke();
    }
}
