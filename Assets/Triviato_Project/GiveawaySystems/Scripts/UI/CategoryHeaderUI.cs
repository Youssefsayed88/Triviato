using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GiveawaySystems.Scripts.UI
{
    public class CategoryHeaderUI : MonoBehaviour
    {
        public TMP_Text categoryName;
        public TMP_Text giftCount;
        public Image categoryIcon;
        public Image backgroundImage;
        
        [Header("Style Settings")]
        public float backgroundOpacity = 0.1f;
        public bool useCustomFont = false;
        public TMP_FontAsset customFont;

        public void Initialize(GiveawayCategory category, int itemCount)
        {
            if (category == null) return;

            if (categoryName != null)
            {
                categoryName.text = category.categoryName;
                categoryName.color = category.categoryColor;
                
                if (useCustomFont && customFont != null)
                    categoryName.font = customFont;
            }

            if (giftCount != null)
                giftCount.text = $"({itemCount} items)";

            if (categoryIcon != null && category.categoryIcon != null)
            {
                categoryIcon.sprite = category.categoryIcon;
                categoryIcon.color = category.categoryColor;
            }

            if (backgroundImage != null)
            {
                var color = category.categoryColor;
                color.a = backgroundOpacity;
                backgroundImage.color = color;
            }
        }
    }
}
