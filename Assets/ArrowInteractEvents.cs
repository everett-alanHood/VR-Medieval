using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
public class ArrowInteractEvents : MonoBehaviour
{
    public AmbiBowController bowController;

    [Header("Arrow Settings")]
    public float lifetime = 10f;
    public bool destroyOnHit = false;

    private Rigidbody rb;
    private TrailRenderer trail;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();

        if (trail != null)
        {
            trail.Clear();
            trail.enabled = true;
        }

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (!hasHit && rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        bowController.OnArrowGrab(args);
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        bowController.OnArrowRelease(args);
        Invoke(nameof(DestroySelf), lifetime);
    }

    public void Fire(Vector3 direction, float force)
    {
        if (rb == null) return;

        rb.isKinematic = false;

        // 🔁 Flip direction if arrow is facing backward
        Vector3 adjustedDirection = -direction;

        // 💡 Visual debug ray in Scene view
        Debug.DrawRay(transform.position, adjustedDirection * 2f, Color.red, 2f);

        rb.linearVelocity = adjustedDirection * force;

        if (trail != null)
            trail.emitting = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        hasHit = true;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.parent = collision.transform;

        if (trail != null)
            trail.emitting = false;

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
