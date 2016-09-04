using UnityEngine;
using System.Collections;
using System;

public class Boss : Character
{
    // Testing...
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
    private bool getPlayerPosOnce;
    private bool didSetupAttack;
    public Transform[] g_patrolPoints;
    private int m_destinationPoint = 0;
    private NavMeshAgent m_agent;
    public Transform jumpHeightPosition;
    private bool m_didJump;
    Vector3 playerLastPos;
    public GameObject[] g_landingPos;
    private int m_randomLandingPointIndex;
    private bool m_didGetLandingPoint;
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        m_agent = this.GetComponent<NavMeshAgent>();
        EnableNavMeshAgent(true);
        SetSpeed(3);
        m_agent.speed = GetSpeed();
        didSetupAttack = false;
        m_didGetLandingPoint = false;
        
    }

    public void Update()
    {
        
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
                    //if ((Vector3.Distance(transform.position, m_player.transform.position) < 15) && m_player.GetComponent<CharacterController>().isGrounded == true)
                    //{
                    //    Debug.Log("Player Is Grounded");
                    //}
                    //Debug.Log(((Vector3.Distance(transform.position, m_player.transform.position))));
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
        // Checks if the agent is close to the patrolPoints.
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
        if (!didSetupAttack)
        {
            EnableNavMeshAgent(false);
            SetupAbilityAttack();
        }
        if (m_timer >= g_chargeTimer)
        {
            // Gets the player's last position before chargeAttack
            if (getPlayerPosOnce)
            {
                playerLastPos  = new Vector3(m_player.transform.position.x,transform.position.y,m_player.transform.position.z);
                getPlayerPosOnce = false;
                didSetupAttack = true;
            }

            PerformPositionLerp(this.transform.position, playerLastPos, g_chargeSpeed);

            // Checks if the this agent is close to the player's last position to end the charge attack.
            if (Vector3.Distance(this.transform.position, playerLastPos) < 1f)
            {
                EnableNavMeshAgent(true);
                SetState(EnemyState.Walking);
                m_timer = 0;
                didSetupAttack = false;
            }
        }
    }
  
    void PerformJumpAttack()
    {
        if (!didSetupAttack)
        {
            EnableNavMeshAgent(false);
            SetupAbilityAttack();
            
        }
        
        if(m_timer >= g_delayJump && m_didJump == false)
        {
            
            PerformPositionLerp(this.transform.position, jumpHeightPosition.position, 2);
            g_landingPos[m_randomLandingPointIndex].GetComponent<SpriteRenderer>().enabled = true;
        }

        if (m_timer >= g_landingTimer)
        {
            m_didJump = true;
            didSetupAttack = true;
            PerformPositionLerp(transform.position, g_landingPos[m_randomLandingPointIndex].transform.position, g_LandingSpeed);
            if (Vector3.Distance(this.transform.position, g_landingPos[m_randomLandingPointIndex].transform.position) < 2f)
            {
                EnableNavMeshAgent(true);
            }
        }
        
        if (GetComponent<NavMeshAgent>().isOnNavMesh == true)
        {
            
            // checks if the player is close to the agent once landed & if the player is grounded to deal damage
            if ((Vector3.Distance(transform.position, m_player.transform.position) < 15) && m_player.GetComponent<CharacterController>().isGrounded == true)
            {
                Debug.Log("Player Is Grounded");
            }
            g_landingPos[m_randomLandingPointIndex].GetComponent<SpriteRenderer>().enabled = false;
            cameraShake.Shake(0.1f, 0.3f);
            m_didJump = false;
            didSetupAttack = false;
            m_didGetLandingPoint = false;
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
    void SetupAbilityAttack()
    {
        if (!m_didGetLandingPoint)
        {
            m_randomLandingPointIndex = UnityEngine.Random.Range(0, g_landingPos.Length);
            m_didGetLandingPoint = true;
        }
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
   

 
