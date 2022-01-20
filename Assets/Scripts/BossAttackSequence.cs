using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackSequence : MonoBehaviour {

    public List<BossAttack> attackDatas;
    public int attackIndex;
    public float attackTimer;

    //�{�X�̉�]�@�����_�����l
    private float randomNum;
    //�{�XHP
    private int bossHealth = 500;
    //�_���[�W�擾
    private int damagePoint;

    void Start() {

        

    }
    void Update() {

       
        //�{�X�̉�]�������_���Ɏ擾
        randomNum = Random.Range(3f, 5f);
        transform.Rotate(randomNum, 0f, randomNum);
        
        //�^�C�����C��
        attackTimer += Time.deltaTime;

       /* if (attackTimer > attackDatas[attackIndex].time) {
            Attack(attackDatas[attackIndex]);
            attackIndex++;
            attackTimer = 0;
        }*/

    }

    private void OnCollisionEnter(Collision collision) {

        damagePoint = GameObject.Find("unitychan").GetComponent<PlayerController>().AttackPoint();
        GameObject.Find("DamageManager").GetComponent<DamageViewScript>().ViewDamage(damagePoint,"PLAYER");
        if (collision.gameObject.CompareTag("Bullet")) {

            bossHealth -= damagePoint;
            if (bossHealth < 0) {
                Destroy(this.gameObject);
            }

        }
        
    }

    private void Attack(BossAttack bossAttack) {
        
    }

}

public class BossAttack {

    public float time;
    public int damage;
    public string animationName;
    //���̑��U���ɕK�v�ȏ��
}
