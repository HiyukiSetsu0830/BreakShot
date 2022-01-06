using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using CharacterState;


public class PlayerController : MonoBehaviour {

    //�v���C���[�̃A�j���[�V����������
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private GameObject magicBullet;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;
    private GameObject bullet;

    //�ύX�O�̃X�e�[�g��
    private string prevStateName;

    //�X�e�[�g
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
    public CharacterStateJump StateJump { get; set; } = new CharacterStateJump();
    public CharacterStateFall StateFall { get; set; } = new CharacterStateFall();

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

        /*******************************************************
         * �v���C���[�̃X�e�[�^�X�ݒ�
         * ****************************************************/

        //�X�^�[�g���̏��
        StateProcessor.State.Value = StateIdle;

        //�������p�̊֐���o�^����
        StateIdle.InitializeExecAction = InitializeIdle;
        StateRun.InitializeExecAction = InitializeRun;
        StateJump.InitializeExecAction = InitializeJump;
        StateFall.InitializeExecAction = InitializeFall;
        //���s�p�̊֐���o�^����
        StateIdle.UpdateExecAction = UpdateIdle;
        StateRun.UpdateExecAction = UpdateRun;
        StateJump.UpdateExecAction = UpdateJump;
        StateFall.UpdateExecAction = UpdateFall;
        //�I���p�̊֐���o�^����
        StateIdle.EndExecAction = EndIdle;
        StateRun.EndExecAction = EndRun;
        StateJump.EndExecAction = EndJump;
        StateFall.EndExecAction = EndFall;

        //�X�e�[�g�̒l���ύX���ꂽ����s�������s���悤�ɂ���
        StateProcessor.State.Where(_ => StateProcessor.State.Value.GetStateName() != prevStateName).Subscribe(_ => {
            prevStateName = StateProcessor.State.Value.GetStateName();
            StateProcessor.InitializeExecute();
        }).AddTo(this);

        //HP�ő�
        slider.value = 1;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update() {

        //�W�����v�{�^��
        bool isJumpButton = Input.GetButton("Jump");
        //�ړ��L�[
        float inputMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        //���͂������ꍇ
        bool inputHorizontalKey = Mathf.Abs(inputHorizontal) > 0.01;

        //�ړ��A�W�����v�v���p�e�B���
        isJump = isJumpButton;
        inputHorizontal = inputMoveButton;

        //�}�E�X�̃|�W�V����
        mousePos = Input.mousePosition;

        //���݂̃X�e�[�g���A�b�v�f�[�g����
        StateProcessor.UpdateExecute();
        StateProcessor.State.Where(_ => StateProcessor.State.Value.GetStateName() != prevStateName).Subscribe(_ => {
            prevStateName = StateProcessor.State.Value.GetStateName();
            StateProcessor.UpdateExecute();
        }).AddTo(this);

        //���݂̃X�e�[�^�X���I������
        if (!inputHorizontalKey || this.playerRigidbody.velocity.y > 0f && !isGround) StateProcessor.EndExecute();

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

    private void CommonIdle() {

        StateProcessor.State.Value = StateIdle;

    }

    private void CommonRun() {

        //�X�e�[�^�X�̍X�V
        StateProcessor.State.Value = StateRun;
        //�ړ��l���擾
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

    private void CommonJump() {

       
        playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
        isGround = false;

        //�X�e�[�^�X�X�V�@�W�����v���Ȃ�Jump�X�e�[�^�X
        if (playerRigidbody.velocity.y > 0f && !isGround) {

            StateProcessor.State.Value = StateJump;
            this.playerAnimator.SetBool("jumpBool", true);
        }        
    }

    private void CommonFall() {

        StateProcessor.State.Value = StateFall;
        //Fall�A�j���[�V������true�ɂ���
        this.playerAnimator.SetBool("Fall", true);
    }


    /*********************************************************************
     *�X�e�[�g�������֐�
     ********************************************************************/

    /// <summary>
    /// <param name="InitializeJump">Jump�X�e�[�g�̏�����</param>
    /// </summary>
    private void InitializeJump() {

        Debug.Log("�y�������zState��Jump��ԂɑJ�ڂ��܂����B");

       
    }

    /// <summary>
    /// /// <param name="InitializeRun">Run�X�e�[�g�̏�����</param>
    /// </summary>
    private void InitializeRun() {

        Debug.Log("�y�������zState��Run��ԂɑJ�ڂ��܂����B");
        //���ɏ����Ȃ�
    }

    /// <summary>
    /// <param name="InitializeIdle">Idle�X�e�[�g�̏�����</param>
    /// </summary>
    private void InitializeIdle() {
        Debug.Log("�y�������zState��Idle��ԂɑJ�ڂ��܂����B");
        playerAnimator.SetFloat("Run", 0f);

    }

    /// <summary>
    /// <param name="InitializeIdle">Idle�X�e�[�g�̏�����</param>
    /// </summary>
    private void InitializeFall() {
        Debug.Log("�y�������zState��Fall��ԂɑJ�ڂ��܂����B");

        
    }

    /************************************************************
     * �X�e�[�^�X���s���̏���
     * *********************************************************/

    private void UpdateRun() {

        Debug.Log("State��Run��ԂɑJ�ڂ��܂����B");
        
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (inputHorizontal == 0f && !isGround) CommonIdle();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();

    }

    private void UpdateIdle() {

        Debug.Log("State��Idle��ԂɑJ�ڂ��܂����B");
        
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (inputHorizontal == 0f && !isGround) CommonIdle();
        if (playerRigidbody.velocity.y < 0f) CommonFall();
    }

    private void UpdateJump() {
        Debug.Log("State��Jump��ԂɑJ�ڂ��܂����B");
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();

    }

    private void UpdateFall() {
        Debug.Log("State��Fall��ԂɑJ�ڂ��܂����B");
        Shot();
        if (Mathf.Abs(inputHorizontal) > 0.01) CommonRun();
        if (isJump && isGround) CommonJump();
        if (playerRigidbody.velocity.y < 0f) CommonFall();
    }

    /****************************************************************
     * �X�e�[�^�X�I���̏���
     * *************************************************************/

    private void EndIdle() {
        Debug.Log("Idle��Ԃ��I�����܂����B");
        
    }

    private void EndRun() {
        Debug.Log("Run��Ԃ��I�����܂����B");
        this.playerAnimator.SetFloat("Run",0f);
        
    }

    private void EndJump() {
        Debug.Log("Jump��Ԃ��I�����܂����B");
        this.playerAnimator.SetBool("jumpBool", false);
    }

    private void EndFall() {
        Debug.Log("Fall��Ԃ��I�����܂����B");
        //Fall�A�j���[�V������false�ɂ���
        this.playerAnimator.SetBool("Fall", false);
    }
}
