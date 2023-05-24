using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class QuizChoice: NetworkBehaviour
{
    public bool rightAns;
    public GameObject cookingAnchor;
    public GameObject dancingAnchor;
    public GameObject startButton;
    public bool roundOne;
    public bool problemOne;
    public AudioSource right;
    public AudioSource wrong;

    public void OnTriggerChoose()
    {
        if (rightAns) 
            right.Play();
        else
        {
            wrong.Play();
            StartCoroutine(DisableChoice());
        }
        if (problemOne)
        {
            if (rightAns)
            {
                Quiz.Instance.questionOne = false;
                Quiz.Instance.NotifyRightAns(roundOne ? 0:1);
            }
            
        }
        else
        {
            if (rightAns && roundOne)
            {
                cookingAnchor.SetActive(true);
            } 
            else if (rightAns)
            {
                dancingAnchor.SetActive(true);
                startButton.SetActive(true);
            }    
        }
    }

    IEnumerator DisableChoice()
    {
        QuizChoice[] choices = FindObjectsOfType<QuizChoice>();
        foreach (QuizChoice choice in choices)
        {
            choice.enabled = false;
        }
        yield return new WaitForSeconds(3);
        foreach (QuizChoice choice in choices)
        {
            choice.enabled = true;
        }
    }
}