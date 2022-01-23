using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossAttackSequence : MonoBehaviour {


    
    public List<BossAttack> attackDatas;
    public int attackIndex;
    public float attackTimer;

    

    //ボスの回転　ランダム数値
    private float randomNum;
    
   

    void Start() {

        
        
    }
    void Update() {

       
        //ボスの回転をランダムに取得
        randomNum = Random.Range(3f, 5f);
        transform.Rotate(randomNum, 0f, randomNum);
       

        //タイムライン
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
    //その他攻撃に必要な情報
}
