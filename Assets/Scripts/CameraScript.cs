using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    GameObject player;
    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("unitychan");
        playerTransform = player.transform;
    }


    private void LateUpdate() {
        MoveCamera();
    }

    void MoveCamera() {
        //â°ï˚å¸ÇæÇØí«è]
        transform.position = new Vector3(transform.position.x,transform.position.y, playerTransform.position.z);
    }
}
