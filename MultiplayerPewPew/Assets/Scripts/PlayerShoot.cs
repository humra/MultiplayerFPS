using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private Weapons currentWeapon;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;

    private const string PLAYER_TAG = "Player";

    private float currentDivergence;
    private float lastShotTime;

    private void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: no camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
        lastShotTime = Time.time;
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        //currentDivergence = currentWeapon.maxDivergence;

        if(PauseMenu.isOn)
        {
            return;
        }

        //Weapon switching keycodes
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponManager.SwitchWeapon(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponManager.SwitchWeapon(2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponManager.SwitchWeapon(3);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weaponManager.SwitchWeapon(4);
            return;
        }

        //Only allows reloading if the clip is not full
        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButton("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }

        //If a weapon has a fire rate of 0f then it is not automatic and requires
        //a button press for every shot made
        if(currentWeapon.fireRate <= 0f)
        {
            //Checking if enough time has passed since the last shot made by 
            //manual weapons
            if (Input.GetButtonDown("Fire1") && (Time.time - lastShotTime) > currentWeapon.manualShootingFireRate)
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                //Automatic weapons are shooting as long as the fire button is pressed
                InvokeRepeating("Shoot", 0f, currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    //Is called on the server when a player shoots
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //Is called on all clients when we need to 
    //create shoot effects
    [ClientRpc]
    private void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //Normal i sa vector that points directly away from the surface
    [Command]
    private void CmdOnHit(Vector3 hitPosition, Vector3 normal)
    {
        RpcDoHitEffect(hitPosition, normal);
    }

    [ClientRpc]
    private void RpcDoHitEffect(Vector3 position, Vector3 normal)
    {
        GameObject hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, position, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [Client]
    private void Shoot()
    {
        //Debug.Log("Shoot");
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }
        currentWeapon.bullets--;

        //Debug.Log("Remaining bullets: " + currentWeapon.bullets);

        //We are shooting, calling method on the server
        CmdOnShoot();

        for(int i = 0; i < currentWeapon.shotSize; i++)
        {
            SingleShot();
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    private void SingleShot()
    {
        //If we haven't fired for long enough, the recoil is negated
        //Else it is added to the current recoil rate
        if(Time.time - lastShotTime > currentWeapon.spreadResetCooldown)
        {
            currentDivergence = currentWeapon.minDivergence;
        }
        else
        {
            currentDivergence += currentWeapon.divergenceIncreaseRate * Time.deltaTime;
            if (currentDivergence > currentWeapon.maxDivergence)
            {
                currentDivergence = currentWeapon.maxDivergence;
            }
        }

        //Simulates recoil by setting the angle in which the bullet travels from the barrel
        //It is made in a way that the farther the bullet goes the less accuracy you get
        Vector3 divergence = cam.transform.forward;
        divergence.x += (1 - 2 * Random.value) * currentDivergence;
        divergence.y += (1 - 2 * Random.value) * currentDivergence;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, divergence, out hit, currentWeapon.range, mask))
        {

            if (hit.collider.tag.Equals(PLAYER_TAG))
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            //We hit something, call the OnHit method on the server
            CmdOnHit(hit.point, hit.normal);
        }

        lastShotTime = Time.time;
    }

    [Command]
    private void CmdPlayerShot(string playerId, int damage, string sourceId)
    {
        Debug.Log(playerId + " has been shot");

        Player shotPlayer = GameManager.GetPlayer(playerId);
        shotPlayer.RpcTakeDamage(damage, sourceId);
    }
}
