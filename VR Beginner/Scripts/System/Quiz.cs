using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Quiz: MonoBehaviour
{
    private const int ChoiceNum = 4;
    public GameObject[] quizChoice = new GameObject[ChoiceNum];
    public int rightAns = 0;
    public static Quiz Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void PrepareQuestions(int round, bool questionOne = true)
    {
        // TODO: select quiz questions and get right answer 
        if (!questionOne)
            rightAns = (round == 0) ? 1 : 2;
        else
            rightAns = (round == 0) ? 0 : 3;
    }

    public void NotifyRightAns(int round, bool questionOne = true)
    {
        PrepareQuestions(round, questionOne);
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
    }
}