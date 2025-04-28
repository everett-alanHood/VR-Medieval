using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class BowController : MonoBehaviour
{
    [Header("References")]
    public Transform nockPoint;             // where arrow snaps
    public Transform stringAttachPoint;     // follows your pulling hand
    public Transform stringRestPoint;       // rest (zero) position of the string

    [Header("Tuning")]
    public float launchForceMultiplier = 25f;

    // runtime
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable   _bowInteractable;
    Transform            _bowHand;        // who’s holding the bow
    GameObject           _currentArrow;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable   _arrowInteractable;
    Transform            _arrowHand;      // who’s holding the arrow
    Vector3              _stringRestLocalPos;

    void Awake()
    {
        _bowInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _bowInteractable.selectEntered.AddListener(OnBowGrabbed);
        _bowInteractable.selectExited .AddListener(OnBowReleased);

        _stringRestLocalPos = stringAttachPoint.localPosition;
    }

    void OnBowGrabbed(SelectEnterEventArgs args)
    {
        _bowHand = args.interactorObject.transform;
    }

    void OnBowReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform == _bowHand)
            _bowHand = null;
    }

    void OnTriggerEnter(Collider other)
    
    {
        {   
            Debug.Log($"[BowController] TriggerEnter on {other.name}, tag={other.tag}");
            if (_bowHand == null || _currentArrow != null) return;
            if (!other.CompareTag("Arrow")) return;
            // … rest of your nock code …
        }

        // Only if bow is held, no arrow already nocked, and collider is tagged "Arrow"
        if (_bowHand == null || _currentArrow != null) return;
        if (!other.CompareTag("Arrow")) return;

        // Grab its XRGrabInteractable
        var xi = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (xi == null) return;

        _currentArrow       = xi.gameObject;
        _arrowInteractable  = xi;
        xi.selectEntered   .AddListener(OnArrowGrabbed);
        xi.selectExited    .AddListener(OnArrowReleased);
    }

    void OnArrowGrabbed(SelectEnterEventArgs args)
    {
        _arrowHand = args.interactorObject.transform;

        // Snap to the nock point
        _currentArrow.transform.SetParent(nockPoint);
        _currentArrow.transform.localPosition = Vector3.zero;
        _currentArrow.transform.localRotation = Quaternion.identity;

        // Freeze physics
        var rb = _currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // While nocked & held, stretch the string toward your hand
        if (_currentArrow != null && _arrowHand != null)
            stringAttachPoint.position = _arrowHand.position;
    }

    void OnArrowReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject.transform != _arrowHand) return;

        // Calculate draw distance
        float pullDist = Vector3.Distance(stringRestPoint.position,
                                          stringAttachPoint.position);

        // Unhook listeners
        _arrowInteractable.selectEntered.RemoveListener(OnArrowGrabbed);
        _arrowInteractable.selectExited .RemoveListener(OnArrowReleased);

        // Fire!
        _currentArrow.transform.SetParent(null);
        var rb = _currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = nockPoint.forward * pullDist * launchForceMultiplier;

        // Reset
        _currentArrow = null;
        _arrowHand    = null;
        stringAttachPoint.localPosition = _stringRestLocalPos;
    }
}
