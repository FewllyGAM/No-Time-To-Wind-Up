using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour, IRestartable
{
    [SerializeField] BaseWindow pauseMenu;
    [SerializeField] BaseWindow victoryScreen;
    [SerializeField] BaseWindow defeatScreen;

    [SerializeField] TextMeshProUGUI timeText;

    ChooseUpgradeGUI chooseUpgradeGUI;
    public ChooseUpgradeGUI ChooseUpgradeGUI { get { return chooseUpgradeGUI; } }

    [SerializeField] BaseWindow initialScreen;

    public SkillCD skillCD;

    private void OnEnable()
    {
        GameEvents.OnDefeat += DefeatScreen;
        GameEvents.OnClear += VictoryScreen;
    }
    private void OnDisable()
    {
        GameEvents.OnDefeat -= DefeatScreen;
        GameEvents.OnClear -= VictoryScreen;
    }

    private void Awake()
    {
        chooseUpgradeGUI = GetComponentInChildren<ChooseUpgradeGUI>();
        skillCD = GetComponentInChildren<SkillCD>();
    }

    private void Start()
    {
        initialScreen.Show();
    }

    public void StartGame()
    {
        initialScreen.Hide();
    }

    public void UpdateTime(float time)
    {
        timeText.text = time.ToString("0");
    }

    public void TogglePauseMenu(bool value)
    {
        if (value) pauseMenu.Show();
        else pauseMenu.Hide();
    }

    public void VictoryScreen()
    {
        victoryScreen.Show();
    }
    public void DefeatScreen()
    {
        defeatScreen.Show();
    }

    public void ResumeGame()
    {
        GameManager.gm.PauseGame();
    }
    //Chamado ao clicar no botão na interface
    public void RestartGame()
    {
        GameManager.gm.Restart();
    }
    public void QuitGame()
    {
        GameManager.gm.QuitGame();
    }

    //Chamado pelo GameManager após reiniciar
    public void Restart()
    {
        victoryScreen.Hide();
        defeatScreen.Hide();
    }
}
