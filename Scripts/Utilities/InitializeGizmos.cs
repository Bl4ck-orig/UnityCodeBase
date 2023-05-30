using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class InitializeGizmos : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(DestroyInFrames(10));
        }

        private void OnDrawGizmos()
        {
            // Loop through all child objects and initialize their gizmos.
            foreach (Transform child in transform)
            {
                Gizmos.DrawWireCube(child.position, Vector3.one);
            }
        }

        private IEnumerator DestroyInFrames(int _frames)
        {
            for (int i = 0; i < _frames; i++)
                yield return null;

            Destroy(this);
        }
    }
}