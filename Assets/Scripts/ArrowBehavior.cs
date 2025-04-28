using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowBehavior : MonoBehaviour
{
    Rigidbody _rb;
    bool      _stuck = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (_stuck) return;
        _stuck = true;

        // Stop physics
        _rb.isKinematic = true;
        _rb.linearVelocity     = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // Parent to whatever we hit
        transform.SetParent(col.transform, true);

        // (Optional) Destroy after a delay
        Destroy(this, 10f);
    }
}
