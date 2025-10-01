using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedMonoBehaviour.Scripts.Interfaces;
using AdvancedMonoBehaviour.Scripts.Patterns;
using GiveawaySystems.Scripts.DeveloperSettings;
using UnityEngine;

public class GiveawayManager : AdvancedSingletonPresent<GiveawayManager>, OnStart
{
    // Handle Giveaway, based on score, what's left, dashboard leave data here and on PlayerPrefs.
    public List<Giveaway> Gifts = new List<Giveaway>();
    public Giveaway LosingGiveaway;
   
    public int FreeCounter;
    public int ThisFreeRound;
    public SettingsInt RoundCounter;
    public SettingsInt WinningRoundCounter;
    public int WinningCounter;
    
    public void Test()
    {
        Debug.Log(GetGiveaway().Name);
    }
    
    public virtual Giveaway GetGiveaway()
    {

        if (PlayerPrefs.GetInt("ForceGift", -1) != -1)
        {
            var s = PlayerPrefs.GetInt("ForceGift");
            PlayerPrefs.SetInt("ForceGift", -1);
            if (Gifts.First(x => x.ID == s))
            {
                return Gifts.First(x => x.ID == s);
            }
            else
            {
                
            }
        }
        
        Giveaway Gift;
        do
        {
            Gift = Gifts[Random.Range(0, Gifts.Count)];
        } while (!Gift.CounsmeQuantity(1, true));
        return Gift;
    }
    
    public virtual Giveaway GetRandomGiveaway()
    {
        
        Giveaway Gift;
        Gift = Gifts[Random.Range(0, Gifts.Count)];
        return Gift;
    }
    
    public virtual Giveaway GetGiveawayForSegment(int segmentIndex)
    {
        // Map segment index to a specific giveaway
        // This allows you to assign specific giveaways to specific wheel segments
        if (Gifts == null || Gifts.Count == 0)
        {
            Debug.LogError("No gifts available!");
            return null;
        }
        
        // Simple mapping: segment index maps to gift index
        // You can modify this logic to have more complex mappings
        int giftIndex = segmentIndex % Gifts.Count;
        
        // Check if the selected gift has quantity available
        Giveaway selectedGift = Gifts[giftIndex];
        if (selectedGift.CounsmeQuantity(1, true)) // Check only, don't consume yet
        {
            return selectedGift;
        }
        
        // If the mapped gift is not available, try to find any available gift
        return GetGiveaway();
    }

    public bool HaveQuantity()
    {
        return Gifts.Any(x => x.CounsmeQuantity(1, true));
    }

    protected virtual void StartNewFreeRound()
    {
        ThisFreeRound = RoundCounter.GET();
        FreeCounter = 0;
        WinningCounter = 0;
    }

    public void OnStart()
    {
        foreach (var gift in Gifts)
        {
            gift.Load();
        }
    }
}
