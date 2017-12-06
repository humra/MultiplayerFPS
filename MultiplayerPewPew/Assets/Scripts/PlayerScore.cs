using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private Player player;
    private int lastKills = 0;
    private int lastDeaths = 0;
    private void Start()
    {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
    }

    private IEnumerator SyncScoreLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);

            SyncNow();
        }
    }

    private void OnDestroy()
    {
        if(player != null)
        {
            SyncNow();
        }
    }

    private void SyncNow()
    {
        if (UserAccountManager.isLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataReceived);
        }
    }

    private void OnDataReceived(string data)
    {
        if(player.kills <= lastKills && player.deaths <= lastDeaths)
        {
            return;
        }

        int killsSinceLastSync = player.kills - lastKills;
        int deathsSinceLastSync = player.deaths - lastDeaths;

        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = killsSinceLastSync + kills;
        int newDeaths = deathsSinceLastSync + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        Debug.Log("Syncig: " + newData);

        lastKills = player.kills;
        lastDeaths = player.deaths;

        UserAccountManager.instance.SendData(newData);
    }
}
