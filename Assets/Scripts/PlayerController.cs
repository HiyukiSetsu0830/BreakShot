using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using CharacterState;


public class PlayerController : MonoBehaviour {

    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject magicBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    private GameObject bullet;

    //変更前のステート名
    private string prevStateName;

    //ステート
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
    public CharacterStateJump StateJump { get; set; } = new CharacterStateJump();
    public CharacterStateFall StateFall { get; set; } = new CharacterStateFall();

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

        /*******************************************************
         * プレイヤーのステータス設定
         * ****************************************************/

        //スタート時の状態
        StateProcessor.State.Value = StateIdle;

        //初期化用の関数を登録する
        StateIdle.InitializeExecAction = InitializeIdle;
        StateRun.InitializeExecAction = InitializeRun;
        StateJump.InitializeExecAction = InitializeJump;
        StateFall.InitializeExecAction = InitializeFall;
        //実行用の関数を登録する
        StateIdle.UpdateExecAction = UpdateIdle;
        StateRun.UpdateExecAction = UpdateRun;
        StateJump.UpdateExecAction = UpdateJump;
        StateFall.UpdateExecAction = UpdateFall;
        //終了用の関数を登録する
        StateIdle.EndExecAction = EndIdle;
        StateRun.EndExecAction = EndRun;
        StateJump.EndExecAction = EndJump;
        StateFall.EndExecAction = EndFall;

        //ステートの値が変更されたら実行処理を行うようにする
        StateProcessor.State.Where(_ => StateProcessor.State.Value.GetStateName() != prevStateName).Subscribe(_ => {
            prevStateName = StateProcessor.State.Value.GetStateName();
            StateProcessor.InitializeExecute();
        }).AddTo(this);

        //HP最大
        slider.value = 1;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update() {

        //ジャンプボタン
        bool isJumpButton = Input.GetButton("Jump");
        //移動キー
        float inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        //入力が無い場合
        bool inputHorizontalKey = Mathf.Abs(inputHorizontal) > 0.01;

        //移動、ジャンププロパティ代入
        isJump = isJumpButton;
        inputHorizontal = inputMoveButton;

        //マウスのポジション
        mousePos = Input.mousePosition;

        //現在のステートをアップデートする
        StateProcessor.UpdateExecute();
        StateProcessor.State.Where(_ => StateProcessor.State.Value.GetStateName() != prevStateName).Subscribe(_ => {
            prevStateName = StateProcessor.State.Value.GetStateName();
            StateProcessor.UpdateExecute();
        }).AddTo(this);

        //現在のステータスを終了する
        if (!inputHorizontalKey || this.playerRigidbody.velocity.y > 0f && !isGround) StateProcessor.EndExecute();

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

    private void CommonIdle() {

        StateProcessor.State.Value = StateIdle;

    }

    private void CommonRun() {

        //ステータスの更新
        StateProcessor.State.Value = StateRun;
        //移動値を取得
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

    private void CommonJump() {

       
        playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
        isGround = false;

        //ステータス更新　ジャンプ中ならJumpステータス
        if (playerRigidbody.velocity.y > 0f && !isGround) {

            StateProcessor.State.Value = StateJump;
            this.playerAnimator.SetBool("jumpBool", true);
        }        
    }

    private void CommonFall() {

        StateProcessor.State.Value = StateFall;
        //Fallアニメーションをtrueにする
        this.playerAnimator.SetBool("Fall", true);
    }


    /*********************************************************************
     *ステート初期化関数
     ********************************************************************/

    /// <summary>
    /// <param name="InitializeJump">Jumpステートの初期化</param>
    /// </summary>
    private void InitializeJump() {

        Debug.Log("【初期化】StateがJump状態に遷移しました。");

       
    }

    /// <summary>
    /// /// <param name="InitializeRun">Runステートの初期化</param>
    /// </summary>
    private void InitializeRun() {

        Debug.Log("【初期化】StateがRun状態に遷移しました。");
        //特に処理なし
    }

    /// <summary>
    /// <param name="InitializeIdle">Idleステートの初期化</param>
    /// </summary>
    private void InitializeIdle() {
        Debug.Log("【初期化】StateがIdle状態に遷移しました。");
        playerAnimator.SetFloat("Run", 0f);

    }

    /// <summary>
    /// <param name="InitializeIdle">Idleステートの初期化</param>
    /// </summary>
    private void InitializeFall() {
        Debug.Log("【初期化】StateがFall状態に遷移しました。");

        
    }

    /************************************************************
     * ステータス実行中の処理
     * *********************************************************/

    private void UpdateRun() {

        Debug.Log("StateがRun状態に遷移しました。");
        
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (inputHorizontal == 0f && !isGround) CommonIdle();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();

    }

    private void UpdateIdle() {

        Debug.Log("StateがIdle状態に遷移しました。");
        
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (inputHorizontal == 0f && !isGround) CommonIdle();
        if (playerRigidbody.velocity.y < 0f) CommonFall();
    }

    private void UpdateJump() {
        Debug.Log("StateがJump状態に遷移しました。");
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();

    }

    private void UpdateFall() {
        Debug.Log("StateがFall状態に遷移しました。");
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();
    }

    /****************************************************************
     * ステータス終了の処理
     * *************************************************************/

    private void EndIdle() {
        Debug.Log("Idle状態が終了しました。");
        
    }

    private void EndRun() {
        Debug.Log("Run状態が終了しました。");
        this.playerAnimator.SetFloat("Run",0f);
        
    }

    private void EndJump() {
        Debug.Log("Jump状態が終了しました。");
        this.playerAnimator.SetBool("jumpBool", false);
    }

    private void EndFall() {
        Debug.Log("Fall状態が終了しました。");
        //Fallアニメーションをfalseにする
        this.playerAnimator.SetBool("Fall", false);
    }
}
