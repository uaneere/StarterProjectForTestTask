using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class MageController : MonoBehaviour
    {
        public float Speed = 4.2f;
        public bool CanMove = true;
        public Vector2 MoveInput { get; private set; }

        private Rigidbody2D _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            MoveInput = CanMove ? ReadInput() : Vector2.zero;
            if (!CanMove)
                StopMovement();
        }

        private void FixedUpdate()
        {
            if (_body == null)
                return;

            _body.linearVelocity = CanMove ? MoveInput * Speed : Vector2.zero;
        }

        public void StopMovement()
        {
            MoveInput = Vector2.zero;
            if (_body != null)
                _body.linearVelocity = Vector2.zero;
        }

        private static Vector2 ReadInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return Vector2.zero;

            var input = Vector2.zero;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                input.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                input.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                input.x += 1f;

            if (input.sqrMagnitude > 1f)
                input.Normalize();

            return input;
        }
    }
}