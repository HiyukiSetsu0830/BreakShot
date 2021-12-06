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
    private float forceZ = 40f;
    //上方向の速度
    private float forceY = 500f;
    //左右の移動できる範囲
    private float movableRange = 0.5f;
    //ボタンの状態
    private bool isJButtonDown = false;
    private bool isRButtonDown = false;
    private bool isLButtonDown = false;

    //地面接触判定
    private bool isGround;
    //向きを変更 変更前のポジション
    private Vector3 latestPos;

    // Start is called before the first frame update
    void Start()
    {
        //Animatorを取得
        playerAnimator = GetComponent<Animator>();
        //Rigidbodyを取得
        playerRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
        //移動キー設定
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        bool isLeftMove = Input.GetKey(KeyCode.A);
        bool isRightMove = Input.GetKey(KeyCode.D);
        bool isUnderMove = Input.GetKey(KeyCode.S);
        //移動キーを離した時
        bool isLeftUp = Input.GetKeyUp(KeyCode.A);
        bool isRightUp = Input.GetKeyUp(KeyCode.D);

        //X軸の制限
        bool notMove = movableRange <= this.transform.position.x;

        //Playerインスタンス生成
        Player player = new Player();
        player.ForceY = forceY;
        player.ForceZ = forceZ;
        player.PlayerRB = playerRigidbody;
        player.JumpAnimator = playerAnimator;

        //Jumpステートの場合はJumpにfalseをセット
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        bool isMoveSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk");

        //キャラクターの向きを判定
        Vector3 diff = transform.position - latestPos;
        //前回のPositionの更新
        latestPos = transform.position;
        //ベクトルの大きさによって向きを変える
        bool isDirection = diff.magnitude > 0.01f;
        //キャラクターの向きを変える
        Vector3 direction = transform.localScale;

        if (isJump && isGround) player.Jump();
        if (isLeftMove && isDirection) {

            player.LeftMove();
            //向きを変更する
            //transform.rotation = Quaternion.LookRotation(diff); 
        } else if (isLeftUp) {



        }

        if (isRightMove) {

            player.RightMove();
            //向きを変更する
            //transform.rotation = Quaternion.LookRotation(diff);
        }

        if (isJumpSetFalse) this.playerAnimator.SetBool("Jump", false);
        if(isMoveSetFalse) this.playerAnimator.SetBool("Walk", false);
        if(isMoveSetFalse) this.playerAnimator.SetBool("Walk", false);
    }

    private void OnCollisionEnter(Collision collision) {

        //プレイヤーと地面の判定
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) isGround = true;
    }

    //プレイヤー抽象クラス
    abstract class Human {

        //抽象メソッド
        public abstract void Jump();
        public abstract void LeftMove();
        public abstract void RightMove();

    }

    private class Player : Human {

        
        public Rigidbody PlayerRB { get; set; }
        public float ForceY { get; set; }
        public float ForceZ { get; set; }
        public Animator JumpAnimator { get; set; }


        //キャラクターの操作関数
        public override void Jump() {
            JumpAnimator.SetTrigger("jumpTrigger");
            PlayerRB.AddForce(Vector3.up * ForceY, ForceMode.Acceleration);
        }

        public override void LeftMove() {
            PlayerRB.AddForce(Vector3.back * ForceZ, ForceMode.Force);
            JumpAnimator.SetTrigger("walkTrigger");
        }

        public override void RightMove() {
            PlayerRB.AddForce(Vector3.forward * ForceZ, ForceMode.Force);
            JumpAnimator.SetTrigger("walkTrigger");
        }
    } 
}
