using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

    [SerializeField]
    private Text usernameText;
    [SerializeField]
    private Player player;
    [SerializeField]
    private RectTransform healthBar;

    private void Update()
    {
        usernameText.text = player.username;
        healthBar.localScale = new Vector3(player.GetHealthPercentage(), 1f, 1f);
    }
}
