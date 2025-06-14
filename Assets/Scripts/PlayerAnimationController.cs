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
        animator = GetComponent<Animator>();
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
}
