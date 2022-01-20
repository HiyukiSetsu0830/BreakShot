using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletScript : MonoBehaviour
{
    private Rigidbody rb;
    //’e‚Ì‰ñ“]
    private float randomNum;
    //’e‚Ì‘¬“x
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //ƒ{ƒX‚Ì‰ñ“]‚ðƒ‰ƒ“ƒ_ƒ€‚ÉŽæ“¾
        randomNum = Random.Range(3f, 5f);
        transform.Rotate(randomNum, 0f, randomNum);

        rb.velocity = transform.forward * speed * Time.deltaTime;
    }
}
