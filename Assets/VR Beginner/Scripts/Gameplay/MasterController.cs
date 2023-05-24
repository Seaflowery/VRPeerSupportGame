using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

/// <summary>
/// Master script that will handle reading some input on the controller and trigger special events like Teleport or
/// activating the MagicTractorBeam
/// </summary>
public class MasterController : NetworkBehaviour
{

    public XRRig Rig => m_Rig;

    [Header("Setup")]
    public bool DisableSetupForDebug = false;
    public Transform StartingPosition;
    public GameObject TeleporterParent;
    
    [Header("Reference")]
    public XRRayInteractor RightTeleportInteractor;
    public XRRayInteractor LeftTeleportInteractor;

    public XRDirectInteractor RightDirectInteractor;
    public XRDirectInteractor LeftDirectInteractor;

    public MagicTractorBeam RightTractorBeam;
    public MagicTractorBeam LeftTractorBeam;
    
    XRRig m_Rig;
    
    InputDevice m_LeftInputDevice;
    InputDevice m_RightInputDevice;

    XRInteractorLineVisual m_RightLineVisual;
    XRInteractorLineVisual m_LeftLineVisual;

    HandPrefab m_RightHandPrefab;
    HandPrefab m_LeftHandPrefab;
    
    XRReleaseController m_RightController;
    XRReleaseController m_LeftController;

    bool m_PreviousRightClicked = false;
    bool m_PreviousLeftClicked = false;

    bool m_LastFrameRightEnable = false;
    bool m_LastFrameLeftEnable = false;
    
    public bool serverStarted = false;

    LayerMask m_OriginalRightMask;
    LayerMask m_OriginalLeftMask;
    
    List<XRBaseInteractable> m_InteractableCache = new List<XRBaseInteractable>(16);


    
    void OnEnable()
    {
         InputDevices.deviceConnected += RegisterDevices;
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= RegisterDevices;
    }

    void Start()
    {
        Debug.Log("start");
        
        
        if (!DisableSetupForDebug)
        {
            transform.position = StartingPosition.position;
            transform.rotation = StartingPosition.rotation;
            
            if(TeleporterParent != null)
                TeleporterParent.SetActive(false);
            else
            {
                // TeleporterParent = GameObject.Find("TeleportAnchors");
                // TeleporterParent.SetActive(false);
            }
        }
        
        m_Rig = GetComponent<XRRig>();
        
    }

    public void OnConnect()
    {
        RightDirectInteractor = gameObject.transform.Find("Camera Offset/XR RightHand Controller(Clone)").GetComponent<XRDirectInteractor>();
        LeftDirectInteractor = gameObject.transform.Find("Camera Offset/XR LeftHand Controller(Clone)").GetComponent<XRDirectInteractor>();
        RightTractorBeam = gameObject.transform.Find("Camera Offset/XR RightHand Controller(Clone)").GetComponent<MagicTractorBeam>();
        LeftTractorBeam = gameObject.transform.Find("Camera Offset/XR LeftHand Controller(Clone)").GetComponent<MagicTractorBeam>();
        m_RightLineVisual = RightTeleportInteractor.GetComponent<XRInteractorLineVisual>();
        m_RightLineVisual.enabled = false;

        m_LeftLineVisual = LeftTeleportInteractor.GetComponent<XRInteractorLineVisual>();
        m_LeftLineVisual.enabled = false;

        m_RightController = RightTeleportInteractor.GetComponent<XRReleaseController>();
        m_LeftController = LeftTeleportInteractor.GetComponent<XRReleaseController>();

        m_OriginalRightMask = RightTeleportInteractor.interactionLayerMask;
        m_OriginalLeftMask = LeftTeleportInteractor.interactionLayerMask;
        
        InputDeviceCharacteristics leftTrackedControllerFilter = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left;
        List<InputDevice> foundControllers = new List<InputDevice>();
        
        InputDevices.GetDevicesWithCharacteristics(leftTrackedControllerFilter, foundControllers);
        if (!isServer)
            Log.Instance.CmdLog(NetworkLauncher.Instance.numPlayers + " players");
        if (foundControllers.Count > 0 && NetworkLauncher.Instance.numPlayers == 0)
            m_LeftInputDevice = foundControllers[0];
        else if (foundControllers.Count > 1)
        {
            XRRig[] xrRig = GameObject.FindObjectsOfType<XRRig>();
            foreach (XRRig rig in xrRig)
            {
                if (!isServer)
                    Log.Instance.CmdLog("rig: " + rig.gameObject.name);
                if (rig.gameObject != gameObject)
                {
                    rig.transform.Find("Camera Offset/Main Camera").gameObject.SetActive(false);
                    rig.transform.Find("Camera Offset/RightHand Controller").GetComponent<AlignmentTrigger>().enabled = false;
                    rig.transform.Find("Camera Offset/LeftHand Controller").GetComponent<AlignmentTrigger>().enabled = false;
                    rig.gameObject.GetComponent<MasterController>().enabled = false;
                    
                    foreach (var controller in foundControllers)
                    {
                        if (controller != rig.gameObject.GetComponent<MasterController>().m_LeftInputDevice)
                        {
                            m_LeftInputDevice = controller;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        if (!isServer)
            Log.Instance.CmdLog("left controller: " + m_LeftInputDevice.name);
        Debug.Log("left controller: " + m_LeftInputDevice.name);
        
        InputDeviceCharacteristics rightTrackedControllerFilter = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right;

        InputDevices.GetDevicesWithCharacteristics(rightTrackedControllerFilter, foundControllers);

        if (foundControllers.Count > 0)
            m_RightInputDevice = foundControllers[0];
        
        if (!isServer)
            Log.Instance.CmdLog("right controller" + m_RightInputDevice.name);

        if (m_Rig.currentTrackingOriginMode != TrackingOriginModeFlags.Floor)
            m_Rig.cameraYOffset = 1.8f;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdConnect()
    {
        RpcBlockXRRig();
    }

    [ClientRpc]
    public void RpcBlockXRRig()
    {
        XRRig[] xrRig = GameObject.FindObjectsOfType<XRRig>();
        foreach (XRRig rig in xrRig)
        {
            if (rig.name != "Original XR Rig")
            {
                GameObject rightHand = GameObject.Find("/XR RightHand Controller(Clone)");
                GameObject leftHand = GameObject.Find("/XR LeftHand Controller(Clone)");
                rightHand.GetComponent<AlignmentTrigger>().enabled = false;
                leftHand.GetComponent<AlignmentTrigger>().enabled = false;
                rightHand.GetComponent<MagicTractorBeam>().enabled = false;
                leftHand.GetComponent<MagicTractorBeam>().enabled = false;
                rightHand.GetComponent<XRController>().enabled = false;
                leftHand.GetComponent<XRController>().enabled = false;
                rightHand.GetComponent<XRDirectInteractor>().enabled = false;
                leftHand.GetComponent<XRDirectInteractor>().enabled = false;
                leftHand.transform.SetParent(rig.transform.Find("Camera Offset").transform);
                rightHand.transform.SetParent(rig.transform.Find("Camera Offset").transform);
                
                rig.transform.Find("Camera Offset/RightHandTeleportation").gameObject.SetActive(false);
                rig.transform.Find("Camera Offset/LeftHandTeleportation").gameObject.SetActive(false);
                rig.transform.Find("Camera Offset/LeftUIInteractor").gameObject.SetActive(false);
                rig.transform.Find("Camera Offset/RightUIInteractor").gameObject.SetActive(false);
                // rig.transform.Find("Camera Offset/XR RightHand Controller").GetComponent<AlignmentTrigger>().enabled = false;
                // rig.transform.Find("Camera Offset/XR RightHand Controller").GetComponent<XRController>().enabled = false;
                // rig.transform.Find("Camera Offset/XR RightHand Controller").GetComponent<XRDirectInteractor>().enabled = false;
                // rig.transform.Find("Camera Offset/XR LeftHand Controller").GetComponent<AlignmentTrigger>().enabled = false;
                // rig.transform.Find("Camera Offset/XR LeftHand Controller").GetComponent<XRController>().enabled = false;
                // rig.transform.Find("Camera Offset/XR LeftHand Controller").GetComponent<XRDirectInteractor>().enabled = false;
                GameObject mainCamera = GameObject.Find("/Main Camera(Clone)");
                mainCamera.transform.SetParent(rig.transform.Find("Camera Offset").transform);
                mainCamera.GetComponent<Camera>().enabled = false;
                mainCamera.transform.GetComponent<TrackedPoseDriver>().enabled = false;
                mainCamera.transform.GetComponent<UniversalAdditionalCameraData>().enabled = false;
                rig.gameObject.GetComponent<MasterController>().enabled = false;
            }
        }
    } 
    
    

    void RegisterDevices(InputDevice connectedDevice)
    {
        // if (connectedDevice.isValid)
        // {
        //     if ((connectedDevice.characteristics & InputDeviceCharacteristics.HeldInHand) == InputDeviceCharacteristics.HeldInHand)
        //     {
        //         if ((connectedDevice.characteristics & InputDeviceCharacteristics.Left) == InputDeviceCharacteristics.Left)
        //         {
        //             m_LeftInputDevice = connectedDevice;
        //         }
        //         else if ((connectedDevice.characteristics & InputDeviceCharacteristics.Right) == InputDeviceCharacteristics.Right)
        //         {
        //             m_RightInputDevice = connectedDevice;
        //         }
        //     }
        // }
    }
    
    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();

        if (serverStarted)
        {
            RightTeleportUpdate();
            // Debug.Log(m_LeftInputDevice.name);
            LeftTeleportUpdate();
        }
    }

    void RightTeleportUpdate()
    {
        Vector2 axisInput;
        m_RightInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisInput);
        
        m_RightLineVisual.enabled = axisInput.y > 0.5f;
        
        RightTeleportInteractor.interactionLayerMask = m_LastFrameRightEnable ? m_OriginalRightMask : new LayerMask();
        
        if (axisInput.y <= 0.5f && m_PreviousRightClicked)
        {
            m_RightController.Select();
            
        }

        
        if (axisInput.y <= -0.5f)
        {
            if(!RightTractorBeam.IsTracting)
                RightTractorBeam.StartTracting();
        }
        else if(RightTractorBeam.IsTracting)
        {
            RightTractorBeam.StopTracting();
        }

        //if the right animator is null, we try to get it. It's not the best performance wise but no other way as setup
        //of the model by the Interaction Toolkit is done on the first update.
        if (m_RightHandPrefab == null)
        {
            m_RightHandPrefab = RightDirectInteractor.GetComponentInChildren<HandPrefab>();
        }

        m_PreviousRightClicked = axisInput.y > 0.5f;

        if (m_RightHandPrefab != null)
        {
            m_RightHandPrefab.Animator.SetBool("Pointing", m_PreviousRightClicked);
        }

        m_LastFrameRightEnable = m_RightLineVisual.enabled;
    }

    void LeftTeleportUpdate()
    {
        Vector2 axisInput;
        m_LeftInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axisInput);
        
        m_LeftLineVisual.enabled = axisInput.y > 0.5f;
        
        LeftTeleportInteractor.interactionLayerMask = m_LastFrameLeftEnable ? m_OriginalLeftMask : new LayerMask();
        
        if (axisInput.y <= 0.5f && m_PreviousLeftClicked)
        {
            m_LeftController.Select();
        }
        
        if (axisInput.y <= -0.5f)
        {
            if(!LeftTractorBeam.IsTracting)
                LeftTractorBeam.StartTracting();
        }
        else if(LeftTractorBeam.IsTracting)
        {
            LeftTractorBeam.StopTracting();
        }
        
        //if the left animator is null, we try to get it. It's not the best performance wise but no other way as setup
        //of the model by the Interaction Toolkit is done on the first update.
        if (m_LeftHandPrefab == null)
        {
            m_LeftHandPrefab = LeftDirectInteractor.GetComponentInChildren<HandPrefab>();
        }

        m_PreviousLeftClicked = axisInput.y > 0.5f;
        
        if (m_LeftHandPrefab != null)
            m_LeftHandPrefab.Animator.SetBool("Pointing", m_PreviousLeftClicked);
        
        m_LastFrameLeftEnable = m_LeftLineVisual.enabled;
    }
}
