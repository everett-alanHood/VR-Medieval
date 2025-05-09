using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    public Animator animator; // Assign this in the Inspector (Bob's Animator)
    private Rigidbody[] ragdollBodies; // All rigidbodies in Bob's limbs
    private Collider[] ragdollColliders; // All limb colliders

    void Start()
    {
        // Get all Rigidbodies and Colliders in child limbs
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Disable physics initially (normal animation state)
        SetRagdollState(false);
    }

    // Turn ragdoll on or off
    public void SetRagdollState(bool isRagdoll)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.gameObject == this.gameObject) continue; // Skip root
            rb.isKinematic = !isRagdoll; // Enable physics
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject == this.gameObject) continue; // Skip root collider
            col.enabled = isRagdoll; // Enable collisions on limbs
        }

        // Disable Animator to let physics take over
        if (animator) animator.enabled = !isRagdoll;
    }

    // Public method to trigger ragdoll
    public void TriggerRagdoll()
    {
        SetRagdollState(true);
    }
}
