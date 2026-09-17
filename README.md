# falling-blocks

レトロ調の落ちものパズル。GDD（自然言語の仕様書）を親にして、自律開発ハーネス
**[tamaroulet/game-harness](https://github.com/tamaroulet/game-harness)** で作る。

## 構成

| 場所 | 中身 |
|:--|:--|
| `Game/` | Unity 6000.3.23f1 のプロジェクト（URP 2D） |
| `Game/Assets/Core/` | Pure C# のゲームロジック（`Game.Core.asmdef` は `noEngineReferences`） |
| `tests/Core.Tests/` | Unity を使わない dotnet のテスト（CI の `test`） |
| `docs/gdd/` | 人間が dispatch から送る GDD |
| `docs/spec/` | 分解役が GDD から作る構造化仕様 |
| `.harness.toml` | harness の契約（`docs/design/contract.md`）。main の先頭からだけ読まれる |

## main の保護

main にはルールセット `ms4-protect-default-branch` が掛かっている。**所有者を含め、誰も迂回できない。**

- 直接 push・force push・削除はできない。変更はすべて PR から入る
- マージには必須チェックが 2 つとも success であることが要る
  - `test` — `.github/workflows/core-tests.yml`（Pure C# の dotnet test）
  - `approval` — `.github/workflows/approval.yml`。承認者（`.github/ms4-approvers`）が、
    **今の head のコミットに対して** `ms4:approved` を付けたか。push されると承認は外れる
- 承認は人間が dispatch から行う（`python tools/dispatch.py --approve falling-blocks#<PR>`）
- approval の一式は game-harness の `harness/templates/game-repo/` から写したもの。ここで直接編集しない
