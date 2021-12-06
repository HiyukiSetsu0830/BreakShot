using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace CharacterState {

    /// �X�e�[�g�̎��s���Ǘ�����N���X
    public class StateProcessor {

        //�X�e�[�g�{��
        public ReactiveProperty<CharacterState> State { get; set; } = new ReactiveProperty<CharacterState>();
        //���s�u���b�W
        public void Execute() => State.Value.Execute();
    }

    //�X�e�[�g�̃N���X
    public abstract class CharacterState {

        //�f���Q�[�g
        public Action ExecAction { get; set; }

        //���s����
        public virtual void Execute() {
            if (ExecAction != null) ExecAction();
        }

        //�X�e�[�g�����擾���郁�\�b�h
        public abstract string GetStateName();
    }

    //=================================================================================
    //�ȉ���ԃN���X
    //=================================================================================

    /// <summary>
    /// �������Ă��Ȃ����
    /// </summary>
    public class CharacterStateIdle : CharacterState {

        public override string GetStateName() {

            return "State:Idle";
        }

    }

    /// <summary>
    /// �����Ă�����
    /// </summary>
    public class CharacterStateRun : CharacterState{

        public override string GetStateName() {
            return "State:Run";
        }

    }

    /// <summary>
    /// �U�����Ă�����
    /// </summary>
    public class CharacterStateAttack : CharacterState {

        public override string GetStateName() {

            return "State:Attack";
        }

        public override void Execute() {
            if (ExecAction != null) ExecAction();
        }

    }

}

