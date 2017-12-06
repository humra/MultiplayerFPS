using UnityEngine;

//Superclass for all the weapons
public class Weapons {

    public string name = "Default";
    public int damage;
    public float range;
    public float fireRate;
    public int maxBullets;
    [HideInInspector]
    public int bullets;
    public float reloadTime;
    public GameObject graphics;
    public float maxDivergence;
    public float minDivergence;
    public int shotSize;
    public float spreadResetCooldown;
    public float divergenceIncreaseRate;
    public float manualShootingFireRate;
}