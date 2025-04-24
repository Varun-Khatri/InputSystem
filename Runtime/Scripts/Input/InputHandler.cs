using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VK.Input
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Touch Settings")]
        [SerializeField] private float _swipeMinDistance = 50f;
        [SerializeField] private float _swipeMaxDuration = 0.3f;
        private PlayerInputActions _actions;
        private Vector2 _movementInput = new();
        private Vector2 _touchStartPosition = new();
        private Vector2 _currentDragPosition = new();
        private Vector2 _previousDragPosition = new();
        private float _touchStartTime = 0f;
        private bool _holdingJump = false;
        private bool _holdingDash = false;
        private bool _holdingCrouch = false;
        private bool _holdingInteract = false;
        private bool _crouchPressedThisFrame = false;
        private bool _crouchReleasedThisFrame = false;
        private bool _interactPressedThisFrame = false;
        private bool _interactReleasedThisFrame = false;
        private bool _jumpPressedThisFrame = false;
        private bool _jumpReleasedThisFrame = false;
        private bool _dashPressedThisFrame = false;
        private bool _dashReleasedThisFrame = false;
        private bool _holdingTouch = false;
        private bool _touchPressedThisFrame = false;
        private bool _touchReleasedThisFrame = false;
        public Vector2 MovementInput => _movementInput;
        public Vector2 DragDelta => _currentDragPosition - _previousDragPosition;
        public Vector2 DragDirection => (_currentDragPosition - _previousDragPosition).normalized;
        public bool InteractPressedThisFrame => _interactPressedThisFrame;
        public bool InteractReleasedThisFrame => _interactReleasedThisFrame;
        public bool JumpPressedThisFrame => _jumpPressedThisFrame;
        public bool JumpReleasedThisFrame => _jumpReleasedThisFrame;
        public bool DashPressedThisFrame => _dashPressedThisFrame;
        public bool DashReleasedThisFrame => _dashReleasedThisFrame;
        public bool CrouchPressedThisFrame => _crouchPressedThisFrame;
        public bool CrouchReleasedThisFrame => _crouchReleasedThisFrame;
        public bool HoldingCrouch => _holdingCrouch;
        public bool HoldingJump => _holdingJump;
        public bool HoldingDash => _holdingDash;
        public bool HoldingInteract => _holdingInteract;
        public bool HoldingTouch => _holdingTouch;
        public bool TouchPressedThisFrame => _touchPressedThisFrame;
        public bool TouchReleasedThisFrame => _touchReleasedThisFrame;

        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnDashPressed;
        public event Action OnDashReleased;
        public event Action OnCrouchPressed;
        public event Action OnCrouchReleased;
        public event Action OnInteractPressed;
        public event Action OnInteractReleased;
        public event Action<Vector2> OnTouchPressed;  // Fires on initial touch
        public event Action<Vector2> OnTouchReleased; // Fires on release
        public event Action<Vector2> OnTap;
        public event Action<Vector2> OnSwipe;
        public event Action<Vector2, Vector2> OnDrag; // (currentPosition, delta)

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _actions.Enable();
        }

        private void OnEnable()
        {
            _actions.Player.Crouch.performed += context => OnButtonPerformedOrCanceled(ref _holdingCrouch, ref _crouchPressedThisFrame, ref _crouchReleasedThisFrame, OnCrouchPressed, context);
            _actions.Player.Crouch.canceled += context => OnButtonPerformedOrCanceled(ref _holdingCrouch, ref _crouchPressedThisFrame, ref _crouchReleasedThisFrame, OnCrouchReleased, context);
            _actions.Player.Interact.performed += context => OnButtonPerformedOrCanceled(ref _holdingInteract, ref _interactPressedThisFrame, ref _interactReleasedThisFrame, OnInteractPressed, context);
            _actions.Player.Interact.canceled += context => OnButtonPerformedOrCanceled(ref _holdingInteract, ref _interactPressedThisFrame, ref _interactReleasedThisFrame, OnInteractReleased, context);
            _actions.Player.Jump.performed += context => OnButtonPerformedOrCanceled(ref _holdingJump, ref _jumpPressedThisFrame, ref _jumpReleasedThisFrame, OnJumpPressed, context);
            _actions.Player.Jump.canceled += context => OnButtonPerformedOrCanceled(ref _holdingJump, ref _jumpPressedThisFrame, ref _jumpReleasedThisFrame, OnJumpReleased, context);
            _actions.Player.Dash.performed += context => OnButtonPerformedOrCanceled(ref _holdingDash, ref _dashPressedThisFrame, ref _dashReleasedThisFrame, OnDashPressed, context);
            _actions.Player.Dash.canceled += context => OnButtonPerformedOrCanceled(ref _holdingDash, ref _dashPressedThisFrame, ref _dashReleasedThisFrame, OnDashReleased, context);
            _actions.Player.Move.performed += OnMove;
            _actions.Player.Move.canceled += OnMove;
            _actions.Touch.PrimaryContact.performed += context =>
             {
                 OnButtonPerformedOrCanceled(
                    ref _holdingTouch,
                    ref _touchPressedThisFrame,
                    ref _touchReleasedThisFrame,
                    () =>
                    {
                        Vector2 pos = _actions.Touch.PrimaryPosition.ReadValue<Vector2>();

                        // Initialize tracking
                        _touchStartPosition = pos;
                        _currentDragPosition = pos;
                        _previousDragPosition = pos;
                        _touchStartTime = Time.time;

                        OnTouchPressed?.Invoke(pos);
                    },
                    context
                );
             };

            _actions.Touch.PrimaryContact.canceled += context =>
            {
                OnButtonPerformedOrCanceled(
                    ref _holdingTouch,
                    ref _touchPressedThisFrame,
                    ref _touchReleasedThisFrame,
                    () =>
                    {
                        Vector2 pos = _actions.Touch.PrimaryPosition.ReadValue<Vector2>();
                        // Final drag position update
                        _previousDragPosition = _currentDragPosition;
                        _currentDragPosition = pos;

                        // Fire final drag update
                        OnDrag?.Invoke(
                            _currentDragPosition,
                            _currentDragPosition - _previousDragPosition
                        );

                        // Swipe/tap detection
                        float duration = Time.time - _touchStartTime;
                        Vector2 swipeDelta = pos - _touchStartPosition;

                        if (duration <= _swipeMaxDuration)
                        {
                            if (swipeDelta.magnitude >= _swipeMinDistance)
                            {
                                OnSwipe?.Invoke(swipeDelta.normalized);
                            }
                            else
                            {
                                OnTap?.Invoke(pos);
                            }
                        }

                        // Reset drag tracking
                        _currentDragPosition = Vector2.zero;
                        _previousDragPosition = Vector2.zero;
                        _touchStartPosition = Vector2.zero;

                        OnTouchReleased?.Invoke(pos);
                    },
                    context
                );
            };

            _actions.Touch.PrimaryPosition.performed += OnTouchMoved;
        }

        private void OnDisable()
        {
            _actions.Player.Crouch.performed -= context => OnButtonPerformedOrCanceled(ref _holdingCrouch, ref _crouchPressedThisFrame, ref _crouchReleasedThisFrame, OnCrouchPressed, context);
            _actions.Player.Crouch.canceled -= context => OnButtonPerformedOrCanceled(ref _holdingCrouch, ref _crouchPressedThisFrame, ref _crouchReleasedThisFrame, OnCrouchReleased, context);
            _actions.Player.Interact.performed -= context => OnButtonPerformedOrCanceled(ref _holdingInteract, ref _interactPressedThisFrame, ref _interactReleasedThisFrame, OnInteractPressed, context);
            _actions.Player.Interact.canceled -= context => OnButtonPerformedOrCanceled(ref _holdingInteract, ref _interactPressedThisFrame, ref _interactReleasedThisFrame, OnInteractReleased, context);
            _actions.Player.Jump.performed -= context => OnButtonPerformedOrCanceled(ref _holdingJump, ref _jumpPressedThisFrame, ref _jumpReleasedThisFrame, OnJumpPressed, context);
            _actions.Player.Jump.canceled -= context => OnButtonPerformedOrCanceled(ref _holdingJump, ref _jumpPressedThisFrame, ref _jumpReleasedThisFrame, OnJumpReleased, context);
            _actions.Player.Dash.performed -= context => OnButtonPerformedOrCanceled(ref _holdingDash, ref _dashPressedThisFrame, ref _dashReleasedThisFrame, OnDashPressed, context);
            _actions.Player.Dash.canceled -= context => OnButtonPerformedOrCanceled(ref _holdingDash, ref _dashPressedThisFrame, ref _dashReleasedThisFrame, OnDashReleased, context);
            _actions.Player.Move.performed -= OnMove;
            _actions.Player.Move.canceled -= OnMove;
            _actions.Touch.PrimaryContact.performed -= context =>
            {
                OnButtonPerformedOrCanceled(
                    ref _holdingTouch,
                    ref _touchPressedThisFrame,
                    ref _touchReleasedThisFrame,
                    () =>
                    {
                        Vector2 pos = _actions.Touch.PrimaryPosition.ReadValue<Vector2>();

                        // Initialize tracking
                        _touchStartPosition = pos;
                        _currentDragPosition = pos;
                        _previousDragPosition = pos;
                        _touchStartTime = Time.time;

                        OnTouchPressed?.Invoke(pos);
                    },
                    context
                );
            };

            _actions.Touch.PrimaryContact.canceled -= context =>
            {
                OnButtonPerformedOrCanceled(
                    ref _holdingTouch,
                    ref _touchPressedThisFrame,
                    ref _touchReleasedThisFrame,
                    () =>
                    {
                        Vector2 pos = _actions.Touch.PrimaryPosition.ReadValue<Vector2>();
                        // Final drag position update
                        _previousDragPosition = _currentDragPosition;
                        _currentDragPosition = pos;

                        // Fire final drag update
                        OnDrag?.Invoke(
                            _currentDragPosition,
                            _currentDragPosition - _previousDragPosition
                        );

                        // Swipe/tap detection
                        float duration = Time.time - _touchStartTime;
                        Vector2 swipeDelta = pos - _touchStartPosition;

                        if (duration <= _swipeMaxDuration)
                        {
                            if (swipeDelta.magnitude >= _swipeMinDistance)
                            {
                                OnSwipe?.Invoke(swipeDelta.normalized);
                            }
                            else
                            {
                                OnTap?.Invoke(pos);
                            }
                        }

                        // Reset drag tracking
                        _currentDragPosition = Vector2.zero;
                        _previousDragPosition = Vector2.zero;
                        _touchStartPosition = Vector2.zero;

                        OnTouchReleased?.Invoke(pos);
                    },
                    context
                );
            };

            _actions.Touch.PrimaryPosition.performed -= OnTouchMoved;
        }

        private void OnButtonPerformedOrCanceled(ref bool holdingButton, ref bool buttonPressedThisFrame, ref bool buttonReleasedThisFrame, Action action, InputAction.CallbackContext context)
        {
            var val = context.ReadValue<float>();
            buttonPressedThisFrame = val == 1 && !holdingButton;
            buttonReleasedThisFrame = val == 0 && holdingButton;
            holdingButton = val == 1;
            action?.Invoke();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        private void OnTouchMoved(InputAction.CallbackContext context)
        {
            if (!_holdingTouch) return;

            // Update drag positions
            _previousDragPosition = _currentDragPosition;
            _currentDragPosition = context.ReadValue<Vector2>();

            // Fire drag event with position and delta
            OnDrag?.Invoke(
                _currentDragPosition,
                _currentDragPosition - _previousDragPosition
            );
        }


    }
}