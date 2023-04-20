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
            AuthorityManager.Instance.CmdAuthorize(hoveredObject, identity);
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();

        if (identity.isOwned)
        {
            AuthorityManager.Instance.CmdRemoveAuthority(identity);
        } 
    }

}