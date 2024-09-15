using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VK.Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerInputActions _actions;
        private Vector2 _movementInput = new();
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
        public Vector2 MovementInput => _movementInput;
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
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnDashPressed;
        public event Action OnDashReleased;
        public event Action OnCrouchPressed;
        public event Action OnCrouchReleased;
        public event Action OnInteractPressed;
        public event Action OnInteractReleased;

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

    }
}