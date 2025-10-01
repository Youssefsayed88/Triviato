# Win Sequence Integration

This integration adds a spin wheel and giveaway system that triggers when the player wins the trivia game.

## Flow
1. Player completes trivia quiz
2. If player wins → Game state changes to "Win"
3. **Spin Wheel** starts automatically
4. After wheel stops → **Giveaway system** activates
5. Player receives their prize

## Components Added

### 1. SpinWheelManager
- Handles wheel spinning animation
- Calculates which segment the wheel lands on
- Configurable spin duration and speed

### 2. WinStateManager
- Coordinates between spin wheel and giveaway systems
- Manages the win sequence flow
- Handles timing between systems

### 3. WinSequenceUIController
- Manages UI for the win sequence
- Shows spin wheel interface
- Displays giveaway results
- Handles user interactions

### 4. GameManager (Updated)
- Added `OnWinStateEntered` event
- Automatically triggers win sequence when state changes to Win
- Integrated with existing game flow

## Setup Instructions

### 1. Create GameObjects
Create the following GameObjects in your scene:

```
WinSequenceManager (Empty GameObject)
├── SpinWheelManager (Script: SpinWheelManager)
├── WinStateManager (Script: WinStateManager)
└── WinSequenceUI (Canvas)
    ├── WinSequenceUIController (Script: WinSequenceUIController)
    ├── SpinWheelPanel
    └── GiveawayPanel
```

### 2. Configure SpinWheelManager
- Assign the wheel Transform (the actual wheel object that rotates)
- Set spin duration (default: 3 seconds)
- Set number of segments (default: 8)
- Configure spin speed and curve

### 3. Configure WinStateManager
- Assign SpinWheelManager reference
- Assign GiveawayManager reference
- Set delay between spin and giveaway (default: 1 second)

### 4. Configure WinSequenceUIController
- Assign UI panel references
- Assign button references
- Assign text and image references
- Assign manager references

### 5. Update GameManager
- Assign WinStateManager reference in the inspector
- The GameManager will automatically trigger the win sequence

## Usage

The integration works automatically:

1. **No code changes needed** in existing quiz system
2. When `GameManager.ShowResult(true)` is called, the win sequence starts
3. The sequence runs: Spin Wheel → Giveaway → Complete

## Testing

Use the `WinSequenceSetup` script to:
- Auto-setup components
- Test the win sequence
- Reset the sequence
- Validate the setup

## Customization

### Wheel Segments
- Modify `numberOfSegments` in SpinWheelManager
- Adjust `segmentAngle` calculation
- Update UI to match segment count

### Timing
- Change `spinDuration` in SpinWheelManager
- Adjust `delayBetweenSpinAndGiveaway` in WinStateManager

### UI
- Customize panels, buttons, and text in WinSequenceUIController
- Add animations and effects as needed

## Events

### SpinWheelManager Events
- `OnSpinStarted` - When wheel starts spinning
- `OnSpinComplete(int segment)` - When wheel stops (segment 0-based)

### WinStateManager Events
- `OnGiveawaySelected(Giveaway)` - When giveaway is selected
- `OnWinSequenceComplete` - When entire sequence finishes

### GameManager Events
- `OnWinStateEntered` - When game state changes to Win

## Troubleshooting

### Common Issues
1. **Win sequence doesn't start**
   - Check if WinStateManager is assigned to GameManager
   - Verify GameManager is calling ShowResult(true) for wins

2. **Spin wheel doesn't spin**
   - Check if wheel Transform is assigned to SpinWheelManager
   - Verify SpinWheelManager is assigned to WinStateManager

3. **Giveaway not working**
   - Check if GiveawayManager is assigned to WinStateManager
   - Verify giveaways have quantity > 0
   - Check GiveawayManager.HaveQuantity() returns true

4. **UI not showing**
   - Check if WinSequenceUIController is assigned to GameManager
   - Verify UI panels are assigned in WinSequenceUIController

### Debug Commands
Use the WinSequenceSetup script context menu:
- "Setup Win Sequence" - Auto-configure components
- "Test Win Sequence" - Test the sequence manually
- "Reset Win Sequence" - Reset everything
