using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD}

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;

    private CharacterStats characterStats;

    [Header("basic Settings")]

    //敌人追击玩家视野
    public float sightRadius;

    //是否是守卫状态的敌人，有的是站桩的，有的是巡逻的（还未实现）
    public bool isGuard;

    private float speed;

    //敌人的攻击目标（玩家）
    private GameObject attackTarget;

    private float lastAttackTime;


    //bool配合动画

    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;


    void Awake()
    {
       
            
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }

        //如果发现player，切换到chase

        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                //1.追玩家

                
                //4.配合动画
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //2.玩家跑了脱战,回到上一个状态（站桩or巡逻）
                    isFollow = false;
                    agent.destination = transform.position;
                }
                else
                {
                    //追击
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }

                //3.在攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    //技能冷却时间
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

                        //执行攻击
                        Attack();
                    }

                    

                }
                    



                break;
            case EnemyStates.DEAD:

                agent.enabled = false;

                Destroy(gameObject, 1f);
                break;

        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");

        }
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");

        }
        

    }

    //敌人查找身边玩家
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            //find the player
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;

    }

    //Animation Event

    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);

        }
       
    }
}


