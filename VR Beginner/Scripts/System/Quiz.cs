using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Quiz: MonoBehaviour
{
    private const int ChoiceNum = 4;
    public GameObject[] quizChoice = new GameObject[ChoiceNum];
    public int rightAns = 0;

    private void PrepareQuestions(int round)
    {
        // TODO: select quiz questions and get right answer 
        rightAns = (round == 0) ? 0 : 3;
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
            }
        }
    }
}