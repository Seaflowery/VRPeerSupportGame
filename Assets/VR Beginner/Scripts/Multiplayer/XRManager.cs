using UnityEngine;
using Mirror;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRigScript : NetworkBehaviour
{
    // The XR Rig component
    /*private XRRig xrRig;

    // Start is called before the first frame update
    void Start()
    {
        // Get the XR Rig component
        xrRig = GetComponent<XRRig>();

        // Check if this is the local player or not
        if (isLocalPlayer)
        {
            // This is the local player
            // Enable the XR Rig component and its children
            xrRig.enabled = true;
            foreach (Transform child in xrRig.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            // This is not the local player
            // Disable the XR Rig component and its children
            xrRig.enabled = false;
            foreach (Transform child in xrRig.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }*/
}