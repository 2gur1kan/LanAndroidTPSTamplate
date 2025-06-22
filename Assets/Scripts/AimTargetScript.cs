using UnityEngine;

public class AimTargetScript : MonoBehaviour
{
    [SerializeField] private Transform gunAim;
    private int layerMask;

    private void Start()
    {
        Invoke("CheckTeamInvoke", 1.2f);
    }

    private void CheckTeamInvoke()
    {
        int ignoreLayer = DataBaseManager.Instance.Team == TeamName.A ? LayerMask.NameToLayer("TeamA") : LayerMask.NameToLayer("TeamB");

        layerMask = ~(1 << ignoreLayer);
    }

    private void FixedUpdate()
    {
        SetGunAim();
    }

    private void SetGunAim()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            gunAim.position = hit.point;
        }
        else
        {
            gunAim.position = transform.position + transform.forward * 100f;
        }
    }
}
