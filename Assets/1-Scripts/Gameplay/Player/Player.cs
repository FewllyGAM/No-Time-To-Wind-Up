using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IRestartable
{
    HUD hud;
    CursorManager cursor;
    VFXSpawner vfxSpawner;

    Collider mainCollider;
    WindingKey windingKey;

    [SerializeField] float speed = 1f;
    [SerializeField] float rotateSpeed = 10f;

    [SerializeField] float stoppedTimeToWind = 1.5f;
    float notSpendingTime = 0;

    [SerializeField] float costToMove = 1f;

    Vector3 dir;

    [SerializeField] float initialWind = 25f;
    [SerializeField] float maxWind = 100f;
    [SerializeField] float windUpPower = 5f;
    [SerializeField] float windUpCooldown = .5f;

    [SerializeField] float passiveWindUp = 0f;
    [SerializeField] float windTime;
    bool canWindUp;
    //Coroutine manualWindUpRoutine;

    [SerializeField] float interactionSpeed = 12f;

    [SerializeField] int woundTimes = 0;

    //Percentage values
    [SerializeField] float bonusSpeed = 1.0f;
    public float BonusSpeed { get { return bonusSpeed; } set { bonusSpeed = value; } }

    //[Header("Upgradable values")]
    Dictionary<UpgrableStat, float> upgradableStats;
    float Speed => (speed * bonusSpeed * (1 + upgradableStats[UpgrableStat.Speed]));// + upgradableStats[UpgrableStat.Speed];
    float WindUpPower => windUpPower + upgradableStats[UpgrableStat.WindUpPower];
    float WindUpCooldown => windUpCooldown + upgradableStats[UpgrableStat.WindUpCooldown];
    float WindUpSpeed => passiveWindUp + upgradableStats[UpgrableStat.WindUpSpeed];
    float TimeToStartWindUp => stoppedTimeToWind - upgradableStats[UpgrableStat.TimeToStartWindUp];
    float InteractionSpeed => interactionSpeed * (1 + upgradableStats[UpgrableStat.InteractionSpeed]);// + upgradableStats[UpgrableStat.InteractionSpeed];
    float WalkEfficiency => upgradableStats[UpgrableStat.WalkEfficiency];
    float InteractionEfficiency => upgradableStats[UpgrableStat.InteractionEfficiency];

    [SerializeField] List<StatUpgrade> upgrades = new List<StatUpgrade>();
    //public List<Upgrade> Upgrades { get { return upgrades; } }
    [SerializeField] SkillUpgrade skill;
    public SkillUpgrade Skill { get { return skill; } }

    [Header("Helmet Light")]
    [SerializeField] Light helmentLight;
    [SerializeField] float fadeLightTime = .5f;
    [HideInInspector] public float startLightIntensity;
    [HideInInspector] public float baseLightIntensity;
    Coroutine lightingUpRoutine;

    [SerializeField] BrightLightEffect brightLight;

    [Header("General")]

    [SerializeField] bool invencible = false;
    [SerializeField] bool winding = false;
    [SerializeField] bool gameEnded = false;
    bool spending = false;
    float lastWindTime;

    [SerializeField] float collisionDetectionRange = .5f;
    LayerMask collisionMask;
    LayerMask playerClickMask;

    Animator animator;
    [SerializeField] float walkAnimSpeedFactor = .5f;

    [SerializeField] SkinnedMeshRenderer helmetMesh;
    [SerializeField] Material normalHelmet;
    [SerializeField] Material highlightedHelmet;
    bool hovering;

    [Header("VFX/SFX")]

    [SerializeField] AudioSource sfx_windup;
    [SerializeField] ParticleSystem vfx_windup;

    [SerializeField] AudioSource sfx_spendingWind;


    private void OnEnable()
    {
        GameEvents.OnDefeat += StopCommands;
        GameEvents.OnClear += StopCommands;
    }
    private void OnDisable()
    {
        GameEvents.OnDefeat -= StopCommands;
        GameEvents.OnClear -= StopCommands;

        if (skill) skill.SetActive(false);
    }

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
        helmentLight = GetComponentInChildren<Light>();
        startLightIntensity = helmentLight.intensity;
        baseLightIntensity = startLightIntensity;

        windingKey = GetComponentInChildren<WindingKey>();
        animator = GetComponentInChildren<Animator>();

        SetUpgrdableStats();
    }
    void SetUpgrdableStats()
    {
        windTime = initialWind;

        upgradableStats = new Dictionary<UpgrableStat, float>();
        for (int i = 0; i < Enum.GetNames(typeof(UpgrableStat)).Length; i++)
        {
            upgradableStats.Add((UpgrableStat)i, 0f);
        }
        upgradableStats[UpgrableStat.WalkEfficiency] = 1;
        upgradableStats[UpgrableStat.InteractionEfficiency] = 1;
    }

    private void Start()
    {
        collisionMask = LayerMask.GetMask("Wall");
        playerClickMask = LayerMask.GetMask("Player");
        hud = GameManager.gm.hud;
        cursor = GameManager.gm.cursor;
        vfxSpawner = GameManager.gm.vfxSpawner;
    }

    public void Update()
    {
        if (gameEnded) return;

        //if (!winding)
        //{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Mathf.Infinity, playerClickMask))
        {
            if (!hovering && canWindUp)
            {
                Highlight(true);
            }

            if (Input.GetMouseButtonDown(0) && !winding && canWindUp && !EventSystem.current.IsPointerOverGameObject())
            {
                //if (manualWindUpRoutine != null) return;
                woundTimes++;
                GameEvents.CallOnWindUp(woundTimes);
                StartCoroutine(WindUp(WindUpPower, WindUpCooldown));

                if (windTime < maxWind)
                {
                    float pitch = 1 - (WindUpCooldown / sfx_windup.clip.length) + 1;
                    sfx_windup.pitch = pitch;
                    sfx_windup.Play();
                }
            }
        }
        else
        {
            if (hovering)
            {
                Highlight(false);
            }
        }
        //}

        dir = Vector3.zero;

        if (windTime >= .2f)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (!Physics.Raycast(this.transform.position, Vector3.forward, collisionDetectionRange, collisionMask))
                    dir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (!Physics.Raycast(this.transform.position, Vector3.forward* -1, collisionDetectionRange, collisionMask))
                    dir += Vector3.forward * -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (!Physics.Raycast(this.transform.position, Vector3.right* -1, collisionDetectionRange, collisionMask))
                    dir += Vector3.right * -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!Physics.Raycast(this.transform.position, Vector3.right, collisionDetectionRange, collisionMask))
                    dir += Vector3.right;
            }
            dir.Normalize();
        }

        if (dir == Vector3.zero)
        {
            notSpendingTime += Time.deltaTime;
            if (notSpendingTime >= TimeToStartWindUp)
            {
                windTime += Time.deltaTime * WindUpSpeed;
                windTime = Mathf.Clamp(windTime, 0, maxWind);
                canWindUp = true;
                windingKey.SetInteractable(true);
            }
            animator.speed = 0;
        }
        else
        {
            //StopWinding();
            transform.position += dir * Time.deltaTime * Speed;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * rotateSpeed);

            StartSpending();

            windTime -= costToMove * Time.deltaTime * Speed * WalkEfficiency;

            CheckRemainingWind();

            animator.speed = Speed * walkAnimSpeedFactor;
        }
        if (spending && windTime == lastWindTime)
        {
            sfx_spendingWind.Stop();
            spending = false;
        }

        hud.UpdateTime(windTime);
        if (brightLight) brightLight.UpdateLight(windTime);
        windingKey.UpdateRotation(windTime);

        lastWindTime = windTime;
    }

    void Highlight(bool value)
    {
        hovering = value;
        if (value)
        {
            cursor.SetHover();
            helmetMesh.material = highlightedHelmet;
        }
        else
        {
            cursor.SetNormal();
            helmetMesh.material = normalHelmet;
        }
    }

    void StartSpending(bool interaction = false)
    {
        if (canWindUp)
        {
            Highlight(false);

            woundTimes = 0;
            notSpendingTime = 0;
            canWindUp = false;
            windingKey.SetInteractable(false);

            StopWinding();
        }
        if (!spending && !interaction)
        {
            spending = true;
            sfx_spendingWind.Play();
        }
    }

    void StopWinding()
    {
        if (!winding) return;

        StopCoroutine("WindUp");
        winding = false;
    }

    public float GetWind()
    {
        if (gameEnded) return 0 ;

        StopWinding();

        float gotWind = Time.deltaTime * InteractionSpeed;
        if (windTime >= gotWind)
        {
            notSpendingTime = 0;

            windTime -= gotWind * InteractionEfficiency;
            StartSpending(true);

            CheckRemainingWind();
            return gotWind;
        }
        else
        {
            return 0;
        }
    }

    public IEnumerator WindUp(float power, float time)
    {
        //Debug.Log("WINDUP");
        winding = true;
        float woundQuant = 0;
        if (windTime < maxWind) vfx_windup.Play();

        while (woundQuant < power)
        {
            float woundUp = (Time.deltaTime * power) / time;
            windTime += woundUp;
            windTime = Mathf.Clamp(windTime, 0, maxWind);
            woundQuant += woundUp;
            hud.UpdateTime(windTime);

            yield return new WaitForEndOfFrame();
        }
        windTime -= woundQuant - power;
        //hud.UpdateTime(windTime);
        //if (brightLight) brightLight.UpdateLight(windTime);

        //manualWindUpRoutine = null;
        winding = false;
    }

    public void AddUpgrade(StatUpgrade upgrade)
    {
        StatUpgrade up;
        if (TryGetUpgrade(upgrade.Name, out up))
        {
            if (up.doLevelUp) up.LevelUp();
        }
        else
        {
            up = Instantiate(upgrade);
            //up.LevelUp();
            upgrades.Add(up);
        }

        if (up.doChangeStat) up.ChangeValue(ref upgradableStats);


        for (int i = 0; i < Enum.GetNames(typeof(UpgrableStat)).Length; i++)
        {
            Debug.Log($"{(UpgrableStat)i} is {upgradableStats[(UpgrableStat)i]}");
        }
    }

    public bool TryGetUpgrade(string _name, out StatUpgrade upgrade)
    {
        upgrade = FindUpgrade(_name);
        return upgrade;
    }

    StatUpgrade FindUpgrade(string _name)
    {
        int index = upgrades.FindIndex(x => x.Name.Equals(_name));
        if (index < 0) return null;
        else return upgrades[index];
    }

    public void SetSkill(SkillUpgrade _skill)
    {
        if (!skill)
        {
            skill = Instantiate(_skill);
            if (skill.active) skill.SetActive(true);
            if (skill.skillObject)
            {
                skill.skillObject = Instantiate(skill.skillObject, this.transform);
                brightLight = skill.skillObject.GetComponent<BrightLightEffect>();
                if (brightLight) brightLight.Initialize(this, skill, helmentLight);
            }
            hud.skillCD.SetSkill(skill.sprite);
            hud.skillCD.Show();
        }
        else
        {
            skill.LevelUp();
        }
    }

    public void TriggerSkill()
    {
        StartCoroutine(SkillCooldown(skill.cooldown));

        if (skill.grantInvencibility) invencible = true;
        if (skill.acceleratePlayer) bonusSpeed += skill.power;

        vfxSpawner.SpawnVFX(skill.vfx, this.transform, (skill.setVFXDuration ? skill.duration : 0));

        StartCoroutine(SkillDuration(skill.duration));
    }

    IEnumerator SkillDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (skill.grantInvencibility)
        {
            invencible = false;
            mainCollider.enabled = false;
            mainCollider.enabled = true;
        }
        bonusSpeed = 1;
    }

    IEnumerator SkillCooldown(float cooldown)
    {
        //Debug.Log("ENTER SKILL COOLDOWN");

        skill.inCooldown = true;
        hud.skillCD.SkillCooldown(cooldown);
        yield return new WaitForSeconds(cooldown);
        skill.inCooldown = false;
    }

    public void OnHit()
    {
        GameEvents.CallOnHitPlayer();
        if (invencible) return;
        GameEvents.CallOnDefeat();
    }

    public void StopCommands()
    {
        gameEnded = true;
    }

    //Chamado quando o jogador está continuamente gastando corda
    void CheckRemainingWind()
    {
        if (windTime < .2f)
        {
            //Esgotou a corda toda, pode ter algum evento nisso

            LightOffHelmet();
        }
    }

    void LightOffHelmet()
    {
        helmentLight.intensity = 0;
        if (lightingUpRoutine != null) StopCoroutine(lightingUpRoutine);

        LightHelmet();
    }
    void LightHelmet()
    {
        lightingUpRoutine = StartCoroutine(LightHelmetRoutine());
    }

    protected virtual IEnumerator LightHelmetRoutine()
    {
        yield return new WaitForSeconds(TimeToStartWindUp + .5f);
        float time = 0;

        while (time < 1)
        {
            helmentLight.intensity = Mathf.Lerp(0, baseLightIntensity, time);
            time += Time.deltaTime / fadeLightTime;
            yield return new WaitForEndOfFrame();
        }

        helmentLight.intensity = baseLightIntensity;
    }

    public void Restart()
    {
        StopAllCoroutines();
        baseLightIntensity = startLightIntensity;
        LightHelmet();

        SetUpgrdableStats();

        upgrades.Clear();
        if (skill)
        {
            if (skill.skillObject) Destroy(skill.skillObject);
            brightLight = null;
            skill = null;
        }

        TeleportToSpawn();
        StopAudios();

        notSpendingTime = 0;
        gameEnded = false;
    }

    public void StopAudios()
    {
        animator.speed = 0;
        sfx_spendingWind.Stop();
    }

    public void TeleportToSpawn()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
