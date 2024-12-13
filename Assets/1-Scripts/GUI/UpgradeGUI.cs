using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeGUI : BaseLayoutElement
{
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] TextMeshProUGUI _level;

    [SerializeField] Image _image;

    [SerializeField] Image _backgroudImage;
    [SerializeField] Color _upgradeColor;
    [SerializeField] Color _skillColor;

    Upgrade myUpgrade;
    public Upgrade Ugprade { get { return myUpgrade; } }

    ChooseUpgradeGUI chooseUpgradeGUI;
    public ChooseUpgradeGUI ChooseUpgradeGUI { set { chooseUpgradeGUI = value; } }

    public void Set(Upgrade upgrade)
    {
        upgrade.UpdateDescription();

        _name.text = upgrade.Name;
        _description.text = upgrade.UpdatedDescription;
        _level.text = $"Level: {upgrade.Level}";

        _image.sprite = upgrade.sprite;

        myUpgrade = upgrade;

        _backgroudImage.color = (upgrade is StatUpgrade) ? _upgradeColor : _skillColor;
    }

    public void Choose()
    {
        chooseUpgradeGUI.Hide();
        GameManager.gm.PauseGame(false);

        GameManager.gm.AddUpgradeToPlayer(myUpgrade);
    }
}
