using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 30f;
    public Transform tip;
    public AudioClip shootingSound;
    public AudioClip impactSound;
    [Range(0f, 1f)] public float firingVolume = 0.5f;
    [Range(0f, 1f)] public float impactVolume = 0.5f;

    private Rigidbody _rigidBody;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;
    private AudioSource _audioSource;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        PullInteraction.PullActionReleased += Release;
        Stop();
    }

    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
    }

    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;

        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        _rigidBody.AddForce(force, ForceMode.Impulse);

        // 🔊 Play shooting sound at lower volume
        if (shootingSound != null)
            _audioSource.PlayOneShot(shootingSound, firingVolume);

        StartCoroutine(RotateWithVelocity());
        _lastPosition = tip.position;
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();

        while (_inAir)
        {
            if (_rigidBody.linearVelocity != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(_rigidBody.linearVelocity, transform.up);
                transform.rotation = newRotation;
            }
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (_inAir)
        {
            CheckCollision();
            _lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(_lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8) // Assuming layer 8 is "Ignore"
            {
                if (hitInfo.transform.TryGetComponent(out Rigidbody body))
                {
                    _rigidBody.interpolation = RigidbodyInterpolation.None;
                    transform.parent = hitInfo.transform;
                    body.AddForce(_rigidBody.linearVelocity, ForceMode.Impulse);
                }

                // 🎯 Play impact sound
                if (impactSound != null)
                    _audioSource.PlayOneShot(impactSound, impactVolume);

                if (hitInfo.transform.TryGetComponent(out Animator animator))
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName("Idle"))
                    {
                        animator.SetTrigger("Hit");
                    }
                }

                if (hitInfo.transform.name == "Bob" || hitInfo.transform.root.name == "Bob")
                {
                    var ragdoll = hitInfo.transform.GetComponentInParent<RagdollToggle>();
                    if (ragdoll != null)
                    {
                        ragdoll.TriggerRagdoll();
                    }
                }

                Stop();
            }
        }
    }

    private void Stop()
    {
        _inAir = false;
        SetPhysics(false);
    }

    private void SetPhysics(bool usePhysics)
    {
        _rigidBody.useGravity = usePhysics;
        _rigidBody.isKinematic = !usePhysics;
    }
}
