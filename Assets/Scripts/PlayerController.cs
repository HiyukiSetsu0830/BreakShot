using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    

    //�v���C���[�̃A�j���[�V����������
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;

    //�O�����̑��x
    private float speed = 10f;
    //������̑��x
    private float jumpPower = 9f;
    //�n�ʐڐG����
    private bool isGround;


    // Start is called before the first frame update
    void Start()
    {
          
        //Animator���擾
        playerAnimator = GetComponent<Animator>();
        //RigidBody�̎擾
        playerRigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update() {

        //Jump�X�e�[�g�̏ꍇ��Jump��false���Z�b�g����
        bool isJumpSetFalse = this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumpBool");
        this.playerAnimator.SetBool("jumpBool", false);

        //�ړ��L�[
        bool isJumpButton = Input.GetButton("Jump");
        float isMoveButton = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        if (isGround && isJumpButton) {
            this.playerAnimator.SetBool("jumpBool", true);
            playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
            isGround = false;
        }

       //���E�ړ�
        transform.position += new Vector3(0f, 0f, isMoveButton);
      
    }

    private void OnCollisionEnter(Collision collision) {

        bool isGrounded = collision.gameObject.CompareTag("Ground");
        if (isGrounded) {
            isGround = true;
        }
    }
}
