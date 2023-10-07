using Polyperfect.Animals;
using Polyperfect.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Polyperfect.Common.Common_WanderScript;

public class Animal_AI : AI
{
    [SerializeField]
    public Animal_Stats stats;

    public float idleT = 3;
    public float eatMul = 5;

    [SerializeField, Range(0, 100)]
    public float staminaLowerPerc = 20f;
    [SerializeField, Range(0, 100)]
    public float staminaUpperPerc = 90f;
    [SerializeField, Range(0, 100)]
    public float hungerLowerPerc = 50f;

    public bool doLog = false;


    private enum State {Wandering, Idle, Searching, Eating, Chasing, Fleeing, Dead}
    private State state;

    private const float contingencyDistance = 1f;
    private AI target;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 startPos;
    private Vector3 targetPosition;

    private readonly List<string> AnimNames = new List<string>() { "isWalking", "isDead", "isEating", "isRunning", "isAttacking" };

    private float idleTimer;
    private float stamina;
    private float hunger;

    private void Awake()
    {
        state = State.Idle;
        stamina = stats.stamina;
        hunger = stats.hunger;
        target = null;
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        idleTimer = 0;

        startPos = this.transform.position;
        targetPosition = startPos;

        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent)
        {
            //useNavMesh = true;
            navMeshAgent.stoppingDistance = contingencyDistance;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (doLog)
        {
            DebugSplash();
        }

        switch(state)
        {
            case State.Idle:
                HandleIdle();
                UpdateAI();
                break;

            case State.Wandering:
                HandleWander();
                UpdateAI();
                break;

            case State.Searching:
                HandleSearching();
                UpdateAI();
                break;

            case State.Eating:
                HandleEating();
                UpdateAI();
                break;
        }



        // Tell navmesh where to go 
        if (navMeshAgent)
        {
            navMeshAgent.destination = targetPosition;
            navMeshAgent.speed = stats.speed;
            navMeshAgent.angularSpeed = stats.angularSpeed;
        }
    }

    private AI closestAI(AI_type type)
    {
        var position = transform.position;
        AI closest = this;

        // if awareness is greater than 0
        if (stats.awareness > 0)
        {
            var closestPos = stats.awareness;
            // if any AI exist in scene
            if (AllAI.Count > 0)
            {
                // for every AI
                foreach (AI ai in AllAI)
                {
                    // if AI is Animal
                    if (ai.AI_TYPE == type)
                    {
                        //Debug.Log("closest: " + ai);
                        var distance = Vector3.Distance(position, ai.transform.position);

                        if (distance < closestPos)
                        {
                            closestPos = distance;
                            closest = ai;
                        }
                    }
                }
            }
        }

        return closest;
    }

    
    void UpdateAI()
    {
        // Se rileva presenza ostile
        // codice


        if (hunger >= stats.hunger)
        {
            state = State.Wandering;
        }

        // Se non rileva pericoli e non ha fame
        float bottomBound = (stats.stamina / 100) * staminaLowerPerc;
        if (stamina<=bottomBound && state != State.Eating)
        {
            state = State.Idle;
        }


        float upperBound = (stats.stamina / 100) * staminaUpperPerc;
        if(stamina>upperBound)
        {
            state = State.Wandering;
        }

        float hungerBound = (stats.hunger / 100) * hungerLowerPerc;
        if(hunger<hungerBound && state != State.Eating)
        {
            state = State.Searching;
        }
    }

    void HandleIdle()
    {
        SetAnim(AnimNames[0], false);
        state = State.Idle;

        Idle();
    }

    void Idle()
    {
        target = null;
        targetPosition = this.transform.position;
        idleTimer += Time.deltaTime;
        stamina = Mathf.MoveTowards(stamina, stats.stamina, Time.deltaTime);
        hunger = Mathf.MoveTowards(hunger, 0, Time.deltaTime);
        FaceDirection((targetPosition - this.transform.position).normalized);
    }

    void HandleWander()
    {
        SetAnim(AnimNames[0], true);
        state = State.Wandering;
        float handle = 1f;

        stamina = Mathf.MoveTowards(stamina, 0, Time.deltaTime);
        hunger = Mathf.MoveTowards(hunger, 0, Time.deltaTime);

        if (Vector3.Distance(this.transform.position, startPos) > stats.wanderZone)
        {
            targetPosition = startPos;
        }


        if (Vector3.Distance(this.transform.position, targetPosition) < handle)
        {
            Wander();
        }
    }

    void Wander()
    {
        target = null;
        var rand = Random.insideUnitSphere * stats.wanderZone;
        targetPosition = startPos + rand;
        ValidatePosition(ref targetPosition);

        FaceDirection((targetPosition - this.transform.position).normalized);

        //target = targetPos;
        //SetMoveSlow();
    }

    void HandleSearching()
    {
        if(target is null || target.AI_TYPE == AI_type.ANIMAL)
        {
            Search();
        }

        SetAnim(AnimNames[0], true);
        stamina = Mathf.MoveTowards(stamina, 0, Time.deltaTime);
        hunger = Mathf.MoveTowards(hunger, 0, Time.deltaTime);

        float handle = 0.3f;

        if (Vector3.Distance(this.transform.position, targetPosition) < handle)
        {
            state = State.Eating;
        }
    }

    void Search()
    {
        target = closestAI(AI_type.PLANT);
        targetPosition = target.transform.position;
        ValidatePosition(ref targetPosition);

        FaceDirection((targetPosition - this.transform.position).normalized);
    }

    void HandleEating()
    {
        if(target.AI_TYPE != AI_type.PLANT)
        {
            state = State.Searching;
            return;
        }

        SetAnim(AnimNames[2], true);
        Eat();
    }

    void Eat()
    {
        targetPosition = target.transform.position;
        hunger = Mathf.MoveTowards(hunger, stats.hunger, Time.deltaTime * eatMul);
        stamina = Mathf.MoveTowards(stamina, stats.stamina, Time.deltaTime);
        FaceDirection((targetPosition - this.transform.position).normalized);
    }

    void ValidatePosition(ref Vector3 targetPos)
    {
        if (navMeshAgent)
        {
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(targetPos, out hit, Mathf.Infinity, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                Debug.LogError("Unable to sample nav mesh. Please ensure there's a Nav Mesh layer with the name Walkable");
                enabled = false;
                return;
            }

            targetPos = hit.position;
        }
    }

    void FaceDirection(Vector3 facePosition)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.RotateTowards(transform.forward,
            facePosition, stats.angularSpeed * Time.deltaTime * Mathf.Deg2Rad, 0f), Vector3.up), Vector3.up);
    }

    void SetAnim(string animName, bool value)
    {
        foreach (string anim in AnimNames)
        {
            animator.SetBool(anim, false);
        }

        animator.SetBool(animName, value);
    }

    void OnEnable()
    {
        AllAI.Add(this);
    }

    void OnDisable()
    {
        AllAI.Remove(this);
        StopAllCoroutines();
    }

    void DebugSplash()
    {
        Debug.Log("Hunger: " + hunger);
        Debug.Log("Stamina: " + stamina);
        Debug.Log("Target: " + target);
        Debug.Log("State: " + state);
    }
}
