using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using AdvancedMonoBehaviour.Scripts.Patterns; // Assuming this contains AdvancedMonoSingletonPresent

public static class JTokenExtensions
{
    public static T GetValueOrDefault<T>(this JToken token, T defaultValue = default)
    {
        if (token == null || token.Type == JTokenType.Null)
            return defaultValue;
        
        return token.Value<T>();
    }
}

[DefaultExecutionOrder(-999)]
public class Config : AdvancedMonoSingletonPresent<Config>
{
    public JObject settings;

    public override void Awake()
    {
        base.Awake();
        string configLocation = Path.Combine(Application.streamingAssetsPath, "config.json");

        if (!File.Exists(configLocation))
        {
            Debug.LogWarning($"Config file not found at {configLocation}. Using empty JObject as fallback.");
            settings = new JObject();
            return;
        }

        string json = File.ReadAllText(configLocation);
        settings = JObject.Parse(json);

        // Example usage:
        string apiUrl = settings["API_URL"].GetValueOrDefault("ws://localhost:4455");
        int timerValue = settings["Timer"].GetValueOrDefault(30);

        Debug.Log($"Loaded settings from config.json: API_URL={apiUrl}, Timer={timerValue}");
    }
}