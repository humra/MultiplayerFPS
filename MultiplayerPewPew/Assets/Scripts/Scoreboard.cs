using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    private GameObject playerScoreboardItemPrefab;
    [SerializeField]
    private Transform playerScoreboardList;

    private void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();

        foreach(Player player in players)
        {
            GameObject itemGO = (GameObject)Instantiate(playerScoreboardItemPrefab, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }
    }

    private void OnDisable()
    {
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
