using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletScript : MonoBehaviour
{
    private Rigidbody rb;
    //’e‚Ì‰ñ“]
    private float randomNum;
    //’e‚Ì‘¬“x
    private float speed = 4000f;
    //’e‚ÌŒü‚«
    private Vector3 bossBulletRotate;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("unitychan").transform.position;
        bossBulletRotate = new Vector3(0f, 0f, playerPos.z);
        this.transform.LookAt(bossBulletRotate);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //’e‚Ì“]‚ðƒ‰ƒ“ƒ_ƒ€‚ÉŽæ“¾
        /* randomNum = Random.Range(3f, 5f);
         transform.Rotate(randomNum, 0f, randomNum);*/

        rb.velocity = transform.forward * speed * Time.deltaTime;
        bool rend = GetComponent<Renderer>().isVisible;
        if (!rend) {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        bool bulletDestroy = collision.gameObject.CompareTag("Player");
        if (bulletDestroy) Destroy(this.gameObject);
    }
}
