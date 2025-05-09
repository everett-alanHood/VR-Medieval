using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    public Animator animator;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    void Awake()
    {
        // ✅ Auto-assign Animator if not set manually
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on " + gameObject.name);
        }

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollActive(false); // Start with animator on
    }

    public void SetRagdollActive(bool isRagdoll)
    {
        if (animator != null)
            animator.enabled = !isRagdoll;

        foreach (var rb in ragdollBodies)
        {
            if (rb.gameObject == gameObject) continue;
            rb.isKinematic = !isRagdoll;
        }

        foreach (var col in ragdollColliders)
        {
            if (col.gameObject == gameObject) continue;
            col.enabled = isRagdoll;
        }
    }

    public void TriggerRagdoll()
    {
        SetRagdollActive(true);
    }

    public void RecoverFromRagdoll()
    {
        SetRagdollActive(false);
    }
}
