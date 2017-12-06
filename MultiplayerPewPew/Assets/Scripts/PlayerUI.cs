using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform thrusterFuelAmount;
    [SerializeField]
    private RectTransform healthBarAmount;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject scoreboard;
    [SerializeField]
    private Text ammoCountText;

    private PlayerController controller;
    private Player player;
    private WeaponManager weaponManager;

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    private void SetFuelAmount(float amount)
    {
        thrusterFuelAmount.localScale = new Vector3(1f, amount, 1f);
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void SetHealthAmount(float amount)
    {
        healthBarAmount.localScale = new Vector3(1f, amount, 1f);
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPercentage());
        SetAmmoCount(weaponManager.GetCurrentWeapon().bullets);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    private void SetAmmoCount(int amount)
    {
        ammoCountText.text = amount.ToString();
    }
}
