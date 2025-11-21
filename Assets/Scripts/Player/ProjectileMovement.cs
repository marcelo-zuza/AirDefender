using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody projectileRigidBody;
    [SerializeField] float projectileSpeed = 600f;
    [SerializeField] float destroyProjectileTime = 3f;

    void Start()
    {
        projectileRigidBody = GetComponent<Rigidbody>();
        projectileRigidBody.linearVelocity = transform.forward * projectileSpeed;
        Destroy(gameObject,destroyProjectileTime);
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
