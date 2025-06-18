using Mirror;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private float BulletSpeed = 31f;

    [Server]
    public void Fire(Vector3 Target)
    {
        if (Bullet == null || Target == null) return;

        GameObject bulletInstance = Instantiate(Bullet, transform.position, Quaternion.identity);

        Vector3 direction = (Target - transform.position).normalized;
        bulletInstance.transform.forward = direction;

        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = direction * BulletSpeed;
    }
}

