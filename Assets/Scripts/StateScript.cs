using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateScript : MonoBehaviour
{

    //�X�e�[�g
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

            case State.Jump:  //�W�����v��
                break;

            case State.Move:    //�ړ���
                break;

            case State.Attack:  //�U����
                break;
            
        }
    }

}
