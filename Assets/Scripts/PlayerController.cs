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
    private float speed = 2f;
    //������̑��x
    private float jumpPower = 5f;
    //�W�����v�\���ǂ���
    bool canJump = false;
    //�n�ʐڐG����
    private bool isGround = false;
    //�v���C���[�̍��W
    private Vector3 startPos;
    private Vector3 playerPos;
    //�v���C���[�����ʒu
    private const float startPosZ = -20f;
    private const float startPosY = 0.25f;

    //�ړ��L�[�̔���
    bool getJButtonDown = false;
    bool getRButtonDown = false;
    bool getLButtonDown = false;
   


    // Start is called before the first frame update
    void Start()
    {
        //Animator���擾
        playerAnimator = GetComponent<Animator>();
        //Rigidbody���擾
        playerRigidbody = GetComponent<Rigidbody>();
        //�v���C���[�̏������W
        startPos = new Vector3(0f, startPosY, startPosZ);
        //�v���C���[�̍��W
        playerPos = transform.position;

    }

    // Update is called once per frame
    void Update() {

        //�ړ��L�[
        float isJumpButton = Input.GetAxis("Jump") * jumpPower;
        float isMoveButton = Input.GetAxis("Horizontal") * speed;

        if (canJump) {
            canJump = false;
            playerPos += new Vector3(0f, isJumpButton, 0f);
        }

        playerPos += new Vector3(0f, 0f, isMoveButton);
    }

    private void OnCollisionEnter(Collision collision) {

        
        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            canJump = true;
        }
    }
}
