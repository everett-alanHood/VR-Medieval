using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class BowController : MonoBehaviour
{
    [Tooltip("Where the arrow should snap when nocked")]
    public Transform nockPoint;

    [Tooltip("Impulse applied to the arrow when released")]
    public float launchForce = 25f;

    // runtime state
    private GameObject _currentArrow;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _arrowGrabInteractable;
    private Rigidbody _arrowRb;

    void Reset()
    {
        // ensure this track collider is a trigger
        var col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // only one arrow at a time
        if (_currentArrow != null) return;

        // must be tagged "Arrow"
        if (!other.CompareTag("Arrow")) return;

        // find its grab interactable on the root
        var grab = other.GetComponentInParent<
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab == null) return;

        // cache references
        _currentArrow = grab.gameObject;
        _arrowGrabInteractable = grab;
        _arrowRb = _currentArrow.GetComponent<Rigidbody>();

        // disable grabbing so it stays in place
        _arrowGrabInteractable.enabled = false;
        _arrowGrabInteractable.selectExited.AddListener(OnArrowReleased);

        // freeze physics and snap to the nockPoint
        _arrowRb.isKinematic = true;
        _currentArrow.transform.SetParent(nockPoint, false);
        _currentArrow.transform.localPosition = Vector3.zero;
        _currentArrow.transform.localRotation = Quaternion.identity;
    }

    private void OnArrowReleased(SelectExitEventArgs args)
    {
        // only react to our arrow’s release event
        if (args.interactableObject != _arrowGrabInteractable) return;

        // unhook before we modify state
        _arrowGrabInteractable.selectExited.RemoveListener(OnArrowReleased);

        // detach and restore physics
        _currentArrow.transform.SetParent(null, true);
        _arrowRb.isKinematic = false;

        // launch
        _arrowRb.linearVelocity = nockPoint.forward * launchForce;

        // re-enable grabbing
        _arrowGrabInteractable.enabled = true;

        // clear state
        _currentArrow = null;
        _arrowGrabInteractable = null;
        _arrowRb = null;
    }
}
