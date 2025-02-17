﻿using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Subclass of the classic Socket Interactor from the Interaction toolkit that will only accept object with the right
/// SocketTarget 
/// </summary>
public class XRExclusiveSocketInteractor : XRSocketInteractor
{
    public string AcceptedType;

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        SocketTarget socketTarget = interactable.GetComponent<SocketTarget>();

        if (socketTarget == null)
            return false;

        return base.CanSelect(interactable) && (socketTarget.SocketType == AcceptedType);
    }
    

    public override bool CanHover(XRBaseInteractable interactable)
    {
        return CanSelect(interactable);
    }
}
