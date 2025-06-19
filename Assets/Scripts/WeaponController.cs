using Mirror;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private float BulletSpeed = 31f;
    [SerializeField] private int Damage = 20;

    public TeamName team;

    [Server]
    public void Fire(Vector3 Target)
    {
        if (Bullet == null) return;

        Vector3 direction = (Target - transform.position).normalized;

        GameObject bulletInstance = Instantiate(Bullet, transform.position, Quaternion.identity);
        bulletInstance.transform.forward = direction;

        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = direction * BulletSpeed;

        Destroy(bulletInstance, 2f);

        // merminin çarptýðý yer playersa hasar ver deðilse bir delik oluþtur
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            if (hitInfo.collider.CompareTag("Player") && hitInfo.transform.GetComponent<Player>().TeamName != team)
            {
                hitInfo.collider.GetComponent<Player>().TakeDamage(Damage);
                return;
            }

            GameObject hole = Instantiate(DataBaseManager.Instance.bulletHole,
                hitInfo.point + hitInfo.normal * 0.01f,
                Quaternion.LookRotation(hitInfo.normal));

            // Patlama alaný
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = hitInfo.point;
            explosion.transform.localScale = Vector3.one;
            explosion.GetComponent<Collider>().isTrigger = true;
            explosion.GetComponent<MeshRenderer>().enabled = false;

            Collider[] hitColliders = Physics.OverlapSphere(hitInfo.point, 1.5f);
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player") && col.GetComponent<Player>().TeamName != team)
                {
                    col.GetComponent<Player>()?.TakeDamage(Damage / 2);
                }
            }

            Destroy(hole, 15f);
            Destroy(explosion, 0.2f);
        }
    }
}

