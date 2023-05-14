using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class WoodTrigger: NetworkBehaviour
{
    public UnityEvent OnWoodCollision;
    public bool DestroyedOnTriggered;
    
    void OnCollisionEnter(Collision other)
    {
        var proj = other.rigidbody.GetComponent<WoodenLogBehaviour>();

        if (proj != null)
        {
            Destroy(proj);
            OnWoodCollision.Invoke();
            if (!isServer)
            {
                CmdInvoke();
            }
            if(DestroyedOnTriggered)
                Destroy(this);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdInvoke()
    {
        Debug.Log("wood invoked");
        RpcInvoke();
        OnWoodCollision.Invoke();
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcInvoke()
    {
        OnWoodCollision.Invoke();
    }
}