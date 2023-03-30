using UnityEngine;
using UnityEngine.Events;

public class WoodTrigger: MonoBehaviour
{
    public UnityEvent OnWoodCollision;
    public bool DestroyedOnTriggered;
    
    void OnCollisionEnter(Collision other)
    {
        var proj = other.rigidbody.GetComponent<WoodenLogBehaviour>();

        if (proj != null)
        {
            Destroy(proj);
            OnWoodCollision.Invoke();
            if(DestroyedOnTriggered)
                Destroy(this);
        }
    }
}