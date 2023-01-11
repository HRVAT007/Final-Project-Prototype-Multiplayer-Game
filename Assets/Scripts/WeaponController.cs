using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    //bullet
    public GameObject bullet;
    
    //bullet force
    public float shootForce, upwardForce;
    
    //gun stats
    [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShoots, range;
    [SerializeField] private int magazineSize, bulletsPerTap;
    [SerializeField] private bool alloweButtonHold;
    [SerializeField] private int bulletsLeft, bulletsShot, damage;

    //recoil
    private float recoil = 0.0f;
    private float maxRecoil_x = -20f;
    private float maxRecoil_y = 20f;
    private float recoilSpeed = 2f;

    //bools
    private bool shooting, readyToShoot, reloading;
    public bool allowInvoke;

    //references
    Camera playerCamera;
    public Transform attackPoint;
    private LayerMask whatIsEnemy;
    public AudioSource shootingAudio;
    public AudioSource reloadAudio;
    //private HealthSystem healthSystem = new HealthSystem();
    
    //graphics
    [SerializeField] private GameObject muzzleFlash, bulletHoleGraphics;
    [SerializeField] private Text bulletsLeftText;
    [SerializeField] private Text magazineSizeText;
    [SerializeField] private GameObject bloodEffect;

    //controls
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        MyInput();

        //set ammo dispolay, if it exists
        if (bulletsLeftText != null)
        {
            int _bulletsLeft = bulletsLeft / bulletsPerTap;
            bulletsLeftText.text = _bulletsLeft.ToString();
        }

        if (magazineSizeText != null)
        {
            int _magazineSize = magazineSize / bulletsPerTap;
            magazineSizeText.text = _magazineSize.ToString();
        }

        if (bulletsLeftText && magazineSizeText == null)
        {
            bulletsLeftText.text = " ";
            magazineSizeText.text = " ";
        }
        
    }

    private void MyInput()
    {
        //check if allowed to shoot while holding down a button
        if (alloweButtonHold) shooting = Input.GetKey(shootKey);
        else shooting = Input.GetKeyDown(shootKey);

        //reloading
        if (Input.GetKeyDown(reloadKey) && bulletsLeft < magazineSize && !reloading) Reload();
        //reload automatically when shooting with an empty gun
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();

            StartRecoil(0.2f, 10f, 2f);
        }

        Recoiling();
    }
    
    private void Shoot()
    {
        readyToShoot = false;
        int headshotDamage = damage * 2;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        GetComponent<AudioSource>().Play();


        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(hit.collider.name);
            var buildingHealth = hit.collider.GetComponent<BuildingStats>();
            //instantiate sphere at hit point and add force to it and destroy it after 2 seconds
            //Destroy(Instantiate(bullet, hit.point, Quaternion.identity), 0.5f);
            if (buildingHealth == null) {
                if (hit.collider.name == "Charachter")
                {
                    //targetPoint = hit.point;
                    ////var objectHealth = hit.collider.GetComponent<CharacterStats>();
                    //var _objectHealth = hit.collider.GetComponentInParent<CharacterStats>();
                    //if (_objectHealth != null)
                    //    _objectHealth.TakeDamage(0);
                }

                else if (hit.collider.name != "Charachter")
                {
                    targetPoint = hit.point;
                    //var objectHealth = hit.collider.GetComponent<CharacterStats>();
                    var objectHealth = hit.collider.GetComponentInParent<CharacterStats>();
                    if (objectHealth != null)
                        if (hit.collider.name == "mixamorig:Neck")
                        {
                            SpawnBloodEffect(hit.point, hit.normal);
                            objectHealth.TakeDamage(headshotDamage);
                        }
                        else if (hit.collider.name != "mixamorig:Neck")
                        {
                            objectHealth.TakeDamage(damage);
                            SpawnBloodEffect(hit.point, hit.normal);
                        }
                }
            }
            else if (buildingHealth != null)
            {
                buildingHealth.TakeDamage(damage);
            }
            else
            {
                targetPoint = ray.GetPoint(75);
            }

        }

        //instantiate bullet hole
        if (bulletHoleGraphics != null)
            Instantiate(bulletHoleGraphics, hit.point, Quaternion.Euler(0, 180, 0));

        //instantiate muzzle flash
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShoots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        reloadAudio.Play();
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    private void SpawnBloodEffect(Vector3 position, Vector3 normal)
    {
        Instantiate(bloodEffect, position, Quaternion.FromToRotation(Vector3.up, normal));
    }

    public void StartRecoil(float recoilParam, float maxRecoil_xParam, float recoilSpeedParam)
    {
        // in seconds
        recoil = recoilParam;
        maxRecoil_x = maxRecoil_xParam;
        recoilSpeed = recoilSpeedParam;
        maxRecoil_y = Random.Range(-maxRecoil_xParam, maxRecoil_xParam);
    }

    private void Recoiling()
    {
        if (recoil > 0f)
        {
            Quaternion maxRecoil = Quaternion.Euler(maxRecoil_x, maxRecoil_y, 0f);
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0f;
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2);
        }
    }
}