using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBoxController : MonoBehaviour
{
    [SerializeField] private GameObject punchBox;

    public void EnablePunchBox()
    {
        punchBox.SetActive(true);
    }

    public void DisablePunchBox()
    {
        punchBox.SetActive(false);
    }
}
