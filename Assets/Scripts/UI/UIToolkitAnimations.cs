using LitMotion;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI Toolkit (VisualElement) 向けの LitMotion アニメーションユーティリティ。
/// 旧 UIAnimations.cs（uGUI 版）の UI Toolkit 対応版。
///
/// VisualElement には Transform.localScale が存在しないため、
/// element.transform.scale / element.style.opacity を直接操作する。
///
/// 旧 UIAnimations.cs との対応:
///   Pop(Transform)       → Pop(VisualElement)
///   PanelOpen(Transform) → PanelOpen(VisualElement)
///   FadeIn(CanvasGroup)  → FadeIn(VisualElement)
///   NumberCount(...)     → NumberCount(...)  ← 変更なし（UI非依存）
///   DamageFlash(Image)   → uGUI のまま運用（HUD は uGUI 継続）
/// </summary>
public static class UIToolkitAnimations
{
    /// <summary>
    /// ポップアニメ（1.2x → 1.0x スケール）。
    /// アイコンやテキストが更新された時の強調表現に使う。
    /// </summary>
    public static MotionHandle Pop(VisualElement element, float duration = 0.2f)
    {
        element.transform.scale = new Vector3(1.2f, 1.2f, 1f);
        return LMotion.Create(1.2f, 1.0f, duration)
            .WithEase(Ease.OutBack)
            .Bind(v => element.transform.scale = new Vector3(v, v, 1f));
    }

    /// <summary>
    /// パネル出現アニメ（0 → 1 スケール）。
    /// Time.timeScale = 0 のままでも動作する。
    /// </summary>
    public static MotionHandle PanelOpen(VisualElement element, float duration = 0.3f)
    {
        element.transform.scale = Vector3.zero;
        return LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.OutBack)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .Bind(v => element.transform.scale = new Vector3(v, v, 1f));
    }

    /// <summary>
    /// フェードイン（opacity 0 → 1）。
    /// </summary>
    public static MotionHandle FadeIn(VisualElement element, float duration = 0.25f)
    {
        element.style.opacity = 0f;
        return LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.OutQuad)
            .Bind(v => element.style.opacity = v);
    }

    /// <summary>
    /// 数値カウントアニメーション。
    /// uGUI 版と同一。UI 非依存なため変更なし。
    /// onUpdate に float を渡すので、呼び出し元で Mathf.RoundToInt などを使うこと。
    /// </summary>
    public static MotionHandle NumberCount(
        float from, float to, float duration,
        System.Action<float> onUpdate,
        bool ignoreTimeScale = false)
    {
        var builder = LMotion.Create(from, to, duration).WithEase(Ease.OutQuad);
        if (ignoreTimeScale)
            builder = builder.WithScheduler(MotionScheduler.UpdateIgnoreTimeScale);
        return builder.Bind(onUpdate);
    }
}
