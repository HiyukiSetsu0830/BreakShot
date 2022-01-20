using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageViewScript : MonoBehaviour
{
    [SerializeField] private GameObject DamageObj;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject enemyObj;
    [SerializeField] private Vector3 AdjPos;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewDamage(int damage, string type) {
                
        GameObject _damageObj = Instantiate(DamageObj);
        _damageObj.GetComponent<TextMesh>().text = damage.ToString();
        if(type == "PLAYER") _damageObj.transform.position = enemyObj.transform.position + AdjPos;
        else if(type == "ENEMY") _damageObj.transform.position = playerObj.transform.position + AdjPos;

    }

}
