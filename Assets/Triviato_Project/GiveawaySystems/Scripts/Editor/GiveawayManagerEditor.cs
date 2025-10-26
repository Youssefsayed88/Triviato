using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace GiveawaySystems.Scripts.Editor
{
    [CustomEditor(typeof(GiveawayManager))]
    public class GiveawayManagerEditor : UnityEditor.Editor
    {
        private GiveawayManager giveawayManager;
        private CategoryManager categoryManager;
        private bool showGiftList = true;
        private bool showBulkOperations = true;
        private Vector2 giftListScrollPosition;
        private Dictionary<int, bool> giftFoldouts = new Dictionary<int, bool>();

        private void OnEnable()
        {
            giveawayManager = (GiveawayManager)target;
            categoryManager = giveawayManager.GetComponent<CategoryManager>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();
            EditorGUILayout.Space(10);

            DrawGiftManagement();
            EditorGUILayout.Space(10);

            DrawBulkOperations();
            EditorGUILayout.Space(10);

            DrawStatistics();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGiftManagement()
        {
            showGiftList = EditorGUILayout.Foldout(showGiftList, "Gift Management", true);
            if (!showGiftList) return;

            EditorGUI.indentLevel++;

            // Filter options
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sort by Category"))
            {
                SortGiftsByCategory();
            }
            if (GUILayout.Button("Sort by Priority"))
            {
                SortGiftsByPriority();
            }
            EditorGUILayout.EndHorizontal();

            // Gift list
            giftListScrollPosition = EditorGUILayout.BeginScrollView(giftListScrollPosition, GUILayout.Height(300));
            
            for (int i = 0; i < giveawayManager.Gifts.Count; i++)
            {
                var gift = giveawayManager.Gifts[i];
                if (gift == null) continue;

                if (!giftFoldouts.ContainsKey(i))
                {
                    giftFoldouts[i] = false;
                }

                EditorGUILayout.BeginHorizontal();
                giftFoldouts[i] = EditorGUILayout.Foldout(giftFoldouts[i], gift.Name, true);
                
                GUI.enabled = i > 0;
                if (GUILayout.Button("↑", GUILayout.Width(25)))
                {
                    MoveGiftUp(i);
                }
                GUI.enabled = i < giveawayManager.Gifts.Count - 1;
                if (GUILayout.Button("↓", GUILayout.Width(25)))
                {
                    MoveGiftDown(i);
                }
                GUI.enabled = true;
                
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    Selection.activeObject = gift;
                }
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    if (EditorUtility.DisplayDialog("Remove Gift", 
                        $"Are you sure you want to remove {gift.Name}?", "Yes", "No"))
                    {
                        RemoveGift(i);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (giftFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Category:", gift.category ? gift.category.categoryName : "None");
                    EditorGUILayout.LabelField("Priority:", gift.priority.ToString());
                    EditorGUILayout.LabelField("Quantity:", gift.Quantity.ToString());
                    EditorGUILayout.LabelField("Active:", gift.isActive.ToString());
                    EditorGUI.indentLevel--;
                }
            }
            
            EditorGUILayout.EndScrollView();

            // Add gift button
            if (GUILayout.Button("Add New Gift"))
            {
                CreateNewGift();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawBulkOperations()
        {
            showBulkOperations = EditorGUILayout.Foldout(showBulkOperations, "Bulk Operations", true);
            if (!showBulkOperations) return;

            EditorGUI.indentLevel++;

            if (categoryManager != null && categoryManager.categories.Count > 0)
            {
                if (GUILayout.Button("Assign Categories to Uncategorized"))
                {
                    AssignCategoriesToUncategorized();
                }
            }

            if (GUILayout.Button("Reset All Quantities"))
            {
                if (EditorUtility.DisplayDialog("Reset Quantities", 
                    "Are you sure you want to reset all gift quantities to their default values?", 
                    "Yes", "No"))
                {
                    ResetAllQuantities();
                }
            }

            if (GUILayout.Button("Validate All Images"))
            {
                ValidateAllImages();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawStatistics()
        {
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            int totalGifts = giveawayManager.Gifts.Count;
            int activeGifts = giveawayManager.Gifts.Count(g => g.isActive);
            int totalQuantity = giveawayManager.Gifts.Sum(g => g.Quantity);

            EditorGUILayout.LabelField("Total Gifts:", totalGifts.ToString());
            EditorGUILayout.LabelField("Active Gifts:", activeGifts.ToString());
            EditorGUILayout.LabelField("Total Quantity:", totalQuantity.ToString());

            if (categoryManager != null)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("By Category:", EditorStyles.boldLabel);
                foreach (var category in categoryManager.categories)
                {
                    int categoryCount = giveawayManager.Gifts.Count(g => g.category == category);
                    EditorGUILayout.LabelField($"{category.categoryName}:", categoryCount.ToString());
                }
            }

            EditorGUI.indentLevel--;
        }

        private void CreateNewGift()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Gift",
                "New Gift",
                "asset",
                "Choose where to save the new gift asset"
            );

            if (string.IsNullOrEmpty(path)) return;

            var gift = CreateInstance<Giveaway>();
            gift.Name = "New Gift";
            gift.ID = GetNextAvailableID();
            gift.isActive = true;

            AssetDatabase.CreateAsset(gift, path);
            AssetDatabase.SaveAssets();

            giveawayManager.Gifts.Add(gift);
            EditorUtility.SetDirty(giveawayManager);
        }

        private int GetNextAvailableID()
        {
            if (giveawayManager.Gifts.Count == 0) return 0;
            return giveawayManager.Gifts.Max(g => g.ID) + 1;
        }

        private void MoveGiftUp(int index)
        {
            if (index <= 0 || index >= giveawayManager.Gifts.Count) return;
            
            var gift = giveawayManager.Gifts[index];
            giveawayManager.Gifts.RemoveAt(index);
            giveawayManager.Gifts.Insert(index - 1, gift);
            EditorUtility.SetDirty(giveawayManager);
        }

        private void MoveGiftDown(int index)
        {
            if (index < 0 || index >= giveawayManager.Gifts.Count - 1) return;
            
            var gift = giveawayManager.Gifts[index];
            giveawayManager.Gifts.RemoveAt(index);
            giveawayManager.Gifts.Insert(index + 1, gift);
            EditorUtility.SetDirty(giveawayManager);
        }

        private void RemoveGift(int index)
        {
            if (index < 0 || index >= giveawayManager.Gifts.Count) return;
            
            giveawayManager.Gifts.RemoveAt(index);
            EditorUtility.SetDirty(giveawayManager);
        }

        private void SortGiftsByCategory()
        {
            if (categoryManager == null) return;
            
            categoryManager.SortGiftsByCategory();
            EditorUtility.SetDirty(giveawayManager);
        }

        private void SortGiftsByPriority()
        {
            giveawayManager.Gifts = giveawayManager.Gifts
                .OrderByDescending(g => g.priority)
                .ThenBy(g => g.Name)
                .ToList();
            EditorUtility.SetDirty(giveawayManager);
        }

        private void AssignCategoriesToUncategorized()
        {
            var uncategorized = giveawayManager.Gifts.Where(g => g.category == null).ToList();
            if (uncategorized.Count == 0) return;

            foreach (var gift in uncategorized)
            {
                // Assign to first category by default
                gift.category = categoryManager.categories[0];
                EditorUtility.SetDirty(gift);
            }
            
            EditorUtility.SetDirty(giveawayManager);
        }

        private void ResetAllQuantities()
        {
            foreach (var gift in giveawayManager.Gifts)
            {
                if (gift.disableChangeQuantity) continue;
                
                gift.Quantity = 0;
                EditorUtility.SetDirty(gift);
            }
            
            EditorUtility.SetDirty(giveawayManager);
        }

        private void ValidateAllImages()
        {
            var constraintManager = FindObjectOfType<ImageConstraintManager>();
            if (constraintManager == null)
            {
                EditorUtility.DisplayDialog("Validation Error",
                    "No ImageConstraintManager found in the scene. Please add one first.",
                    "OK");
                return;
            }

            int invalidCount = 0;
            foreach (var gift in giveawayManager.Gifts)
            {
                if (gift.sprite == null) continue;
                
                if (!constraintManager.ValidateImage(gift.sprite.texture))
                {
                    invalidCount++;
                    Debug.LogWarning($"Gift '{gift.Name}' has invalid image dimensions");
                }
            }

            EditorUtility.DisplayDialog("Validation Complete",
                invalidCount == 0 
                    ? "All images meet the constraints!"
                    : $"Found {invalidCount} gifts with invalid images. Check the console for details.",
                "OK");
        }
    }
}
