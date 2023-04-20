using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddToNetwork: MonoBehaviour
{
    public GameObject XRRig;
    private string _IPAddress = "172.25.104.69";
    private Vector3 _pos = new Vector3(-1.812F, 0, -1.425F);
    
    public void OnPressStartClient()
    {
        // XRRig.AddComponent<NetworkIdentity>();
        // XRRig.AddComponent<NetworkTransform>();
        // XRRig.transform.position = _pos;
        // NetworkLauncher.Instance.playerPrefab = XRRig;
        XRRig.SetActive(false);
        NetworkLauncher.Instance.StartClient();
        /*SceneManager.LoadScene("EscapeRoom");
        SceneManager.UnloadSceneAsync("StartRoom");*/
    }
}