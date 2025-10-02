using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;


    public float moveSpeed = 5f;

    public float traceRange = 15f;   //추적 시작 거리

    public float safeDistance = 30f; // 도망 후 안전 거리

    public float attackRange = 6f;   //공격 시작 거리

    public float attackCooldown = 1.5f;



    public GameObject projectilePrefab;   // 투사체 프리팹
    public Transform firePoint;   //발사 위치



    private Transform player;

    private float lastAttackTime;

    public int MaxHp = 5;

    private int currentHp;

    public Slider hpSlider;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown;
        currentHp = MaxHp;

        hpSlider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        float healthPercent = currentHp / MaxHp;

        //FSM 상태 전환
        switch (state)
        {
            case EnemyState.Idle:
                if (healthPercent <= 0.2f)
                    state = EnemyState.RunAway;

                if(dist < traceRange)
                    state = EnemyState.Trace;
                
            break;

            case EnemyState.Trace:  //추격할 때
                if (healthPercent <= 0.2f)
                    state = EnemyState.RunAway;//도망가기
                if (dist < attackRange)  //공격 사거리보다 작을 때
                    state = EnemyState.Attack;  //공격을 하고
                else if (dist > traceRange)     //공격 사거리보다 멀때
                    state = EnemyState.Idle;   //아이들 상태
                else
                    TracePlayer();  //아니면 따라가기
                break;

            case EnemyState.Attack:
                if (healthPercent <= 1/5f)
                    state = EnemyState.RunAway;
                if (dist > attackRange)
                    state = EnemyState.Trace;
                else
                    AttackPlayer();
                break;

            case EnemyState.RunAway:
                if (dist > safeDistance)
                    state = EnemyState.Idle;
                else
                    RunAway();
                break;

        }
    }

    void TracePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void RunAway()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void AttackPlayer()
    {
        //일정 쿨다운마다 발사
        if(Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }


    void ShootProjectile()
    {
        if(projectilePrefab != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            EnemyProjectTile ep = proj.GetComponent<EnemyProjectTile>();
            if ((ep != null))
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                ep.SetDirection(dir);
            }
        } 
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        hpSlider.value = (float)currentHp / MaxHp;

        if (currentHp <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
