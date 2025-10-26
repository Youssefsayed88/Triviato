using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace GiveawaySystems.Scripts.UI
{
    public class CategoryFilterUI : MonoBehaviour
    {
        [Header("UI Components")]
        public TMP_Dropdown categoryDropdown;
        public TMP_InputField searchInput;
        public Toggle showInactiveToggle;
        public Transform filterTagContainer;
        public GameObject filterTagPrefab;

        [Header("Sort Buttons")]
        public Button sortNameButton;
        public Button sortQuantityButton;
        public Button sortPriorityButton;
        public Button clearFiltersButton;

        [Header("Style")]
        public Color activeFilterColor = new Color(0.2f, 0.6f, 1f);
        public Color inactiveFilterColor = new Color(0.7f, 0.7f, 0.7f);

        private List<GameObject> activeTags = new List<GameObject>();
        private System.Action<FilterSettings> onFilterChanged;

        public void Initialize(CategoryManager categoryManager, System.Action<FilterSettings> filterCallback)
        {
            onFilterChanged = filterCallback;
            SetupCategoryDropdown(categoryManager);
            SetupEventListeners();
        }

        private void SetupCategoryDropdown(CategoryManager categoryManager)
        {
            if (categoryDropdown == null || categoryManager == null) return;

            var options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("All Categories")
            };

            options.AddRange(categoryManager.categories
                .OrderBy(c => c.sortOrder)
                .Select(c => new TMP_Dropdown.OptionData(c.categoryName)));

            categoryDropdown.options = options;
            categoryDropdown.value = 0;
        }

        private void SetupEventListeners()
        {
            if (categoryDropdown != null)
                categoryDropdown.onValueChanged.AddListener(_ => UpdateFilters());

            if (searchInput != null)
                searchInput.onValueChanged.AddListener(_ => UpdateFilters());

            if (showInactiveToggle != null)
                showInactiveToggle.onValueChanged.AddListener(_ => UpdateFilters());

            if (sortNameButton != null)
                sortNameButton.onClick.AddListener(() => UpdateSort(SortType.Name));

            if (sortQuantityButton != null)
                sortQuantityButton.onClick.AddListener(() => UpdateSort(SortType.Quantity));

            if (sortPriorityButton != null)
                sortPriorityButton.onClick.AddListener(() => UpdateSort(SortType.Priority));

            if (clearFiltersButton != null)
                clearFiltersButton.onClick.AddListener(ClearFilters);
        }

        private void UpdateFilters()
        {
            UpdateFilterTags();
            NotifyFilterChanged();
        }

        private void UpdateFilterTags()
        {
            ClearFilterTags();

            // Add category tag
            if (categoryDropdown.value > 0)
            {
                AddFilterTag($"Category: {categoryDropdown.options[categoryDropdown.value].text}");
            }

            // Add search tag
            if (!string.IsNullOrEmpty(searchInput.text))
            {
                AddFilterTag($"Search: {searchInput.text}");
            }

            // Add inactive tag
            if (showInactiveToggle.isOn)
            {
                AddFilterTag("Show Inactive");
            }
        }

        private void AddFilterTag(string text)
        {
            if (filterTagPrefab == null || filterTagContainer == null) return;

            var tag = Instantiate(filterTagPrefab, filterTagContainer);
            var tagText = tag.GetComponentInChildren<TMP_Text>();
            if (tagText != null)
                tagText.text = text;

            activeTags.Add(tag);
        }

        private void ClearFilterTags()
        {
            foreach (var tag in activeTags)
            {
                if (tag != null)
                    Destroy(tag);
            }
            activeTags.Clear();
        }

        private void ClearFilters()
        {
            categoryDropdown.value = 0;
            searchInput.text = "";
            showInactiveToggle.isOn = false;
            UpdateFilters();
        }

        private void UpdateSort(SortType sortType)
        {
            // Update button visuals
            UpdateSortButtonState(sortNameButton, sortType == SortType.Name);
            UpdateSortButtonState(sortQuantityButton, sortType == SortType.Quantity);
            UpdateSortButtonState(sortPriorityButton, sortType == SortType.Priority);

            NotifyFilterChanged();
        }

        private void UpdateSortButtonState(Button button, bool isActive)
        {
            if (button == null) return;
            
            var colors = button.colors;
            colors.normalColor = isActive ? activeFilterColor : inactiveFilterColor;
            button.colors = colors;
        }

        private void NotifyFilterChanged()
        {
            var settings = new FilterSettings
            {
                CategoryIndex = categoryDropdown.value,
                SearchText = searchInput.text,
                ShowInactive = showInactiveToggle.isOn,
                CurrentSort = GetCurrentSortType()
            };

            onFilterChanged?.Invoke(settings);
        }

        private SortType GetCurrentSortType()
        {
            if (sortNameButton != null && sortNameButton.colors.normalColor == activeFilterColor)
                return SortType.Name;
            if (sortQuantityButton != null && sortQuantityButton.colors.normalColor == activeFilterColor)
                return SortType.Quantity;
            if (sortPriorityButton != null && sortPriorityButton.colors.normalColor == activeFilterColor)
                return SortType.Priority;
            
            return SortType.Name; // Default sort
        }
    }

    public enum SortType
    {
        Name,
        Quantity,
        Priority
    }

    public class FilterSettings
    {
        public int CategoryIndex { get; set; }
        public string SearchText { get; set; }
        public bool ShowInactive { get; set; }
        public SortType CurrentSort { get; set; }
    }
}
