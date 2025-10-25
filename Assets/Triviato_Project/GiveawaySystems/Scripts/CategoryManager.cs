using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GiveawaySystems.Scripts
{
    public class CategoryManager : MonoBehaviour
    {
        public List<GiveawayCategory> categories = new List<GiveawayCategory>();
        private GiveawayManager giveawayManager;

        private void Start()
        {
            giveawayManager = GetComponent<GiveawayManager>();
            if (giveawayManager == null)
            {
                giveawayManager = FindObjectOfType<GiveawayManager>();
            }
        }

        public List<Giveaway> GetGiftsByCategory(GiveawayCategory category)
        {
            if (category == null || giveawayManager == null) return new List<Giveaway>();
            
            return giveawayManager.Gifts
                .Where(g => g.isActive && g.category == category)
                .OrderByDescending(g => g.priority)
                .ThenBy(g => g.Name)
                .ToList();
        }

        public void SortGiftsByCategory()
        {
            if (giveawayManager == null) return;
            
            giveawayManager.Gifts = giveawayManager.Gifts
                .OrderBy(g => g.category?.sortOrder ?? int.MaxValue)
                .ThenByDescending(g => g.priority)
                .ThenBy(g => g.Name)
                .ToList();
        }

        public List<Giveaway> FilterGiftsByCategory(GiveawayCategory category)
        {
            if (category == null || giveawayManager == null)
                return giveawayManager?.Gifts ?? new List<Giveaway>();

            return GetGiftsByCategory(category);
        }

        public List<Giveaway> GetActiveGifts()
        {
            if (giveawayManager == null) return new List<Giveaway>();
            
            return giveawayManager.Gifts
                .Where(g => g.isActive)
                .OrderBy(g => g.category?.sortOrder ?? int.MaxValue)
                .ThenByDescending(g => g.priority)
                .ThenBy(g => g.Name)
                .ToList();
        }

        public Dictionary<GiveawayCategory, List<Giveaway>> GetGiftsByCategories()
        {
            var result = new Dictionary<GiveawayCategory, List<Giveaway>>();
            
            foreach (var category in categories)
            {
                result[category] = GetGiftsByCategory(category);
            }

            // Handle uncategorized gifts
            var uncategorized = giveawayManager?.Gifts
                .Where(g => g.isActive && g.category == null)
                .OrderByDescending(g => g.priority)
                .ThenBy(g => g.Name)
                .ToList() ?? new List<Giveaway>();

            if (uncategorized.Any())
            {
                result[null] = uncategorized;
            }

            return result;
        }
    }
}
