using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstBossController : MonoBehaviour {

    [SerializeField] private GameObject attackCube;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private Slider slider;
    //�{�XHP
    private const int MIN_HEALTH = 0;
    public int bossMaxHP { get; private set; } = 500;
    public int bossCurrentHP { get; private set; }
    //�v���C���[�̈ʒu���擾
    public Vector3 playerPos { get;  set; }
    //�U���Ԋu
    private const float INTERVAL = 0.3f;
    private float intervalTime = 0f;
    //�{�X�̉�]�@�����_�����l
    private float randomNum;

    //�_���[�W�擾
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
        //�{�X�̉�]�������_���Ɏ擾
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
