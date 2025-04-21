using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AmbiBowController : MonoBehaviour
{
    [Header("Bow Setup")]
    public Transform stringStart;           // Resting point of the bowstring
    public Transform stringVisual;          // The movable part of the string (optional visual)
    public Transform arrowSpawnPoint;       // Where arrows shoot from
    public GameObject arrowPrefab;          // Arrow prefab to instantiate
    public float maxPullDistance = 0.5f;    // Maximum string pull
    public float arrowForce = 50f;          // Max velocity multiplier

    [Header("Animation")]
    public Animator bowAnimator;            // Animator component for tension animation

    private XRBaseInteractor bowHand;       // The hand holding the bow
    private XRBaseInteractor pullingHand;   // The hand pulling the string
    private GameObject currentArrow;
    private bool isPulling = false;

    void Update()
    {
        if (isPulling && pullingHand != null)
        {
            float pullDistance = Mathf.Clamp(Vector3.Distance(stringStart.position, pullingHand.transform.position), 0f, maxPullDistance);

            // Move string visually
            if (stringVisual != null)
                stringVisual.position = Vector3.Lerp(stringStart.position, pullingHand.transform.position, pullDistance / maxPullDistance);

            // Move arrow with the string
            if (currentArrow != null)
                currentArrow.transform.position = stringVisual.position;
        }
    }

    public void OnBowGrab(SelectEnterEventArgs args)
    {
        bowHand = args.interactorObject as XRBaseInteractor;
    }

    public void OnBowRelease(SelectExitEventArgs args)
    {
        bowHand = null;
        pullingHand = null;
    }

    public void OnArrowGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInteractor;
        if (bowHand == null || interactor == bowHand) return;

        pullingHand = interactor;
        isPulling = true;

        currentArrow = args.interactableObject.transform.gameObject;

        var rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (bowAnimator != null)
            bowAnimator.SetBool("Tension", true);
    }


    public void OnArrowRelease(SelectExitEventArgs args)
    {
        if (!isPulling || currentArrow == null) return;

        float pullDistance = Mathf.Clamp(Vector3.Distance(stringStart.position, pullingHand.transform.position), 0f, maxPullDistance);
        Vector3 shootDir = arrowSpawnPoint.forward.normalized;

        var arrowScript = currentArrow.GetComponent<ArrowInteractEvents>();
        if (arrowScript != null)
        {
            arrowScript.Fire(shootDir, pullDistance / maxPullDistance * arrowForce);
        }

        if (stringVisual != null)
            stringVisual.position = stringStart.position;

        if (bowAnimator != null)
            bowAnimator.SetBool("Tension", false);

        currentArrow = null;
        pullingHand = null;
        isPulling = false;
    }

}
