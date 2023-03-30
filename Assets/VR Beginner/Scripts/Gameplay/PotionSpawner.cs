using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PotionBrewed will be hooked to the OnBrew event of the CauldronContent in the scene, and this script will take care
/// of dispatching the right prefab to the ObjectSpawner to make the potion spawn
/// </summary>
public class PotionSpawner : MonoBehaviour
{
    public GameObject PotionPrefab;
    public GameObject BadPotionPrefab;
    public ObjectSpawner SpawnerCorrect;
    public ObjectSpawner SpawnerIncorrect;
    public GameObject quizManager;
    

    public void PotionBrewed(CauldronContent.Recipe recipe)
    {
        if (recipe != null)
        {
            /*Quiz quiz = quizManager.GetComponent<Quiz>();
            quiz.SetQuestionOne(true);
            quiz.NotifyRightAns(1);*/
            /*Quiz.Instance.SetQuestionOne(true);
            Quiz.Instance.NotifyRightAns(1);*/
            SpawnerCorrect.Prefab = PotionPrefab;
            SpawnerCorrect.enabled = true;
            SpawnerCorrect.Spawn();
        }
        else
        {
            SpawnerIncorrect.Prefab = BadPotionPrefab;
            SpawnerIncorrect.enabled = true;
            SpawnerIncorrect.Spawn();
        }
    }
}
