using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType { None, Lever }

public class InteractableObject : MonoBehaviour, IRestartable
{
    [SerializeField] protected float windCost;
    [SerializeField] protected float currentWind;

    [SerializeField] protected bool activated = false;

    [SerializeField] protected float cooldown = 60f;
    protected Coroutine cooldownRoutine;
    protected bool inCooldown;
    protected float gotWind;

    protected Player player;

    InteractableObjectUI objectUI;

    [SerializeField] InteractionType interactionType;

    [Header("Lever")]
    [SerializeField] Transform lever;
    [SerializeField] Vector2 angleLimit;
    [SerializeField] string axis = "x";

    protected AudioSource source;
    [SerializeField] ParticleSystem onTriggerVFX;

    protected bool pressing;

    protected virtual void Awake()
    {
        objectUI = GetComponentInChildren<InteractableObjectUI>();
        source = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        player = GameManager.gm.player;
        ObjectAnimation(0);
    }

    public virtual void WindUp(float windUpValue)
    {
        currentWind += windUpValue;
        objectUI.Fill(currentWind, windCost);
        ObjectAnimation(currentWind / windCost);

        if (currentWind >= windCost) Active();
    }

    public virtual void Active()
    {
        currentWind = windCost;
        ObjectAnimation(1);

        activated = true;

        cooldownRoutine = StartCoroutine(Cooldown());
        if (source) source.Play();
        if (onTriggerVFX) onTriggerVFX.Play();
    }

    protected IEnumerator Cooldown()
    {
        inCooldown = true;
        float time = 1;

        while (time > 0)
        {
            objectUI.Fill(time);
            ObjectAnimation(time);
            time -= Time.deltaTime / cooldown;
            yield return new WaitForEndOfFrame();
        }
        objectUI.Fill(0);

        Restart();
    }

    protected virtual void ObjectAnimation(float rate)
    {
        switch (interactionType)
        {
            case InteractionType.None:
                break;
            case InteractionType.Lever:
                float angle = Mathf.Lerp(angleLimit.x, angleLimit.y, rate);

                lever.transform.localEulerAngles = new Vector3((axis == "x") ? angle : 0, (axis == "y") ? angle : 0, (axis == "z") ? angle : 0);
                break;
            default:
                break;
        }        
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.E) && !activated)
        {
            pressing = true;
            gotWind = player.GetWind();
            WindUp(gotWind);
        }
        else
        {
            pressing = false;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        objectUI.Toggle(true);
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        objectUI.Toggle(false);
        pressing = false;
    }

    public virtual void Restart()
    {
        currentWind = 0;
        objectUI.Fill(0, windCost);

        activated = false;
        ObjectAnimation(0);

        if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
        inCooldown = false;
    }
}
