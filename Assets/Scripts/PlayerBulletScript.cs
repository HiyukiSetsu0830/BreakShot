using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerController playerController;

    private float speed = 2500f;
    private Vector3 mousePos;

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
