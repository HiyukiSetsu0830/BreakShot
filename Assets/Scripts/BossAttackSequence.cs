using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossAttackSequence : MonoBehaviour {


    
    public List<FirstBossAttack> attackDatas;
    public int attackIndex;
    public float attackTimer;
    
    void Start() {

        

    }

    void Update() {

        //ƒ^ƒCƒ€ƒ‰ƒCƒ“
        attackTimer += Time.deltaTime;

        if (attackTimer > attackDatas[attackIndex].time) {
            Attack(attackDatas[attackIndex]);
            attackIndex++;
            attackTimer = 0;
        }
    }

    private void Attack(FirstBossAttack firstBossAttack) {
        
    }

}

