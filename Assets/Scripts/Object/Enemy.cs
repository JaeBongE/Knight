using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("�� ����")]
    [SerializeField] private float maxHp = 3.0f;
    private float curHp = 0f;
    [SerializeField] private float moveSpeed = 1.0f;
    private Rigidbody2D rigid;
    Animator anim;
    private bool isPlayerLookAtRight = false;

    [Header("�� ���")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Collider2D wallCheckBox;
    [SerializeField] private Collider2D groundCheckBox;

    [SerializeField] Transform player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        curHp = maxHp;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        checkPlayer();
        moving();
        checkDirection();
    }

    private void checkPlayer()
    {
        if (player == null)
        {
            GameObject objPlayer = GameObject.Find("Player");
            player = objPlayer.transform;
        }
    }

    private void FixedUpdate()
    {
        if (wallCheckBox.IsTouchingLayers(ground) == true)//wallcheckbox�� ���� ��Ҵٸ� ��
        {
            turning();
        }

        if (groundCheckBox.IsTouchingLayers(ground) == false)//groundcheckbox�� ������ �������ٸ� ��
        {
            turning();
        }
        
    }

    /// <summary>
    /// ���� �¿�� �����̴� �Լ�
    /// </summary>
    private void moving()
    {
        rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
    }

    private void checkDirection()
    {
        if (player.transform.localScale.x >= 1)
        {
            isPlayerLookAtRight = true;
        }
        else if (player.transform.localScale.x <= -1)
        {
            isPlayerLookAtRight = false;
        }
    }

    /// <summary>
    /// �տ� ���� �ְų� ���� ���ٸ� ���ϴ� �Լ�
    /// </summary>
    private void turning()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        moveSpeed *= -1;
    }

    public void Hit(float _damage)
    {
        curHp -= _damage;
        hitPosition();
        if (curHp <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject,0.5f);
        }
    }

    private void hitPosition()
    {
        anim.SetTrigger("Hit");
        Vector3 position = transform.position;
        if (isPlayerLookAtRight == true)
        {
            position.x += 1;
        }
        else
        {
            position.x -= 1;
        }
        transform.position = position;
    }
}
