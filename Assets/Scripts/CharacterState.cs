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
        public void Execute() => State.Value.Execute();
    }

    //ステートのクラス
    public abstract class CharacterState {

        //デリゲート
        public Action ExecAction { get; set; }

        //実行処理
        public virtual void Execute() {
            if (ExecAction != null) ExecAction();
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
}

