using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MagicReceiverColor: MagicReceiver
{
    public string acceptedColor;
    public UnityEvent OnWrongColorCollision;

    protected override void OnCollisionEnter(Collision other)
    {
        if (!ColorConcerned)
            return;
        var proj = other.rigidbody.GetComponent<MagicBallProjectile>();
        

        if (proj != null)
        {
            Destroy(proj);
            if (proj.color == acceptedColor)
            {
                OnMagicCollision.Invoke();
                if (!isServer)
                {
                    CmdInvoke();
                }
            }
            else
            {
                OnWrongColorCollision.Invoke();
                if (!isServer)
                {
                    CmdWrongColorInvoke();
                }
            }
        }
        if(DestroyedOnTriggered)
            Destroy(this); 
    }
    
    [Command(requiresAuthority = false)]
    protected void CmdWrongColorInvoke()
    {
        Debug.Log("wrong color invoked");
        OnWrongColorCollision.Invoke();
        RpcWrongColorInvoke();
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcWrongColorInvoke()
    {
        OnWrongColorCollision.Invoke();
    }
}