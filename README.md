# Input System

A comprehensive Unity input handling system built on the new Input System. Supports both traditional button inputs and advanced touch gestures with a clean, event-driven architecture.

## 📦 Installation & Setup

### Package Structure

```text
Assets/Packages/[Package Name]/
├── Runtime/                 # Core system files
│   ├── [MainSystemFiles].cs
│   └── ...
└── Samples/                 # Sample implementations
    ├── ExampleComponent1.cs
    ├── ExampleComponent2.cs
    └── ExampleScene.unity   (if included)
```

### Installation Methods
**Method 1: Unity Package Manager (Recommended)**

- Open Window → Package Manager
- Click + → Add package from git URL
- Enter your repository URL:

```text
https://github.com/[username]/[repository-name].git
The system will be installed in Assets/Packages/[System Name]/
```

**Method 2: Manual Installation**

- Download the repository or clone it
- Copy the entire package folder to:

```text
Assets/Packages/[System Name]/
The system is ready to use
```

### Accessing Samples

After installation, access samples at Assets/Packages/[System Name]/Samples/

## 🎯 Features

- **Dual Input Support** - Keyboard/Gamepad + Touch controls in one system
- **Gesture Recognition** - Swipe, tap, and drag detection
- **Frame-Perfect Input** - Pressed/Released this frame detection
- **Event-Driven Architecture** - Clean separation between input and game logic
- **Hold State Tracking** - Continuous button hold detection
- **Extensible Design** - Easy to add new input types and gestures

## 🏗️ Architecture

### Core Components

| Component | Description |
|-----------|-------------|
| `InputHandler` | Main input controller handling all input sources |
| `TouchTest` | Example implementation demonstrating touch events |
| `PlayerInputActions` | Generated Input System asset (not included) |

## 📦 Installation

1. **Prerequisites**: Unity 2019.4+ with Input System package
2. **Setup Input Actions**: Create `PlayerInputActions` asset using Input System
3. **Add InputHandler** to your player or game manager GameObject

## 🚀 Quick Start

### Basic Setup

```csharp
using UnityEngine;
using VK.Input;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    
    private void OnEnable()
    {
        _inputHandler.OnJumpPressed += OnJump;
        _inputHandler.OnDashPressed += OnDash;
        _inputHandler.OnMovementInput += OnMove;
    }
    
    private void OnDisable()
    {
        _inputHandler.OnJumpPressed -= OnJump;
        _inputHandler.OnDashPressed -= OnDash;
        _inputHandler.OnMovementInput -= OnMove;
    }
    
    private void OnJump()
    {
        // Handle jump input
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
    }
    
    private void OnDash()
    {
        // Handle dash input
        StartCoroutine(DashRoutine());
    }
    
    private void OnMove(Vector2 direction)
    {
        // Handle movement input
        transform.Translate(direction * Time.deltaTime);
    }
}
```
**Touch Input Example**

```csharp
public class TouchController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    
    private void OnEnable()
    {
        _inputHandler.OnSwipe += OnSwipeDetected;
        _inputHandler.OnTap += OnTapDetected;
        _inputHandler.OnDrag += OnDragDetected;
    }
    
    private void OnDisable()
    {
        _inputHandler.OnSwipe -= OnSwipeDetected;
        _inputHandler.OnTap -= OnTapDetected;
        _inputHandler.OnDrag -= OnDragDetected;
    }
    
    private void OnSwipeDetected(Vector2 direction)
    {
        Debug.Log($"Swipe detected: {direction}");
        
        if (direction.y > 0.5f)
        {
            // Swipe up - jump
            OnJump();
        }
        else if (Mathf.Abs(direction.x) > 0.5f)
        {
            // Swipe left/right - dash
            OnDash(direction.x > 0 ? Vector2.right : Vector2.left);
        }
    }
    
    private void OnTapDetected(Vector2 position)
    {
        Debug.Log($"Tap at: {position}");
        // Handle tap interaction
        TryInteractAtPosition(position);
    }
    
    private void OnDragDetected(Vector2 currentPosition, Vector2 delta)
    {
        // Handle dragging objects or camera pan
        transform.position += (Vector3)delta * 0.01f;
    }
}
```

## 📖 API Reference
### Button Input Events
| Event	| Description |
|-----------|-------------|
| OnJumpPressed / OnJumpReleased	|	Jump button events |
| OnDashPressed / OnDashReleased	|	Dash button events |
| OnCrouchPressed / OnCrouchReleased	|	Crouch button events |
| OnInteractPressed / OnInteractReleased	|	Interaction button events |
### Touch Input Events
| Event	| Description |
|-----------|-------------|
| OnTouchPressed / OnTouchReleased	|	Touch start/end with position |
| OnTap	|	Quick tap gesture |
| OnSwipe	|	Swipe gesture with direction vector |
| OnDrag	|	Continuous drag with position and delta |
### Input State Properties
| Property	|	Description |
|-----------|-------------|
| MovementInput	|	Current movement direction vector |
| JumpPressedThisFrame	|	True if jump was pressed this frame |
| HoldingJump	|	True if jump button is currently held |
| DragDelta	|	Current drag movement delta |
| DragDirection	|	Normalized drag direction |

## 🔧 Configuration
### Input Action Asset Setup
Create a PlayerInputActions asset with these action maps:

**Player Action Map:**

- Move (Vector2) - Movement input
- Jump (Button) - Jump action
- Dash (Button) - Dash action
- Crouch (Button) - Crouch action
- Interact (Button) - Interaction action

**Touch Action Map:**

- PrimaryContact (Button) - Touch start/end
- PrimaryPosition (Vector2) - Touch position

### Inspector Configuration
```csharp
[Header("Touch Settings")]
[SerializeField] private float _swipeMinDistance = 50f;    // Minimum swipe distance in pixels
[SerializeField] private float _swipeMaxDuration = 0.3f;   // Maximum time for swipe recognition
```

### 💡 Usage Patterns
## Frame-Perfect Input Detection
```csharp
private void Update()
{
    if (_inputHandler.JumpPressedThisFrame)
    {
        // Only triggers on the frame jump was pressed
        StartJump();
    }
    
    if (_inputHandler.HoldingJump)
    {
        // Continuous while jump is held
        ChargeJump();
    }
    
    if (_inputHandler.JumpReleasedThisFrame)
    {
        // Only triggers on the frame jump was released
        ReleaseJump();
    }
}
```
### Movement Handling
```csharp
private void FixedUpdate()
{
    Vector2 movement = _inputHandler.MovementInput;
    _rigidbody.AddForce(movement * moveSpeed);
    
    // Or use with character controller
    _characterController.Move(movement * Time.fixedDeltaTime);
}
```

### Gesture-Based Controls
```csharp
private void OnSwipeDetected(Vector2 direction)
{
    // Classify swipe direction
    if (direction.y > 0.7f)
    {
        // Up swipe - jump
        OnJump();
    }
    else if (direction.y < -0.7f)
    {
        // Down swipe - crouch
        OnCrouch();
    }
    else if (Mathf.Abs(direction.x) > 0.7f)
    {
        // Horizontal swipe - dash
        OnDash(direction.x > 0 ? 1 : -1);
    }
}
```

## 🛡️ Best Practices

### 1. Always Subscribe/Unsubscribe
```csharp
private void OnEnable() => SubscribeEvents();
private void OnDisable() => UnsubscribeEvents();

private void SubscribeEvents()
{
    _inputHandler.OnJumpPressed += OnJump;
    _inputHandler.OnDashPressed += OnDash;
}

private void UnsubscribeEvents()
{
    _inputHandler.OnJumpPressed -= OnJump;
    _inputHandler.OnDashPressed -= OnDash;
}
```

### 2. Use Frame-Perfect Properties for Instant Actions
```csharp
// Good - frame perfect
if (_inputHandler.JumpPressedThisFrame)
    StartJump();

// Avoid - might miss quick taps
if (_inputHandler.HoldingJump)
    StartJump();
```

### 3. Separate Input from Logic
```csharp
// Input handler only deals with input
private void OnJumpPressed()
{
    OnJump?.Invoke();
}

// Game logic handles the actual jump
private void PerformJump()
{
    // Jump implementation
}
```

## 🎮 Input Action Reference
### Recommended Bindings
| Action |	Keyboard |	Gamepad |	Touch |
|--------|-----------|----------|-------|
| Move |	WASD/Arrows |	Left Stick	N/A |
| Jump |	Space |	A Button |	Swipe Up |
| Dash |	Left | Shift	X Button |	Swipe Horizontal |
| Crouch |	C |	B Button |	Swipe Down |
| Interact |	E |	Y Button |	Tap |

## 🤝 Contributing
This system is part of my professional portfolio. Feel free to:

- Use in your personal or commercial projects
- Extend with additional input types
- Adapt for your specific control schemes

