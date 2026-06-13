# Unity エディタ作業ガイド — project Tanker

# ゲームループ セットアップ

## STEP A-1 — GameManager の配置

1. Hierarchy を右クリック → `Create Empty` → 名前を **`GameManager`** に変更
2. `Add Component → GameManager`
3. Inspector で各フィールドをアサイン

| フィールド      | アサイン先                                 |
| --------------- | ------------------------------------------ |
| Player Status   | PlayerTank の `TankStatus` コンポーネント  |
| Enemy Container | Hierarchy の `EnemyContainer` オブジェクト |
| Result UI       | 次の手順で作る `ResultPanel`               |

---

## STEP A-2 — Result UI の作成

既存の Canvas 直下に以下の GameObject 階層を作成する。

```
Canvas
└── ResultPanel  ← Panel コンポーネント、GameResultUI.cs をアタッチ
    ├── TitleText  ← TextMeshPro "STAGE CLEAR"
    ├── RetryButton  ← Button → On Click: GameResultUI.OnRetry
    └── TitleButton  ← Button → On Click: GameResultUI.OnTitle
```

1. `Canvas` を右クリック → `UI → Panel` → 名前を **`ResultPanel`** に変更
2. `ResultPanel` に `Add Component → GameResultUI`
3. Inspector でフィールドをアサイン

| フィールド | アサイン先                 |
| ---------- | -------------------------- |
| Panel      | `ResultPanel` 自身         |
| Title Text | `TitleText` の TextMeshPro |

4. `ResultPanel` の `TitleText` を追加: `ResultPanel` 右クリック → `UI → Text - TextMeshPro`
5. リトライ/タイトルボタンを追加: `ResultPanel` 右クリック → `UI → Button - TextMeshPro`
   - 各ボタンの On Click に `GameResultUI.OnRetry` / `OnTitle` を設定
6. **`ResultPanel` を非活性にしておく**（Inspector の チェックボックスを OFF）
7. `GameManager` の ResultUI フィールドに `ResultPanel` をアサイン

---

## STEP A-3 — Build Settings に 2_title を追加

`File → Build Settings` を開き、`Assets/ProjectTanker/Scene/2_title.unity` が
リストに含まれていなければ **Add Open Scenes** またはドラッグ&ドロップで追加する。

---

# ビジュアル強化 セットアップ

## STEP B-1 — キャタピラースクロール

### tank_tracks.png の Import 設定変更

1. Project ウィンドウで `Assets/ProjectTanker/Art/Image/tank_tracks.png` を選択
2. Inspector → **Wrap Mode** を `Repeat` に変更
3. `Apply` を押す

### プレイヤータンクへの設定

1. `PlayerTank` の子にある tracks SpriteRenderer を持つオブジェクトを選択
2. `Add Component → TrackScroller`
3. Inspector でフィールドをアサイン

| フィールド     | アサイン先                           |
| -------------- | ------------------------------------ |
| Track Renderer | tracks オブジェクトの SpriteRenderer |
| Rb             | PlayerTank ルートの Rigidbody2D      |

### エネミータンクへの設定

1. `EnemyTank` の子の tracks SpriteRenderer を持つオブジェクトを選択
2. 上記と同様に `TrackScroller` をアタッチし、エネミータンクの Rigidbody2D をアサイン

---

## STEP B-2 — エネミー HP バー

`EnemyTank` の子に以下を作成する（複数エネミーがいる場合は 1 体ずつ設定）。

1. `EnemyTank` を右クリック → `Create Empty` → 名前 **`HPBar`**
   - Transform: Position `(0, 0.8, 0)`（頭上）
2. `HPBar` の子に `Create Empty` → 名前 **`HPBarBG`**
   - `Add Component → SpriteRenderer`
   - Sprite: Unity 組み込みの白い正方形（`Sprites/Square` を検索）
   - Color: 白 `(0.3, 0.3, 0.3)`（濃いグレー=背景）
   - Scale: `(0.6, 0.08, 1)`
   - Order in Layer: `5`
3. `HPBar` の子に `Create Empty` → 名前 **`HPBarFill`**
   - `Add Component → SpriteRenderer`
   - Sprite: 白い正方形
   - Color: 赤 `(1, 0.2, 0.2)`
   - Scale: `(0.6, 0.08, 1)`（HPBarBG と同じ）
   - Order in Layer: `6`
4. `HPBar` に `Add Component → EnemyHPBar`
5. Inspector でフィールドをアサイン

| フィールド | アサイン先                    |
| ---------- | ----------------------------- |
| Fill       | `HPBarFill` の SpriteRenderer |
| Status     | `EnemyTank` の `TankStatus`   |

---

## 動作確認（ゲームループ + ビジュアル強化）

| 確認項目                    | 期待する結果                             |
| --------------------------- | ---------------------------------------- |
| 敵タンクの HP を 0 にする   | 爆発エフェクト再生 → EnemyTank が非表示  |
| 全敵が倒される              | "STAGE CLEAR" パネルが表示され時間停止   |
| プレイヤーの HP が 0 になる | "GAME OVER" パネルが表示され時間停止     |
| リトライボタンを押す        | シーンが再読込みされ再プレイできる       |
| タンクが前進する            | キャタピラーのテクスチャが縦方向に流れる |
| 敵にダメージを与える        | 頭上の赤バーが縮む                       |
