﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private float weaponSwitchTime = 0.5f;

    private Weapons currentWeapon;
    private WeaponGraphics currentGraphics;
    private int currentWeaponIndex;

    public bool isReloading = false;

    private void Start()
    {
        //EquipWeapon(new AssaultRifleScript());
        CmdSwitchWeapon(1);
    }

    public void EquipWeapon(Weapons newWeapon)
    {
        currentWeapon = newWeapon;

        GameObject weaponInst = (GameObject)Instantiate(newWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInst.transform.SetParent(weaponHolder);

        currentGraphics = weaponInst.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
        {
            Debug.LogError("No weapon graphics component on the weapon object: " + weaponInst.name);
        }

        if(isLocalPlayer)
        {
            Util.SetLayerRecursive(weaponInst, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    private void ClearOldWeapon()
    {
        foreach(Transform child in weaponHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public Weapons GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void Reload()
    {
        if(isReloading)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentWeapon.bullets = currentWeapon.maxBullets;
        isReloading = false;
    }

    [Command]
    private void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    private void RpcOnReload()
    {
        Animator animator = currentGraphics.GetComponent<Animator>();
        if(animator != null)
        {
            animator.SetTrigger("Reload");
        }
    }

    [Command]
    public void CmdSwitchWeapon(int selectedWeaponIndex)
    {
        if (selectedWeaponIndex != currentWeaponIndex)
        {
            RpcSwitchWeaponCrt(selectedWeaponIndex);
        }
    }

    [ClientRpc]
    public void RpcSwitchWeaponCrt(int selectedWeaponIndex)
    {

        StartCoroutine(weaponSwitchTimeout(selectedWeaponIndex));

        
    }

    private IEnumerator weaponSwitchTimeout(int selectedWeaponIndex)
    {
        ClearOldWeapon();

        yield return new WaitForSeconds(weaponSwitchTime);

        switch (selectedWeaponIndex)
        {
            case 2:
                EquipWeapon(new AssaultRifleScript());
                break;
            case 1:
                EquipWeapon(new PistolScript());
                break;
            case 3:
                EquipWeapon(new SniperScript());
                break;
            case 4:
                EquipWeapon(new HeavyScript());
                break;
            default:
                break;
        }

        currentWeaponIndex = selectedWeaponIndex;
    }
}
