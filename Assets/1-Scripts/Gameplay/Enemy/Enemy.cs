using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    Player player;

    NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }
    WindingKey windingKey;

    Coroutine freezeRoutine;

    float currentSpeed;

    float slowRate = 0;
    public float SlowRate { get { return slowRate; } set { slowRate = value; } }

    Animator animator;
    [SerializeField] float walkAnimSpeedFactor = .5f;

    private void OnEnable()
    {
        GameEvents.OnDefeat += Stop;
        GameEvents.OnClear += Stop;

        GameEvents.OnFreezeEnemies += Freeze;
        GameEvents.OnSlowEnemies += TimedSlow;
    }
    private void OnDisable()
    {
        GameEvents.OnDefeat -= Stop;
        GameEvents.OnClear -= Stop;

        GameEvents.OnFreezeEnemies -= Freeze;
        GameEvents.OnSlowEnemies -= TimedSlow;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        windingKey = GetComponentInChildren<WindingKey>();
        animator = GetComponentInChildren<Animator>();

        currentSpeed = agent.speed;
        animator.speed = agent.speed * walkAnimSpeedFactor;
    }

    private void Start()
    {
        player = GameManager.gm.player;
    }

    private void FixedUpdate()
    {
        if (!agent.enabled) return;
        agent.SetDestination(player.transform.position);        
    }

    private void Update()
    {
        if (!agent.enabled) return;
        if (!agent.isStopped)
        {
            windingKey.UpdateContinuousRotation(agent.speed * Time.deltaTime);
            animator.speed = agent.speed * walkAnimSpeedFactor;
        }
        else animator.speed = 0;
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public void Freeze(float freezeTime)
    {
        Stop();
        if (freezeRoutine != null) StopCoroutine(freezeRoutine);
        freezeRoutine = StartCoroutine(Unfreeze(freezeTime));
    }

    IEnumerator Unfreeze(float freezeTime)
    {
        yield return new WaitForSeconds(freezeTime);

        agent.isStopped = false;
    }

    public void SpeedUp(float speed)
    {
        currentSpeed += speed;
        agent.speed = currentSpeed - currentSpeed * slowRate;
    }

    public void ApplySlow(float rate)
    {
        slowRate = rate;
        agent.speed -= agent.speed * slowRate;
    }

    public void TimedSlow(float slow, float duration, float range)
    {
        if ((GameManager.gm.player.transform.position - transform.position).sqrMagnitude > range * range) return;

        ApplySlow(slow);

        ClearSlow(duration);       
    }

    public void ClearSlow(float time = 0)
    {
        StartCoroutine(RemoveSlow(time));
    }

    IEnumerator RemoveSlow(float time)
    {
        yield return new WaitForSeconds(time);

        slowRate = 0;
        agent.speed = currentSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player")) player.OnHit();
    }
}
