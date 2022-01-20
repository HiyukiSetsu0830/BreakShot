using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossController : MonoBehaviour {


    private const int MIN_HEALTH = 0;
    public List<BossAttack> attackDatas;
    public int attackIndex;
    public float attackTimer;

    [SerializeField] private Slider slider;

    //�{�X�̉�]�@�����_�����l
    private float randomNum;
    //�{�XHP
    public int bossMaxHP { get; private set; } = 500;
    public int bossCurrentHP { get; private set; }
    //�_���[�W�擾
    private int damagePoint;

    void Start() {

        slider.value = 1;
        bossCurrentHP = bossMaxHP;
        
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

            bossCurrentHP -= damagePoint;
            slider.value = (float)bossCurrentHP / (float)bossMaxHP;

            if (bossCurrentHP < 0) {
                bossCurrentHP = MIN_HEALTH;
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
