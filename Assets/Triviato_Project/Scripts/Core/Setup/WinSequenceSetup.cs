using UnityEngine;

/// <summary>
/// This script helps set up the win sequence integration.
/// Attach this to a GameObject in your scene to automatically configure the win sequence.
/// </summary>
public class WinSequenceSetup : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WinStateManager winStateManager;
    [SerializeField] private SpinWheelManager spinWheelManager;
    [SerializeField] private GiveawayManager giveawayManager;
    [SerializeField] private ScoreDisplayManager scoreDisplayManager;
    [SerializeField] private WinSequenceUIController uiController;
    
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupWinSequence();
        }
    }
    
    [ContextMenu("Setup Win Sequence")]
    public void SetupWinSequence()
    {
        Debug.Log("Setting up Win Sequence integration...");
        
        // Find components if not assigned
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
            
        if (winStateManager == null)
            winStateManager = FindObjectOfType<WinStateManager>();
            
        if (spinWheelManager == null)
            spinWheelManager = FindObjectOfType<SpinWheelManager>();
            
        if (giveawayManager == null)
            giveawayManager = FindObjectOfType<GiveawayManager>();
            
        if (scoreDisplayManager == null)
            scoreDisplayManager = FindObjectOfType<ScoreDisplayManager>();
            
        if (uiController == null)
            uiController = FindObjectOfType<WinSequenceUIController>();
        
        // Validate setup
        ValidateSetup();
        
        Debug.Log("Win Sequence setup complete!");
    }
    
    private void ValidateSetup()
    {
        bool hasErrors = false;
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Please assign it in the inspector.");
            hasErrors = true;
        }
        
        if (winStateManager == null)
        {
            Debug.LogError("WinStateManager not found! Please create one and assign it.");
            hasErrors = true;
        }
        
        if (spinWheelManager == null)
        {
            Debug.LogError("SpinWheelManager not found! Please create one and assign it.");
            hasErrors = true;
        }
        
        if (giveawayManager == null)
        {
            Debug.LogError("GiveawayManager not found! Please create one and assign it.");
            hasErrors = true;
        }
        
        if (scoreDisplayManager == null)
        {
            Debug.LogWarning("ScoreDisplayManager not found! Score display will not be available when SpinWheelManager is null.");
        }
        
        if (uiController == null)
        {
            Debug.LogWarning("WinSequenceUIController not found! UI will not be displayed.");
        }
        
        if (!hasErrors)
        {
            Debug.Log("✓ All required components found!");
        }
    }
    
    [ContextMenu("Test Win Sequence")]
    public void TestWinSequence()
    {
        if (winStateManager != null)
        {
            Debug.Log("Testing win sequence...");
            winStateManager.StartWinSequence();
        }
        else
        {
            Debug.LogError("WinStateManager not found! Cannot test win sequence.");
        }
    }
    
    [ContextMenu("Reset Win Sequence")]
    public void ResetWinSequence()
    {
        if (winStateManager != null)
        {
            winStateManager.ResetWinSequence();
        }
        
        if (uiController != null)
        {
            uiController.ResetWinSequence();
        }
        
        Debug.Log("Win sequence reset!");
    }
}
