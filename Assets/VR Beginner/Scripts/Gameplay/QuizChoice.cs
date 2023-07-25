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
        Debug.Log("choose");
        if (rightAns)
        {
            right.Play();
            if (!isServer)
                Log.Instance.CmdLog("client right");
            else
            {
                Debug.Log("server right");
            }
        }
        else
        {
            wrong.Play();
            StartCoroutine(DisableChoice());
            if (!isServer)
                Log.Instance.CmdLog("client wrong");
            else
            {
                Debug.Log("server wrong");
            }
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
    
    // [Command(requiresAuthority = false)]
    // void NotifyRightAns()
    // {
    //     Quiz.Instance.questionOne = false;
    //     Quiz.Instance.NotifyRightAns(roundOne ? 0:1);
    // }

    IEnumerator DisableChoice()
    {
        QuizChoice[] choices = FindObjectsOfType<QuizChoice>();
        Debug.Log("length: " + choices.Length);
        foreach (QuizChoice choice in choices)
        {
            if (choice != this)
            {
                choice.enabled = false;
            }
        }
        yield return new WaitForSeconds(3);
        foreach (QuizChoice choice in choices)
        {
            choice.enabled = true;
        }
    }
}