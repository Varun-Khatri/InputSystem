using System;
using UnityEngine;

namespace VK.Input
{
    public class TouchTest : MonoBehaviour
    {

        [SerializeField] private InputHandler _inputHandler;

        void OnEnable()
        {
            _inputHandler.OnTouchPressed += OnTouchPressed;
            _inputHandler.OnTouchReleased += OnTouchReleased;
            _inputHandler.OnDrag += OnDrag;
            _inputHandler.OnSwipe += OnSwipe;
        }


        void OnDisable()
        {
            _inputHandler.OnTouchPressed -= OnTouchPressed;
            _inputHandler.OnTouchReleased -= OnTouchReleased;
            _inputHandler.OnDrag -= OnDrag;
            _inputHandler.OnSwipe -= OnSwipe;
        }

        private void OnSwipe(Vector2 direction)
        {
            Debug.Log($"OnSwipe, Direction: {direction}");
        }

        private void OnDrag(Vector2 currentPosition, Vector2 delta)
        {
            Debug.Log($"OnDrag, Current Position: {currentPosition} Delta: {delta}");
        }

        private void OnTouchReleased(Vector2 position)
        {
            Debug.Log($"OnTouchReleased, Position: {position}");
        }

        private void OnTouchPressed(Vector2 position)
        {
            Debug.Log($"OnTouchPressed, Position: {position}");
        }



    }
}
