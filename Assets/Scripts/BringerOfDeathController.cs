using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerOfDeathController : MonoBehaviour
{
    [SerializeField] private float m_attackRange = 1;
    [SerializeField] private GameObject m_projectile;

    private Transform m_player;
    private BaseEnemy m_baseEnemy;
    private Rigidbody2D m_rbody2D;
    private Animator m_animator;
    private BoxCollider2D m_attackColl;
    private Transform m_lifeBar;
    private bool m_shouldMove = false;
    private bool m_isDead = false;
    private float m_direction;
    private float m_timeSinceAttack;

    void Awake()
    {
        m_player = GameObject.Find("Player").transform;
        m_baseEnemy = GetComponent<BaseEnemy>();
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_attackColl = transform.GetChild(1).GetComponent<BoxCollider2D>();
        m_lifeBar = transform.GetChild(0);
    }

    void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_player.position.x - transform.position.x > 0)
        {
            m_direction = -m_attackRange;
            transform.localScale = new Vector3(-1, 1, 1);
            m_lifeBar.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            m_direction = m_attackRange;
            transform.localScale = Vector3.one;
            m_lifeBar.localScale = Vector3.one;
        }

        float moveX = (m_player.position.x + m_direction) - transform.position.x;
        float moveY = m_player.position.y - transform.position.y;

        if (Mathf.Abs(moveX) <= 0.1 && Mathf.Abs(moveY) <= 0.1)
        {
            m_shouldMove = false;
            m_animator.SetInteger("AnimState", 0);
        }
        if (m_shouldMove && m_timeSinceAttack >= 4.0f)
        {
            m_animator.SetTrigger("Attack2");
            m_timeSinceAttack = 0;
        }
        if (m_shouldMove)
        {
            m_rbody2D.velocity = (new Vector2(moveX, moveY)).normalized * m_baseEnemy.GetEnemySpeed();
        }
        else
        {
            m_rbody2D.velocity = Vector2.zero;
            if (!m_isDead)
            {
                if (Mathf.Abs(moveX) >= Mathf.Abs(m_direction) / 2.0f || Mathf.Abs(moveY) >= 0.1f)
                {
                    m_shouldMove = true;
                    m_animator.SetInteger("AnimState", 1);
                }
                else if (m_timeSinceAttack >= 3.0f)
                {
                    m_animator.SetTrigger("Attack1");
                    m_timeSinceAttack = 0;
                }
            }
        }
    }

    public void EnableAttack1()
    {
        m_animator.SetBool("IsAttaking", true);
        m_attackColl.enabled = true;
        m_attackColl.transform.localScale = Vector3.one;
    }
    public void DisableAttack1()
    {
        m_animator.SetBool("IsAttaking", false);
        m_attackColl.enabled = false;
        m_attackColl.transform.localScale = Vector3.zero;
    }
    public void EnableAttack2()
    {
        var v = Instantiate(m_projectile, m_player.position, transform.rotation).GetComponent<ProjectileController>();
        v.Dmg = m_baseEnemy.GetEnemyDmg();
        v.LifeSpan = 1.5f;
    }
    public void IsDead()
    {
        m_isDead = true;
        m_shouldMove = false;
    }
}
