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
        public void InitializeExecute() => State.Value.InitializeExecute();
        //���sExecute
        public void UpdateExecute() => State.Value.UpdateExecute();
        //�I��Execute
        public void EndExecute() => State.Value.EndExecute();
    }

    //�X�e�[�g�̃N���X
    public abstract class CharacterState {

        //�f���Q�[�g
        //��������Action
        public Action InitializeExecAction { get; set; } 
        //���sAction
        public Action UpdateExecAction { get; set; }
        //�I��Action
        public Action EndExecAction { get; set; }

        //���s����
        public virtual void InitializeExecute() {
            if (InitializeExecAction != null) InitializeExecAction();
        }
        //���sExecute
        public virtual void UpdateExecute() {
            if (UpdateExecAction != null) UpdateExecAction();
        }
        //�I��Execute
        public virtual void EndExecute() {
            if (EndExecAction != null) EndExecAction();
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
    /// �W�����v���Ă�����
    /// </summary>
    public class CharacterStateJump : CharacterState {

        public override string GetStateName() {

            return "State:Jump";
        }

    }

    /// <summary>
    /// �󒆂ɂ�����
    /// </summary>
    public class CharacterStateFall : CharacterState {

        public override string GetStateName() {

            return "State:Fall";
        }

    }
}

