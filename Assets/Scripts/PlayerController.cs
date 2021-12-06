using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    

    //�v���C���[�̃A�j���[�V����������
    [SerializeField] private Animator playerAnimator;
    //�v���C���[��Rigidbody������
    [SerializeField] private Rigidbody playerRigidbody;

    //�O�����̑��x
    private float forceZ = 40f;
    //������̑��x
    private float forceY = 500f;
    //���E�̈ړ��ł���͈�
    private float movableRange = 0.5f;
    //�{�^���̏��
    private bool isJButtonDown = false;
    private bool isRButtonDown = false;
    private bool isLButtonDown = false;

    //�n�ʐڐG����
    private bool isGround;
    //������ύX �ύX�O�̃|�W�V����
    private Vector3 latestPos;

    // Start is called before the first frame update
    void Start()
    {
        //Animator���擾
        playerAnimator = GetComponent<Animator>();
        //Rigidbody���擾
        playerRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
        //�ړ��L�[�ݒ�
        bool isJump = Input.GetKeyDown(KeyCode.Space);
        bool isLeftMove = Input.GetKey(KeyCode.A);
        bool isRightMove = Input.GetKey(KeyCode.D);
        bool isUnderMove = Input.GetKey(KeyCode.S);
        //�ړ��L�[�𗣂�����
        bool isLeftUp = Input.GetKeyUp(KeyCode.A);
        bool isRightUp = Input.GetKeyUp(KeyCode.D);

        //X���̐���
        bool notMove = movableRange <= this.transform.position.x;

        //Player�C���X�^���X����
        Player player = new Player();
        player.ForceY = forceY;
        player.ForceZ = forceZ;
        player.PlayerRB = playerRigidbody;
        player.JumpAnimator = playerAnimator;

        //Jump�X�e�[�g�̏ꍇ��Jump��false���Z�b�g
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        bool isMoveSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk");

        //�L�����N�^�[�̌����𔻒�
        Vector3 diff = transform.position - latestPos;
        //�O���Position�̍X�V
        latestPos = transform.position;
        //�x�N�g���̑傫���ɂ���Č�����ς���
        bool isDirection = diff.magnitude > 0.01f;
        //�L�����N�^�[�̌�����ς���
        Vector3 direction = transform.localScale;

        if (isJump && isGround) player.Jump();
        if (isLeftMove && isDirection) {

            player.LeftMove();
            //������ύX����
            //transform.rotation = Quaternion.LookRotation(diff); 
        } else if (isLeftUp) {



        }

        if (isRightMove) {

            player.RightMove();
            //������ύX����
            //transform.rotation = Quaternion.LookRotation(diff);
        }

        if (isJumpSetFalse) this.playerAnimator.SetBool("Jump", false);
        if(isMoveSetFalse) this.playerAnimator.SetBool("Walk", false);
        if(isMoveSetFalse) this.playerAnimator.SetBool("Walk", false);
    }

    private void OnCollisionEnter(Collision collision) {

        //�v���C���[�ƒn�ʂ̔���
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) isGround = true;
    }

    //�v���C���[���ۃN���X
    abstract class Human {

        //���ۃ��\�b�h
        public abstract void Jump();
        public abstract void LeftMove();
        public abstract void RightMove();

    }

    private class Player : Human {

        
        public Rigidbody PlayerRB { get; set; }
        public float ForceY { get; set; }
        public float ForceZ { get; set; }
        public Animator JumpAnimator { get; set; }


        //�L�����N�^�[�̑���֐�
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
