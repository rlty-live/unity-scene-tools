using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RLTY.UI
{
    /// <summary>
    /// Receive mouse drag events on the screen, and forward them to anyone listening (useful for camera mouse control)
    /// </summary>
    public class UIDrag : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public event Action<Vector2> OnUserDrag;
        public event Action<Ray> OnUserClick;

        public LayerMask raycastMask;
        public int dragMinPixelMove = 3;
        private bool _drag = false;
        private Camera _view;
        private float _farClip;
        private float _fieldOfView;
        private int _width, _height;
        private Transform _canvas;

        private RLTYMouseEvent _pointedObject;
        private RLTYMouseEvent _clickedObject;
        private Vector2 _mousePosition;
        private Vector2 _pressPosition;
        private RaycastHit _hit;

        public void OnDrag(PointerEventData eventData)
        {
            _drag = true;
            OnUserDrag?.Invoke(new Vector2(eventData.delta.x / Screen.width, eventData.delta.y / Screen.height));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressPosition = eventData.position;
            _mousePosition = eventData.position;
            _drag = false;
            //raycast
            if (Raycast())
            {
                _clickedObject = _pointedObject;
                if (_clickedObject)
                    _clickedObject.NotifyOnPointerDown();
            }    
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_clickedObject) 
                _pointedObject.NotifyOnPointerUp();
            if (!_drag)
            {
                if (_clickedObject)
                    _clickedObject.NotifyOnClick();
                OnUserClick?.Invoke(Camera.main.ScreenPointToRay(eventData.pressPosition));
            }
            _clickedObject = null;
            _drag = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _mousePosition = eventData.position;
        }

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>().transform;
        }

        void Update()
        {
            if (_drag)
                return;
            RLTYMouseEvent previousPointed = _pointedObject;
            Collider previous = _hit.collider;
            Raycast();
            if (_pointedObject != previousPointed)
            {
                if (previousPointed)
                    previousPointed.NotifyOnPointerExit();
                if (_pointedObject)
                    _pointedObject.NotifyOnPointerEnter();
            }
        }

        bool Raycast()
        {
            bool result = Physics.Raycast(Camera.main.ScreenPointToRay(_mousePosition), out _hit, 10000f, raycastMask);
            if (result)
            {
                _hit.collider.TryGetComponent<RLTYMouseEvent>(out _pointedObject);
                if (_pointedObject && (_hit.point - Camera.main.transform.position).magnitude > _pointedObject.mouseDetectDistance)
                    _pointedObject = null;
            }   
            else
                _pointedObject = null;
            return result;
        }

        void LateUpdate()
        {
            if (_view == null)
                _view = GetComponentInParent<Camera>();
            if (_farClip!=_view.farClipPlane || _fieldOfView!=_view.fieldOfView || _width!=Screen.width || _height!=Screen.height)
            {
                _width = Screen.width;
                _height = Screen.height;
                _farClip = _view.farClipPlane;
                _fieldOfView = _view.fieldOfView;
                _canvas.transform.localPosition = new Vector3(0, 0, _farClip * 0.99f);
                Vector3 pos = _view.transform.InverseTransformPoint(_view.ViewportToWorldPoint(new Vector3(1,1, _farClip * 0.99f)));
                _canvas.transform.localScale = new Vector3(2*pos.x, 2*pos.y, 1);
            }
        }
    }
}
