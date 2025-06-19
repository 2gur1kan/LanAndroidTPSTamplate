using UnityEngine;

public class AimTargetScript : MonoBehaviour
{
    [SerializeField] private Transform gunAim;

    private void FixedUpdate()
    {
        SetGunAim();
    }

    private void SetGunAim()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            gunAim.position = hit.point;
        }
        else
        {
            gunAim.position = transform.position + transform.forward * 100f;
        }
    }
}
