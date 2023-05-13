using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractor))]
public class InteractorEventDispatcher : NetworkBehaviour
{
    public SelectEnterEvent OnSelectedEnter;
    // public List<GameObject> authorizeObjects;

    void Start()
    {
        var interactor = GetComponent<XRBaseInteractor>();
        interactor.selectEntered.AddListener(evt => OnSelectedEnter.Invoke(evt));
    }
    
    // protected virtual void InvokeAfterAuthorized(SelectEnterEventArgs evt)
    // {
    //     foreach (GameObject authorizeObject in authorizeObjects)
    //     {
    //         if (authorizeObject != null && authorizeObject.GetComponent<NetworkIdentity>() != null)
    //         {
    //             // get the game object in unityEvent
    //             CmdGiveAuthority(authorizeObject.GetComponent<NetworkIdentity>());
    //             CmdLog("waiting for " + authorizeObject + " to be owned");
    //         }
    //     }
    //        
    //     OnSelectedEnter.Invoke(evt);
    // }
    //
    // [Command(requiresAuthority = false)]
    // protected virtual void CmdGiveAuthority(NetworkIdentity identity, NetworkConnectionToClient sender = null)
    // {
    //     identity.AssignClientAuthority(sender);
    // }
    //
    // [Command(requiresAuthority = false)]
    // void CmdLog(string s)
    // {
    //     Debug.Log(s);
    // }
}
