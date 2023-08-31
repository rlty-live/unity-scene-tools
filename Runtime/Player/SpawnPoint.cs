using UnityEngine;

namespace Judiva.Metaverse.Interactions
{
    public class SpawnPoint : MonoBehaviour
    {
        public static float spawnRadius = 5;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1);
        }
    }
}
