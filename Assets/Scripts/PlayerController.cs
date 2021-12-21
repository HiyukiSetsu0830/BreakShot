using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour {
    //ステート
    public enum State { Idle, Jump, Move };
    State nowState;
    State preState;

    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject magicBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    private GameObject bullet;

    //HPの最小値
    private const int MIN_HEALTH = 0;

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
    private const float INTERVAL = 0.3f;
    private float intervalTime = 0f;
    //操作プロパティ
    private float inputHorizontal { get; set; }
    private bool isJump { get; set; }


    // Start is called before the first frame update
    void Start() {
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
        nowState = State.Idle;

    }

    // Update is called once per frame
    void Update() {

        //移動キー
        bool isJumpButton = Input.GetButton("Jump");
        float inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        //移動、ジャンププロパティ代入
        isJump = isJumpButton;
        inputHorizontal = inputMoveButton;

        //マウスのポジション
        mousePos = Input.mousePosition;

        //初期化処理
        if (nowState != preState) {
            switch (nowState) {
                //各ステートごとの初期化処理
                case State.Idle:
                    playerAnimator.SetFloat("Run", 0f);
                    break;

                case State.Move:
                    break;

                case State.Jump:
                    this.playerAnimator.SetBool("jumpBool", false);
                    playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
                    isGround = false;
                    break;

            }
            preState = nowState;
        }
        
        //ステート条件
        switch (nowState) {
            
            case State.Idle:    //立っている状態
                CommonStateTransition();
                break;

            case State.Move:    //移動状態
                CommonStateTransition();
                break;

            case State.Jump:    //ジャンプ状態
                CommonStateTransition();
                break;
        }

        //ステートごとの実行可能行動
        switch (nowState) {
            case State.Idle:
                Jump();
                Shot();
                break;

            case State.Move:
                Jump();
                Move();
                Shot();
                break;

            case State.Jump:
                Jump();
                Shot();
                break;
        }

        //ステート終了
        if (nowState != preState) {
            switch (nowState) {

                case State.Idle:
                    break;

                case State.Move:
                    break;

                case State.Jump:
                    this.playerAnimator.SetBool("jumpBool", true);
                    break;

            }
        }

        //HPが0以下にならないように設定
        if (currentHP < 0) currentHP = MIN_HEALTH;
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

        if (Time.time <= intervalTime) return;
 
        bool isLMouseButton = Input.GetMouseButton(0);

        if (isLMouseButton) {
            bullet = Instantiate(magicBullet, transform.position + Vector3.forward * 0.5f + Vector3.up, Quaternion.identity);
        }

        intervalTime = INTERVAL + Time.time;
    }

    private void Jump() {
        //Jumpステートの場合はJumpにfalseをセットする
        //this.playerAnimator.SetBool("jumpBool", true);
    }

    private void Move() {

        inputHorizontal = this.inputHorizontal;

        //左右移動
        transform.position += new Vector3(0f, 0f, inputHorizontal);
        playerAnimator.SetFloat("Run", Mathf.Abs(inputHorizontal));
        Vector3 diffPos = transform.position - playerPos;
        diffPos.y = 0f;
        if (diffPos.magnitude > 0.01f) {
            var lookRotation = Quaternion.LookRotation(diffPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.5f);
        }
        playerPos = transform.position;
    }

    private void CommonStateTransition() {

        inputHorizontal = this.inputHorizontal;
        isJump = this.isJump;

        
        if (inputHorizontal == 0f) nowState = State.Idle;                                   //移動入力が無ければIdleに遷移
        if (inputHorizontal > 0.01f || inputHorizontal < -0.01f) nowState = State.Move;        //移動キーが押されたらMoveに遷移
        if (isGround && isJump) nowState = State.Jump;                        //ジャンプキーが押されたらJumpに遷移
    }
}
