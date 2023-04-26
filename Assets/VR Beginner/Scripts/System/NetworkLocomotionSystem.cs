using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkLocomotionSystem: LocomotionSystem
{
    public static NetworkLocomotionSystem Instance;

    protected new void Awake()
    {
        base.Awake();
        Instance = this;
    }
    
    public void FindXRRig()
    {
       if (xrRig == null)
       {
           xrRig = FindObjectOfType<XRRig>();
       }
       Debug.Log("xrrig == null" + (xrRig == null));
    }
}