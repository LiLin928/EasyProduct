# AGENTS.md - EasyProduct 项目开发指引

> EasyProduct = EasyWebSite（官网）+ EasyProject（电商管理）+ EasyCRM（CRM/ERP 前端）三项目整合后的模块化单体项目。

## 项目路径规范

**重要：所有命令和操作必须基于项目根目录**

- **项目根目录**：`D:\4-MyProject\EasyProduct`
- **前端 Admin**：`D:\4-MyProject\EasyProduct\EasyProduct.Admin`
- **前端 Site**：`D:\4-MyProject\EasyProduct\EasyProduct.Site`
- **前端 MiniApp**：`D:\4-MyProject\EasyProduct\EasyProduct.MiniApp`
- **后端 API**：`D:\4-MyProject\EasyProduct\EasyProduct.WebApi`
- **Mock 服务器**：`D:\4-MyProject\EasyProduct\mock-server`
- **文档目录**：`D:\4-MyProject\EasyProduct\docs`

**启动命令示例：**
```bash
# 启动前端 Admin（在项目根目录执行）
cd D:\4-MyProject\EasyProduct\EasyProduct.Admin && pnpm dev

# 启动 Mock 服务器（在项目根目录执行）
cd D:\4-MyProject\EasyProduct\mock-server && pnpm dev

# 启动后端 API（在项目根目录执行）
cd D:\4-MyProject\EasyProduct\EasyProduct.WebApi && dotnet run
```

## 项目概览

- **后端**：EasyProduct.WebApi（.NET 8 模块化单体，SqlSugar + Autofac + MySQL 8）
- **前端**：EasyProduct.Admin（Vue3 + Element Plus 管理后台）、EasyProduct.Site（官网门户）、EasyProduct.MiniApp（微信小程序）
- **Mock**：mock-server（Node + Express，端口 7700），随后端交付逐模块退役
- **数据库**：MySQL 8 单库 `easyproduct`，表前缀 basic_/site_/product_/mall_/crm_/wf_/rpt_/ops_

## 核心事实速查

- 后端：单一 .NET 8 模块化单体（EasyProduct.WebApi），SqlSugar + Autofac（构造器注入）+ Serilog + Quartz；API 三分区 `/api/admin/**`、`/api/site/**`、`/api/app/**`
- Mock：独立 `mock-server/`（Node + Express + mockjs，端口 7700），三端共享，随后端交付逐模块退役
- 前端：EasyProduct.Admin（Vue3+EP 管理后台）、EasyProduct.Site（官网门户）、EasyProduct.MiniApp（微信原生小程序）
- 数据库：MySQL 8 单库 `easyproduct`，表前缀 basic_/site_/product_/mall_/crm_/wf_/rpt_/ops_，snake_case
- 统一响应：`{code, message, data, timestamp}`，HTTP 一律 200，code===200 成功；GUID 主键；JSON camelCase；分页 pageIndex/pageSize + list/total；状态字段用小写字符串常量
- 部署：单租户，Docker Compose 单机（nginx + api + mysql + redis 可选）

## 必读文档（按任务类型）

| 任务 | 先读 |
|------|------|
| 理解整体架构、模块划分、数据模型、阶段规划 | `docs/superpowers/specs/2026-08-08-easyproduct-integration-design.md` |
| **任何前端开发**（Admin/Site/MiniApp） | `docs/frontend-guidelines.md`（强制，含冲突裁决；与源项目旧规范冲突时以它为准） |
| **任何后端开发**（EasyProduct.WebApi） | `docs/backend-guidelines.md`（强制，含三源差异裁决附录；与 EasyWechatWeb 旧写法冲突时以它为准） |
| **Mock 数据开发 / 前后端联调切换** | `docs/mock-guidelines.md`（契约先行；mock 与真后端切换流程见其第 8、9 节） |

## 开发纪律

- 修改前先读对应规范文档；规范冲突以 docs/ 下文档为准
- 前端提交前：vue-tsc 零错误、ESLint 通过、check:i18n 无硬编码中文
- 后端提交前：dotnet build 0 错误 0 新警告；涉钱逻辑（库存流水/冲销/收付款核销）先写 xUnit
- mock 新增/修改必须与后端规范第 5 节路由和信封逐字一致，禁止自创接口
- Commit 遵循 Conventional Commits，scope 用端名（admin/site/miniapp/api/mock）

## 开发规范

### 前端规范
- 提交前：`vue-tsc` 零错误、ESLint 通过、`check:i18n` 无硬编码中文
- API 层：统一响应 `{code, message, data, timestamp}`，HTTP 一律 200，code===200 成功
- 数据契约：GUID 主键、JSON camelCase、分页 pageIndex/pageSize + list/total
- 状态字段：使用小写字符串常量（如 `"pending"`, `"paid"`）

### 后端规范
- 提交前：`dotnet build` 0 错误 0 新警告
- 涉钱逻辑（库存流水/冲销/收付款核销）先写 xUnit 测试
- **所有方法必须添加中文解释备注**（见下方代码规范）
- 分层：Controller（收参/调用/返回）→ Service（业务逻辑）→ Common/Helper（工具）
- 依赖注入：一律构造器注入，禁止属性注入
- 异常处理：Controller 不捕获，统一由全局异常中间件处理

### Mock 规范
- 新增/修改必须与后端规范第 5 节路由和信封逐字一致
- 禁止自创接口

## 代码规范

### C# 代码规范（后端）

#### 方法注释规范（强制）

**所有后端方法必须添加中文解释备注**，包括：
- Controller 的所有公开方法
- Service 的所有公开方法
- Repository/DataAccess 的所有公开方法
- Helper/Utility 的所有公开方法

**注释格式：**

```csharp
/// <summary>
/// [功能简述 - 一句话说明方法用途]
/// </summary>
/// <param name="参数名">参数说明</param>
/// <returns>返回值说明</returns>
/// <remarks>
/// [可选：详细说明、业务逻辑、注意事项]
/// </remarks>
/// <example>
/// [可选：使用示例]
/// </example>
public async Task<ApiResponse<List<ProductDto>>> GetProductList(ProductQueryDto query)
{
    // 实现代码...
}
```

**示例：**

```csharp
/// <summary>
/// 获取商品列表（支持分页、筛选）
/// </summary>
/// <param name="query">查询参数，包含分页、分类、关键词等</param>
/// <returns>商品列表分页结果</returns>
/// <remarks>
/// 1. 支持按分类、品牌、价格区间筛选
/// 2. 支持关键词模糊搜索（商品名称、编码）
/// 3. 默认按创建时间倒序排列
/// </remarks>
public async Task<PageResult<ProductDto>> GetProductList(ProductQueryDto query)
{
    // 实现代码...
}
```

#### 其他 C# 规范

- 命名：PascalCase（类、方法、属性）、camelCase（私有字段、局部变量）、_前缀（私有字段）
- 文件命名：与主类名一致，一个文件一个类
- 大括号：Allman 风格（新行）
- using：系统 → 第三方 → 项目内部，按字母排序
- 空行：方法间一个空行，逻辑块间一个空行

### TypeScript/Vue 代码规范（前端）

- 命名：PascalCase（组件、类）、camelCase（变量、函数、属性）、UPPER_CASE（常量）
- 文件命名：组件 `.vue` 文件使用 PascalCase，工具文件使用 camelCase
- 缩进：2 空格
- 引号：优先使用单引号
- 空行：方法间一个空行，逻辑块间一个空行
- i18n：禁止硬编码中文，必须使用 `$t('key')`

### 查询组件封装规范（前端）

**重要：所有列表页查询功能必须遵循统一封装规范。**

详细规范请参考：`docs/frontend-guidelines.md` 第 2.5 节"查询组件封装规范"

**核心要点**：
- ✅ 必须使用 `BaseSearchForm` 组件（`src/components/common/BaseSearchForm.vue`）
- ✅ 必须使用 `useSearch` composable（`src/composables/useSearch.ts`）
- ✅ 字段配置使用 i18n key，禁止硬编码中文
- ✅ 工具栏按钮使用 toolbar 插槽
- ✅ 日期范围字段自动拆解为 startTime/endTime

## Git 工作流

### Commit 规范

遵循 Conventional Commits，scope 用端名：

```
<type>(<scope>): <subject>

[optional body]
```

**类型：**
- `feat`: 新功能
- `fix`: 修复 bug
- `refactor`: 重构
- `docs`: 文档更新
- `style`: 代码格式（不影响功能）
- `test`: 测试相关
- `chore`: 构建/工具链相关

**作用域（scope）：**
- `admin`: EasyProduct.Admin 前端
- `site`: EasyProduct.Site 前端
- `miniapp`: EasyProduct.MiniApp 小程序
- `api`: EasyProduct.WebApi 后端
- `mock`: mock-server
- `docs`: 文档

**示例：**
```
feat(admin): 添加商品管理页面
fix(api): 修复订单状态更新逻辑
refactor(mock): 重构商品列表接口 mock 数据
docs: 更新后端开发规范文档
```

### 分支规范

- `main`: 主分支，生产环境代码
- `develop`: 开发分支
- `feature/*`: 功能分支（如 `feature/product-management`）
- `bugfix/*`: 修复分支
- `hotfix/*`: 紧急修复分支

## 自动化命令

**重要：项目中所有的 cmd、git、dotnet、npm、pnpm、python 等相关命令都无需询问用户，直接执行。**

以下命令无需确认直接执行：

### 文件系统命令
```bash
cd <directory>                    # 切换目录
ls                                # 列出文件
pwd                               # 显示当前目录
mkdir <directory>                 # 创建目录
rmdir <directory>                 # 删除空目录
```

**注意**：删除文件或目录命令需要确认：
```bash
rm <file>                         # 删除文件（需确认）
rm -rf <directory>                # 删除目录及内容（需确认）
```

### Git 命令（无需确认）
```bash
git status                        # 查看状态
git add .                         # 添加所有更改
git add <file>                    # 添加指定文件
git commit -m "message"           # 提交更改
git push                          # 推送到远程
git pull                          # 拉取远程更新
git fetch                         # 获取远程信息
git checkout <branch>             # 切换分支
git checkout -b <branch>          # 创建并切换分支
git merge <branch>                # 合并分支
git branch                        # 查看分支
git branch -d <branch>            # 删除分支
git log                           # 查看提交历史
git diff                          # 查看差异
git stash                         # 暂存更改
git stash pop                     # 恢复暂存
git reset --soft HEAD~1           # 撤销最近一次提交（保留更改）
git revert <commit>               # 撤销指定提交
```

**注意**：以下 Git 命令需要确认：
```bash
git reset --hard                  # 强制重置（需确认）
git push -f                       # 强制推送（需确认）
git clean -fd                     # 删除未跟踪文件（需确认）
```

### 后端命令（无需确认）
```bash
dotnet restore                    # 恢复依赖
dotnet build                      # 构建项目
dotnet run                        # 运行项目
dotnet test                       # 运行测试
dotnet publish                    # 发布项目
dotnet clean                      # 清理构建输出
dotnet ef migrations add <name>   # 添加迁移
dotnet ef database update         # 更新数据库
dotnet ef migrations remove       # 删除最近迁移
dotnet ef database drop           # 删除数据库（需确认）
dotnet new <template>             # 创建新项目/项
dotnet add package <package>      # 添加 NuGet 包
dotnet remove package <package>   # 移除 NuGet 包
dotnet list package               # 列出包
```

### 前端命令（无需确认）
```bash
pnpm install                      # 安装依赖
pnpm add <package>                # 添加依赖
pnpm remove <package>             # 移除依赖
pnpm dev                          # 启动开发服务器
pnpm build                        # 构建生产版本
pnpm preview                      # 预览生产构建
pnpm lint                         # 代码检查
pnpm lint:fix                     # 自动修复代码问题
pnpm type-check                   # 类型检查
pnpm test                         # 运行测试
pnpm test:coverage                # 测试覆盖率
pnpm run <script>                 # 运行脚本
pnpm update                       # 更新依赖
pnpm outdated                     # 检查过期依赖

npm install                       # 安装依赖（pnpm 不可用时）
npm run dev                       # 启动开发服务器
npm run build                     # 构建生产版本
npm run lint                      # 代码检查
npm test                          # 运行测试
```

### Python 命令（无需确认）
```bash
python --version                  # 查看 Python 版本
python <script.py>                # 运行 Python 脚本
python -m venv <name>             # 创建虚拟环境
pip install <package>             # 安装包
pip install -r requirements.txt   # 安装依赖列表
pip freeze                        # 列出已安装包
pip list                          # 列出包
pip uninstall <package>           # 卸载包（需确认）
pip cache purge                   # 清理缓存
```

### Windows CMD 命令（无需确认）
```bash
dir                               # 列出目录内容
type <file>                       # 显示文件内容
copy <source> <dest>              # 复制文件
move <source> <dest>             # 移动文件
ren <old> <new>                   # 重命名文件
```

**注意**：删除命令需要确认：
```bash
del <file>                        # 删除文件（需确认）
rd /s /q <directory>              # 删除目录（需确认）
```

### 组合命令（无需确认）
```bash
# 前端开发流程
cd EasyProduct.Admin && pnpm install && pnpm dev

# 后端开发流程
cd EasyProduct.WebApi && dotnet restore && dotnet build && dotnet run

# Mock 服务器启动
cd mock-server && pnpm install && pnpm dev

# Git 提交流程
git add . && git commit -m "feat: 新功能" && git push

# 前端检查流程
pnpm lint:fix && pnpm type-check && pnpm build

# 后端检查流程
dotnet build && dotnet test

# 数据库迁移流程
dotnet ef migrations add InitialCreate && dotnet ef database update
```

### 命令执行规则总结

**无需确认的命令类型**：
1. ✅ 文件系统：cd、ls、pwd、mkdir（创建）
2. ✅ Git：所有非破坏性操作（status、add、commit、push、pull、checkout、merge、branch、log、diff、stash、reset --soft）
3. ✅ 后端：所有 dotnet 命令（除 ef database drop）
4. ✅ 前端：所有 pnpm/npm 命令
5. ✅ Python：所有 python/pip 命令（除 pip uninstall）
6. ✅ Windows CMD：所有非删除命令

**需要确认的命令类型**：
1. ❌ 删除文件/目录：rm、del、rd、git clean
2. ❌ 危险 Git 操作：git reset --hard、git push -f
3. ❌ 删除依赖：pip uninstall、dotnet ef database drop
4. ❌ 任何显式要求确认的命令

## 统一响应格式

```typescript
interface ApiResponse<T> {
  code: number       // 200 成功；400 参数错误；401 未授权；403 无权限；404 不存在；500 服务错误
  message: string    // 提示信息
  data: T            // 业务数据
  timestamp: number  // 时间戳
}

interface PageResult<T> {
  list: T[]          // 数据列表
  total: number      // 总条数
}
```

## API 路由规范

- `/api/admin/**` - 管理端（需要 Admin JWT）
- `/api/site/**` - 官网公开（匿名 + 限流）
- `/api/app/**` - 小程序会员（需要 Member JWT）

## 技术栈

### 后端
- .NET 8.0 (LTS)
- SqlSugarCore 5.1.4.x
- Autofac 8.x/9.x
- Serilog 8.x
- Mapster 10.x
- Quartz 3.14.x
- xUnit（单元测试）

### 前端
- Vue 3.x
- TypeScript 5.x
- Element Plus
- Pinia
- Vue Router
- Axios
- vue-i18n

### Mock
- Node.js
- Express
- mockjs

## 开发环境设置

### 快速启动

**推荐方式：同时启动 Mock + 前端（并行运行）**

在项目根目录 `D:\4-MyProject\EasyProduct` 打开两个终端：

**终端 1 - Mock 服务器：**
```bash
cd mock-server
pnpm dev
```
- 访问地址：http://localhost:7700
- 提供 API 模拟数据

**终端 2 - 前端 Admin：**
```bash
cd EasyProduct.Admin
pnpm dev
```
- 访问地址：http://localhost:5173（端口冲突时自动切换）
- 默认会连接到 Mock 服务器

### 单独启动

1. **后端**：
   ```bash
   cd EasyProduct.WebApi
   dotnet restore
   dotnet build
   dotnet run
   ```

2. **前端（Admin）**：
   ```bash
   cd EasyProduct.Admin
   pnpm install
   pnpm dev
   ```

3. **Mock 服务器**：
   ```bash
   cd mock-server
   pnpm install
   pnpm dev
   ```

## 常见任务

### 添加新的 API 接口
1. 阅读 `docs/backend-guidelines.md`
2. 在对应模块创建 DTO（Request/Response）
3. 在 Service 层实现业务逻辑（添加中文注释）
4. 在 Controller 层实现接口（添加中文注释）
5. 编写单元测试（涉钱逻辑必须）
6. 更新 Mock 数据（如果需要）

### 添加新的前端页面
1. 阅读 `docs/frontend-guidelines.md`
2. 创建 Vue 组件
3. 实现 API 调用（使用统一的 request 工具）
4. 添加 i18n 翻译（禁止硬编码中文）
5. 提交前检查：vue-tsc、ESLint、check:i18n

### 添加 Mock 数据
1. 阅读 `docs/mock-guidelines.md`
2. 在 `mock-server/routes/` 下添加路由
3. 确保响应格式与后端规范一致
4. 测试接口返回

## 注意事项

- **规范冲突**：以 `docs/` 下文档为准
- **源项目旧代码**：与规范冲突时，以本规范为准
- **权限控制**：管理端 Admin JWT，小程序 Member JWT，官网匿名
- **分页**：pageIndex 从 1 开始，pageSize 默认 10
- **主键**：一律 GUID，类型为 `string`
- **多语言**：三端均支持 zh-CN/en-US

## 快速参考

### 数据库表前缀
- `basic_`: 基础管理（用户、角色、字典等）
- `site_`: 官网相关
- `product_`: 商品管理
- `mall_`: 商城订单、购物车等
- `crm_`: 客户关系管理
- `wf_`: 工作流
- `rpt_`: 报表
- `ops_`: 运维管理

### 常用端口
- 前端开发：http://localhost:5173
- Mock 服务器：http://localhost:7700
- 后端 API：http://localhost:5000
- MySQL：3306
- Redis（可选）：6379
