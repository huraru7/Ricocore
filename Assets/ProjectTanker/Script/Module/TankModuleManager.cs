using System.Collections.Generic;
using UnityEngine;

public class TankModuleManager : MonoBehaviour
{
    //モジュール機能の管理　tank自体につける
    //do:モジュールの獲得・消去　モジュール一覧 インベントリ モジュールのセット(インベントリからの移動)

    [Tooltip("モジュール一覧")] public List<ModuleData> moduleLists { get; private set; }

    public List<ModuleData> mosuleInventory;

    /// <summary>
    /// 新規モジュールを獲得しインベントリへ追加
    /// </summary>
    public void moduleEarn()
    {

    }
}
