//#define FISHNET_SUPPORT
using UnityEngine;
using UnityEngine.AI;
#if FISHNET_SUPPORT
using FishNet.Component.Animating;
#endif
using Judiva.Metaverse.Interactions;
using RLTY.UI;

#if UNITY_EDITOR
namespace RLTY.Customisation.Testing
{
    /// <summary>
    /// Handle input controls
    /// - keys to move
    /// - mouse drag to rotate view
    /// - mouse click to go to
    /// - on mobile, virtual sticks to move and rotate view
    /// </summary>
    /// 
    public class PlayerControllerForTesting : MonoBehaviour
    {
        #region Fields

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // player

        private float _animationBlend;

        private float _targetRotation = 0.0f;

        [SerializeField]
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
#if FISHNET_SUPPORT
        private NetworkAnimator _networkAnimator;
#endif
        private CharacterController _characterController;
        private AvatarPlayerInputForTesting _playerInput;

        private bool _hasAnimator;

        public float stickRotationDampening = 1f;
        private Camera _view;
        private NavMeshAgent _agent;

        public LayerMask clickMask;

        public GameObject destinationGizmo;

        public bool controlAllDirectionsWithStick = true;

        private bool _forceGoTo = false;
        private float _forceWalkForward = 0;
        private float _forceWalkForwardDirection;
        private Vector3 _debugClickPosition;
        private Vector3 _additionalSpeed;

        public enum CtrlMode
        {
            None,
            Full,
            WalkForward
        }

        #endregion

        #region Properties

        public Vector3 Destination { get { return _destinationGizmo.transform.position; } }
        private GameObject _destinationGizmo;

        public float TargetVelocity { get { return _targetSpeed; } }
        [SerializeField]
        private float _targetSpeed = 0;

        public Vector3 TargetDirection { get { return _speed == 0 || _agent.enabled ? transform.forward : _targetDirection; } }
        private Vector3 _targetDirection = Vector3.forward;

        public float RotationVelocity { get { return _agent.enabled ? 0 : _rotationVelocity; } }
        private float _rotationVelocity;

        public float Speed { get { return _speed; } }
        [SerializeField]
        private float _speed = 0;

        //we use this for camera placement
        public float NormalizedSpeed
        {
            get
            {
                return Mathf.Clamp01(_speed / Mathf.Max(_targetSpeed, 1));
            }
        }

        public CtrlMode ControlMode
        {
            get
            {
                return _controlMode;
            }
            set
            {
                if (_controlMode == value)
                    return;
                _controlMode = value;
                _agent.enabled = false;
                switch (_controlMode)
                {
                    case CtrlMode.None:
                        _playerInput.enabled = false;
                        break;
                    case CtrlMode.WalkForward:
                        _playerInput.enabled = true;
                        break;
                    case CtrlMode.Full:
                        _playerInput.enabled = true;
                        break;
                }
            }
        }
        private CtrlMode _controlMode = CtrlMode.Full;

        public bool IsDrivenByNavMeshAgent
        {
            get
            {
                return _agent.enabled;
            }
            set
            {
                _agent.enabled = false;
            }
        }

        #endregion

        #region UnityLoop
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
#if FISHNET_SUPPORT
            _networkAnimator = GetComponent<NetworkAnimator>();
#endif
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<AvatarPlayerInputForTesting>();
        }

        private void Start()
        {
            _view = Camera.main;

            _destinationGizmo = Instantiate(destinationGizmo);

            _hasAnimator = TryGetComponent(out _animator);

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (!_forceGoTo && _agent.enabled && _playerInput.Move.sqrMagnitude > 0)
                _agent.enabled = false;
            if (_forceWalkForward > 0)
            {
                _forceWalkForward -= Time.deltaTime;
                _agent.enabled = false;
            }

            if (_agent.enabled)
            {
                Vector3 dist = _agent.destination - transform.position;
                dist.y = 0;
                if (dist.magnitude <= _agent.radius * 2f)
                {
                    //Debug.Log("Arrived");
                    _agent.enabled = false;
                    _forceGoTo = false;
                    _targetSpeed = 0;
                }
                else if (!_forceGoTo)
                    _targetSpeed = _playerInput.Sprint ? SprintSpeed : MoveSpeed;
                _speed = Mathf.Lerp(_speed, _targetSpeed, Time.deltaTime * SpeedChangeRate);
                _agent.speed = _speed;
                _targetDirection = transform.forward;
            }
            else
            {
                JumpAndGravity();
                GroundedCheck();
                Move();
            }

            // update animator if using character
            if (_hasAnimator)
            {
                _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * SpeedChangeRate);
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                //_animator.SetFloat(_animIDMotionSpeed, 1);
            }

            if (_destinationGizmo)
                _destinationGizmo.SetActive(_agent.enabled);
            if (_agent.enabled)
                _destinationGizmo.transform.position = _agent.destination;
        }

        public bool move = true;

        private void OnDisable()
        {
            _agent.enabled = false;
            if (_destinationGizmo) _destinationGizmo.SetActive(false);
            if (_forceWalkForward <= 0)
                _targetSpeed = 0;
            UIDrag d = FindObjectOfType<UIDrag>();
            if (d != null) d.OnUserClick -= OnUserClick;
        }

        private void OnEnable()
        {
            if (_forceWalkForward <= 0)
                _targetSpeed = 0;
            _agent.enabled = false;
            UIDrag d = FindObjectOfType<UIDrag>();
            if (d != null) d.OnUserClick += OnUserClick;
        }

        #endregion

        #region Public Method 
        public void ForceGoTo(Vector3 position, bool run = false)
        {
            if (!enabled)
                return;
            _debugClickPosition = position;
            _agent.enabled = true;
            _agent.destination = position;
            _forceGoTo = true;
            _targetSpeed = run ? SprintSpeed : MoveSpeed;
        }

        public void ForceWalkForward(Vector3 direction, float time, bool run = false)
        {
            if (!enabled)
                return;
            _agent.enabled = false;
            _forceWalkForward = time;
            _targetSpeed = run ? SprintSpeed : MoveSpeed;
            _forceWalkForwardDirection = Quaternion.LookRotation(direction).eulerAngles.y;
        }

        public void GoTo(Vector3 position)
        {
            if (!enabled)
                return;
            _debugClickPosition = position;

            //check existence of navmesh first
            NavMeshHit hit;
            if (NavMesh.SamplePosition(_debugClickPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                _agent.enabled = true;
                _agent.destination = position;
            }
        }

        public void SetAdditionalSpeed(Vector3 v)
        {
            _additionalSpeed = v;
        }

        public void SetVerticalVelocity(float value)
        {
            _verticalVelocity = value;
        }

        public void Teleport(Vector3 position, Quaternion rotation, bool keepViewOrientation = false)
        {
            //Tell every TriggerZone that player left
            //To do: this is not accurate, we should tell TriggerZones where the player is moving so it can decide if that's an exit or not
            foreach (TriggerZone zone in TriggerZone.All)
                zone.ForcePlayerExit(position);

            //move player AND view
            Vector3 move = position - transform.position;
            Transform helper = new GameObject("_helper").transform;
            Transform tView = GetComponent<ViewControllerForTesting>().View;
            Transform playerParent = transform.parent;
            Transform viewParent = tView.parent;
            helper.SetParent(transform);
            helper.localPosition = Vector3.zero;
            helper.localRotation = Quaternion.identity;
            helper.SetParent(null);
            if (keepViewOrientation)
                tView.transform.Translate(move, Space.World);
            else
                tView.SetParent(helper);
            transform.SetParent(helper);

            enabled = false;
            _characterController.enabled = false;
            helper.position = position;
            helper.rotation = rotation;
            enabled = true;
            _characterController.enabled = true;
            tView.SetParent(viewParent);
            transform.SetParent(playerParent);
            Destroy(helper.gameObject);
        }

        #endregion

        #region Editor
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_debugClickPosition, 0.1f);
        }

        #endregion

        #region Private

        private void OnUserClick(Ray ray)
        {
            //find hit on environment
            if (_controlMode == CtrlMode.Full && Physics.Raycast(ray, out RaycastHit hit, 1000, clickMask))
            {
                //Debug.Log("Goto " + hit.collider.name + " " + hit.point);
                GoTo(hit.point);
            }
        }
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            //_animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            //_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private float WalkTargetSpeed
        {
            get
            {
                if (_controlMode == CtrlMode.None)
                    return 0;
                return _playerInput.Sprint ? SprintSpeed : MoveSpeed;
            }
        }

        private void RefreshSpeed()
        {

            // a reference to the players current horizontal velocity
            Vector3 velocity = _characterController.velocity - _additionalSpeed;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _playerInput.Move.magnitude;
            _speed = Mathf.Lerp(_speed, _targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            return;
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < _targetSpeed - speedOffset || currentHorizontalSpeed > _targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = _targetSpeed;
            }
        }

        private void Move()
        {
            Vector2 inputMove = _playerInput.Move;
            if (!controlAllDirectionsWithStick)
                inputMove.x = 0;
            // set target speed based on move speed, sprint speed and if sprint is pressed
            if (_forceWalkForward <= 0)
            {
                _targetSpeed = WalkTargetSpeed;

                // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is no input, set the target speed to 0
                if (_controlMode == CtrlMode.Full && inputMove.magnitude < 0.01f) _targetSpeed = 0.0f;

                if (_controlMode == CtrlMode.WalkForward)
                {
                    Vector3 vF = _view.transform.forward;
                    vF.y = 0;
                    vF = vF.normalized;
                    Vector3 stickDir = vF * _playerInput.Move.y + _view.transform.right * _playerInput.Move.x;
                    if (Vector3.Dot(stickDir, transform.forward) <= 0)
                        _targetSpeed = 0.0f;
                }
            }

            RefreshSpeed();

            if (_controlMode == CtrlMode.Full)
            {
                // normalise input direction
                Vector3 inputDirection = new Vector3(inputMove.x, 0.0f, inputMove.y).normalized;

                // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is a move input rotate player when the player is moving
                if (_forceWalkForward > 0)
                {
                    _targetRotation = _forceWalkForwardDirection;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
                else
                if (Mathf.Abs(inputDirection.magnitude) > 0)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _view.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }

            _targetDirection = _controlMode == CtrlMode.Full ? Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward : transform.forward;
            if (!move)
                return;
            // move the player
            if (_speed > 0 || Mathf.Abs(_verticalVelocity) > 0.1f)
                _characterController.Move(_additionalSpeed * Time.deltaTime + _targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_additionalSpeed.sqrMagnitude > 0 && _verticalVelocity < -2 && Grounded)
            {
                _additionalSpeed = Vector3.zero;
            }

        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    //_animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -1;

                // Jump
                if (_playerInput.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

#if FISHNET_SUPPORT
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _networkAnimator.SetTrigger("Jump2");
                        //_animator.SetBool(_animIDJump, true);
                        //we can't set that trigger directly on the animator because we need to ensure it is also set remotely
                        //_networkAnimator.SetTrigger("Jump");
                    }
#else
                    if (_hasAnimator)
                        _animator.SetTrigger("Jump2");
#endif
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _playerInput.Jump = false;

                // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                if (_verticalVelocity < _terminalVelocity)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
            }
        }

        #endregion
    }
}
#endif