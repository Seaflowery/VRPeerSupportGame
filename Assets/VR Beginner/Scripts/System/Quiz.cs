using System;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions;

public class Quiz: NetworkBehaviour
{
    private const int ChoiceNum = 4;
    public GameObject[] quizChoice = new GameObject[ChoiceNum];
    public int rightAns = 0;
    public bool questionOne;
    public Texture2D[] questionTextures = new Texture2D[4];
    public Texture2D[] answerTextures = new Texture2D[4];
    public Material questionMaterial;
    public Material answerMaterial;
    public static Quiz Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetQuestionOne(bool one)
    {
        questionOne = one;
    }

    private void PrepareQuestions(int round)
    {
        // select quiz questions and get right answer 
        if (!questionOne)
        {
            rightAns = (round == 0) ? 1 : 3;
            int seq = (round == 0) ? 1 : 3;
            questionMaterial.mainTexture = questionTextures[seq];
            questionMaterial.SetTexture("_EmissionMap", questionTextures[seq]);
            answerMaterial.mainTexture = answerTextures[seq];
        }
        else
        {
            rightAns = (round == 0) ? 1 : 2;
            int seq = (round == 0) ? 0 : 2;
            questionMaterial.mainTexture = questionTextures[seq];
            questionMaterial.SetTexture("_EmissionMap", questionTextures[seq]);
            answerMaterial.mainTexture = answerTextures[seq];

        }
    }


    public void NotifyRightAns(int round)
    {
        PrepareQuestions(round);
        for (int i = 0; i < ChoiceNum; i++)
        {
            QuizChoice choice = quizChoice[i].GetComponent<QuizChoice>();
            if (choice != null)
            {
                choice.rightAns = (i == rightAns);
                choice.roundOne = (round == 0);
                choice.problemOne = questionOne;
            }
        }
        // CmdLog("all notified");
    }

}