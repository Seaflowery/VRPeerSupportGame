using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Potion will check if it have a PotionReceiver under it when it's poured, and OnPotionPoured event will be called if
/// it does.
/// </summary>
public class PotionReceiver : NetworkBehaviour
{
    private bool correctPoured = false;

    [System.Serializable]
    public class PotionPouredEvent : UnityEvent<string> { }

    public string[] AcceptedPotionType;
    
    public PotionPouredEvent OnPotionPoured;

    public void ReceivePotion(string PotionType)
    {
        if(AcceptedPotionType.Contains(PotionType) && !correctPoured)
        {
            OnPotionPoured.Invoke(PotionType);
            CmdReceivePotion(PotionType);
            correctPoured = true;
        }                      
    }
    
    [Command(requiresAuthority = false)]
    void CmdReceivePotion(string PotionType)
    {
        OnPotionPoured.Invoke(PotionType);
        RpcReceivePotion(PotionType);
    }
    
    [ClientRpc]
    void RpcReceivePotion(string PotionType)
    {
        OnPotionPoured.Invoke(PotionType);
    }
}
