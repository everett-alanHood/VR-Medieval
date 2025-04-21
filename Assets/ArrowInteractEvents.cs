using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ArrowInteractEvents : MonoBehaviour
{
    public AmbiBowController bowController;

    public void OnGrab(SelectEnterEventArgs args)
    {
        bowController.OnArrowGrab(args);
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        bowController.OnArrowRelease(args);
    }
}
