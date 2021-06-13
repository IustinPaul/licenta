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
    private PlayerStats m_playerStats;
    private InventoryController m_inventoryController;
    private bool m_isRolling = false;
    private bool m_isBlocking = false;
    private int m_directionX = 1;
    private int m_directionY = 1;
    private int m_currentAttack = 0;
    private float m_timeSienceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;

    [HideInInspector] public bool CanMove = true;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_playerStats = GetComponent<PlayerStats>();
        m_inventoryController = transform.GetChild(1).GetChild(2).GetComponent<InventoryController>();
        m_inventoryController.Initialized();
    }

    void Update()
    {
        m_timeSienceAttack += Time.deltaTime;

        if (CanMove)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !m_isRolling)
            {
                m_inventoryController.gameObject.SetActive(true);
                CanMove = false;
            }
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            if (inputX > 0)
            {
                m_spriteRenderer.flipX = false;
                m_directionX = 1;
            }
            else if (inputX < 0)
            {
                m_spriteRenderer.flipX = true;
                m_directionX = -1;
            }

            if (inputY > 0)
            {
                m_directionY = 1;
            }
            else if (inputY < 0)
            {
                m_directionY = -1;
            }

            //Movement
            if ( !m_isBlocking)
            {
                m_rbody2D.velocity = new Vector2(inputY * m_speed, m_rbody2D.velocity.y);
                m_rbody2D.velocity = new Vector2(inputX * m_speed, m_rbody2D.velocity.x);
            }
            else
            {
                m_rbody2D.velocity = new Vector2(0, m_rbody2D.velocity.y);
                m_rbody2D.velocity = new Vector2(0, m_rbody2D.velocity.x);
            }
            //Hurt, momentan pt testing
            if (Input.GetKeyDown("q") && !m_isRolling)
            {
                m_playerStats.TakeDmg(20);
            }
            //Attack
            else if (Input.GetMouseButtonDown(0) && m_timeSienceAttack > 0.25f && !m_isRolling && m_playerStats.CanAttack())
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
            else if (Input.GetMouseButtonDown(1) && !m_isRolling && m_playerStats.CanBlock())
            {
                m_animator.SetTrigger("Block");
                m_animator.SetBool("IdleBlock", true);
                m_isBlocking = true;
            }
            else if (Input.GetMouseButtonUp(1) || (m_isBlocking && !m_playerStats.CanBlock()))
            {
                m_animator.SetBool("IdleBlock", false);
                m_isBlocking = false;
            }

            //Roll
            else if (Input.GetKeyDown("left shift") && !m_isRolling && m_playerStats.CanUseRoll())
            {
                m_isRolling = true;
                m_animator.SetTrigger("Roll");
            }
            //Run
            else if (Mathf.Abs(inputY) > Mathf.Epsilon || Mathf.Abs(inputX) > Mathf.Epsilon)
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
        else
        {

            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);

            m_rbody2D.velocity = new Vector2(0, m_rbody2D.velocity.y);
            m_rbody2D.velocity = new Vector2(0, m_rbody2D.velocity.x);
        }
    }

    private void Player_ResetRoll()
    {
        m_isRolling = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (m_inventoryController.AddToInventory(collision.GetComponent<Item>()))
                Destroy(collision.gameObject);
        }
    }
}
