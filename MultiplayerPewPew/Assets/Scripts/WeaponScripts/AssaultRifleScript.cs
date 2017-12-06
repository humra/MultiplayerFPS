using UnityEngine;

public class AssaultRifleScript : Weapons {

    public AssaultRifleScript()
    {
        name = "AssaultRifle";
        damage = 5;
        range = 100f;
        fireRate = 0.1f;
        maxBullets = 20;
        bullets = maxBullets;
        reloadTime = 2f;
        graphics = (GameObject)Resources.Load("AssaultRifle");
        maxDivergence = 0.05f;
        minDivergence = 0f;
        shotSize = 1;
        spreadResetCooldown = 0.5f;
        divergenceIncreaseRate = 0.8f;
        manualShootingFireRate = 0f;
    }
}
