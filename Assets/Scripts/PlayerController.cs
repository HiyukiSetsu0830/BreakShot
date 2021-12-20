using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour {
    //�X�e�[�g
    public enum State { Idle, Jump, Move, Shot };
    State nowState;
    State preState;

    //�v���C���[�̃A�j���[�V����������
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject magicBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    private GameObject bullet;

    //HP�̍ŏ��l
    const int minHealth = 0;

    //�O�����̑��x
    private float speed = 10f;
    //������̑��x
    private float jumpPower = 9f;
    //�n�ʐڐG����
    private bool isGround;
    //�v���C���[�̍��W
    private Vector3 playerPos;
    //�}�E�X�J�[�\���̍��W
    private Vector3 mousePos;
    //HP�n�̃v���p�e�B
    public int currentHP { get; private set; }
    public int maxHP { get; private set; } = 100;
    //�U���Ԋu
    private float interval = 0.3f;
    private float intervalTime = 0f;
    //����v���p�e�B
    private float isMove { get; set; }
    private bool isLMouse { get; set; }
    private bool isJump { get; set; }


    // Start is called before the first frame update
    void Start() {
        //�J�[�\���ύX
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
        //Animator���擾
        playerAnimator = GetComponent<Animator>();
        //RigidBody�̎擾
        playerRigidbody = GetComponent<Rigidbody>();
        //�ŏ��̍��W���擾
        playerPos = GetComponent<Transform>().position;

        //HP�ő�
        slider.value = 1;
        currentHP = maxHP;

        //�v���C���[�̏󋵏�����
        nowState = State.Idle;

    }

    // Update is called once per frame
    void Update() {

        //Jump�X�e�[�g�̏ꍇ��Jump��false���Z�b�g����
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumpBool");
        this.playerAnimator.SetBool("jumpBool", false);

        //�ړ��L�[
        bool isJumpButton = Input.GetButton("Jump");
        float isMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        //�U���L�[
        bool isLeftMouseButton = Input.GetButton("Fire1");

        //�}�E�X�̃|�W�V����
        mousePos = Input.mousePosition;

        //�ˌ��Ԋu
        if (intervalTime > 0.0f) intervalTime -= Time.deltaTime;
        Debug.Log(nowState);
        switch (nowState) {

            case State.Idle:    //�����Ă�����
                if (isMoveButton == 0f) nowState = State.Idle;                                   //�ړ����͂��������Idle�ɑJ��
                if (isMoveButton > 0.01f || isMoveButton < -0.01f) nowState = State.Move;        //�ړ��L�[�������ꂽ��Move�ɑJ��
                if (isGround && isJumpButton) nowState = State.Jump;                        //�W�����v�L�[�������ꂽ��Jump�ɑJ��
                if (isLeftMouseButton && intervalTime <= 0.0f) nowState = State.Shot;           //���N���b�N���ꂽ��Shot��ԂɑJ��
                break;

            case State.Move:
                if (isMoveButton == 0f) nowState = State.Idle;                                   //�ړ����͂��������Idle�ɑJ��
                if (isMoveButton > 0.01f || isMoveButton < -0.01f) nowState = State.Move;        //�ړ��L�[�������ꂽ��Move�ɑJ��
                if (isGround && isJumpButton) nowState = State.Jump;                        //�W�����v�L�[�������ꂽ��Jump�ɑJ��
                if (isLeftMouseButton && intervalTime <= 0.0f) nowState = State.Shot;           //���N���b�N���ꂽ��Shot��ԂɑJ��
                break;

            case State.Jump:
                if (isMoveButton == 0f) nowState = State.Idle;                                   //�ړ����͂��������Idle�ɑJ��
                if (isMoveButton > 0.01f || isMoveButton < -0.01f) nowState = State.Move;        //�ړ��L�[�������ꂽ��Move�ɑJ��
                if (isGround && isJumpButton) nowState = State.Jump;                        //�W�����v�L�[�������ꂽ��Jump�ɑJ��
                if (isLeftMouseButton && intervalTime <= 0.0f) nowState = State.Shot;           //���N���b�N���ꂽ��Shot��ԂɑJ��
                break;

            case State.Shot:
                if (isMoveButton == 0f) nowState = State.Idle;                                   //�ړ����͂��������Idle�ɑJ��
                if (isMoveButton > 0.01f || isMoveButton < -0.01f) nowState = State.Move;        //�ړ��L�[�������ꂽ��Move�ɑJ��
                if (isGround && isJumpButton) nowState = State.Jump;                        //�W�����v�L�[�������ꂽ��Jump�ɑJ��
                if (isLeftMouseButton && intervalTime <= 0.0f) nowState = State.Shot;           //���N���b�N���ꂽ��Shot��ԂɑJ��
                break;
        }

        switch (nowState) {
            case State.Idle:
                playerAnimator.SetFloat("Run", 0f);
                break;

            case State.Move:
                isMove = isMoveButton;
                Move();
                break;

            case State.Jump:
                Jump();
                break;

            case State.Shot:
                Shot();
                break;
        }

        //HP��0�ȉ��ɂȂ�Ȃ��悤�ɐݒ�
        if (currentHP < 0) currentHP = minHealth;
    }

    private void OnCollisionEnter(Collision collision) {

        //�W�����v����
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            isGround = true;
        }

        //Enemy�ɓ������HP������(��)
        bool isEnemyHit = collision.gameObject.CompareTag("Enemy");
        if (isEnemyHit) currentHP -= 10;
        slider.value = (float)currentHP / (float)maxHP;
    }

    //�ˌ����\�b�h
    private void Shot() {

        bool isLMouseButton = Input.GetMouseButton(0);

        if (isLMouseButton) {
            bullet = Instantiate(magicBullet, transform.position + Vector3.forward * 0.5f + Vector3.up, Quaternion.identity);
        }

        intervalTime = interval;
    }

    private void Jump() {

        this.playerAnimator.SetBool("jumpBool", true);
        playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
        isGround = false;
    }

    private void Move() {

        isMove = this.isMove;

        //���E�ړ�
        transform.position += new Vector3(0f, 0f, isMove);
        playerAnimator.SetFloat("Run", Mathf.Abs(isMove));
        Vector3 diffPos = transform.position - playerPos;
        diffPos.y = 0f;
        if (diffPos.magnitude > 0.01f) {
            var lookRotation = Quaternion.LookRotation(diffPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.5f);
        }
        playerPos = transform.position;
    }
}
