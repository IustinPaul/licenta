using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{
    [SerializeField] private float m_attackRange = 1;
    [SerializeField] private float m_knockbackPower = 30;

    private Transform m_player;
    private BaseEnemy m_baseEnemy;
    private Rigidbody2D m_rbody2D;
    private Animator m_animator;
    private BoxCollider2D m_attackColl;
    private Transform m_lifeBar;
    private bool m_shouldMove = false;
    private bool m_isDead = false;
    private float m_direction;
    private float m_timeSinceAttack = 3.0f;
    private int m_attack = 2;

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

        if(m_player.position.x - transform.position.x > 0)
        {
            m_direction = -m_attackRange;
            transform.localScale = Vector3.one;
            m_lifeBar.localScale = Vector3.one;
        }
        else
        {
            m_direction = m_attackRange;
            transform.localScale = new Vector3(-1, 1, 1);
            m_lifeBar.localScale = new Vector3(-1, 1, 1);
        }

        float moveX = (m_player.position.x + m_direction) - transform.position.x;
        float moveY = m_player.position.y - transform.position.y;

        if(Mathf.Abs(moveX) <= 0.1 && Mathf.Abs(moveY) <= 0.1)
        {
            m_shouldMove = false;
            m_animator.SetInteger("AnimState", 0);
        }

        if(m_shouldMove)
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
                    if (m_attack == 0)
                        m_attack = 2;
                    m_animator.SetTrigger("Attack" + m_attack);
                    if (m_attack == 2)
                        m_timeSinceAttack = 2;
                    else
                        m_timeSinceAttack = 0;
                    m_attack--;
                }
            }
        }
    }
    public void EnableAttack()
    {
        m_attackColl.enabled = true;
        m_attackColl.transform.localScale = Vector3.one;
        if (Mathf.Abs((m_player.position.x + m_direction) - transform.position.x) < Mathf.Abs(m_direction) / 2.0f)
            m_player.GetComponent<Rigidbody2D>().velocity = new Vector2(-m_direction*m_knockbackPower, 0);
    }
    public void DisableAttack()
    {
        m_attackColl.enabled = false;
        m_attackColl.transform.localScale = Vector3.zero;
    }
    public void IsDead()
    {
        m_isDead = true;
        m_shouldMove = false;
    }
}
