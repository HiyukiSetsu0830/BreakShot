using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthTextScript : MonoBehaviour
{
    [SerializeField] Text hpText;
    [SerializeField] Text bossHPText;

    PlayerController playerController;
    FirstBossController firstBossController;
    

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤーのHP情報
        hpText = GameObject.Find("HPText").GetComponent<Text>();
        playerController = GameObject.Find("unitychan").GetComponent<PlayerController>();
        //1ボスのHP情報
        bossHPText = GameObject.Find("BossHPText").GetComponent<Text>();
        firstBossController = GameObject.Find("FirstBoss").GetComponent<FirstBossController>();
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーのHP管理
        int playerHP = playerController.currentHP;
        int playerMaxHP = playerController.maxHP;
        hpText.text = playerHP + "/" + playerMaxHP;

        //1ボスのHP管理
        int bossHP = firstBossController.bossCurrentHP;
        int bossMaxHP = firstBossController.bossMaxHP;
        bossHPText.text = bossHP + "/" + bossMaxHP;
    }
}
