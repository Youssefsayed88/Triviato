using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace GiveawaySystems.Scripts
{
    public class EnhancedDashboardManager : MonoBehaviour
    {
        [Header("References")]
        public CategoryManager categoryManager;
        public ImageConstraintManager imageConstraintManager;
        public GiveawayManager giveawayManager;

        [Header("UI Components")]
        public Transform contentContainer;
        public GameObject giftEntityPrefab;
        public GameObject categoryHeaderPrefab;
        public TMP_Dropdown categoryFilter;
        public TMP_InputField searchField;
        public Toggle showInactiveToggle;
        public Button sortByNameButton;
        public Button sortByQuantityButton;
        public Button sortByPriorityButton;
        public Button saveButton;

        [Header("Layout Settings")]
        public float categorySpacing = 20f;
        public float giftSpacing = 10f;

        private List<GiveawayDashboardEntity> activeEntities = new List<GiveawayDashboardEntity>();
        private Dictionary<GiveawayCategory, List<GiveawayDashboardEntity>> categorizedEntities = 
            new Dictionary<GiveawayCategory, List<GiveawayDashboardEntity>>();

        private void Start()
        {
            InitializeComponents();
            SetupEventListeners();
            RefreshDashboard();
        }

        private void InitializeComponents()
        {
            if (giveawayManager == null)
                giveawayManager = FindObjectOfType<GiveawayManager>();
            
            if (categoryManager == null)
                categoryManager = FindObjectOfType<CategoryManager>();
            
            if (imageConstraintManager == null)
                imageConstraintManager = FindObjectOfType<ImageConstraintManager>();

            InitializeCategoryDropdown();
        }

        private void InitializeCategoryDropdown()
        {
            if (categoryFilter == null) return;

            var options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("All Categories") };
            
            if (categoryManager != null)
            {
                options.AddRange(categoryManager.categories
                    .Select(c => new TMP_Dropdown.OptionData(c.categoryName)));
            }

            categoryFilter.options = options;
            categoryFilter.value = 0;
        }

        private void SetupEventListeners()
        {
            if (categoryFilter != null)
                categoryFilter.onValueChanged.AddListener(OnCategoryFilterChanged);

            if (searchField != null)
                searchField.onValueChanged.AddListener(OnSearchTextChanged);

            if (showInactiveToggle != null)
                showInactiveToggle.onValueChanged.AddListener(OnShowInactiveChanged);

            if (sortByNameButton != null)
                sortByNameButton.onClick.AddListener(() => SortGifts(SortType.Name));

            if (sortByQuantityButton != null)
                sortByQuantityButton.onClick.AddListener(() => SortGifts(SortType.Quantity));

            if (sortByPriorityButton != null)
                sortByPriorityButton.onClick.AddListener(() => SortGifts(SortType.Priority));

            if (saveButton != null)
                saveButton.onClick.AddListener(SaveAllChanges);
        }

        private void RefreshDashboard()
        {
            ClearDashboard();

            var filteredGifts = FilterGifts();
            var categorizedGifts = CategorizeGifts(filteredGifts);
            DisplayCategorizedGifts(categorizedGifts);
        }

        private void ClearDashboard()
        {
            foreach (var entity in activeEntities)
            {
                if (entity != null)
                    Destroy(entity.gameObject);
            }

            activeEntities.Clear();
            categorizedEntities.Clear();

            // Clear content container
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private List<Giveaway> FilterGifts()
        {
            var gifts = giveawayManager.Gifts.ToList();
            
            // Filter by active state
            if (!showInactiveToggle.isOn)
                gifts = gifts.Where(g => g.isActive).ToList();

            // Filter by search
            if (!string.IsNullOrEmpty(searchField.text))
            {
                string search = searchField.text.ToLower();
                gifts = gifts.Where(g => 
                    g.Name.ToLower().Contains(search) || 
                    (g.description?.ToLower().Contains(search) ?? false)
                ).ToList();
            }

            // Filter by category
            if (categoryFilter.value > 0 && categoryManager != null)
            {
                var selectedCategory = categoryManager.categories[categoryFilter.value - 1];
                gifts = gifts.Where(g => g.category == selectedCategory).ToList();
            }

            return gifts;
        }

        private Dictionary<GiveawayCategory, List<Giveaway>> CategorizeGifts(List<Giveaway> gifts)
        {
            var result = new Dictionary<GiveawayCategory, List<Giveaway>>();

            foreach (var gift in gifts)
            {
                var category = gift.category;
                if (!result.ContainsKey(category))
                    result[category] = new List<Giveaway>();
                
                result[category].Add(gift);
            }

            return result;
        }

        private void DisplayCategorizedGifts(Dictionary<GiveawayCategory, List<Giveaway>> categorizedGifts)
        {
            foreach (var categoryGroup in categorizedGifts)
            {
                // Create category header
                if (categoryHeaderPrefab != null && categoryGroup.Key != null)
                {
                    var header = Instantiate(categoryHeaderPrefab, contentContainer);
                    var headerText = header.GetComponentInChildren<TMP_Text>();
                    if (headerText != null)
                    {
                        headerText.text = categoryGroup.Key.categoryName;
                        headerText.color = categoryGroup.Key.categoryColor;
                    }
                }

                // Create gift entities
                foreach (var gift in categoryGroup.Value)
                {
                    CreateGiftEntity(gift);
                }

                // Add spacing after category
                var spacer = new GameObject("CategorySpacer");
                spacer.transform.SetParent(contentContainer);
                var layoutElement = spacer.AddComponent<LayoutElement>();
                layoutElement.minHeight = categorySpacing;
            }
        }

        private void CreateGiftEntity(Giveaway gift)
        {
            var entityObj = Instantiate(giftEntityPrefab, contentContainer);
            var entity = entityObj.GetComponent<GiveawayDashboardEntity>();
            
            if (entity != null)
            {
                entity.Load(gift);
                activeEntities.Add(entity);

                var category = gift.category;
                if (!categorizedEntities.ContainsKey(category))
                    categorizedEntities[category] = new List<GiveawayDashboardEntity>();
                
                categorizedEntities[category].Add(entity);
            }
        }

        private void OnCategoryFilterChanged(int value)
        {
            RefreshDashboard();
        }

        private void OnSearchTextChanged(string value)
        {
            RefreshDashboard();
        }

        private void OnShowInactiveChanged(bool value)
        {
            RefreshDashboard();
        }

        private enum SortType { Name, Quantity, Priority }

        private void SortGifts(SortType sortType)
        {
            foreach (var categoryGroup in categorizedEntities)
            {
                switch (sortType)
                {
                    case SortType.Name:
                        categoryGroup.Value.Sort((a, b) => 
                            string.Compare(a.myGiveaway.Name, b.myGiveaway.Name));
                        break;
                    case SortType.Quantity:
                        categoryGroup.Value.Sort((a, b) => 
                            b.myGiveaway.Quantity.CompareTo(a.myGiveaway.Quantity));
                        break;
                    case SortType.Priority:
                        categoryGroup.Value.Sort((a, b) => 
                            b.myGiveaway.priority.CompareTo(a.myGiveaway.priority));
                        break;
                }

                // Reorder in hierarchy
                foreach (var entity in categoryGroup.Value)
                {
                    entity.transform.SetAsLastSibling();
                }
            }
        }

        private void SaveAllChanges()
        {
            foreach (var entity in activeEntities)
            {
                if (entity != null)
                    entity.Save();
            }
        }
    }
}
