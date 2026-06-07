# Unity エディタ作業ガイド — インベントリ廃止・装備スロット一本化

> コード側の変更（STEP 1）は完了済み。このファイルの手順に沿って Unity エディタ側を整理してください。

---

## カラーリファレンス（コピー用）

```
背景         #F0EDE8
パネル       #E9E5DF
枠線         #CFC9C2
テキスト主   #3D3833
テキスト副   #7A766F

Earth        #7CB87E
Water        #7BAFD4
Fire         #E07A5F
Wind         #F2CC6A
None         #B0ABA3
```

---

# STEP 1 — インベントリ関連オブジェクトの削除

## 1-1. コンパイルエラーの確認

Unity がスクリプト変更を検知して自動リコンパイルします。

1. `Window > General > Console`（`Ctrl+Shift+C`）を開く
2. **赤いエラーが 0 件**になるまで待つ

> エラーが残っている場合は次の作業に進まないでください。

---

## 1-2. シーンから ModuleInventory を削除

`1_MainGame.unity` を開いた状態で行います。

1. Hierarchy ウィンドウで **`Canvas`** の左の三角を展開する
2. `Canvas` 以下に **`ModuleInventory`**（または `InventoryUI` という名前）のオブジェクトを探す
3. 見つけたらクリックして選択 → **`Delete` キー** で削除
4. Hierarchy から消えたことを確認

> 名前が違う場合は Inspector で `InventoryUI` スクリプトがついているオブジェクトを探してください。

---

## 1-3. TankPresenter の Inspector を確認・整理

1. Hierarchy で **`Canvas`** または **`TankPresenter`** がアタッチされているオブジェクトを選択
2. Inspector の `TankPresenter` コンポーネントを確認する
3. 以下のフィールドが**残っていないこと**を確認する
   - `Inventory UI` フィールド（スクリプト変更により自動で消えているはず）
4. もし `None (Inventory UI)` のような欄が残っていたら、スクリプトのリコンパイルが完了していない可能性があります。Console のエラーを確認してください。

**現在残っているべきフィールド（View セクション）:**

| フィールド名 | アサインされているもの |
|---|---|
| `Get Module Select UI` | モジュール3択パネルの GameObject |
| `Slot UI` | スロットUIの GameObject |

---

## 1-4. ModuleInventoryPanel Prefab の確認

`InventoryItemUI.cs` は削除しましたが、Prefab ファイル自体はまだ残っています。

1. Project ウィンドウで `Assets/ProjectTanker/Prefab/ModuleInventoryPanel.prefab` を探す
2. Prefab を選択して Inspector を開く
3. **Missing Script** の警告（黄色または赤）が出ていたら以下を行う：
   - Prefab をダブルクリックして開く
   - Missing Script がついているコンポーネントの右の `⋮` メニュー → `Remove Component` で削除
   - `Ctrl+S` で Prefab を保存して閉じる
4. この Prefab はもう使わないため、削除してもかまいません
   - Project ウィンドウで右クリック → `Delete`

---

## 1-5. SlotItemUI の Inspector 確認（7つ分）

ドラッグ&ドロップ関連の Script を撤去したため、SlotItem の Inspector が変わっています。

1. Hierarchy で `Canvas > Slot` を展開する
2. `SlotItem` × 7 を順番に選択し、Inspector の `SlotItemUI` コンポーネントを確認する
3. 以下のフィールドが残っていること（アサインが外れていないか）を確認する

| フィールド | 内容 |
|---|---|
| `Icon Image` | アイコン表示用 Image |
| `Name Text` | モジュール名 TextMeshProUGUI |
| `Accent Line` | 属性カラーライン Image |
| `Slot Number` | スロット番号 TextMeshProUGUI |
| `Theme` | ThemeColor アセット |

> ドラッグ&ドロップ関連のフィールド（`On Drop` など）は消えているはずです。正常です。

---

## 1-6. EventSystem の確認

Hierarchy に `EventSystem` が残っていることを確認します。  
削除してしまうとボタンクリックが一切効かなくなります。

1. Hierarchy で `EventSystem` を探す
2. **存在していれば OK**（何もしない）
3. なければ `GameObject > UI > Event System` で追加する

---

## 1-7. シーンを保存

`Ctrl + S` でシーンを保存します。

---

## 1-8. 動作確認（プレイモード）

`▶` でプレイモードを開始し、以下をすべて確認してください。

| 確認項目 | 期待する結果 |
|---|---|
| Console にエラーが出ない | ✅ |
| ゲーム開始時に3択カードが表示される | ✅ |
| カードを選択する | 最初の空きスロットに自動装備される ✅ |
| 7スロット全部が埋まった状態でカードを選択 | 何も起きない（エラーなし）✅ |
| E キーを押してもインベントリが開かない | ✅（削除済みのため） |

---

# STEP 2 — ModuleReplaceUI の作成（スロット満杯時の入れ替え/破棄）

STEP 1 の動作確認が完了してから進めてください。

---

## 2-1. ModuleReplacePanel オブジェクトの作成

1. Hierarchy で **`Canvas`** を右クリック → `Create Empty`
2. 名前を **`ModuleReplacePanel`** に変更
3. `ModuleReplaceUI` スクリプトをアタッチ（Inspector の `Add Component` から検索）

---

## 2-2. パネルの構成を組む

`ModuleReplacePanel` 以下に以下の構造を作ります。

```
ModuleReplacePanel
└── Panel                      Image（背景、Color: #E9E5DF）
    ├── NewModuleArea          空 GameObject（取得モジュール表示エリア）
    │   ├── NewModuleIcon      Image（取得したモジュールのアイコン）
    │   └── NewModuleName      TextMeshProUGUI（モジュール名）
    ├── Label                  TextMeshProUGUI（「どうしますか？」など）
    ├── SlotButtons            空 GameObject（スロット選択ボタンの親）
    │   └── SlotOptionButton × 7   ModuleOptionButton コンポーネントつき
    └── DiscardButton          Button（「破棄する」テキスト付き）
```

### Panel の設定

- Anchor: **Stretch / Stretch**（画面全体を薄く覆う半透明背景）または **Middle/Center** で中央固定パネル
- Color: `#E9E5DF`、Alpha: `220`（約86%）
- Width/Height を中央固定にする場合: 例 Width `500`、Height `400`

### NewModuleIcon

- サイズ: 64×64
- Sprite: 実行時にスクリプトがセットするので空白でOK

### NewModuleName

- フォントサイズ: 16
- Color: `#3D3833`
- Alignment: Center

### SlotButtons の設定

- `Vertical Layout Group` コンポーネントを追加
  - Spacing: `8`
  - Child Force Expand Width: ON、Height: OFF

### SlotOptionButton × 7 の作成

既存の `ModuleOptionButton` Prefab を複製するか、新たに以下の構成で7つ作ります:

```
SlotOptionButton  （Button コンポーネント + ModuleOptionButton スクリプト）
├── AccentBar     Image（属性カラーバー）
├── IconImage     Image（スロットのモジュールアイコン）
└── NameText      TextMeshProUGUI（スロットのモジュール名）
```

各 `SlotOptionButton` の `ModuleOptionButton` コンポーネントに以下をアサイン:
- `Button` → 自身の Button コンポーネント
- `Icon Image` → `IconImage`
- `Name Text` → `NameText`
- `Accent Bar` → `AccentBar`
- `Theme` → `ThemeColor.asset`

### DiscardButton の設定

- Button コンポーネントつきの GameObject
- 子に TextMeshProUGUI で「破棄する」のテキスト
- Color: `#DDD8D1`（通常）、Highlighted: `#B5AFA7`

---

## 2-3. パネルを初期非表示にする

`ModuleReplacePanel` 以下の **`Panel`** オブジェクトを選択し、Inspector 上部のチェックを **OFF（非表示）** にする。

> `ModuleReplaceUI.cs` の `panel` フィールドにはこの `Panel` を指定します（ルートの `ModuleReplacePanel` ではなく）。

---

## 2-4. ModuleReplaceUI の Inspector をアサイン

`ModuleReplacePanel` の `ModuleReplaceUI` コンポーネントに以下をアサイン:

| フィールド | アサインするもの |
|---|---|
| `Panel` | `Panel` オブジェクト（非表示にしたもの） |
| `New Module Icon` | `NewModuleIcon` の Image コンポーネント |
| `New Module Name` | `NewModuleName` の TextMeshProUGUI |
| `Slot Option Buttons` (配列 × 7) | `SlotOptionButton` × 7 の ModuleOptionButton コンポーネント |
| `Discard Button` | `DiscardButton` の Button コンポーネント |

---

## 2-5. TankPresenter の Inspector に追加

1. `TankPresenter` がアタッチされているオブジェクトを選択
2. Inspector の `TankPresenter` コンポーネントを確認
3. **`Module Replace UI`** フィールドに `ModuleReplacePanel` の **`ModuleReplaceUI` コンポーネント** をドラッグ

---

## 2-6. シーンを保存

`Ctrl + S` でシーンを保存します。

---

## 2-7. 動作確認（プレイモード）

| 確認項目 | 期待する結果 |
|---|---|
| スロット7つが埋まった状態でカードを選択 | `ModuleReplaceUI` が表示される ✅ |
| 「破棄する」ボタンを押す | パネルが閉じ、スロットに変化なし ✅ |
| スロットボタンのいずれかを押す | そのスロットが新モジュールに入れ替わる ✅ |
| 入れ替え後パネルが閉じる | 通常プレイに戻る ✅ |
| Console にエラーが出ない | ✅ |

---

## トラブルシューティング

### Missing Script の警告が出る
→ 削除したスクリプト（InventoryUI / InventoryItemUI）がまだどこかの GameObject にアタッチされています。  
Hierarchy を検索（`Ctrl+F`）して該当オブジェクトを見つけ、`Remove Component` で削除してください。

### スロットに自動装備されない
→ `TankPresenter` の `Slot UI` フィールドに SlotUI がアサインされているか確認してください。

### ModuleReplaceUI が表示されない（スロット満杯時）
→ `TankPresenter` の `Module Replace UI` フィールドに ModuleReplaceUI がアサインされているか確認してください。  
→ `Panel` の初期状態が非表示（チェック OFF）になっているか確認してください。

### ボタンを押しても反応しない
→ Hierarchy に `EventSystem` が存在するか確認してください。
