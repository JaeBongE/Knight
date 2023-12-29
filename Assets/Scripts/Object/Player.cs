using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static GameManager;

public class Player : MonoBehaviour
{
    private Camera mainCam;

    [Header("�÷��̾� ������")]
    Rigidbody2D rigid;
    PolygonCollider2D polygonColider2D;
    Animator anim;
    Vector3 moveDir;//default 0,0,0
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float jumpForce = 5.0f;
    float maxHp = 10;
    [SerializeField] float curHp = 0;
    Slider playerHp;

    [Header("����")]
    [SerializeField] float gravity = 9.81f;
    [SerializeField] bool isGround = false;
    private bool isJump = false;
    private bool isDoubleJump = false;
    private bool doubleJump = false;
    private float verticalVelocity;//�������� �޴� ��

    [Header("����")]
    [SerializeField] Collider2D swordHitBox;
    Enemy enemy;
    private float VskillCoolTime = 5.0f;
    private float VskillCoolTimer = 5.0f;
    private bool isVcoolTime = false;
    Image vCoolTime;
    TMP_Text vCoolTimeText;

    float timerHit = 0.0f;
    float timerHitLimit = 0.5f;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == GameTag.Potal.ToString())
    //    {
    //        Debug.Log("���� �������� �̵�");
    //    }
    //}

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        polygonColider2D = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        curHp = maxHp;
        
    }

    private void Start()
    {
        mainCam = Camera.main;
        enemy = GetComponent<Enemy>();
        //zCoolTime.enabled = false;
    }

    void Update()
    {
        checkPlayerUI();
        checkGround();
        checkTimer();

        moving();
        turning();

        jumping();
        checkGravity();
        attack();
        heal();
        healTimer();

        doAnimation();
        NextStage();
    }

    private void checkPlayerUI()
    {
        if (playerHp == null)//UI���� �����͸� ���� ���� ����
        {
            GameObject objPlayerUI = GameObject.Find("PlayerUI");

            if (objPlayerUI == null)
            { 
                return;
            }

            PlayerUI scUI = objPlayerUI.GetComponent<PlayerUI>();
            //(Image _VcoolTime, Slider _PlayerHp, TMP_Text _VCoolTimeText) data = scUI.GetProperty();

            //var data2 = scUI.GetProperty();
            playerHp = scUI.GetPlayerHp();
            (Image _vCoolTime, TMP_Text _vCoolTimeText) VskillData = scUI.GetVskill();
            vCoolTime = VskillData._vCoolTime;
            vCoolTimeText = VskillData._vCoolTimeText;
            //playerHp = data._PlayerHp;
            //vCoolTime = data._VcoolTime;
            //vCoolTimeText = data._VCoolTimeText;
            playerHp.maxValue = maxHp;
            vCoolTime.fillAmount = 0f;

            //vCoolTime = scUI.GetImage();
        }



        //if (playerHp == null)
        //{
        //    GameObject objCanvas = GameObject.Find("Canvas");

        //    if (objCanvas == null)
        //    {
        //        return;
        //    }

        //    GameObject objPlayerHp = objCanvas.transform.Find("PlayerIcon/PlayerHp").gameObject;
        //    playerHp = objPlayerHp.GetComponent<Slider>();

        //}
    }
    /// <summary>
    /// �÷��̾ ���� ��� �ִ��� üũ�ϴ� �Լ�
    /// </summary>
    private void checkGround()
    {
        isGround = false;
        if (verticalVelocity > 0)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.BoxCast(polygonColider2D.bounds.center, polygonColider2D.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (hit.transform != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Debug.Log(hit.transform.gameObject.name);
            isGround = true;
        }
    }

    private void checkTimer()
    {
        if (timerHit > 0.0f)
        {
            timerHit -= Time.deltaTime;
            if(timerHit < 0.0f)
            {
                timerHit = 0.0f;
            }
        }
    }

    /// <summary>
    /// horizontal�� �Է����� �� �÷��̾� �̵��Լ�
    /// </summary>
    private void moving()
    {
        if (timerHit > 0.0f)
            return;

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
    /// ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    private void jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)//�����̽��ٸ� ������ ���� �پ� �ִٸ� �������� Ȱ��ȭ
        {
            isJump = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGround == false && isJump == false && isDoubleJump == true)//�����̽��ٸ� ������ ���߿� �ְ� ���� ��Ȱ��ȭ ���� ���������� Ȱ��ȭ
        {
            doubleJump = true;//�������� Ȱ��ȭ
        }
    }

    /// <summary>
    /// �߷� �����ͷ� �������� �޴� ���� �����ϴ� �Լ�(����,��������)
    /// </summary>
    private void checkGravity()
    {
        if (isGround == false)//���߿� �� ���� ��
        {
            verticalVelocity -= gravity * Time.deltaTime;//�������� �޴� ���� gravity * Time.deltaTime��ŭ �پ���
            if (verticalVelocity < -10.0f)//�������� �޴� ���� -10�� �Ѵ´ٸ�
            {
                verticalVelocity = -10.0f;//-10���� �޴´�
            }
        }
        else//���� �پ� ���� ��
        {
            verticalVelocity = 0.0f;//�������� �޴� ���� 0�� �ȴ�.
        }

        if (isJump == true)//������ Ȱ��ȭ �Ǿ��� ��
        {
            isJump = false;
            isDoubleJump = true;//�������� ���� Ȱ��ȭ
            verticalVelocity = jumpForce;//���� ������ŭ �������� �޴� ���� �ش�
        }

        if (doubleJump == true)//���� ������ Ȱ��ȭ �Ǿ��� ��
        {
            doubleJump = false;
            isDoubleJump = false;
            verticalVelocity = jumpForce;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }

    /// <summary>
    /// zŰ�� ������ �� ���� Ȱ��
    /// </summary>
    private void attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("isAttacking");
        }
    }

    private void heal()
    {
        if (curHp == maxHp) return;
        
        if (Input.GetKeyDown(KeyCode.V) && isVcoolTime == false)
        {
            isVcoolTime = true;
            //zCoolTime.enabled = true;
            curHp += 3;
            if (curHp >= maxHp)
            {
                curHp = maxHp;
            }
            playerHp.value = curHp;
        }

        if (VskillCoolTime == 0f)
        {
            //zCoolTime.enabled = false;
            VskillCoolTime = VskillCoolTimer;
        }

    }

    private void healTimer()
    {
        if (isVcoolTime == true && VskillCoolTime > 0f)
        {
            VskillCoolTime -= Time.deltaTime;
            vCoolTime.fillAmount = VskillCoolTime / VskillCoolTimer;
            //zCoolTimeText.text = ($"{Mathf.Floor(ZskillCoolTime * 10f) / 10f}");
            vCoolTimeText.text = ($"{VskillCoolTime.ToString("F1")}");
            if (VskillCoolTime <= 0f)
            {
                VskillCoolTime = 0f;
                vCoolTimeText.text = "";
                isVcoolTime = false;
            }
        }
    }

    /// <summary>
    /// �ִϸ��̼� ���� ���� �Լ�
    /// </summary>
    private void doAnimation()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);//�¿� �̵� �ִϸ��̼�
        anim.SetBool("isGround", isGround);//���� ��Ҵ��� üũ�ؼ� ���� ������ ���� �ִϸ��̼�
    }

    /// <summary>
    /// _pos�� ���� ��Ҵٸ� player�� �ڷ� ���ư��� �ִϸ��̼��� �۵���
    /// </summary>
    /// <param name="_pos"> ��</param>
    public void onDamaged(Vector2 _pos)
    {
        //�������¶�� return;
        timerHit = timerHitLimit;
        rigid.velocity = Vector2.zero;
        verticalVelocity = 0.0f;
        Vector3 position = transform.position;
        if (position.x > _pos.x)//���� ��밡 player���� ���ʿ� �ִٸ�
        {
            rigid.AddForce(new Vector2(2, 1) * 2, ForceMode2D.Impulse);//���������� ���ư�
        }
        else if (position.x <= _pos.x)//�����ʿ� �ִٸ�
        {
            rigid.AddForce(new Vector2(-2, 1) * 2, ForceMode2D.Impulse);//�������� ���ư�
        }
        verticalVelocity = rigid.velocity.y;

        anim.SetTrigger("isPlayerHit");//hit�ִϸ��̼� �۵�
    }

    /// <summary>
    /// player�� �������� �Դ� �Լ�
    /// </summary>
    /// <param name="_damage"></param>
    public void Hit(float _damage)
    {
        curHp -= _damage;//hp�� _damage��ŭ ����
        playerHp.value = curHp;
        if (curHp <= 0)//�÷��̾ �״� ���
        {
            Debug.Log("�÷��̾ �׾����ϴ�");
            anim.SetTrigger("isPlayerDeath");//�÷��̾� ��� �ִϸ��̼� 
            GameManager.Instance.GameOver();//���ӿ��� �޴��� ���´�
        }
    }

    /// <summary>
    /// �÷��̾ ���� ���� �� �ҵ� ��Ʈ�ڽ��� ����� ���ִ� �Լ�
    /// </summary>
    public void EnableAttack()
    {
        swordHitBox.enabled = true;
    }
    /// <summary>
    /// ������ ���� �� �ҵ� ��Ʈ�ڽ��� �����ִ� �Լ�
    /// </summary>
    public void DisableAttack()
    {
        swordHitBox.enabled = false;
    }

    public void NextStage()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SceneManager.LoadSceneAsync((int)enumScene.Stage2);
        }
    }
}
