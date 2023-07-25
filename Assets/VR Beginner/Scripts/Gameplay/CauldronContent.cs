using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

/// <summary>
/// Handle all operations related to dropping thing in the cauldron and brewing things.
/// </summary>
public class CauldronContent : NetworkBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string name;
        public string[] ingredients;
        public int temperature;
        public int rotation;
    }

    [System.Serializable]
    public class BrewEvent : UnityEvent<Recipe> { };
    
    public Recipe[] Recipes;
    public int TemperatureIncrement;

    [Header("Effects")]
    public GameObject SplashEffect;
    public Animator CauldronAnimator;
    
    private VisualEffect splashVFX;
    public VisualEffect brewEffect;

    /// <summary>
    /// Will be called when the cauldron finish brewing, with the recipe as parameters or null if no recipe match.
    /// </summary>
    public BrewEvent OnBrew;

    [Header("Audio")]
    public AudioSource AmbientSoundSource;
    public AudioSource BrewingSoundSource;
    public AudioClip[] SplashClips;
    
    bool m_CanBrew = false;
    
    List<string> m_CurrentIngredientsIn = new List<string>();
    int m_Temperature = 0;
    int m_Rotation = -1;

    float m_StartingVolume;
    
    private CauldronEffects m_CauldronEffect;
    // private DateTime _date;

    private void Start()
    {
        m_CauldronEffect = GetComponent<CauldronEffects>();
        splashVFX = SplashEffect.GetComponent<VisualEffect>();

        m_StartingVolume = AmbientSoundSource.volume;
        AmbientSoundSource.volume = m_StartingVolume * 0.2f;
        // _date = DateTime.Now;
    }

    void OnTriggerEnter(Collider other)
    {
        // CauldronIngredient ingredient = other.attachedRigidbody.GetComponentInChildren<CauldronIngredient>();
        
        Vector3 contactPosition = other.attachedRigidbody.gameObject.transform.position;
        contactPosition.y = gameObject.transform.position.y;
        if (!isServer)
            CmdTrigger(contactPosition, other.attachedRigidbody.gameObject);
    }

    [Command(requiresAuthority = false)]
    void CmdTrigger(Vector3 contactPosition, GameObject collide)
    {
        SplashEffect.transform.position = contactPosition;
        
        SFXPlayer.Instance.PlaySFX(SplashClips[Random.Range(0, SplashClips.Length)], contactPosition, new SFXPlayer.PlayParameters()
        {
            Pitch = Random.Range(0.8f, 1.2f),
            SourceID = 17624,
            Volume = 1.0f
        }, 0.2f, true);

        splashVFX.Play();

        if (collide != null && collide.GetComponent<CauldronIngredient>() != null)
        {
            CauldronIngredient ingredient = collide.GetComponent<CauldronIngredient>();

            RespawnableObject respawnableObject = ingredient;
            if (ingredient != null)
            {
                m_CurrentIngredientsIn.Add(ingredient.IngredientType);
                Debug.Log("ingredient " + ingredient.IngredientType + " added to the cauldron");
            }
            else
            {
                //added an object that is not an ingredient, it will make automatically fail any recipe
                m_CurrentIngredientsIn.Add("INVALID");
                respawnableObject = collide.GetComponent<RespawnableObject>();
            }

            if (respawnableObject != null)
            {
                respawnableObject.Respawn();
            }
            else
            {
                Destroy(collide, 0.5f);
            }
        }
    }

    public void ChangeTemperature(int step)
    {
        // Log.Instance.CmdLog("change temperature!");
        CmdChangeTemperature(step);
        m_CauldronEffect.SetBubbleIntensity(step);
    }
    
    [Command(requiresAuthority = false)]
    void CmdChangeTemperature(int step)
    {
        m_Temperature = TemperatureIncrement * step;
        m_CauldronEffect.SetBubbleIntensity(step);
        // Debug.Log("Temperature changed to " + m_Temperature);
    }

    public void ChangeRotation(int step)
    {
        CmdChangeRotation(step);
        m_CauldronEffect.SetRotationSpeed(m_Rotation);
    }
    
    [Command(requiresAuthority = false)]
    void CmdChangeRotation(int step)
    {
        m_Rotation = step - 1;
        m_CauldronEffect.SetRotationSpeed(m_Rotation);
        Debug.Log("Rotation changed to " + m_Rotation);
    }

    public void Brew(Collider other)
    {
        // DateTime currentTime = DateTime.Now;
        // if (!isServer)
        //     Log.Instance.CmdLog(NetworkClient.connection.ToString());
            // Log.Instance.CmdLog("current: " + currentTime + " previous: " + _date + " difference: " + (currentTime - _date).TotalSeconds);

        if (!isServer && other != null)
        {
            Log.Instance.CmdLog(other.name);

            XRRig xrRig = other.GetComponentInParent<XRRig>();
            Log.Instance.CmdLog("is local player: " + xrRig.GetComponent<NetworkIdentity>().isLocalPlayer);
            if (xrRig != null && xrRig.GetComponent<NetworkIdentity>().isLocalPlayer)
                // _date = currentTime;
                CmdBrew();
        }

        // StartCoroutine(SetAnimation());
    }
    
    [Command(requiresAuthority = false)]
    void CmdBrew()
    {
        Debug.Log("brewing!");
        // brewEffect.SendEvent("StartLongSpawn");
        // CauldronAnimator.SetTrigger("Brew");
        
        Recipe recipeBewed = null;
        foreach (Recipe recipe in Recipes)
        {
            if(recipe.temperature != m_Temperature || recipe.rotation != m_Rotation)
                continue;

            List<string> copyOfIngredient = new List<string>(m_CurrentIngredientsIn);
            int ingredientCount = 0;
            foreach (var ing in recipe.ingredients)
            {
                if (copyOfIngredient.Contains(ing))
                {
                    ingredientCount += 1;
                    copyOfIngredient.Remove(ing);
                }
            }

            if (ingredientCount == recipe.ingredients.Length)
            {
                recipeBewed = recipe;
                break;
            }
        }

        ResetCauldron();
        RpcAnimation();

        StartCoroutine(WaitForBrewCoroutine(recipeBewed));
    }

    IEnumerator WaitForBrewCoroutine(Recipe recipe)
    {
        // BrewingSoundSource.Play();
        // AmbientSoundSource.volume = m_StartingVolume * 0.2f;
        m_CanBrew = false;
        yield return new WaitForSeconds(3.0f);
        // brewEffect.SendEvent("EndLongSpawn");
        // CauldronAnimator.SetTrigger("Open");
        // BrewingSoundSource.Stop();
        
        OnBrew.Invoke(recipe);
        m_CanBrew = true;
        // AmbientSoundSource.volume = m_StartingVolume;
    }
    
    [ClientRpc]
    void RpcAnimation()
    {
        StartCoroutine(SetAnimation());
    }
    
    IEnumerator SetAnimation()
    {
        brewEffect.SendEvent("StartLongSpawn");
        CauldronAnimator.SetTrigger("Brew");
        BrewingSoundSource.Play();
        AmbientSoundSource.volume = m_StartingVolume * 0.2f;
        yield return new WaitForSeconds(3.0f);
        brewEffect.SendEvent("EndLongSpawn");
        CauldronAnimator.SetTrigger("Open");
        BrewingSoundSource.Stop();
        AmbientSoundSource.volume = m_StartingVolume;
    }

    void ResetCauldron()
    {
        m_CurrentIngredientsIn.Clear();        

    }

    public void Open()
    {
        CauldronAnimator.SetTrigger("Open");
        m_CanBrew = true;
        AmbientSoundSource.volume = m_StartingVolume;
    }
}
