using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WinSequenceUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject winSequencePanel;
    [SerializeField] private GameObject spinWheelPanel;
    [SerializeField] private GameObject giveawayPanel;
    [SerializeField] private Button spinButton;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button giveawayButton; // New button to open the gift
    
    [Header("Text Elements")]
    [SerializeField] private TMP_Text spinResultText;
    [SerializeField] private TMP_Text giveawayResultText;
    [SerializeField] private Image giveawayImage;
    
    [Header("Managers")]
    [SerializeField] private WinStateManager winStateManager;
    [SerializeField] private SpinWheelManager spinWheelManager;
    [SerializeField] private ScoreDisplayManager scoreDisplayManager;
    
    private Giveaway currentGiveaway;
    
    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void InitializeUI()
    {
        // Hide all panels initially
        if (winSequencePanel != null) winSequencePanel.SetActive(false);
        if (spinWheelPanel != null) spinWheelPanel.SetActive(false);
        if (giveawayPanel != null) giveawayPanel.SetActive(false);
        
        // Setup button listeners
        if (spinButton != null) spinButton.onClick.AddListener(OnSpinButtonClicked);
        if (claimButton != null) claimButton.onClick.AddListener(OnClaimButtonClicked);
        if (giveawayButton != null) giveawayButton.onClick.AddListener(OnGiveawayButtonClicked);
    }
    
    private void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWinStateEntered += OnWinStateEntered;
            GameManager.Instance.OnLoseStateEntered += OnLoseStateEntered;
        }
        
        if (winStateManager != null)
        {
            winStateManager.OnGiveawaySelected += OnGiveawaySelected;
            winStateManager.OnWinSequenceComplete += OnWinSequenceComplete;
            winStateManager.OnWheelStopped += OnWheelStopped;
        }
        
        if (spinWheelManager != null)
        {
            spinWheelManager.OnSpinStarted += OnSpinStarted;
            spinWheelManager.OnSpinComplete += OnSpinComplete;
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.OnScoreDisplayComplete += OnScoreDisplayComplete;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWinStateEntered -= OnWinStateEntered;
            GameManager.Instance.OnLoseStateEntered -= OnLoseStateEntered;
        }
        
        if (winStateManager != null)
        {
            winStateManager.OnGiveawaySelected -= OnGiveawaySelected;
            winStateManager.OnWinSequenceComplete -= OnWinSequenceComplete;
            winStateManager.OnWheelStopped -= OnWheelStopped;
        }
        
        if (spinWheelManager != null)
        {
            spinWheelManager.OnSpinStarted -= OnSpinStarted;
            spinWheelManager.OnSpinComplete -= OnSpinComplete;
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.OnScoreDisplayComplete -= OnScoreDisplayComplete;
        }
    }
    
    private void OnWinStateEntered()
    {
        ShowWinSequenceUI();
    }
    
    private void OnLoseStateEntered()
    {
        ShowLoseSequenceUI();
    }
    
    private void ShowWinSequenceUI()
    {
        if (winSequencePanel != null)
        {
            winSequencePanel.SetActive(true);
        }
        
        // Check if spin wheel manager exists
        if (spinWheelManager != null)
        {
            // Show spin wheel panel
            if (spinWheelPanel != null)
            {
                spinWheelPanel.SetActive(true);
            }
            
            if (spinResultText != null)
            {
                spinResultText.text = "Congratulations! You won! Spin the wheel to see your prize!";
            }
        }
        else
        {
            // No spin wheel manager, show winning panel but score display will be shown by WinStateManager
            if (spinWheelPanel != null)
            {
                spinWheelPanel.SetActive(false);
            }
            
            if (spinResultText != null)
            {
                spinResultText.text = "Congratulations! You won!";
            }
        }
    }
    
    private void ShowLoseSequenceUI()
    {
        if (winSequencePanel != null)
        {
            winSequencePanel.SetActive(true);
        }
        
        if (spinWheelPanel != null)
        {
            spinWheelPanel.SetActive(false);
        }
        
        if (giveawayPanel != null)
        {
            giveawayPanel.SetActive(false);
        }
        
        // The score display will be shown by the WinStateManager if there's a score
    }
    
    
    private void OnSpinButtonClicked()
    {
        if (spinWheelManager != null && !spinWheelManager.IsSpinning)
        {
            spinWheelManager.StartSpin();
        }
    }
    
    private void OnSpinStarted()
    {
        if (spinResultText != null)
        {
            spinResultText.text = "Spinning...";
        }
    }
    
    private void OnSpinComplete(int selectedSegment)
    {
        if (spinResultText != null)
        {
            spinResultText.text = $"Landed on segment {selectedSegment + 1}!";
        }
    }
    
    private void OnWheelStopped(int selectedSegment)
    {
        if (selectedSegment == -1)
        {
            // Score display case - no wheel was spun
            if (spinResultText != null)
            {
                spinResultText.text = "Click the button to open your gift!";
            }
        }
        else
        {
            // Normal wheel case
            if (spinResultText != null)
            {
                spinResultText.text = $"Wheel stopped at segment {selectedSegment + 1}! Click the button to open your gift!";
            }
        }
        
        // Show the giveaway panel (which contains the button)
        if (giveawayPanel != null)
        {
            giveawayPanel.SetActive(true);
        }
        
        // Hide the spin wheel panel
        if (spinWheelPanel != null)
        {
            spinWheelPanel.SetActive(false);
        }
    }
    
    private void OnGiveawayButtonClicked()
    {
        // Trigger the giveaway process
        if (winStateManager != null)
        {
            winStateManager.TriggerGiveaway();
        }
    }
    
    private void OnGiveawaySelected(Giveaway giveaway)
    {
        currentGiveaway = giveaway;
        ShowGiveawayResult(giveaway);
    }
    
    private void ShowGiveawayResult(Giveaway giveaway)
    {
        if (giveaway == null)
        {
            Debug.LogError("Giveaway is null! Cannot display giveaway result.");
            return;
        }
        
        if (giveawayPanel != null)
        {
            giveawayPanel.SetActive(true);
        }
        
        if (spinWheelPanel != null)
        {
            spinWheelPanel.SetActive(false);
        }
        
        // Display giveaway name
        if (giveawayResultText != null)
        {
            string giveawayName = !string.IsNullOrEmpty(giveaway.Name) ? giveaway.Name : "Unknown Prize";
            giveawayResultText.text = $"You won: {giveawayName}!";
            Debug.Log($"Displaying giveaway: {giveawayName}");
        }
        else
        {
            Debug.LogWarning("Giveaway result text component is not assigned!");
        }
        
        // Display giveaway image
        if (giveawayImage != null)
        {
            if (giveaway.sprite != null)
            {
                giveawayImage.sprite = giveaway.sprite;
                Debug.Log($"Displaying giveaway image: {giveaway.sprite.name}");
            }
            else
            {
                Debug.LogWarning($"Giveaway '{giveaway.Name}' has no sprite assigned!");
                // Optionally hide the image or show a default image
                giveawayImage.sprite = null;
            }
        }
        else
        {
            Debug.LogWarning("Giveaway image component is not assigned!");
        }
    }
    
    private void OnClaimButtonClicked()
    {
        // Handle claiming the giveaway
        if (currentGiveaway != null)
        {
            Debug.Log($"Player claimed: {currentGiveaway.Name}");
            // Here you can add logic to actually give the prize to the player
            // For example, reduce quantity, save to player inventory, etc.
        }
        
        // Hide the win sequence UI
        HideWinSequenceUI();
    }
    
    private void OnWinSequenceComplete()
    {
        Debug.Log("Win sequence completed!");
    }
    
    private void HideWinSequenceUI()
    {
        if (winSequencePanel != null) winSequencePanel.SetActive(false);
        if (spinWheelPanel != null) spinWheelPanel.SetActive(false);
        if (giveawayPanel != null) giveawayPanel.SetActive(false);
    }
    
    private void OnScoreDisplayComplete()
    {
        Debug.Log("Score display completed!");
        // The score display will automatically proceed to giveaway
    }
    
    public void ResetWinSequence()
    {
        HideWinSequenceUI();
        currentGiveaway = null;
        
        if (winStateManager != null)
        {
            winStateManager.ResetWinSequence();
        }
        
        if (scoreDisplayManager != null)
        {
            scoreDisplayManager.ResetScoreDisplay();
        }
    }
}
