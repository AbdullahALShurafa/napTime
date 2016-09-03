using UnityEngine;
using System.Collections;
using System;

public class Boss : Character
{
    enum EnemyState
    {
        Walking,
        Charge,
        Jump,
        Firepit,
        Dead
    };
    EnemyState state = EnemyState.Walking;
    float m_timer;
    public float g_chargeTimer;
    public float g_chargeSpeed;
    public float g_delayJump;
    public float g_LandingSpeed;
    public float g_landingTimer;
    GameObject m_player;
    Rigidbody rb;
    bool getPlayerPosOnce;
    public Transform[] g_patrolPoints;
    private int m_destinationPoint = 0;
    private NavMeshAgent m_agent;
    public Transform jumpHeightPosition;
    private bool m_didJump;
    Vector3 playerLastPos;
    public GameObject g_landingPos;
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        m_agent = GetComponent<NavMeshAgent>();
        EnableNavMeshAgent(true);
        SetSpeed(3);
        m_agent.speed = GetSpeed();
        
    }

    public void Update()
    {
       

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("ITS WORKING!!");
            Debug.Log(m_player);

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetState(EnemyState.Charge);
            getPlayerPosOnce = true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetState(EnemyState.Jump);
            getPlayerPosOnce = true;
        }
        switch (state)
        {
            case EnemyState.Walking:
                {
                    OnWalking();
                }
                break;
            case EnemyState.Charge:
                {
                    PerformChargeAttack();
                }
                break;
            case EnemyState.Jump:
                {
                    PerformJumpAttack();
                }
                break;
            case EnemyState.Firepit:
                {

                }
                break;
           case EnemyState.Dead:
                {

                }
                break;
            default:
                break;
        }
    }

    void OnWalking()
    {
        if (m_agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }
    void GoToNextPoint()
    {
        if (g_patrolPoints.Length == 0)
        {
            return;
        }
        m_agent.destination = g_patrolPoints[m_destinationPoint].position;
        m_destinationPoint = (m_destinationPoint + 1) % g_patrolPoints.Length;
    }

    // MAKE SURE TO RESTRICT THE BOSS DASH DEPENDING ON MAP
    void PerformChargeAttack()
    {
        if (getPlayerPosOnce)
        {
            SetupAbilityAttack(false);
        }
        if (m_timer >= g_chargeTimer)
        {
            if (getPlayerPosOnce)
            {
                playerLastPos  = new Vector3(m_player.transform.position.x,transform.position.y,m_player.transform.position.z);
                getPlayerPosOnce = false;
            }
            PerformPositionLerp(this.transform.position, playerLastPos, g_chargeSpeed);
            if (Vector3.Distance(this.transform.position, playerLastPos) < 1f)
            {
                EnableNavMeshAgent(true);
                SetState(EnemyState.Walking);
                m_timer = 0;
            }
        }
    }
  
    void PerformJumpAttack()
    {
        if (getPlayerPosOnce)
        {
            SetupAbilityAttack(false);
        }
        
        if(m_timer >= g_delayJump && m_didJump == false)
        {
            PerformPositionLerp(this.transform.position, jumpHeightPosition.position, 2);
        }

        if (m_timer >= g_landingTimer)
        {
            m_didJump = true;
            if (getPlayerPosOnce)
            {
                playerLastPos = new Vector3(m_player.transform.position.x, 0, m_player.transform.position.z);
                getPlayerPosOnce = false;
            }

            PerformPositionLerp(transform.position, g_landingPos.transform.position, g_LandingSpeed);
            EnableNavMeshAgent(true);
        }
        if (GetComponent<NavMeshAgent>().isOnNavMesh == true)
        {
            //Debug.Log("Agent Is Grounded");
            if (m_player.GetComponent<CharacterController>().isGrounded == true)
            {
                // Deal Damage if player is grounded once the boss lands on the ground
                Debug.Log("Player Is Grounded");
            }

            cameraShake.Shake(0.1f, 0.3f);
            m_didJump = false;

            m_timer = 0;
            SetState(EnemyState.Walking);
        }

        //if (Vector3.Distance(this.transform.position, playerLastPos) < 1.5f)
        //{

        //    if (m_player.GetComponent<CharacterController>().isGrounded == true)
        //    {
        //        // Deal Damage if player is grounded once the boss lands on the ground
        //        Debug.Log("Player Is Grounded");
        //    }
        //    cameraShake.Shake(0.1f, 0.3f);
        //    m_didJump = false;

        //    EnableNavMeshAgent(true);
        //    m_timer = 0;
        //    SetState(EnemyState.Walking);
        //}
    }
    void SetupAbilityAttack(bool a_enableNavMeshAgent)
    {
        EnableNavMeshAgent(a_enableNavMeshAgent);
        m_timer += Time.deltaTime;
        LookAtPlayer();
    }
    void SetState(EnemyState a_state)
    {
        state = a_state;
    }
    void PerformPositionLerp(Vector3 a_vector1, Vector3 a_vector2,float a_speed)
    {
        transform.position = Vector3.Lerp(a_vector1, a_vector2, a_speed * Time.deltaTime);
    }
    void EnableNavMeshAgent(bool a_bool)
    {
        m_agent.GetComponent<NavMeshAgent>().enabled = a_bool;
    }
    void LookAtPlayer()
    {
        Vector3 playerPos = m_player.transform.position;
        transform.LookAt(playerPos);
    }
    void OnTriggerEnter(Collider obj)
    {
        if(obj.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Has Been Hit");
        }
    }
}   
   

 
