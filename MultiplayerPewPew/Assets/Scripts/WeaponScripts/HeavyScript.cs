using UnityEngine;

public class HeavyScript : Weapons {

    public HeavyScript()
    {
        name = "Heavy";
        damage = 15;
        range = 20f;
        fireRate = 0f;
        maxBullets = 5;
        bullets = maxBullets;
        reloadTime = 1.5f;
        graphics = (GameObject)Resources.Load("Heavy");
        maxDivergence = 0.12f;
        minDivergence = 0.12f;
        shotSize = 7;
        spreadResetCooldown = 0.3f;
        divergenceIncreaseRate = 1.5f;
        manualShootingFireRate = 0.8f;
    }
}
