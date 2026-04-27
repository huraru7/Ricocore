using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TankModuleManager : MonoBehaviour
{
    //モジュール機能の管理　tank自体につける
    //do:モジュールの獲得・消去　モジュール一覧 インベントリ モジュールのセット(インベントリからの移動)

    [Tooltip("モジュール一覧")][SerializeField] private List<ModuleData> moduleLists;

    [Tooltip("インベントリ")] public List<ModuleData> mosuleInventory { get; private set; }

    [Tooltip("装備欄")] public ModuleData[] equipmentModule { get; private set; } = new ModuleData[5];


    /// <summary>
    /// 新規モジュールを獲得しインベントリへ追加
    /// </summary>
    public void moduleEarn()
    {

    }
}
