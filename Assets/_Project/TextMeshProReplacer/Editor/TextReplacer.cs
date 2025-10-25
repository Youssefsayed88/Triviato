
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTLTMPro;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TextMeshProReplacer
{
    internal class TextReplacer
    {
        [MenuItem("Text Mesh Replacer/Replace Current Scene")]
        internal static void ReplaceCurrentScene()
        {

            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            //Debug.Log(rootGameObjects.Length);
            // for (int i = 0; i < rootGameObjects.Length; i++)
            // {
            //     GameObject root = rootGameObjects[i];
            //     for (int j = 0; j < root.transform.childCount; j++)
            //     {
            //         TextMeshProUGUI text = root.transform.GetChild(j).GetComponent<TextMeshProUGUI>();
            //         if (text)
            //             ReplaceUnityText(text);
            //     }
            // }

            DumpScene();
        }
        
        public static void DumpScene()
        {
            var gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            
                foreach (GameObject gameObject in gameObjects)
                {
                    DumpGameObject(gameObject);
                }
        }

        private static void DumpGameObject(GameObject gameObject)
        {
            //Debug.Log(gameObject.name);
            TextMeshProUGUI text = gameObject.GetComponent<TextMeshProUGUI>();
            if (text)
                ReplaceUnityText(text);

            foreach (Transform child in gameObject.transform)
            {
                DumpGameObject(child.gameObject);
            }
        }

        [MenuItem("Text Mesh Replacer/Replace All Scene")]
        internal static void ReplaceAllScene()
        {
            SceneAsset[] scenes = FindAssets<SceneAsset>();
            for (int i = 0; i < scenes.Length; i++)
            {
                SceneAsset scene = scenes[i];
                EditorUtility.DisplayProgressBar("Replacing in scene...", scene.name, (float)i / scenes.Length);
                Scene loadedScene=EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene),OpenSceneMode.Single);
                while (!loadedScene.isLoaded)
                {
                    //wait
                }
               
                ReplaceCurrentScene();

                EditorSceneManager.SaveScene(loadedScene);
            }
            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Text Mesh Replacer/Test/Test Canvas")]
        internal static void GenerateDemoCanvas()
        {
            //generate new canvas
            GameObject newCanvas=new GameObject("TestCanvas");
            Canvas canvas=newCanvas.AddComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            
            //find all fonts
            Font[] allFonts = FindAssets<Font>();
            //generate texts inside canvas
            StringBuilder stringBuilder=new StringBuilder();

            for (int i = 0; i < 100; i++)
            {
                Text testText=new GameObject("test text").AddComponent<Text>();
                testText.transform.SetParent(newCanvas.transform,false);
                testText.rectTransform.anchoredPosition=new Vector2(Random.Range(-300,300), Random.Range(-300, 300));
                if (allFonts.Length > 0)
                    testText.font = allFonts[Random.Range(0, allFonts.Length)];
                //generate random string
                int stringLength = Random.Range(5, 20);
                stringBuilder.Length = 0;
                while (stringLength>0)
                {
                    stringBuilder.Append(char.ConvertFromUtf32(Random.Range(48, 123)));
                    stringLength--;
                }
                testText.text = stringBuilder.ToString();

            }
            
            EditorUtility.SetDirty(canvas);

            ReplaceUnityText(canvas.GetComponentsInChildren<TextMeshProUGUI>());
        }
        internal static void ReplaceUnityText(TextMeshProUGUI[] unityTexts)
        {
            TMP_FontAsset[] fonts = FindAssets<TMP_FontAsset>();
            if(fonts.Length==0)
                return;
            List<TMPro.TMP_FontAsset> missingFonts=new List<TMP_FontAsset>();
            for (int i = 0; i < unityTexts.Length; i++)
            {
                TextMeshProUGUI text = unityTexts[i];
                
                if (!missingFonts.Contains(text.font))
                {
                    TMP_FontAsset font = GetTMPFont(text.font, fonts);
                    if (font != null)
                        ReplaceUnityText(text, font);
                    else
                        missingFonts.Add(text.font);
                }
            }
            //log missing fonts if any
            if (missingFonts.Count==0)
                return;
            //Debug.LogWarningFormat("Missing {0} fonts",missingFonts.Count);
            for (int i = 0; i < missingFonts.Count; i++)
            {
                //Debug.LogWarningFormat("Text Mesh pro Font {0} is missing", missingFonts[i].name);
            }
        }

        internal static void ReplaceUnityText(TextMeshProUGUI unityText)
        {
            TMP_FontAsset[] fonts = FindAssets<TMP_FontAsset>();
            if (fonts.Length == 0)
                return;
            TMP_FontAsset font = GetTMPFont(unityText.font, fonts);
            if (font != null)
                ReplaceUnityText(unityText, font);
            else
                Debug.LogWarningFormat("Text Mesh pro Font {0} is missing",unityText.font.name);
        }
        private static void ReplaceUnityText(TextMeshProUGUI unityText,TMP_FontAsset font)
        {
            TextMeshProUGUI currentText = unityText;
            //check if tmpro text already exist, if not dont replace
            
            //for some reason adding tmpro component messed up the rect transform size
            //this will fix it
            Vector2 size = currentText.rectTransform.sizeDelta;

            TextData textData = new TextData();
            GameObject obj = unityText.gameObject;
            textData.write(currentText,font);

            //Selection.activeObject = obj;
            Undo.DestroyObjectImmediate(currentText);

            
            RTLTextMeshPro tmproText = Undo.AddComponent<RTLTMPro.RTLTextMeshPro>(obj);
            tmproText.autoSizeTextContainer = false;
            textData.read(tmproText);
            tmproText.rectTransform.sizeDelta = size;
  
                
            EditorUtility.SetDirty(tmproText);
        }
        private static T[] FindAssets<T>() where T : Object
        {
            List<T> result = new List<T>();
            string[] assetList = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
            for (int i = 0; i < assetList.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetList[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    result.Add(asset);
                }
            }
            return result.ToArray();
        }

        private static TMP_FontAsset GetTMPFont(TMP_FontAsset m_font, TMP_FontAsset[] fonts)
        {
            return m_font;
        }
    }
    internal struct TextData
    {
        private string text;
        private TextAlignmentOptions anchor;
        private TMP_FontAsset tmProfont;
        private Color color;
        private float fontSize;
        private FontStyles fontStyle;
        private bool autoResize;
        private Vector2 minMaxSize;
        public void write(TextMeshProUGUI textObject,TMP_FontAsset tmpFont)
        {

            color = textObject.color;
            fontSize = textObject.fontSize;
            fontStyle = GetFontStyle(textObject.fontStyle);
            tmProfont = tmpFont;
            anchor = GetTextAlignment(textObject.alignment);
            autoResize = textObject.enableAutoSizing;
            minMaxSize = new Vector2(textObject.minHeight, textObject.minWidth);
            text = textObject.text;
            //Debug.Log(text);
        }

        public void read(RTLTextMeshPro textObject)
        {
            textObject.text = text;
            textObject.color = color;
            textObject.fontSize = fontSize;
            textObject.fontStyle = fontStyle;

            textObject.alignment = anchor;

            textObject.enableAutoSizing = autoResize;
            textObject.fontSizeMin = minMaxSize.x;
            textObject.fontSizeMax = minMaxSize.y;

            if (tmProfont != null)
                textObject.font = tmProfont;

        }
        private FontStyles GetFontStyle(FontStyles style)
        {
            return style;
        }
        private TextAlignmentOptions GetTextAlignment(TextAlignmentOptions m_anchor)
        {
            return m_anchor;
        }

    }
}
