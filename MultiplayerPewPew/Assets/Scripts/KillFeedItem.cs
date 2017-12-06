using UnityEngine.UI;
using UnityEngine;

public class KillFeedItem : MonoBehaviour {

    [SerializeField]
    private Text text;

    public void Setup(string player, string source)
    {
        text.text = "<color=blue>" + source + "</color> killed <color=red>" + player + "</color>"; 
    }
}
