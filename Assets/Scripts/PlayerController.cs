using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    
    //プレイヤーのアニメーションを入れる
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;

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
    //プレイヤーのHP
    private int maxHP = 100;
    private int currentHP;
   
    
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

        if (isGround && isJumpButton) {
            this.playerAnimator.SetBool("jumpBool", true);
            playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
            isGround = false;
        }

        //左右移動
        transform.position += new Vector3(0f, 0f, isMoveButton);
        
        //走るアニメーション
        playerAnimator.SetFloat("Run", Mathf.Abs(isMoveButton));

        //キャラクターの移動向きを取得
        Vector3 diffPos = transform.position - playerPos;
        diffPos.y = 0f;

        if (diffPos.magnitude > 0.01f) {
            var lookRotation = Quaternion.LookRotation(diffPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f); ;
        }
        playerPos = transform.position;

        //マウスのポジション
        mousePos = Input.mousePosition;
        //マウスの左クリックで攻撃
        if (isLeftMouseButton) Shot();

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
        mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Enemy"))) {
            Destroy(hit.collider.gameObject);
        }
    }
}
