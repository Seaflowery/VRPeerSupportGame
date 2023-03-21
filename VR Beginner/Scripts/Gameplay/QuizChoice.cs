using UnityEngine;

public class QuizChoice: MonoBehaviour
{
    public bool rightAns;
    public GameObject cookingAnchor;
    public GameObject dancingAnchor;
    public bool roundOne;

    public void OnTriggerChoose()
    {
        if (rightAns && roundOne)
        {
            cookingAnchor.SetActive(true);
        } 
        else if (rightAns)
        {
            dancingAnchor.SetActive(true);
        }
    }
}