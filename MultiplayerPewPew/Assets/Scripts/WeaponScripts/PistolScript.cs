using UnityEngine;

public class PistolScript : Weapons {

    public PistolScript()
    {
        name = "Pistol";
        damage = 20;
        range = 50f;
        fireRate = 0f;
        maxBullets = 8;
        bullets = maxBullets;
        reloadTime = 1.2f;
        graphics = (GameObject)Resources.Load("Pistol");
        maxDivergence = 0.05f;
        minDivergence = 0f;
        shotSize = 1;
        spreadResetCooldown = 0.7f;
        divergenceIncreaseRate = 1.5f;
        manualShootingFireRate = 0.3f;
    }
}
