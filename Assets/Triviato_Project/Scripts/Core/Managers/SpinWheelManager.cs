using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class SpinWheelManager : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private Transform wheelTransform;
    [SerializeField] private float initialSpinSpeed = 720f; // Initial rotation speed in degrees per second
    [SerializeField] private float friction = 0.95f; // Friction coefficient (0-1, higher = more friction)
    [SerializeField] private float minSpeedThreshold = 0.1f; // Minimum speed before stopping
    
    [Header("Wheel Segments")]
    [SerializeField] private int numberOfSegments = 8;
    [SerializeField] private float segmentAngle = 45f; // 360 / numberOfSegments
    
    [Header("Winner Detection")]
    [SerializeField] private Transform arrowTransform; // The upward-facing arrow that determines the winner
    
    public Action<int> OnSpinComplete;
    public Action OnSpinStarted;
    
    private bool isSpinning = false;
    private int selectedSegment = -1;
    private float currentRotationSpeed = 0f;
    private float currentRotation = 0f;
    
    public bool IsSpinning => isSpinning;
    
    public void StartSpin()
    {
        if (isSpinning) return;
        
        isSpinning = true;
        currentRotation = wheelTransform.eulerAngles.z;
        currentRotationSpeed = initialSpinSpeed + Random.Range(-100f, 100f); // Add some randomness
        OnSpinStarted?.Invoke();
        StartCoroutine(SpinWheel());
    }
    
    private IEnumerator SpinWheel()
    {
        while (isSpinning && Mathf.Abs(currentRotationSpeed) > minSpeedThreshold)
        {
            // Apply friction to slow down the wheel naturally
            currentRotationSpeed *= friction;
            
            // Update rotation based on current speed
            currentRotation += currentRotationSpeed * Time.deltaTime;
            
            // Normalize rotation to keep it within 0-360 range
            currentRotation = currentRotation % 360f;
            if (currentRotation < 0) currentRotation += 360f;
            
            // Apply rotation to the wheel
            wheelTransform.rotation = Quaternion.Euler(0, 0, currentRotation);
            
            yield return null;
        }
        
        // Wheel has stopped naturally
        isSpinning = false;
        
        // Calculate which segment was selected based on final position
        selectedSegment = CalculateSelectedSegment(currentRotation);
        
        OnSpinComplete?.Invoke(selectedSegment);
    }
    
    
    private int CalculateSelectedSegment(float finalRotation)
    {
        // Normalize the angle to be between 0 and 360 degrees
        float normalizedAngle = finalRotation % 360f;
        if (normalizedAngle < 0) normalizedAngle += 360f;
        
        // Calculate which segment the angle falls into
        // Each segment represents a range: segment 0 = 0-45°, segment 1 = 45-90°, etc.
        int segment = Mathf.FloorToInt(normalizedAngle / segmentAngle);
        
        // Ensure segment is within valid range
        segment = segment % numberOfSegments;
        
        return segment;
    }
    
    public void ResetWheel()
    {
        if (wheelTransform != null)
        {
            wheelTransform.rotation = Quaternion.identity;
        }
        selectedSegment = -1;
        isSpinning = false;
        currentRotation = 0f;
        currentRotationSpeed = 0f;
    }
    
    public Giveaway GetGiveawayForSegment(int segmentIndex)
    {
        // Use the GiveawayManager to get the appropriate giveaway for this segment
        if (GiveawayManager.Instance != null)
        {
            return GiveawayManager.Instance.GetGiveawayForSegment(segmentIndex);
        }
        
        Debug.LogError("GiveawayManager instance not found!");
        return null;
    }
    
    public Giveaway GetLastSelectedGiveaway()
    {
        return GetGiveawayForSegment(selectedSegment);
    }
    
    public int GetLastSelectedSegment()
    {
        return selectedSegment;
    }
}
