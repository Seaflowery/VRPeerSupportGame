using UnityEngine;

public class Rotation : MonoBehaviour
{
    // rotate the gameobject 90 degree
    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }
}