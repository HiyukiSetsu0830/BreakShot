using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace CharacterState {

    /// ステートの実行を管理するクラス
    public class StateProcessor {

        //ステート本体
        public ReactiveProperty<CharacterState> State { get; set; } = new ReactiveProperty<CharacterState>();
        //実行ブリッジ
        public void InitializeExecute() => State.Value.InitializeExecute();
        //実行Execute
        public void UpdateExecute() => State.Value.UpdateExecute();
        //終了Execute
        public void EndExecute() => State.Value.EndExecute();
    }

    //ステートのクラス
    public abstract class CharacterState {

        //デリゲート
        //初期化のAction
        public Action InitializeExecAction { get; set; } 
        //実行Action
        public Action UpdateExecAction { get; set; }
        //終了Action
        public Action EndExecAction { get; set; }

        //実行処理
        public virtual void InitializeExecute() {
            if (InitializeExecAction != null) InitializeExecAction();
        }
        //実行Execute
        public virtual void UpdateExecute() {
            if (UpdateExecAction != null) UpdateExecAction();
        }
        //終了Execute
        public virtual void EndExecute() {
            if (EndExecAction != null) EndExecAction();
        }

        //ステート名を取得するメソッド
        public abstract string GetStateName();
    }

    //=================================================================================
    //以下状態クラス
    //=================================================================================

    /// <summary>
    /// 何もしていない状態
    /// </summary>
    public class CharacterStateIdle : CharacterState {

        public override string GetStateName() {

            return "State:Idle";
        }

    }

    /// <summary>
    /// 走っている状態
    /// </summary>
    public class CharacterStateRun : CharacterState{

        public override string GetStateName() {

            return "State:Run";
        }

    }

    /// <summary>
    /// ジャンプしている状態
    /// </summary>
    public class CharacterStateJump : CharacterState {

        public override string GetStateName() {

            return "State:Jump";
        }

    }

    /// <summary>
    /// 空中にいる状態
    /// </summary>
    public class CharacterStateFall : CharacterState {

        public override string GetStateName() {

            return "State:Fall";
        }

    }
}

