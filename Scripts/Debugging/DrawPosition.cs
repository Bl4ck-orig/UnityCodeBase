using UnityEngine;

namespace Debugging
{
    public class DrawPosition : MonoBehaviour
    {
        [SerializeField] private bool toggle;
        [SerializeField] private Color color = Color.red;
        [SerializeField] private Color rotationColor = Color.green;
        [SerializeField] private bool wired = false;
        [SerializeField] private float radius = 0.1f;

        private void OnDrawGizmos()
        {
            if (!toggle)
                return;

            Gizmos.color = color;

            if(wired)
                Gizmos.DrawWireSphere(transform.position, radius);
            else
                Gizmos.DrawSphere(transform.position, radius);

            UnityEngine.Debug.DrawLine(transform.position, transform.position + transform.forward * (radius * 1.5f), rotationColor, 0.1f);
        }
    }
}
