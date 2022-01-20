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
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject leftMuzzle;
    [SerializeField] private GameObject rightMuzzle;

    //UnityちゃんのHeadをセットする
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject RightArm;

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
    //走り撃ち時の減速割合
    private float runShotSpeed = 0.7f;
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
    //プレイヤーの攻撃力
    private int attackPoint;


    // Start is called before the first frame update
    void Start() {
        //カーソル変更
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
        //Animatorを取得
        playerAnimator = GetComponent<Animator>();
        //プレイヤーのRigidBodyの取得
        playerRigidbody = GetComponent<Rigidbody>();
        //最初の座標を取得
        playerPos = GetComponent<Transform>().position;
        //AudioSource取得
        audioSource = GetComponent<AudioSource>();
        

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

        //前の位置を取得
        Vector3 lastPos = transform.position;
        //ジャンプボタン
        bool isJumpButton = Input.GetButton("Jump");
        //移動キー
        float inputMoveButton;
        //走り撃ちの時、スピードを落とす
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("RunShot") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("BackRun")) inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * runShotSpeed * Time.deltaTime;
        else inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        //移動、ジャンププロパティ代入
        isJump = isJumpButton;
        inputHorizontal = inputMoveButton;

        //マウスのポジション
        mousePos = GetMousePosition();
        

        //現在のステートをアップデートする
        StateProcessor.UpdateExecute();

        //HPが0以下にならないように設定
        if (currentHP < 0) currentHP = MIN_HEALTH;

        //HP0になったらDeadアニメーション再生
        this.playerAnimator.SetInteger("Health", currentHP);

        //プレイヤーの向き
        Vector3 playerToMouse = mousePos - playerPos;
        float playerRotationY = transform.eulerAngles.y;
        playerToMouse.y = 0f;
        var lookRotation = Quaternion.LookRotation(playerToMouse);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.5f);
        playerAnimator.SetFloat("PlayerDirection", playerRotationY);
        //過去と現在の位置の比較して、パラメータに渡す
        float playerPosZ = playerPos.z - lastPos.z;
        playerAnimator.SetFloat("PlayerPosZ",playerPosZ);

    }

    private void OnCollisionEnter(Collision collision) {

        //ジャンプ判定
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            isGround = true;
        }

        //Enemyに当たるとHPが減る(仮)
        bool isEnemyHit = collision.gameObject.CompareTag("Enemy");
        if (isEnemyHit) currentHP -= 100;
        slider.value = (float)currentHP / (float)maxHP;
        
    }

    //IKアニメーション
    private void OnAnimatorIK(int layerIndex) {

        //走っているのみの場合はカーソルの方向を向かない
        if (StateProcessor.State.Value == StateRun && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("RunShot") == false) {

            this.playerAnimator.SetLookAtPosition(this.mousePos);

            //その他はカーソルを見る
        } else {

            this.playerAnimator.SetLookAtWeight(0.5f, 0.8f, 0f, 0.0f, 0f);
            this.playerAnimator.SetLookAtPosition(this.mousePos);

        }

       
    }

    public int AttackPoint() {

        attackPoint = Random.Range(1, 4);

        return attackPoint;
    }


    //射撃メソッド
    private void Shot() {

        if (Time.time <= intervalTime) return;

        bool isLMouseButton = Input.GetMouseButton(0);
        //攻撃力のブレ
        attackPoint = AttackPoint();

        if (isLMouseButton) {
            this.playerAnimator.SetBool("ShotBool", true);
            bullet = Instantiate(playerBullet, leftMuzzle.transform.position, Quaternion.identity);
            bullet.transform.LookAt(mousePos);
            bullet = Instantiate(playerBullet, rightMuzzle.transform.position, Quaternion.identity);
            bullet.transform.LookAt(mousePos);


        } else {
            this.playerAnimator.SetBool("ShotBool", false);
        }

            intervalTime = INTERVAL + Time.time;
        
    }

    private void OnDrawGizmos() {

        Gizmos.DrawLine(leftMuzzle.transform.position, mousePos);

    }

    //移動メソッド
    private void Move() {

        //移動値を取得
        inputHorizontal = this.inputHorizontal;
        
        //左右移動
        transform.position += new Vector3(0f, 0f, inputHorizontal);
        playerAnimator.SetFloat("Run", Mathf.Abs(inputHorizontal));
        //プレイヤーの位置を更新      
        playerPos = transform.position;

    }

    //マウスポジションを取得する関数
    public Vector3 GetMousePosition() {

        //位置座標
        Vector3 mousePosition;
        Vector3 screenToWorldPointPosition;

        //Vector3でマウス位置座標を取得する
        mousePosition = Input.mousePosition;
        //Z軸修正
        mousePosition.z = 6.5f;
        //マウス位置座用をスクリーン座標からワールド座標に変換する
        screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        return screenToWorldPointPosition;

    }

    

    /*********************************************************************
     *ステート初期化関数
     ********************************************************************/

    /// <summary>
    /// <param name="InitializeJump">Jumpステートの初期化</param>
    /// </summary>
    private void InitializeJump() {

        Debug.Log("【初期化】StateがJump状態に遷移しました。");
        this.playerAnimator.SetBool("jumpBool", true);
        audioSource.PlayOneShot(clip);
        playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
        isGround = false;

    }

    /// <summary>
    /// /// <param name="InitializeRun">Runステートの初期化</param>
    /// </summary>
    private void InitializeRun() {

        Debug.Log("【初期化】StateがRun状態に遷移しました。");
        
    }

    /// <summary>
    /// <param name="InitializeIdle">Idleステートの初期化</param>
    /// </summary>
    private void InitializeIdle() {
        Debug.Log("【初期化】StateがIdle状態に遷移しました。");


    }

    /// <summary>
    /// <param name="InitializeFall">Idleステートの初期化</param>
    /// </summary>
    private void InitializeFall() {
        Debug.Log("【初期化】StateがFall状態に遷移しました。");
        this.playerAnimator.SetBool("Fall", true);

    }

    /************************************************************
     * ステータス実行中の処理
     * *********************************************************/

    private void UpdateRun() {

        Debug.Log("StateがRun状態中です。。");
        Shot();
        Move();
        if (inputHorizontal == 0f && isGround) {
            EndRun();
            StateProcessor.State.Value = StateIdle;
        }
        if (isJump && isGround) {
            EndRun();
            StateProcessor.State.Value = StateJump;
        }
        //RunからFallに遷移
        if (this.playerRigidbody.velocity.y < 0f && !isGround) {
            EndIdle();
            StateProcessor.State.Value = StateFall;
        }

             
    }

    private void UpdateIdle() {

        Debug.Log("StateがIdle状態中です。");
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) {

            EndIdle();
            StateProcessor.State.Value = StateRun;
        }

        if (isJump && isGround) {

            EndIdle();
            StateProcessor.State.Value = StateJump;

        }
        //IdleからFallに遷移
        if (this.playerRigidbody.velocity.y < 0f && !isGround) {
            EndIdle();
            StateProcessor.State.Value = StateFall;
        }

    }

    private void UpdateJump() {

        Debug.Log("StateがJump状態中です。");
        Shot();
        Move();        
        //JumpからFallに遷移
        if (this.playerRigidbody.velocity.y < 0f && !isGround) {
            EndJump();
            StateProcessor.State.Value = StateFall;
        }

        if (isGround && inputHorizontal == 0f) {
            EndJump();
            StateProcessor.State.Value = StateIdle;
        }

    }

    private void UpdateFall() {

        Debug.Log("StateがFall状態中です。。");
        Shot();
        Move();
        if (isGround && Mathf.Abs(inputHorizontal) > 0.01) {
            EndFall();
            StateProcessor.State.Value = StateRun;
        }
        if (inputHorizontal == 0f && isGround) {
            EndFall();
            StateProcessor.State.Value = StateIdle;
        }
        //Fallからジャンプの処理
        if (this.playerRigidbody.velocity.y > 0f && !isGround) {
            EndFall();
            StateProcessor.State.Value = StateJump;
        }
    }

    /****************************************************************
     * ステータス終了の処理
     * *************************************************************/

    private void EndIdle() {
        Debug.Log("Idle状態が終了しました。");
        
    }

    private void EndRun() {
        Debug.Log("Run状態が終了しました。");
        //this.playerAnimator.SetFloat("Run",0f);
        
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
