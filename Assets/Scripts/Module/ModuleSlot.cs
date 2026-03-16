/// <summary>
/// モジュールを1つ保持するスロット。
/// インベントリスロットと部位スロット（砲塔・エンジン等）の両方に使用する。
/// モジュールが変化した際は OnChanged イベントが発火するため、
/// UI はこのイベントを購読することで自動的に更新できる。
/// </summary>
public class ModuleSlot
{
    /// <summary>このスロットが保持しているモジュール（空なら null）</summary>
    public Module Module { get; private set; }

    public bool IsEmpty   => Module == null;
    public bool HasModule => Module != null;

    /// <summary>モジュールが変化した際に発火するイベント</summary>
    public event System.Action<ModuleSlot> OnChanged;

    // -------------------------------------------------------

    /// <summary>モジュールをこのスロットに配置する（上書き不可。事前に Take すること）</summary>
    public void Place(Module module)
    {
        Module = module;
        OnChanged?.Invoke(this);
    }

    /// <summary>スロットからモジュールを取り出して返す。空なら null を返す。</summary>
    public Module Take()
    {
        var m = Module;
        Module = null;
        OnChanged?.Invoke(this);
        return m;
    }
}
