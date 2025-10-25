using UnityEngine;

namespace GiveawaySystems.Scripts
{
    [CreateAssetMenu(fileName = "GiveawayCategory", menuName = "GiveawaySystem/Category", order = 2)]
    public class GiveawayCategory : ScriptableObject
    {
        public string categoryName;
        public Color categoryColor = Color.white;
        public Sprite categoryIcon;
        public int sortOrder;
        public string description;
        
        // Optional: Category-specific constraints
        public bool useCustomConstraints;
        public ImageConstraints customConstraints;
    }
}
