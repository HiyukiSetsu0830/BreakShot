using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossAttackSequence : MonoBehaviour {


    
    public List<BossAttack> attackDatas;
    public int attackIndex;
    public float attackTimer;
    
    void Start() {

        

    }
    void Update() {

        //�^�C�����C��
        attackTimer += Time.deltaTime;

        if (attackTimer > attackDatas[attackIndex].time) {
            Attack(attackDatas[attackIndex]);
            attackIndex++;
            attackTimer = 0;
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
