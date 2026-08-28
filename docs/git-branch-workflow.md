# Git 分支开发工作流

> 本地创建功能分支 → 开发提交 → 合并回 main → 推送远程的完整操作指南。

## 完整流程图

```
origin/main (远程)
    │
    │  ① git checkout -b feature/xxx
    ▼
本地 main ──●────────────────────────────────●  ← ⑥ git checkout main
                \                            │
                 \  ② 开发 + git add + git commit
                  \                           │
                   ●──●──●──●──● (feature/xxx)  ← ⑤ 删除分支(可选)
                                    │
                                    │  ③ git checkout main
                                    │  ④ git merge feature/xxx
                                    ▼
                              本地 main 合并完成
                                    │
                                    │  ⑦ git push origin main
                                    ▼
                              origin/main 更新
```

## 步骤详解

### 前提条件

```bash
# 确认当前在 main 分支且与远程同步
git checkout main
git pull origin main
```

### 第一步：创建本地功能分支

```bash
# 从 main 创建并切换到新分支
git checkout -b feature/your-feature-name
```

**命名规范**（参考项目 Conventional Commits scope）：

| 前缀 | 用途 | 示例 |
|------|------|------|
| `feature/` | 新功能开发 | `feature/announcement-management` |
| `bugfix/` | Bug 修复 | `bugfix/order-status-fix` |
| `hotfix/` | 紧急修复 | `hotfix/login-crash` |

### 第二步：开发并提交

开发过程中，反复执行以下循环：

```bash
# 查看改动的文件
git status

# 暂存指定文件（推荐）
git add src/views/basic/announcement/index.vue
git add src/types/announcement.ts

# 或暂存所有改动
git add .

# 提交，遵循 Conventional Commits
git commit -m "feat(admin): 添加公告管理列表页"
```

**Commit Message 规范**：

```
<type>(<scope>): <subject>
```

| type | 说明 | scope 可选值 |
|------|------|-------------|
| `feat` | 新功能 | admin / site / miniapp / api / mock |
| `fix` | 修复 bug | admin / site / miniapp / api / mock |
| `refactor` | 重构 | admin / site / miniapp / api / mock |
| `docs` | 文档更新 | docs |
| `style` | 代码格式 | 对应端名 |
| `test` | 测试 | 对应端名 |
| `chore` | 构建/工具链 | 对应端名 |

**示例**：

```bash
git commit -m "feat(admin): 添加公告管理 API 与类型定义"
git commit -m "fix(mock): 修正公告 Mock 数据字段以对齐前端类型定义"
git commit -m "docs: 添加公告管理功能设计文档"
```

### 第三步：切换回 main 分支

```bash
git checkout main
```

### 第四步：合并功能分支到 main

```bash
git merge feature/your-feature-name
```

如果远程 main 有新的提交（团队协作场景），先拉取再合并：

```bash
git pull origin main
git merge feature/your-feature-name
```

**冲突处理**（如果出现合并冲突）：

```bash
# 1. 查看冲突文件
git status

# 2. 手动编辑冲突文件，解决 <<<<<<< ======= >>>>>>> 标记

# 3. 标记已解决
git add <冲突文件>

# 4. 完成合并提交
git commit -m "merge: 合并 feature/your-feature-name 到 main"
```

### 第五步：推送到远程 main

```bash
git push origin main
```

### 第六步：清理本地分支（可选）

合并完成后，功能分支已无用处，可安全删除：

```bash
git branch -d feature/your-feature-name
```

---

## 实际操作示例

以本次「公告管理」开发为例：

```bash
# ── 创建分支 ──
git checkout main
git pull origin main
git checkout -b feature/announcement-management

# ── 开发提交（多次循环）──
git add docs/superpowers/specs/2026-08-19-announcement-design.md
git commit -m "docs: 添加公告管理功能设计文档"

git add EasyProduct.Admin/src/types/announcement.ts
git commit -m "feat(admin): 添加公告管理类型定义"

git add EasyProduct.Admin/src/api/basic/announcement.ts
git commit -m "feat(admin): 添加公告管理 API"

git add EasyProduct.Admin/src/i18n/zh-CN/basic.json EasyProduct.Admin/src/i18n/en-US/basic.json
git commit -m "feat(admin): 添加公告管理国际化文本"

git add EasyProduct.Admin/src/views/basic/announcement/index.vue
git commit -m "feat(admin): 创建公告列表页"

git add mock-server/src/routes/admin/announcement.ts
git commit -m "feat(mock): 添加公告管理端 Mock API 路由"

# ── 还有未提交的改动，继续提交 ──
git add .
git commit -m "feat(admin): 完善公告管理页面（编辑弹窗+路由+i18n）"

# ── 合并到 main ──
git checkout main
git merge feature/announcement-management

# ── 推送到远程 ──
git push origin main

# ── 清理分支 ──
git branch -d feature/announcement-management
```

---

## 常见问题

### Q: `fatal: couldn't find remote ref feature/xxx`

**原因**：本地分支未 push 到远程，却尝试 `git pull origin feature/xxx`。

**解决**：功能分支只需在本地开发，合并到 main 后推送 main 即可。不需要单独 push 功能分支。

### Q: `git push` 报 `Everything up-to-date`

**说明**：远程 main 已经包含了你本地的所有提交，无需再次推送。可用 `git status` 确认。

### Q: 合并时出现冲突

**解决**：

```bash
# 放弃合并，回到合并前状态
git merge --abort

# 重新拉取远程最新代码后再合并
git pull origin main
git merge feature/your-feature-name
```

### Q: 暂存了文件但 lint-staged 报错导致提交失败

**说明**：项目配置了 lint-staged，会在 commit 时自动检查代码。修复 lint 错误后重新 `git add` + `git commit` 即可。

### Q: 想撤销最近一次提交（保留改动）

```bash
git reset --soft HEAD~1
```

---

## 快速参考命令

```bash
# 创建并切换分支
git checkout -b feature/xxx

# 提交改动
git add . && git commit -m "feat(admin): 描述"

# 合并到 main
git checkout main && git merge feature/xxx

# 推送远程
git push origin main

# 删除已合并的本地分支
git branch -d feature/xxx

# 查看所有本地+远程分支
git branch -a

# 查看分支提交历史
git log --oneline main..feature/xxx
```
