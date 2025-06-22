using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Rig aimRig;

    private Animator animator;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        // Ýlk baþta ragdoll kapalý kalmalý
        SetRagdollState(false);

        animator = GetComponent<Animator>();
    }

    public void SetDead()
    {
        animator.enabled = false;

        SetAimRigWeight();
        aimRig.enabled = false;

        SetRagdollState(true);
    }

    #region Punch Box

    [SerializeField] private GameObject punchBox;

    public void EnablePunchBox()
    {
        punchBox.SetActive(true);
    }

    public void DisablePunchBox()
    {
        punchBox.SetActive(false);
    }

    #endregion

    #region IK system

    public void SetAimRigWeight(float gg = 0) => aimRig.weight = gg;

    #endregion

    #region Ragdoll system

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    public void SetRagdollState(bool state)
    {
        foreach (Collider col in ragdollColliders)
            if (col != GetComponent<Collider>()) col.enabled = state;

        foreach (Rigidbody rb in ragdollRigidbodies)
            if (rb != GetComponent<Rigidbody>()) rb.isKinematic = !state;

        if (animator != null)
            animator.enabled = !state;
    }

    #endregion
}
