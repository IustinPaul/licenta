using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_rbody2D;
    private SpriteRenderer m_spriteRenderer;
    private PlayerStats m_playerStats;
    private InventoryController m_inventoryController;
    private BoxCollider2D m_lAttack;
    private BoxCollider2D m_rAttack;
    private GameObject m_menu;
    private bool m_isRolling = false;
    private bool m_isBlocking = false;
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
        m_lAttack = transform.GetChild(2).GetChild(1).GetComponent<BoxCollider2D>();
        m_rAttack = transform.GetChild(2).GetChild(0).GetComponent<BoxCollider2D>();
        m_inventoryController = transform.GetChild(1).GetChild(2).GetComponent<InventoryController>();
        m_inventoryController.Initialized();
        m_menu = transform.GetChild(1).GetChild(4).gameObject;
    }

    void Update()
    {
        m_timeSienceAttack += Time.deltaTime;

        if (CanMove)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !m_isRolling && Time.timeScale != 0)
            {
                m_inventoryController.gameObject.SetActive(true);
                CanMove = false;
                Time.timeScale = 0;
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0)
            {
                Time.timeScale = 0;
                m_menu.SetActive(true);
            }
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            if (inputX > 0)
            {
                m_spriteRenderer.flipX = false;
            }
            else if (inputX < 0)
            {
                m_spriteRenderer.flipX = true;
            }

            //Movement
            if ( !m_isBlocking)
            {
                m_rbody2D.velocity = new Vector2(inputX * m_playerStats.GetPlayerSpeed(), inputY * m_playerStats.GetPlayerSpeed());
            }
            else
            {
                m_rbody2D.velocity = Vector2.zero;
            }
            //Attack
            if (Input.GetKeyDown(KeyCode.Space) && m_timeSienceAttack > 0.5f && !m_isRolling && m_playerStats.CanAttack())
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
            else if (Input.GetKeyDown(KeyCode.C) && !m_isRolling && m_playerStats.CanBlock())
            {
                m_animator.SetTrigger("Block");
                m_animator.SetBool("IdleBlock", true);
                m_isBlocking = true;
            }
            else if (Input.GetKeyUp(KeyCode.C) || (m_isBlocking && !m_playerStats.CanBlock()))
            {
                m_animator.SetBool("IdleBlock", false);
                m_isBlocking = false;
            }

            //Roll
            else if (Input.GetKeyDown(KeyCode.LeftShift) && !m_isRolling && m_playerStats.CanUseRoll())
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

            m_rbody2D.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && CompareTag("Player"))
        {
            if (m_inventoryController.AddToInventory(collision.GetComponent<Item>()))
                Destroy(collision.gameObject);
        }
        else if(collision.CompareTag("EnemyAttack") && CompareTag("Player") && !m_isRolling)
        {
            var v = collision.transform.parent.GetComponent<BaseEnemy>();
            m_playerStats.TakeDmg(v.GetEnemyDmg());
            if (m_isBlocking)
            {
                v.TakeDmg(v.GetEnemyDmg() * m_playerStats.GetPlayerThorns() / 100, 0);
            }
        }
        else if(collision.CompareTag("EnemyProjectile") && CompareTag("Player") && !m_isRolling)
        {
            var v = collision.GetComponent<ProjectileController>();
            m_playerStats.TakeDmg(v.Dmg);
        }
    }

    private void Player_ResetRoll()
    {
        m_isRolling = false;
    }

    public void EnableAttack()
    {
        if (m_spriteRenderer.flipX)
        {
            m_lAttack.enabled = true;
            m_lAttack.transform.localScale = Vector3.one;
        }
        else
        {
            m_rAttack.enabled = true;
            m_rAttack.transform.localScale = Vector3.one;
        }
    }
    public void DisableAttack()
    {
        m_rAttack.enabled = false;
        m_lAttack.enabled = false;
        m_rAttack.transform.localScale = Vector3.zero;
        m_lAttack.transform.localScale = Vector3.zero;
    }

}
