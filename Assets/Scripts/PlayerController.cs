using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //ステート
    public enum State { Idle,Jump,Move,Shot};
    State state;

    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject magicBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    private GameObject bullet;

    //前方向の速度
    private float speed = 10f;
    //上方向の速度
    private float jumpPower = 9f;
    //地面接触判定
    private bool isGround;
    //プレイヤーの座標
    private Vector3 playerPos;
    //マウスカーソルの座標
    private Vector3 mousePos;
    //HP系のプロパティ
    public int currentHP { get; private set; }
    public int maxHP { get; private set; } = 100;
    //攻撃間隔
    private float interval = 0.3f;
    private float intervalTime = 0f;
    


    // Start is called before the first frame update
    void Start()
    {
        //カーソル変更
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
        //Animatorを取得
        playerAnimator = GetComponent<Animator>();
        //RigidBodyの取得
        playerRigidbody = GetComponent<Rigidbody>();
        //最初の座標を取得
        playerPos = GetComponent<Transform>().position;

        //HP最大
        slider.value = 1;
        currentHP = maxHP;

        //プレイヤーの状況初期化
        state = State.Idle;

    }

    // Update is called once per frame
    void Update() {

        //Jumpステートの場合はJumpにfalseをセットする
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumpBool");
        this.playerAnimator.SetBool("jumpBool", false);

        //移動キー
        bool isJumpButton = Input.GetButton("Jump");
        float isMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
   
        //攻撃キー
        bool isLeftMouseButton = Input.GetButton("Fire1");

        //マウスのポジション
        mousePos = Input.mousePosition;

        //射撃間隔
        if (intervalTime > 0.0f) intervalTime -= Time.deltaTime;
        //攻撃
        if (isLeftMouseButton && intervalTime <= 0.0f) {
            state = State.Shot;
            Shot();
            intervalTime = interval;
        }

        //ジャンプ
        if (isGround && isJumpButton) {
            state = State.Jump;
            Jump();
        }
        //移動
        if (isMoveButton > 0.01f || isMoveButton < -0.01f) {
            state = State.Move;
            Move(isMoveButton);
        }

        //走るアニメーション
        playerAnimator.SetFloat("Run", Mathf.Abs(isMoveButton));

        //キャラクターの移動向きを取得
        Vector3 diffPos = transform.position - playerPos;
        diffPos.y = 0f;

        if (diffPos.magnitude > 0.01f) {
            var lookRotation = Quaternion.LookRotation(diffPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.5f);
        }

        playerPos = transform.position;

        switch (state) {

            case State.Idle:
                break;

            case State.Jump:
                break;

            case State.Move:
                break;

            case State.Shot:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        //ジャンプ判定
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            isGround = true;
        }

        //Enemyに当たるとHPが減る(仮)
        bool isEnemyHit = collision.gameObject.CompareTag("Enemy");
        if (isEnemyHit) currentHP -= 10;
        slider.value = (float)currentHP / (float)maxHP;
    }

    //射撃メソッド
    private void Shot() {
        
        bool isLMouseButton = Input.GetMouseButton(0);
        
        if (isLMouseButton) {
            bullet = Instantiate(magicBullet, transform.position + Vector3.forward * 0.5f + Vector3.up, Quaternion.identity);
        }
    }

    private void Jump() {

        this.playerAnimator.SetBool("jumpBool", true);
        playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
        isGround = false;
    }

    private void Move(float isMoveButton) {

        //左右移動
        transform.position += new Vector3(0f, 0f, isMoveButton);
    }
}
