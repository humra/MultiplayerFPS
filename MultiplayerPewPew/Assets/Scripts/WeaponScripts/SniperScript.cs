using UnityEngine;

public class SniperScript : Weapons {

    public SniperScript()
    {
        name = "Sniper";
        damage = 75;
        range = 500f;
        fireRate = 0f;
        maxBullets = 10;
        bullets = maxBullets;
        reloadTime = 2.5f;
        graphics = (GameObject)Resources.Load("Sniper");
        maxDivergence = 0f;
        minDivergence = 0f;
        shotSize = 1;
        spreadResetCooldown = 1f;
        divergenceIncreaseRate = 1f;
        manualShootingFireRate = 1f;
    }
}
