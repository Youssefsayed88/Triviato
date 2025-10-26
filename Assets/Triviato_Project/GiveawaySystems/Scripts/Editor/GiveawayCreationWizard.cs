using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace GiveawaySystems.Scripts.Editor
{
    public class GiveawayCreationWizard : ScriptableWizard
    {
        [Header("Basic Settings")]
        public string giftBaseName = "Gift";
        public int startingID = 0;
        public int numberOfGifts = 1;
        public GiveawayCategory category;
        public int defaultQuantity = 10;
        public int defaultPriority = 0;
        public bool isActive = true;

        [Header("Batch Image Settings")]
        public string imageDirectory = "";
        public bool useImageFilenameAsGiftName = true;
        public bool validateImageConstraints = true;

        [Header("Target")]
        public GiveawayManager targetManager;
        
        [MenuItem("Tools/Giveaway System/Create Gifts Wizard")]
        static void CreateWizard()
        {
            DisplayWizard<GiveawayCreationWizard>("Create Gifts", "Create", "Create & Add More");
        }

        void OnWizardCreate()
        {
            CreateGifts(false);
        }

        void OnWizardOtherButton()
        {
            CreateGifts(true);
        }

        void OnWizardUpdate()
        {
            helpString = "Create multiple gifts at once with optional batch image processing.";
            errorString = ValidateInputs();
            isValid = string.IsNullOrEmpty(errorString);
        }

        private string ValidateInputs()
        {
            if (string.IsNullOrEmpty(giftBaseName))
                return "Gift base name cannot be empty";
            
            if (numberOfGifts <= 0)
                return "Number of gifts must be greater than 0";
            
            if (targetManager == null)
                return "Target GiveawayManager is required";
            
            if (!string.IsNullOrEmpty(imageDirectory) && !Directory.Exists(imageDirectory))
                return "Image directory does not exist";

            return null;
        }

        private void CreateGifts(bool stayOpen)
        {
            var constraintManager = FindObjectOfType<ImageConstraintManager>();
            List<string> imagePaths = new List<string>();

            // Get image files if directory is specified
            if (!string.IsNullOrEmpty(imageDirectory))
            {
                imagePaths.AddRange(Directory.GetFiles(imageDirectory, "*.png"));
                imagePaths.AddRange(Directory.GetFiles(imageDirectory, "*.jpg"));
                imagePaths.AddRange(Directory.GetFiles(imageDirectory, "*.jpeg"));
            }

            for (int i = 0; i < numberOfGifts; i++)
            {
                string giftName = $"{giftBaseName}_{(startingID + i)}";
                string path = EditorUtility.SaveFilePanelInProject(
                    "Save Gift",
                    giftName,
                    "asset",
                    "Choose where to save the gift asset"
                );

                if (string.IsNullOrEmpty(path)) continue;

                var gift = CreateInstance<Giveaway>();
                gift.ID = startingID + i;
                gift.Name = giftName;
                gift.category = category;
                gift.Quantity = defaultQuantity;
                gift.priority = defaultPriority;
                gift.isActive = isActive;

                // Handle image if available
                if (imagePaths.Count > i)
                {
                    if (useImageFilenameAsGiftName)
                    {
                        gift.Name = Path.GetFileNameWithoutExtension(imagePaths[i]);
                    }

                    if (validateImageConstraints && constraintManager != null)
                    {
                        gift.LoadPNG(imagePaths[i], constraintManager);
                    }
                    else
                    {
                        gift.LoadPNG(imagePaths[i]);
                    }
                }

                AssetDatabase.CreateAsset(gift, path);
                targetManager.Gifts.Add(gift);
            }

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(targetManager);

            if (!stayOpen)
            {
                Close();
            }
            else
            {
                // Update starting ID for next batch
                startingID += numberOfGifts;
            }
        }
    }
}
