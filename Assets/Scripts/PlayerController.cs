using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_rollForce = 6.0f;

    private Animator m_animator;
    private Rigidbody2D m_rbody2D;
    private SpriteRenderer m_spriteRenderer;
    private bool m_isRolling = false;
    private int m_direction = 1;
    private int m_currentAttack = 0;
    private float m_timeSienceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        m_timeSienceAttack += Time.deltaTime;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (inputX > 0)
        {
            m_spriteRenderer.flipX = false;
            m_direction = 1;
        }
        else if (inputX < 0)
        {
            m_spriteRenderer.flipX = true;
            m_direction = -1;
        }

        //Movement
        if (!m_isRolling)
        {
            m_rbody2D.velocity = new Vector2(inputY * m_speed, m_rbody2D.velocity.y);
            m_rbody2D.velocity = new Vector2(inputX * m_speed, m_rbody2D.velocity.x);
        }

        //Death, momentan pt testing
        if (Input.GetKeyDown("e") && !m_isRolling)
        {
            m_animator.SetTrigger("Death");
        }
        //Hurt, momentan pt testing
        if (Input.GetKeyDown("q") && !m_isRolling)
        {
            m_animator.SetTrigger("Hurt");
        }
        //Attack
        else if (Input.GetMouseButtonDown(0) && m_timeSienceAttack > 0.25f && !m_isRolling)
        {
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSienceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSienceAttack = 0.0f;
        }

        //Block
        else if (Input.GetMouseButtonDown(1) && !m_isRolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        //Roll
        else if (Input.GetKeyDown("left shift") && !m_isRolling)
        {
            m_isRolling = true;
            m_animator.SetTrigger("Roll");
            m_rbody2D.velocity = new Vector2(m_direction * m_rollForce, m_rbody2D.velocity.y);
        }
        //Run
        else if (Mathf.Abs(inputY) > Mathf.Epsilon || Mathf.Abs(inputX)> Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        //Idle
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    private void Player_ResetRoll()
    {
        m_isRolling = false;
    }
}
