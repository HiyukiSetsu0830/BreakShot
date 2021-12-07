using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    

    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;

    //前方向の速度
    private float speed = 10f;
    //上方向の速度
    private float jumpPower = 9f;
    //地面接触判定
    private bool isGround;


    // Start is called before the first frame update
    void Start()
    {
          
        //Animatorを取得
        playerAnimator = GetComponent<Animator>();
        //RigidBodyの取得
        playerRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update() {

        //Jumpステートの場合はJumpにfalseをセットする
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumpBool");
        this.playerAnimator.SetBool("jumpBool", false);

        //移動キー
        bool isJumpButton = Input.GetButton("Jump");
        float isMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        if (isGround && isJumpButton) {
            this.playerAnimator.SetBool("jumpBool", true);
            playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
            isGround = false;
        }

       //左右移動
        transform.position += new Vector3(0f, 0f, isMoveButton);
      
    }

    private void OnCollisionEnter(Collision collision) {

        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            isGround = true;
        }
    }
}
