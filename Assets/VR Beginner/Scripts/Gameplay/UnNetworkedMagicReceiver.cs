using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Will Trigger the OnMagicCollision when a MagicBallProjectile collide with the collider on which that script is
/// </summary>
public class UnNetworkedMagicReceiver : MonoBehaviour
{
    public UnityEvent OnMagicCollision;
    public bool DestroyedOnTriggered;
    
    void OnCollisionEnter(Collision other)
    {
        if (other == null || other.rigidbody == null)
            return;
        var proj = other.rigidbody.GetComponent<MagicBallProjectile>();

        if (proj != null)
        {
            Destroy(proj);
            OnMagicCollision.Invoke();
            if(DestroyedOnTriggered)
                Destroy(this);
        }
    }
}