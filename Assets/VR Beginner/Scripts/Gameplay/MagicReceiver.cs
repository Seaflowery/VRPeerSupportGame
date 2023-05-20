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
    public bool inactivateOnTriggered = false;
    public bool ColorConcerned = false;
    
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (ColorConcerned)
            return;
        var proj = other.rigidbody.GetComponent<MagicBallProjectile>();

        if (proj != null)
        {
            Destroy(proj);
            OnMagicCollision.Invoke();
            if (!isServer)
            {
                CmdInvoke();
            }

            if (inactivateOnTriggered)
            {
                CmdInactivate();
            }
            if(DestroyedOnTriggered)
                Destroy(this);
        }
    }
    
    [Command(requiresAuthority = false)]
    protected void CmdInvoke()
    {
        Debug.Log("magic invoked");
        OnMagicCollision.Invoke();
        RpcInvoke();
    }

    [Command(requiresAuthority = false)]
    void CmdInactivate()
    {
        Debug.Log("magic inactivated");
        RpcInactivate();
        gameObject.SetActive(false);
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcInactivate()
    {
        gameObject.SetActive(false);
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcInvoke()
    {
        OnMagicCollision.Invoke();
    }
}
