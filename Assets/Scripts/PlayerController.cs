using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    
    //�v���C���[�̃A�j���[�V����������
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Slider slider;

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
    //�v���C���[��HP
    private int maxHP = 100;
    private int currentHP;
   
    
    // Start is called before the first frame update
    void Start()
    {
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

        if (isGround && isJumpButton) {
            this.playerAnimator.SetBool("jumpBool", true);
            playerRigidbody.velocity = new Vector3(0f, jumpPower, 0f);
            isGround = false;
        }

        //���E�ړ�
        transform.position += new Vector3(0f, 0f, isMoveButton);
        
        //����A�j���[�V����
        playerAnimator.SetFloat("Run", Mathf.Abs(isMoveButton));

        //�L�����N�^�[�̈ړ��������擾
        Vector3 diffPos = transform.position - playerPos;
        diffPos.y = 0f;

        if (diffPos.magnitude > 0.01f) {
            var lookRotation = Quaternion.LookRotation(diffPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f); ;
        }
        playerPos = transform.position;

        //�}�E�X�̃|�W�V����
        mousePos = Input.mousePosition;
        //�}�E�X�̍��N���b�N�ōU��
        if (isLeftMouseButton) Shot();

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
        mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Enemy"))) {
            Destroy(hit.collider.gameObject);
        }
    }
}
