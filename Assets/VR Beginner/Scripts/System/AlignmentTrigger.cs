﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;


/// <summary>
/// This allow to do action through the OnEnterAligned/OnExitAlgined event when the list of required vector alignements
/// is met. This is use e.g. to trigger thing when the player view align with an object (e.g. the wrist watch use it)
/// </summary>
public class AlignmentTrigger : MonoBehaviour
{
    public enum Mode
    {
        View,
        World
    }

    [System.Serializable]
    public class AxisMatch
    {
        public Mode ExternalAxisMode;
        public Vector3 LocalAxis;
        public Vector3 ExternalAxis;
        [Range(0.0f, 1.0f)]
        public float Tolerance = 0.3f;
    }

    public AxisMatch[] RequiredMatch;
    public UnityEvent OnEnterAligned;
    public UnityEvent OnExitAligned;

    bool m_WasAligned = false;
    public bool serverStarted = false;
    
    public static AlignmentTrigger Instance;
    
    void Awake()
    {
        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        MasterController master = GetComponentInParent<MasterController>();
        if (master == null || serverStarted)
        {
            bool allMatch = true;

            for (int i = 0; i < RequiredMatch.Length && allMatch; ++i)
            {
                AxisMatch match = RequiredMatch[i];
            
                Vector3 worldLocal = transform.TransformVector(match.LocalAxis);
                Vector3 worldExternal = Vector3.zero;


                if (match.ExternalAxisMode == Mode.View)
                {
                    if (master == null)
                    {
                        if (MasterControllerLocal.Instance == null)
                        {
                            return;
                        }
                        worldExternal = MasterControllerLocal.Instance.Rig.cameraGameObject.transform.TransformVector(match.ExternalAxis);
                    }
                    else
                    {
                        if (master.Rig.cameraGameObject == null)
                        {
                            return;
                        }
                        worldExternal =  master.Rig.cameraGameObject.transform.TransformVector(match.ExternalAxis);
                    }
                }
                else
                {
                    worldExternal = match.ExternalAxis;
                }
                

                float dot = Vector3.Dot(worldLocal, worldExternal);

                allMatch &= dot > 1.0f - match.Tolerance;
            }
            
            // Debug.Log("allMatch: " + allMatch + " m_WasAligned: " + m_WasAligned + "");

            if (allMatch)
            {
                if (!m_WasAligned)
                {
                    OnEnterAligned.Invoke();
                    m_WasAligned = true;
                }
            }
            else
            {
                if (m_WasAligned)
                {
                    OnExitAligned.Invoke();
                    m_WasAligned = false;
                }
            } 
        }
        
    }
}
