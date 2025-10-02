using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject prohectTilePrefabs;

    public Transform firePoint;

    Camera cam;

    [System.Serializable]
    public class Weapon
    {
        public string name;
        public GameObject projectilePrefab;
        public int damage = 1;
        public float fireRate = 0.5f; // seconds per shot
    }

    public Weapon[] weapons = new Weapon[2];

    private int currentWeaponIndex = 0;
    private float nextFireTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        // If a GameObject is assigned for firePoint, map it to Transform
      

        // Backward compatibility: if weapons not configured in Inspector, initialize defaults
        if (weapons[0] == null)
        {
            weapons[0] = new Weapon
            {
                name = "Default",
                projectilePrefab = prohectTilePrefabs,
                damage = 1,
                fireRate = 0.5f
            };
        }
        if (weapons[1] == null)
        {
            weapons[1] = new Weapon
            {
                name = "NewWeapon",
                projectilePrefab = prohectTilePrefabs,
                damage = 3,
                fireRate = 0.3f
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
       


        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchWeapon();
        }

        if (Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    void SwitchWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        // Optionally log switch
        // Debug.Log($"Switched to: {weapons[currentWeaponIndex].name}");
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        if (firePoint == null)
        {
            Debug.LogWarning("PlayerShooting: firePoint is not set. Assign a Transform or a GameObject to firePointObject.");
            return;
        }
        Shoot();
        nextFireTime = Time.time + Mathf.Max(0f, weapons[currentWeaponIndex].fireRate);
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        Weapon weapon = weapons[currentWeaponIndex];
        GameObject prefabToUse = weapon.projectilePrefab != null ? weapon.projectilePrefab : prohectTilePrefabs;
        GameObject proj = Instantiate(prefabToUse, firePoint.position, Quaternion.LookRotation(direction));

        ProjectTile projComponent = proj.GetComponent<ProjectTile>();
        if (projComponent != null)
        {
            projComponent.damage = weapon.damage;
        }
    }
}
