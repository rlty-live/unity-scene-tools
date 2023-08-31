using UnityEngine;
using RLTY.UI;

#if UNITY_EDITOR
namespace RLTY.Customisation.Testing
{

    public class ViewControllerForTesting : MonoBehaviour
    {
        #region Fields

        public float dampening = 1f;
        public float distance = 2;
        public Vector2 distanceLimit = new Vector2(1, 5);
        public Vector2 sideOffset = new Vector2(0.1f, 1f);
        
        /// <summary>
        /// from -1 to 1, to be able to move the camera to each side
        /// </summary>
        public float sideOffsetFactor = 0;
        public float sideOffsetTargetFactor;
        public float sideOffsetDamp=0.05f;

        public Vector2 overrideDistanceLimit = new Vector2(1, 5);
        public Vector3 offset;
        public float mouseRotationSpeed = 90;
        public float mouseScrollSpeed = 1;
        public float viewUpLimit = 0.9f;
        public float viewDownLimit = 0.5f;

        public bool enableInput = true;

        /// <summary>
        /// enables view rotation using joystick X axis
        /// </summary>
        public float joystickRotationSpeed = 90;

        public float collisionRadius=0.5f;
        public LayerMask collisionLayerMask;

        private AvatarPlayerInputForTesting _input;
        private PlayerControllerForTesting _player;

        [Header("OrientationConstraint")]
        public float adjustRotationSpeed = 1;
        [Header("NavAgentOrientationConstraint")]
        public float adjustNavAgentRotationSpeed = 1;

        private Transform _helper;

        public Vector3 orientationConstraint;
        public float orientationConstraintAngle = 30;


        #endregion

        #region Properties
        public Transform View { get { return _view; } }
        private Transform _view;

        public Camera Camera { get { return _cam; } }
        private Camera _cam;

        protected Vector3 Target
        {
            get
            {
                float d = CollisionDistance;
                //we choose sides based on sign of dot product
                
                return transform.TransformPoint(offset) - d * _view.forward + sideOffsetFactor*Mathf.Lerp(sideOffset.x,sideOffset.y,Mathf.Clamp01((d- distanceLimit.x)/(distanceLimit.y-distanceLimit.x)))*_view.right;
            }
        }

        protected Vector3 RotationPoint
        {
            get
            {
                Vector3 v0 = transform.TransformPoint(offset.y * Vector3.up);
                Vector3 v1 = transform.TransformPoint(offset);
                Vector3 vDir = v1-v0;
                if (vDir.sqrMagnitude>0.0001f)
                {
                    if (Physics.SphereCast(new Ray(v0, vDir), collisionRadius*1.1f, out RaycastHit hit, vDir.magnitude, collisionLayerMask))
                        return v0+hit.distance*vDir.normalized;
                }
                
                return v1;
            }
        }

        protected float CollisionDistance
        {
            get
            {
                distance = Mathf.Clamp(distance, overrideDistanceLimit.x, overrideDistanceLimit.y);
                if (Physics.SphereCast(new Ray(RotationPoint, -_view.transform.forward), collisionRadius, out RaycastHit hit, distance, collisionLayerMask))
                    return Mathf.Min(distance, hit.distance);
                return distance;
            }
        }

        #endregion

        #region UnityLoop
        private void Start()
        {
            overrideDistanceLimit = distanceLimit;
            _player = GetComponent<PlayerControllerForTesting>();
            _input = GetComponent<AvatarPlayerInputForTesting>();
            _cam = Camera.main;
            _view = _cam.transform;
                
            _helper = new GameObject("_helper").transform;

            //match player rotation
            _view.transform.rotation = transform.rotation;
            _view.transform.position = Target;
            
        }

        private void OnDisable()
        {
            UIDrag d = FindObjectOfType<UIDrag>();
            if (d) d.OnUserDrag -= OnDrag;
        }

        private void OnEnable()
        {
            UIDrag d = FindObjectOfType<UIDrag>();
            if (d) d.OnUserDrag += OnDrag;
        }

        private void Update()
        {
            DoUpdate(Time.deltaTime);
        }

        public void DoUpdate(float deltaTime)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                distance = Mathf.Clamp(distance * (1 - Input.mouseScrollDelta.y * mouseScrollSpeed), distanceLimit.x, distanceLimit.y);
                _view.transform.position = Vector3.Lerp(_view.transform.position, Target, 0.1f);
            }
            if (joystickRotationSpeed!=0 && Mathf.Abs(_input.Move.x) >0)
                _view.transform.RotateAround(RotationPoint, Vector3.up, _input.Move.x* joystickRotationSpeed*deltaTime);

            //ray cast to limit distance
            _view.transform.position = Vector3.Lerp(_view.transform.position, Target, dampening * deltaTime);

            //match preferredRotation
            if (orientationConstraint.magnitude > 0 && Mathf.Abs(DistanceAngle()) > orientationConstraintAngle)
                _view.transform.RotateAround(RotationPoint, Vector3.up, -DistanceAngle() * 0.01f * adjustRotationSpeed);

            //when using nav mesh, move camera behind player
            if (_player.IsDrivenByNavMeshAgent)
            {
                _view.transform.RotateAround(RotationPoint, Vector3.up, -DistanceAngleFromForwardDirection() * 0.01f * adjustNavAgentRotationSpeed);
            }

            float sign = Mathf.Sign(Vector3.Dot(transform.forward, _view.transform.right));
            if (sign == 0)
                sign = 1;
            sideOffsetTargetFactor = (1-_player.NormalizedSpeed) * sign;
            sideOffsetFactor = Mathf.Lerp(sideOffsetFactor, sideOffsetTargetFactor, sideOffsetDamp);
        }

        #endregion

        #region Public Methods
        public void SetOrientation(Vector3 forward)
        {
            Vector3 tmp = orientationConstraint;
            orientationConstraint = forward;
            _view.transform.RotateAround(RotationPoint, Vector3.up, -DistanceAngle());
            orientationConstraint = tmp;

        }

        #endregion

        #region Private
        private void OnDrag(Vector2 delta)
        {
            if (!enableInput)
                return;
            float xAngle = delta.x * mouseRotationSpeed;
            //rotate around player ? or target
            if (orientationConstraint.magnitude > 0) 
            {
                float distanceAngle = DistanceAngle();
                if (distanceAngle + xAngle > orientationConstraintAngle)
                    xAngle = orientationConstraintAngle - distanceAngle;
                if (distanceAngle + xAngle < -orientationConstraintAngle)
                    xAngle = -orientationConstraintAngle - distanceAngle;
            }
            _view.transform.RotateAround(RotationPoint, Vector3.up, xAngle);
            //constrained vertical rotation
            
            if ((delta.y<0 && _view.forward.y>-viewUpLimit) || (delta.y > 0 && _view.forward.y < viewDownLimit))
                _view.transform.RotateAround(RotationPoint, -_view.transform.right, delta.y * mouseRotationSpeed);
        }

        private float DistanceAngle()
        {
            _helper.position = transform.position;
            _helper.LookAt(_helper.position + orientationConstraint);
            float angle = _view.transform.eulerAngles.y - _helper.eulerAngles.y;
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }

        private float DistanceAngleFromForwardDirection()
        {
            float angle = _view.transform.eulerAngles.y - transform.eulerAngles.y;
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }

        #endregion

        #region Editor
        private void OnDrawGizmos()
        {
            if (_view!=null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(RotationPoint, 0.05f);
                Gizmos.DrawWireSphere(Target, 0.05f);
                Gizmos.DrawLine(RotationPoint, Target);
            }

        }

        #endregion
    }
}
#endif
