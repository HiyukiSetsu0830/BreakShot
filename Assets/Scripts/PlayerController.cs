using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    

    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    //プレイヤーのRigidbodyを入れる
    [SerializeField] private Rigidbody playerRigidbody;

    //前方向の速度
    private float speed = 2f;
    //上方向の速度
    private float jumpPower = 5f;
    //ジャンプ可能かどうか
    bool canJump = false;
    //地面接触判定
    private bool isGround = false;
    //プレイヤーの座標
    private Vector3 playerPos;
    //プレイヤー初期位置
    private const float startPosZ = -20f;
    private const float startPosY = 0.25f;
    
    //移動キーの判定
    bool getJButtonDown = false;
    bool getRButtonDown = false;
    bool getLButtonDown = false;
   


    // Start is called before the first frame update
    void Start()
    {
        //最初の時点でのプレイヤーのポジションを取得
        playerPos = GetComponent<Transform>().position;    
        //Animatorを取得
        playerAnimator = GetComponent<Animator>();
        //Rigidbodyを取得
        playerRigidbody = GetComponent<Rigidbody>();       

    }

    // Update is called once per frame
    void Update() {

       
        //移動キー
        float isJumpButton = Input.GetAxis("Jump");
        float isMoveButton = Input.GetAxis("Horizontal");

        if (canJump) {
            canJump = false;
            playerRigidbody.velocity += new Vector3(0f, jumpPower * isJumpButton * Time.deltaTime, 0f);
        }

        playerRigidbody.velocity += new Vector3(0f, 0f, speed * isMoveButton * Time.deltaTime);
        //プレイヤーの座標更新
        playerPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision) {

        
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            canJump = true;
        }
    }
}
