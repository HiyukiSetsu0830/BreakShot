using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour {
    //�X�e�[�g
    public enum State { Idle, Jump, Move };
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
    private const int MIN_HEALTH = 0;

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
    private const float INTERVAL = 0.3f;
    private float intervalTime = 0f;
    //����v���p�e�B
    private float inputHorizontal { get; set; }
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

        //�ړ��L�[
        bool isJumpButton = Input.GetButton("Jump");
        float inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        //�ړ��A�W�����v�v���p�e�B���
        isJump = isJumpButton;
        inputHorizontal = inputMoveButton;

        //�}�E�X�̃|�W�V����
        mousePos = Input.mousePosition;

        //����������
        if (nowState != preState) {
            switch (nowState) {
                //�e�X�e�[�g���Ƃ̏���������
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
        
        //�X�e�[�g����
        switch (nowState) {
            
            case State.Idle:    //�����Ă�����
                CommonStateTransition();
                break;

            case State.Move:    //�ړ����
                CommonStateTransition();
                break;

            case State.Jump:    //�W�����v���
                CommonStateTransition();
                break;
        }

        //�X�e�[�g���Ƃ̎��s�\�s��
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

        //�X�e�[�g�I��
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

        //HP��0�ȉ��ɂȂ�Ȃ��悤�ɐݒ�
        if (currentHP < 0) currentHP = MIN_HEALTH;
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

        if (Time.time <= intervalTime) return;
 
        bool isLMouseButton = Input.GetMouseButton(0);

        if (isLMouseButton) {
            bullet = Instantiate(magicBullet, transform.position + Vector3.forward * 0.5f + Vector3.up, Quaternion.identity);
        }

        intervalTime = INTERVAL + Time.time;
    }

    private void Jump() {
        //Jump�X�e�[�g�̏ꍇ��Jump��false���Z�b�g����
        //this.playerAnimator.SetBool("jumpBool", true);
    }

    private void Move() {

        inputHorizontal = this.inputHorizontal;

        //���E�ړ�
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

        
        if (inputHorizontal == 0f) nowState = State.Idle;                                   //�ړ����͂��������Idle�ɑJ��
        if (inputHorizontal > 0.01f || inputHorizontal < -0.01f) nowState = State.Move;        //�ړ��L�[�������ꂽ��Move�ɑJ��
        if (isGround && isJump) nowState = State.Jump;                        //�W�����v�L�[�������ꂽ��Jump�ɑJ��
    }
}
