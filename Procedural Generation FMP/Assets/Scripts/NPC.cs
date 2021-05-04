using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    Vector3 targetPos;
    public LayerMask detectionMask;
    public LayerMask unwalkableMask;
    Transform target;
    public int idleMoveArea = 5;
    public int minIdleTime, maxIdleTime;

    public float safeDistance;

    Vector3 safeOrigin;

    System.Random rand;

    float pauseTimer;

    public enum State
    {
        Idle,
        Approaching,
        Hunting,
        Fleeing
    }

    public enum Behaviour
    {
        Passive,
        Preditor,
        Prey
    }

    public State npcState;
    public Behaviour npcBehaviour;
    public bool active = false;

    public AudioClip[] sounds;
    public float soundFrequency = 7;
    float soundTimer;

    private void Start()
    {
        safeOrigin = transform.position;
        rand = new System.Random((int)(transform.position.x + transform.position.y));

        pauseTimer = (minIdleTime + maxIdleTime) / 2f;
    }

    private void OnBecameVisible()
    {
        active = true;
        Debug.Log("Visible");
    }
    private void OnBecameInvisible()
    {
        active = false;
        Debug.Log("Invisible");
    }

    private void Update()
    {
        if (!active)
            return;

        if(npcBehaviour != Behaviour.Passive)
            CheckForTarget();
        SetTargetPosition();

        switch (npcState)
        {
            case State.Idle:
                move = Vector3.zero;
                speed = moveSpeed;
                if (pauseTimer <= 0)
                {             
                    var offset = new Vector3(rand.Next(-idleMoveArea, idleMoveArea), rand.Next(-idleMoveArea, idleMoveArea), 0);
                    targetPos = safeOrigin + offset;
                    npcState = State.Approaching;
                }
                pauseTimer -= Time.deltaTime;
                break;
            case State.Approaching:
                move = targetPos - transform.position;
                var dist = Vector3.Distance(transform.position, targetPos);
                if (dist < 0.1f)
                {
                    npcState = State.Idle;
                    pauseTimer = rand.Next(minIdleTime, maxIdleTime);
                }
                break;
            case State.Hunting:
                move = targetPos - transform.position;
                speed = sprintSpeed;
                var dist2 = Vector3.Distance(transform.position, targetPos);
                if (dist2 > safeDistance)
                {
                    npcState = State.Idle;
                    target = null;
                }
                break;
            case State.Fleeing:
                move = transform.position - targetPos;
                speed = sprintSpeed;
                if (Vector3.Distance(transform.position, targetPos) > safeDistance * 2)
                {
                    npcState = State.Idle;
                    target = null;
                }
                break;
        }

        UpdateAnimator();

        soundTimer -= Time.deltaTime;
    }

    void CheckForTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, safeDistance, detectionMask);

        foreach (var collider in colliders)
        {
            if (collider != null && collider.tag != tag)
            {
                switch (npcBehaviour)
                {
                    case Behaviour.Passive:
                        break;
                    case Behaviour.Preditor:
                        targetPos = collider.transform.position;
                        npcState = State.Hunting;
                        break;
                    case Behaviour.Prey:
                        targetPos = collider.transform.position;
                        npcState = State.Fleeing;
                        break;
                }

                target = collider.transform;
                PlaySFX();
                return;
            }
        }
    }

    void SetTargetPosition()
    {
        if (target != null)
            targetPos = target.position;
    }

    private void FixedUpdate()
    {
        if (!active)
            return;

        MoveTowardsTarget();
    }

    protected override void MoveTowardsTarget()
    {
        base.MoveTowardsTarget();
    }

    void PlaySFX()
    {
        if (soundTimer <= 0)
        {
            if (sounds.Length > 0)
                AudioManager.PlayRandomSound(sounds);

            soundTimer = soundFrequency;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, idleMoveArea);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
