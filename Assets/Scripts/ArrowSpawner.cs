using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrow;
    public GameObject notch;

    private XRGrabInteractable _bow;
    private bool _arrowNotched = false;
    private GameObject _currentArrow = null;

    private void Start()
    {
        _bow = GetComponent<XRGrabInteractable>();

        // Subscribe to the pull release event from PullInteraction
        PullInteraction.PullActionReleased += NotchEmpty;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        PullInteraction.PullActionReleased -= NotchEmpty;
    }

    private void Update()
    {
        // If bow is grabbed and arrow is not yet notched, spawn the arrow
        if (_bow.isSelected && !_arrowNotched)
        {
            _arrowNotched = true;
            StartCoroutine(DelayedSpawn());
        }

        // If bow is released and an arrow exists, destroy the current arrow
        if (!_bow.isSelected && _currentArrow != null)
        {
            Destroy(_currentArrow);
        }
    }

    private void NotchEmpty(float value)
    {
        _arrowNotched = false;
        _currentArrow = null;
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(1f); // Delay before spawning the arrow

        if (_currentArrow == null)
        {
            _currentArrow = Instantiate(arrow, notch.transform);
        }
    }
}
