using UnityEngine;

public class ShowGizmoOnScreen : MonoBehaviour {

    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private float radius = 0.5f;

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z),
            transform.forward + new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z));
    }
}
