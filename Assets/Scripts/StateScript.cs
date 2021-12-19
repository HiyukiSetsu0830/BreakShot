using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateScript : MonoBehaviour
{

    //ステート
    public enum State { Jump, Move, Attack };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        State state = State.Jump;
        switch(state){

            case State.Jump:  //ジャンプ中
                break;

            case State.Move:    //移動中
                break;

            case State.Attack:  //攻撃中
                break;
            
        }
    }

}
