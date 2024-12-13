using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public HUD hud;
    public Player player;
    public EnemiesManager enemiesManager;
    public CursorManager cursor;
    public VFXSpawner vfxSpawner;

    public bool gamePaused = false;
    public bool choosingUpgrade = false;

    [SerializeField] StatUpgrade[] allUpgrades;

    [SerializeField] SkillUpgrade[] allSkills;

    bool started = false;
    bool locked = true;

    private void OnEnable()
    {
        GameEvents.OnDefeat += EndGame;
        GameEvents.OnClear += ClearGame;
    }
    private void OnDisable()
    {
        GameEvents.OnDefeat -= EndGame;
        GameEvents.OnClear -= ClearGame;
    }

    private void Awake()
    {
        if (gm == null) gm = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        PauseGame(false);
        StartCoroutine(Unlock());
    }

    IEnumerator Unlock()
    {
        yield return new WaitForSecondsRealtime(.5f);
        locked = false;
    }

    //Upgrades
    public void ChooseStatUpgradeOptions()
    {
        List<StatUpgrade> chosenUpgrades = new List<StatUpgrade>(), availableUpgrades = new List<StatUpgrade>();
        availableUpgrades.AddRange(allUpgrades);
        for (int i = availableUpgrades.Count - 1; i >= 0; i--)
        {
            StatUpgrade upgrade;
            if (player.TryGetUpgrade(availableUpgrades[i].Name, out upgrade))
            {
                if (!upgrade.doLevelUp || upgrade.Level == upgrade.maxLevel) availableUpgrades.RemoveAt(i);
                else
                {
                    upgrade = Instantiate(upgrade);
                    upgrade.LevelUp();
                    availableUpgrades[i] = upgrade;
                }
            }
        }
        int count = availableUpgrades.Count > 3 ? 3 : availableUpgrades.Count;
        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, availableUpgrades.Count);

            chosenUpgrades.Add(availableUpgrades[rand]);
            availableUpgrades.RemoveAt(rand);

            Debug.Log($"Choosed {chosenUpgrades[i]}");
        }

        if (chosenUpgrades.Count == 0) return;
        GameEvents.CallOnChooseStatUpgrade(chosenUpgrades.ToArray());
    }

    public void ChooseSkillUpgradeOptions()
    {
        List<SkillUpgrade> chosenUpgrades = new List<SkillUpgrade>(), availableUpgrades = new List<SkillUpgrade>();
        if (player.Skill)
        {
            if (player.Skill.Level == player.Skill.maxLevel) return;
            SkillUpgrade skill = Instantiate(player.Skill);
            skill.LevelUp();
            chosenUpgrades.Add(skill);
        }
        else
        {
            availableUpgrades.AddRange(allSkills);
            for (int i = 0; i < 3; i++)
            {
                int rand = UnityEngine.Random.Range(0, availableUpgrades.Count);

                chosenUpgrades.Add(availableUpgrades[rand]);
                availableUpgrades.RemoveAt(rand);

                Debug.Log($"Choosed {chosenUpgrades[i]}");
            }
        }

        GameEvents.CallOnChooseSkillUpgrade(chosenUpgrades.ToArray());
    }

    public void AddUpgradeToPlayer(Upgrade upgrade)
    {
        if (upgrade is StatUpgrade) player.AddUpgrade((StatUpgrade)upgrade);
        else player.SetSkill((SkillUpgrade)upgrade);
    }

    public void ClearGame()
    {
        player.StopAudios();
    }
    public void EndGame()
    {
        Debug.Log("DEFEAT");
    }

    public void PauseGame(bool openMenu = true)
    {
        if (openMenu) hud.TogglePauseMenu(!gamePaused);
        gamePaused = !gamePaused;

        Time.timeScale = gamePaused ? 0 : 1;
    }

    public void Restart()
    {
        var restartableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IRestartable>();

        foreach (IRestartable obj in restartableObjects)
        {
            obj.Restart();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !choosingUpgrade && started)
        {
            PauseGame();
        }

        if (Input.anyKeyDown && !started && !locked)
        {
            started = true;
            hud.StartGame();
            PauseGame(false);
        }
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GameEvents.CallOnFreezeEnemies(5f);
        //}
    }
}

public class GameEvents
{
    public static Action OnDefeat;
    public static Action OnClear;

    public static Action<Upgrade[]> OnChooseStatUpgrade;
    public static Action<Upgrade[]> OnChooseSkillUpgrade;

    public static Action<float> OnFreezeEnemies;
    public static Action<float, float, float> OnSlowEnemies;

    public static Action OnHitPlayer;
    public static Action<int> OnWindUp;

    public static void CallOnDefeat()
    {
        OnDefeat?.Invoke();
    }

    public static void CallOnClear()
    {
        OnClear?.Invoke();
    }

    public static void CallOnChooseStatUpgrade(Upgrade[] upgrades)
    {
        OnChooseStatUpgrade?.Invoke(upgrades);
    }
    public static void CallOnChooseSkillUpgrade(Upgrade[] upgrades)
    {
        OnChooseSkillUpgrade?.Invoke(upgrades);
    }

    public static void CallOnFreezeEnemies(float freezeTime)
    {
        OnFreezeEnemies?.Invoke(freezeTime);
    }
    public static void CallOnSlowEnemies(float slowPower, float slowDuration, float slowRange)
    {
        OnSlowEnemies?.Invoke(slowPower, slowDuration, slowRange);
    }

    public static void CallOnHitPlayer()
    {
        OnHitPlayer?.Invoke();
    }
    public static void CallOnWindUp(int count)
    {
        OnWindUp?.Invoke(count);
    }
}
