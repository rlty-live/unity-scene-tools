using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Plugins.RLTY.Frames.Scripts
{
    public class StaticFrameParameter : MonoBehaviour
    {
        //TODO User who can access the modif
        [SerializeField] Vector3 _size = Vector3.one;
        [SerializeField] Color _gizmoColor = Color.red;
        [SerializeField] private Image.Type imageType = Image.Type.Simple;
        
        public Vector3 Size => _size;
        public Image.Type ImageType => imageType;

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, _size);
            Gizmos.color = Color.white;
        }
    }
}