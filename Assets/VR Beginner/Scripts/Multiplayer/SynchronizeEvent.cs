using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SynchronizeEvent: NetworkBehaviour
{
    public SelectEnterEvent AuthorityOnSelectedEnter;
    public SelectEnterEvent OnSelectedEnter;
    public SelectEnterEvent ServerEvent;
    // public List<GameObject> authorizeObjects;

    void Start()
    {
        var interactor = GetComponent<XRBaseInteractor>();
        if (interactor == null)
        {
            var interactable = GetComponent<SocketTarget>();
            interactable.SocketedEvent.AddListener(InvokeEvent);
        }
        else
        {
            interactor.selectEntered.AddListener(InvokeEvent);
        }
    }
    
    void InvokeEvent(SelectEnterEventArgs evt)
    {
        AuthorityOnSelectedEnter.Invoke(evt);
        OnSelectedEnter.Invoke(evt);
        if (!isServer)
            CmdInvoke(evt);
    }

    [Command(requiresAuthority = false)]
    void CmdInvoke(SelectEnterEventArgs evt)
    {
        Debug.Log("book of spell invoke???");
        OnSelectedEnter.Invoke(evt);
        ServerEvent.Invoke(evt);
        RpcInvoke(evt);
    }
    
    [ClientRpc]
    void RpcInvoke(SelectEnterEventArgs evt)
    {
        OnSelectedEnter.Invoke(evt);
    }
}