using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f; 
    public float projectileSpeed = 50f;
    public float lifetime = 5.0f;



    private float nextfireTime = 5f;



    void Start()
    {
        
    }

    void Update()
    {
        if (Time.time >= nextfireTime)
        {
            Shoot();
            nextfireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null )
        {
            rb.velocity = firePoint.forward * projectileSpeed; 
        }
        Destroy(projectile, lifetime);
    }



}
