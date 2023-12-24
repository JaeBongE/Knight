using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == GameTag.Enemy.ToString())
        {
            Debug.Log("�÷��̾ ��ҽ��ϴ�.");
            Player playerSc = collision.GetComponent<Player>();
            playerSc.Hit(1.0f);
        }
    }
}
