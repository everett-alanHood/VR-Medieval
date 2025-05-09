using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    public Animator animator;
    private Rigidbody[] ragdollBodies;

    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false); // start disabled
    }

    public void SetRagdollState(bool isRagdoll)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            if (rb.gameObject == this.gameObject) continue; // skip root
            rb.isKinematic = !isRagdoll;
            Collider col = rb.GetComponent<Collider>();
            if (col) col.enabled = isRagdoll;
        }

        if (animator) animator.enabled = !isRagdoll;
    }

    // Call this externally when hit
    public void TriggerRagdoll()
    {
        SetRagdollState(true);
    }
}

