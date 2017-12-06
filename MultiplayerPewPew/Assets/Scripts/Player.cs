using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

    [SyncVar]
    private bool _isDead = false;

    public bool isDead
    {
        get
        {
            return _isDead;
        }

        protected set
        {
            _isDead = value;
        }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    [SyncVar]
    public string username = "Loading...";


    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCameraActiveState(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllclients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllclients()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }
        

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage(int amount, string sourceId)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;

        Debug.Log(transform.name + " now has " + currentHealth + "health.");

        if (currentHealth <= 0)
        {
            Die(sourceId);
        }
    }

    private void Die(string sourceId)
    {
        isDead = true;
        deaths++;
        Debug.Log(transform.name + "is dead.");

        Player sourcePlayer = GameManager.GetPlayer(sourceId);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        //Disable components on the player object
        foreach (Behaviour b in disableOnDeath)
        {
            b.enabled = false;
        }

        //Disable game objects on disable
        foreach (GameObject obj in disableGameObjectsOnDeath)
        {
            obj.SetActive(false);
        }

        //Disable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        //Spawn a death effect
        GameObject deathGfxInst = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathGfxInst, 3f);

        //Chance camera of local player to scene camera
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActiveState(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        //Call respawn method
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        //Set components active
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Set game objects active
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable the collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        //Create spawn effect
        GameObject spawnGfxInst = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(spawnGfxInst, 3f);
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
