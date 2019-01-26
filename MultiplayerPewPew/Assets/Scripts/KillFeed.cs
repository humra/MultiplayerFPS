using UnityEngine;

public class KillFeed : MonoBehaviour {

    [SerializeField]
    private GameObject killFeedItemPrefab;

    private void Start()
    {
        GameManager.instance.onPlayerKilledCallback += OnKill;
    }

    public void OnKill(string player, string source)
    {
        GameObject killFeedItemGO = (GameObject)Instantiate(killFeedItemPrefab, this.transform);
        killFeedItemGO.GetComponent<KillFeedItem>().Setup(player, source);
        killFeedItemGO.transform.SetAsFirstSibling();

        Destroy(killFeedItemGO, 5f);
    }
}
