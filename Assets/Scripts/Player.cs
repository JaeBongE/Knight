using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�÷��̾� ������")]
    Rigidbody2D rigid;
    PolygonCollider2D polygonColider2D;
    Animator anim;
    Vector3 moveDir;//default 0,0,0
    [SerializeField] float moveSpeed = 5.0f;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        polygonColider2D = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moving();
        turning();
        doAnimation();
    }
    /// <summary>
    /// horizontal�� �Է����� �� �÷��̾� �̵��Լ�
    /// </summary>
    private void moving()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal") * moveSpeed;//-1 0 1
        moveDir.y = rigid.velocity.y;
        rigid.velocity = moveDir;
    }
    /// <summary>
    /// �÷��̾��� �̵� ���⿡ ���� ���ý����� x ���� �ݴ�� �־� ȸ���ϰ� �ϴ� �Լ�
    /// </summary>
    private void turning()
    {
        if (moveDir.x > 0 && transform.localScale.x < 1 || moveDir.x < 0 && transform.localScale.x > -1)//������ or ���� �̵�
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    /// <summary>
    /// �ִϸ��̼� ���� ���� �Լ�
    /// </summary>
    private void doAnimation()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }
}
