using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

    //Singleton
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    public delegate void OnPlayerKilledCallback(string playerKilled, string sourceOfDamage);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one game manager found");
        }
        else
        {
            instance = this;
        }
    }

    public void SetSceneCameraActiveState(bool isActive)
    {
        if(sceneCamera == null)
        {
            return;
        }

        sceneCamera.SetActive(isActive);
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }

    #region PlayerRegistering
    private const string PLAYER_ID_PREFIX = "Player ";
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string networkID, Player player)
    {
        string playerId = PLAYER_ID_PREFIX + networkID;
        players.Add(playerId, player);
        player.transform.name = playerId;
    }

    public static void UnregisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerID)
    {
        return players[playerID];
    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach(string playerId in players.Keys)
    //    {
    //        GUILayout.Label(playerId + "    -    " + players[playerId].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}

    #endregion
}
