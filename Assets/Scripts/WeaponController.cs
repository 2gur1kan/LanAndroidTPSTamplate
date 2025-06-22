using Mirror;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject FireEffect;
    [SerializeField] private GameObject ExplosionEffect;

    [SerializeField] private float BulletSpeed = 31f;
    [SerializeField] private int Damage = 20;

    public TeamName team;
    public Player CurrentPlayer;

    [Server]
    public void Fire(Vector3 Target)
    {
        GameObject fx = Instantiate(FireEffect, transform.position, transform.rotation);
        NetworkServer.Spawn(fx);

        Vector3 direction = (Target - transform.position).normalized;

        if (Bullet != null)
        {
            GameObject bulletInstance = Instantiate(Bullet, transform.position, Quaternion.identity);
            bulletInstance.transform.forward = direction;

            Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = direction * BulletSpeed;

            Destroy(bulletInstance, 2f);
        }

        // merminin çarptýðý yer playersa hasar ver deðilse bir delik oluþtur
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            if (hitInfo.collider.CompareTag("Player") && hitInfo.transform.GetComponent<Player>().TeamName != team)
            {
                if (hitInfo.collider.GetComponent<Player>().TakeDamage(Damage)) CurrentPlayer.Score++;
                return;
            }

            GameObject hole = Instantiate(DataBaseManager.Instance.bulletHole,
                hitInfo.point + hitInfo.normal * 0.01f,
                Quaternion.LookRotation(hitInfo.normal));
            NetworkServer.Spawn(hole);

            // Patlama alaný
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = hitInfo.point;
            explosion.transform.localScale = Vector3.one;
            explosion.GetComponent<Collider>().isTrigger = true;
            explosion.GetComponent<MeshRenderer>().enabled = false;

            fx = Instantiate(ExplosionEffect, hitInfo.point, transform.rotation);
            NetworkServer.Spawn(fx);

            Collider[] hitColliders = Physics.OverlapSphere(hitInfo.point, 1.5f);
            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player") && col.GetComponent<Player>().TeamName != team)
                {
                    if (col.GetComponent<Player>()?.TakeDamage(Damage / 2) ?? false) CurrentPlayer.Score++;
                }
                else if (col.GetComponent<DynamicObject>())
                {
                    col.GetComponent<DynamicObject>().PushBack(transform.position, Damage);
                }
            }

            Destroy(hole, 15f);
            Destroy(explosion, 0.2f);
        }
    }
}

