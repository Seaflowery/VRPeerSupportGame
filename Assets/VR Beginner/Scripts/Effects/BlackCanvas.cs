
using UnityEngine;

public class BlackCanvas: MonoBehaviour
{
    void Start() {
        // Set the background color of the camera to black
        Camera camera = GetComponentInChildren<Camera>();
        camera.backgroundColor = Color.black;
    } 
}