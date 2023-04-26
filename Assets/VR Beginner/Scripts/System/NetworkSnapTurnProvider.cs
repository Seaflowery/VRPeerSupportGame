using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkSnapTurnProvider: DeviceBasedSnapTurnProvider
{
    public static NetworkSnapTurnProvider Instance;
    
    protected new void Awake()
    {
        base.Awake();
        Instance = this;
    }
    
    public void SetControllers()
    {
        XRRig rig = NetworkLocomotionSystem.Instance.xrRig;
        GameObject cameraOffset = rig.transform.Find("Camera Offset").gameObject;
        GameObject leftHandController = cameraOffset.transform.Find("LeftHand Controller").gameObject;
        GameObject rightHandController = cameraOffset.transform.Find("RightHand Controller").gameObject;
        XRController leftController = leftHandController.GetComponent<XRController>();
        XRController rightController = rightHandController.GetComponent<XRController>();
        controllers.Add(leftController);
        controllers.Add(rightController);
        Debug.Log(controllers.Count);
    }
}

