using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LitMotion を使った共通 UI アニメーションユーティリティ。
///
/// すべてのメソッドは MotionHandle を返すため、呼び出し元でキャンセル可能。
/// timeScale=0 中に再生が必要なアニメ（レベルアップパネル等）は
/// ignoreTimeScale = true または PanelOpen を使用すること。
/// </summary>
public static class UIAnimations
{
    /// <summary>
    /// ポップアニメ（1.2x → 1.0x スケール）。
    /// アイコンやテキストが更新された時の強調表現に使う。
    /// </summary>
    public static MotionHandle Pop(Transform t, float duration = 0.2f)
    {
        t.localScale = Vector3.one * 1.2f;
        return LMotion.Create(Vector3.one * 1.2f, Vector3.one, duration)
            .WithEase(Ease.OutBack)
            .BindToLocalScale(t);
    }

    /// <summary>
    /// パネル出現アニメ（0 → 1 スケール）。
    /// Time.timeScale = 0 のままでも動作する。
    /// </summary>
    public static MotionHandle PanelOpen(Transform t, float duration = 0.3f)
    {
        t.localScale = Vector3.zero;
        return LMotion.Create(Vector3.zero, Vector3.one, duration)
            .WithEase(Ease.OutBack)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .BindToLocalScale(t);
    }

    /// <summary>
    /// 数値カウントアニメーション。
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

    /// <summary>
    /// ダメージフラッシュ（Image を赤く光らせて元の色に戻す）。
    /// </summary>
    public static MotionHandle DamageFlash(Image img, float duration = 0.2f)
    {
        var original = img.color;
        img.color = Color.red;
        return LMotion.Create(Color.red, original, duration)
            .WithEase(Ease.OutQuad)
            .Bind(x => img.color = x);
    }

    /// <summary>
    /// フェードイン（CanvasGroup.alpha 0 → 1）。
    /// </summary>
    public static MotionHandle FadeIn(CanvasGroup cg, float duration = 0.25f)
    {
        cg.alpha = 0f;
        return LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(cg);
    }
}
