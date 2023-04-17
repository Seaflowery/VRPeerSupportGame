using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// PotionBrewed will be hooked to the OnBrew event of the CauldronContent in the scene, and this script will take care
/// of dispatching the right prefab to the ObjectSpawner to make the potion spawn
/// </summary>
public class PotionSpawner : NetworkBehaviour
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
            /*SpawnerCorrect.Prefab = PotionPrefab;
            SpawnerCorrect.enabled = true;
            SpawnerCorrect.Spawn();*/
            CmdSpawnPotion(PotionPrefab, SpawnerCorrect);
        }
        else
        {
            /*SpawnerIncorrect.Prefab = BadPotionPrefab;
            SpawnerIncorrect.enabled = true;
            SpawnerIncorrect.Spawn();*/
            CmdSpawnPotion(BadPotionPrefab, SpawnerIncorrect);
        }
    }

    [Command]
    public void CmdSpawnPotion(GameObject prefab, ObjectSpawner spawner)
    {
        GameObject potion = Instantiate(prefab, spawner.transform.position, spawner.transform.rotation);
        NetworkServer.Spawn(potion);
        NetworkIdentity identity = potion.GetComponent<NetworkIdentity>();
        NetworkConnectionToClient owner = connectionToClient as NetworkConnectionToClient;
        if (owner != null)
        {
            identity.AssignClientAuthority(owner); // assign authority to owner
        }
    }
}
