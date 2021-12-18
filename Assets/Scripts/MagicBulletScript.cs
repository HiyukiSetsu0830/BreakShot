using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBulletScript : MonoBehaviour
{
    private Rigidbody rb;

    private float speed = 2000f;
    private int power = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        rb.velocity = transform.forward * speed * Time.deltaTime;
        bool rend = GetComponent<Renderer>().isVisible;
        if (!rend) {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        bool bulletDestroy = collision.gameObject.CompareTag("Enemy");
        Destroy(this.gameObject);
    }
}
