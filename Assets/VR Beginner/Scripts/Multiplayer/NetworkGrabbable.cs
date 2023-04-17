using Mirror;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class NetworkGrabbable: XROffsetGrabbable
{
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();

        if (!identity.isOwned)
        {
            GameObject hoveredObject = args.interactable.gameObject;
            identity.AssignClientAuthority(hoveredObject.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();

        if (!identity.isOwned)
        {
            identity.RemoveClientAuthority();
        } 
    }
}