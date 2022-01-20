using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletScript : MonoBehaviour
{
    private Rigidbody rb;
    //�e�̉�]
    private float randomNum;
    //�e�̑��x
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //�{�X�̉�]�������_���Ɏ擾
        randomNum = Random.Range(3f, 5f);
        transform.Rotate(randomNum, 0f, randomNum);

        rb.velocity = transform.forward * speed * Time.deltaTime;
    }
}
