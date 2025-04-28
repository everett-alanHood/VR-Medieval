// ColliderVisualizer.cs
using UnityEngine;

[ExecuteAlways]
public class ColliderVisualizer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;  
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            if (!col.enabled) continue;
            if (col is SphereCollider sc)
            {
                var worldCenter = sc.transform.TransformPoint(sc.center);
                var worldRadius = sc.radius * sc.transform.lossyScale.x;
                Gizmos.DrawWireSphere(worldCenter, worldRadius);
            }
            else if (col is CapsuleCollider cc)
            {
                // approximate by drawing the two sphere caps and the connecting line
                Transform t = cc.transform;
                Vector3 center = t.TransformPoint(cc.center);
                float radius = cc.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);
                float halfH  = (cc.height * t.lossyScale.y * 0.5f) - radius;
                Vector3 dir = Vector3.up * halfH;
                Gizmos.DrawWireSphere(center + dir, radius);
                Gizmos.DrawWireSphere(center - dir, radius);
                Gizmos.DrawLine(center + dir + Vector3.right * radius, center - dir + Vector3.right * radius);
                Gizmos.DrawLine(center + dir - Vector3.right * radius, center - dir - Vector3.right * radius);
                Gizmos.DrawLine(center + dir + Vector3.forward * radius, center - dir + Vector3.forward * radius);
                Gizmos.DrawLine(center + dir - Vector3.forward * radius, center - dir - Vector3.forward * radius);
            }
            else if (col is BoxCollider bc)
            {
                var worldCenter = bc.transform.TransformPoint(bc.center);
                var worldSize   = Vector3.Scale(bc.size, bc.transform.lossyScale);
                Gizmos.DrawWireCube(worldCenter, worldSize);
            }
        }
    }
}
