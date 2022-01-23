using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossController : MonoBehaviour {

    [SerializeField] private GameObject attackCube;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private Slider slider;
    //ボスHP
    private const int MIN_HEALTH = 0;
    public int bossMaxHP { get; private set; } = 500;
    public int bossCurrentHP { get; private set; }
    //プレイヤーの位置を取得
    public Vector3 playerPos { get;  set; }
    //攻撃間隔
    private const float INTERVAL = 0.3f;
    private float intervalTime = 0f;
    //ボスの回転　ランダム数値
    private float randomNum;

    //ダメージ取得
    private int damagePoint;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1;
        bossCurrentHP = bossMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        //ボスの回転をランダムに取得
        randomNum = Random.Range(3f, 5f);
        transform.Rotate(randomNum, 0f, randomNum);
    }

    private void OnCollisionEnter(Collision collision) {

        damagePoint = GameObject.Find("unitychan").GetComponent<PlayerController>().AttackPoint();
        GameObject.Find("DamageManager").GetComponent<DamageViewScript>().ViewDamage(damagePoint, "PLAYER");
        if (collision.gameObject.CompareTag("Bullet")) {

            bossCurrentHP -= damagePoint;
            slider.value = (float)bossCurrentHP / (float)bossMaxHP;

            if (bossCurrentHP < 0) {
                bossCurrentHP = MIN_HEALTH;
                Destroy(this.gameObject);
            }

        }

    }

    public void CubeShoot() {

        if (Time.time <= intervalTime) return;
        Instantiate(attackCube, this.transform.position + Vector3.up * 2.0f , Quaternion.identity);
        Instantiate(attackCube, this.transform.position + Vector3.down * 2.0f, Quaternion.identity);
        intervalTime = INTERVAL + Time.time;

    }
}
