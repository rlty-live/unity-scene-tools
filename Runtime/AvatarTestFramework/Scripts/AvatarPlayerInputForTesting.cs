using UnityEngine.InputSystem;
//using LocalHandlerData;
//using Player;
using UnityEngine;

#if UNITY_EDITOR
namespace RLTY.Customisation.Testing
{
    public class AvatarPlayerInputForTesting : MonoBehaviour
    {
        #region Fields

        public bool enableJump = true;

        #endregion

        #region Properties

        public Vector2 Move
        {
            get
            {
                return _move;
            }
        }
        private Vector2 _move;

        public bool IsMoveX { get { return Mathf.Abs(Move.x) > 0; } }
        public bool IsMoveY { get { return Mathf.Abs(Move.y) > 0; } }

        public bool Sprint { get { return _sprint; } }
        private bool _sprint;

        public bool Jump { get; set; }
        private bool _jumpPressed;

        private PlayerInput _inputSystem;
        private bool _started = false;

        #endregion

        #region UnityLoop

        private void Start()
        {
            //we assign this in start, not awake, on purpose, because we don't want to trigger ActivateInput on the first enable
            _inputSystem = GetComponent<PlayerInput>();
            UIManagerHandlerData.OnPlayerInputEnabled += UIManagerHandlerData_OnPlayerInputEnabled;
        }

        private void UIManagerHandlerData_OnPlayerInputEnabled(bool state)
        {
            enabled = state;
        }

        // Update is called once per frame
        void Update()
        {
            //inputs to use on desktop
            Jump = enableJump && _jumpPressed;
            if (Jump)
                _jumpPressed = false;
        }

        private void OnDestroy()
        {
            UIManagerHandlerData.OnPlayerInputEnabled -= UIManagerHandlerData_OnPlayerInputEnabled;
        }

        private void OnEnable()
        {
            if (_inputSystem != null)
                _inputSystem.ActivateInput();
        }

        private void OnDisable()
        {
            if (_inputSystem != null)
                _inputSystem.DeactivateInput();
            _move = Vector2.zero;
            Jump = false;
            _sprint = false;
            _jumpPressed = false;
        }

        #endregion

        #region New Input System

        private void OnMove(InputValue value)
        {
            _move = value.Get<Vector2>();
        }

        private void OnJump(InputValue value)
        {
            _jumpPressed = value.isPressed;
        }

        private void OnSprint(InputValue value)
        {
            _sprint = value.isPressed;
        }

        private void OnEmote(InputValue value)
        {
            /*
            if (value.isPressed)
                RadialEmotesAnimationManagerHandlerData.EmoteRadialMenuOpenRequest();
            else
                RadialMenuManagerHandlerData.RadialMenuCloseRequest(() =>
                {

                });*/
        }


        #endregion
    }
}
#endif