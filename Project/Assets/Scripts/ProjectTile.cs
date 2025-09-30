using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectTile : MonoBehaviour
{
    public float speed = 20f;

    public float lifeTime = 2f;

    public float damage = 1;

    private Rigidbody rb;

    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true; // use kinematic with trigger for stable hits
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);



    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward *  speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Try to find Enemy on the collider or its parents
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = other.GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Projectile hit {enemy.name} for {damage} damage");
        }

        Destroy(gameObject);
    }
}
