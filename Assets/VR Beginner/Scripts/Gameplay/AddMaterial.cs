using UnityEngine;

public class AddMaterial: MonoBehaviour
{
    public Material[] quizInstructions = new Material[2];
    
    void Start()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>(); // get the Renderer component of the GameObject
        skinnedMeshRenderer.materials = quizInstructions; // assign the materials array to the Renderer component
    }
}