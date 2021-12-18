using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthTextScript : MonoBehaviour
{
    [SerializeField]Text hpText;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        hpText = GameObject.Find("HPText").GetComponent<Text>();
        playerController = GameObject.Find("unitychan").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        int playerHP = playerController.currentHP;
        int playerMaxHP = playerController.maxHP;
        hpText.text = playerHP + "/" + playerMaxHP;
    }
}
