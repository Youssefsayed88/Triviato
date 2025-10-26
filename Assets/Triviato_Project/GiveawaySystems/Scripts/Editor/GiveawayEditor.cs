using UnityEngine;
using UnityEditor;
using System.IO;

namespace GiveawaySystems.Scripts.Editor
{
    [CustomEditor(typeof(Giveaway))]
    public class GiveawayEditor : UnityEditor.Editor
    {
        private Giveaway giveaway;
        private ImageConstraintManager constraintManager;
        private bool showImageSettings = true;
        private bool showQuantitySettings = true;
        private bool showCategorySettings = true;
        private bool showAdditionalSettings = true;

        private void OnEnable()
        {
            giveaway = (Giveaway)target;
            // Try to find ImageConstraintManager in the scene
            constraintManager = FindObjectOfType<ImageConstraintManager>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            DrawBasicInformation();
            
            EditorGUILayout.Space(10);
            DrawImageSettings();
            
            EditorGUILayout.Space(10);
            DrawQuantitySettings();
            
            EditorGUILayout.Space(10);
            DrawCategorySettings();
            
            EditorGUILayout.Space(10);
            DrawAdditionalSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBasicInformation()
        {
            EditorGUILayout.LabelField("Basic Information", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isActive"));

            EditorGUI.indentLevel--;
        }

        private void DrawImageSettings()
        {
            showImageSettings = EditorGUILayout.Foldout(showImageSettings, "Image Settings", true);
            if (showImageSettings)
            {
                EditorGUI.indentLevel++;

                // Image preview
                if (giveaway.sprite != null)
                {
                    Rect previewRect = GUILayoutUtility.GetRect(100, 100);
                    EditorGUI.DrawPreviewTexture(previewRect, giveaway.sprite.texture);
                    
                    EditorGUILayout.LabelField($"Size: {giveaway.sprite.texture.width}x{giveaway.sprite.texture.height}");
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"));

                // Image loading buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Load Image"))
                {
                    string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");
                    if (!string.IsNullOrEmpty(path))
                    {
                        LoadImage(path);
                    }
                }

                if (GUILayout.Button("Clear Image"))
                {
                    giveaway.sprite = null;
                    EditorUtility.SetDirty(giveaway);
                }
                EditorGUILayout.EndHorizontal();

                // Constraint validation
                if (constraintManager != null && giveaway.sprite != null)
                {
                    bool isValid = constraintManager.ValidateImage(giveaway.sprite.texture);
                    EditorGUILayout.HelpBox(
                        isValid ? "Image meets size constraints" : "Image does not meet size constraints",
                        isValid ? MessageType.Info : MessageType.Warning
                    );
                }
                else if (constraintManager == null)
                {
                    EditorGUILayout.HelpBox(
                        "No ImageConstraintManager found in scene. Image constraints will not be validated.",
                        MessageType.Warning
                    );
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawQuantitySettings()
        {
            showQuantitySettings = EditorGUILayout.Foldout(showQuantitySettings, "Quantity Settings", true);
            if (showQuantitySettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Quantity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("GiveawayWeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("disableChangeQuantity"));

                EditorGUI.indentLevel--;
            }
        }

        private void DrawCategorySettings()
        {
            showCategorySettings = EditorGUILayout.Foldout(showCategorySettings, "Category Settings", true);
            if (showCategorySettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("category"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"));

                if (giveaway.category != null)
                {
                    EditorGUILayout.LabelField("Category Info:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Name:", giveaway.category.categoryName);
                    EditorGUILayout.LabelField("Sort Order:", giveaway.category.sortOrder.ToString());
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawAdditionalSettings()
        {
            showAdditionalSettings = EditorGUILayout.Foldout(showAdditionalSettings, "Additional Settings", true);
            if (showAdditionalSettings)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("setting"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isSettings"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isStringSettings"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("disableChangeName"));

                EditorGUI.indentLevel--;
            }
        }

        private void LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (constraintManager != null)
            {
                if (giveaway.LoadPNG(path, constraintManager))
                {
                    EditorUtility.SetDirty(giveaway);
                }
            }
            else
            {
                if (giveaway.LoadPNG(path))
                {
                    EditorUtility.SetDirty(giveaway);
                }
            }
        }
    }
}
