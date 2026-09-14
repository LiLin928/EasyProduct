# CRM 主数据管理模块 - 数据库迁移说明

## 📋 文件清单

1. **init-database.sql** - 数据库初始化脚本（包含 Site 模块表）
2. **crm-master-data.sql** - CRM 主数据管理模块建表脚本（本次新增）

## 🚀 执行步骤

### 方式一：命令行执行（推荐）

```bash
# 1. 连接 MySQL
mysql -u root -p

# 2. 执行 CRM 主数据管理模块建表脚本
source D:/4-MyProject/EasyProduct/EasyProduct.WebApi/sql/crm-master-data.sql

# 3. 验证表是否创建成功
USE easyproduct;
SHOW TABLES LIKE 'crm_%';
```

### 方式二：MySQL Workbench 执行

1. 打开 MySQL Workbench
2. 连接到 MySQL 服务器
3. 打开 `crm-master-data.sql` 文件
4. 点击"执行"（闪电图标）

### 方式三：Navicat 执行

1. 打开 Navicat
2. 连接到 MySQL 服务器
3. 打开数据库 `easyproduct`
4. 右键 → 运行 SQL 文件
5. 选择 `crm-master-data.sql` 文件
6. 点击"开始"

## ✅ 验证清单

执行以下 SQL 验证表是否创建成功：

```sql
-- 查看所有 CRM 表
SHOW TABLES LIKE 'crm_%';

-- 查看客户表结构
DESC crm_customer;

-- 查看供应商表结构
DESC crm_supplier;

-- 查看币种表结构
DESC crm_currency;

-- 查看税率表结构
DESC crm_tax_rate;

-- 查看币种初始数据
SELECT * FROM crm_currency;

-- 查看税率初始数据
SELECT * FROM crm_tax_rate;
```

## 📊 表结构概览

### 客户管理（3 张表）

| 表名 | 说明 | 关键字段 |
|------|------|----------|
| crm_customer | 客户主表 | code, name, type, source |
| crm_customer_contact | 客户联系人表 | customer_id, name, is_primary |
| crm_customer_address | 客户地址表 | customer_id, receiver_name, is_default |

### 供应商管理（2 张表）

| 表名 | 说明 | 关键字段 |
|------|------|----------|
| crm_supplier | 供应商主表 | code, name, status |
| crm_supplier_qualification | 供应商资质表 | supplier_id, type, expire_date |

### 币种管理（1 张表）

| 表名 | 说明 | 关键字段 |
|------|------|----------|
| crm_currency | 币种表 | code, name, exchange_rate, is_default |

### 税率管理（1 张表）

| 表名 | 说明 | 关键字段 |
|------|------|----------|
| crm_tax_rate | 税率表 | name, rate, status |

## 🔗 外键关系

```
crm_customer
    └── crm_customer_contact (customer_id)
    └── crm_customer_address (customer_id)

crm_supplier
    └── crm_supplier_qualification (supplier_id)

site_inquiry
    └── crm_customer (customer_id) -- 询价转客户
```

## 📦 初始数据

### 币种数据（5 条）

- CNY - 人民币（默认币种）
- USD - 美元
- EUR - 欧元
- GBP - 英镑
- JPY - 日元

### 税率数据（5 条）

- 13% 税率（一般货物）
- 9% 税率（农产品、图书等）
- 6% 税率（现代服务、生活服务）
- 免税（0%）
- 3% 征收率（小规模纳税人）

## ⚠️ 注意事项

1. **执行顺序**：先执行 `init-database.sql`，再执行 `crm-master-data.sql`
2. **数据覆盖**：脚本中使用 `DROP TABLE IF EXISTS`，会删除已存在的表
3. **外键约束**：确保 `crm_customer` 表先创建，再创建关联表
4. **字符集**：所有表使用 `utf8mb4` 字符集，支持 emoji 等特殊字符
5. **初始数据**：币种和税率表会自动插入初始数据

## 🔧 常见问题

### Q1: 如何重置数据？

```sql
-- 删除所有 CRM 表
DROP TABLE IF EXISTS crm_customer_contact;
DROP TABLE IF EXISTS crm_customer_address;
DROP TABLE IF EXISTS crm_customer;
DROP TABLE IF EXISTS crm_supplier_qualification;
DROP TABLE IF EXISTS crm_supplier;
DROP TABLE IF EXISTS crm_currency;
DROP TABLE IF EXISTS crm_tax_rate;

-- 重新执行建表脚本
source D:/4-MyProject/EasyProduct/EasyProduct.WebApi/sql/crm-master-data.sql
```

### Q2: 如何查看外键约束？

```sql
-- 查看所有外键约束
SELECT
    TABLE_NAME,
    CONSTRAINT_NAME,
    REFERENCED_TABLE_NAME
FROM
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
    TABLE_SCHEMA = 'easyproduct'
    AND REFERENCED_TABLE_NAME IS NOT NULL;
```

### Q3: 如何修改汇率？

```sql
-- 更新美元汇率
UPDATE crm_currency
SET exchange_rate = 7.25
WHERE code = 'USD';
```

## 📝 表设计规范

- ✅ 主键：GUID（VARCHAR(36)）
- ✅ 编码：业务编码唯一（C + yyyyMM + 序号）
- ✅ 状态：INT 类型（0=禁用，1=启用）
- ✅ 软删除：is_deleted 字段（0=未删除，1=已删除）
- ✅ 时间戳：created_at、updated_at
- ✅ 创建人：created_by、updated_by（GUID）
- ✅ 字符集：utf8mb4
- ✅ 排序规则：utf8mb4_unicode_ci
- ✅ 外键约束：级联删除（ON DELETE CASCADE）
- ✅ 索引：主键、唯一键、常用查询字段、外键字段

---

# Report 模块数据库脚本

## 📋 脚本列表

| 脚本文件 | 说明 | 执行顺序 |
|---------|------|---------|
| `rpt-datasource.sql` | 数据源表 | 1 |
| `rpt-definition.sql` | 报表定义表 | 2 |
| `rpt-column-template.sql` | 列模板表 | 3 |

## 🚀 执行方式

### 方式一：命令行执行

```bash
mysql -u root -p easyproduct < rpt-datasource.sql
mysql -u root -p easyproduct < rpt-definition.sql
mysql -u root -p easyproduct < rpt-column-template.sql
```

### 方式二：在 MySQL 客户端中执行

```sql
source D:/4-MyProject/EasyProduct/EasyProduct.WebApi/sql/rpt-datasource.sql;
source D:/4-MyProduct/EasyProduct/EasyProduct.WebApi/sql/rpt-definition.sql;
source D:/4-MyProduct/EasyProduct/EasyProduct.WebApi/sql/rpt-column-template.sql;
```

## 📊 表结构概览

### rpt_datasource（数据源表）

存储报表数据源配置，支持多种数据库类型（MySQL、PostgreSQL、SQL Server、Oracle）。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | char(36) | 主键ID |
| name | varchar(100) | 数据源名称（唯一） |
| type | varchar(20) | 数据源类型 |
| status | int | 连接状态（1-已连接，2-连接错误） |
| password | varchar(500) | 密码（加密存储） |

### rpt_definition（报表定义表）

存储报表定义信息，包括 SQL 模板、列配置、图表类型等。

| 字段 | 类型 | 说明 |
|------|------|------|
| id | char(36) | 主键ID |
| code | varchar(50) | 报表编码（唯一） |
| sql_template | text | 查询 SQL |
| chart_type | varchar(20) | 图表类型 |
| status | int | 状态（1-草稿，2-已发布，3-已归档） |

### rpt_column_template（列模板表）

存储报表列模板，用于快速配置列显示格式。

| 字段 | 类型 | 说明 |
|------|------|------|
| field | varchar(50) | 字段名 |
| type | varchar(20) | 字段类型 |
| width | int | 列宽 |
| format | varchar(50) | 格式化规则 |

## ⚠️ 注意事项

1. **执行顺序**：必须先执行 `rpt-datasource.sql`，再执行 `rpt-definition.sql`（有外键约束）
2. **密码加密**：数据源密码在应用层加密存储，不要在数据库脚本中写入明文密码
3. **SQL 注入防护**：报表 SQL 只允许执行 SELECT 语句，并有黑名单检查
4. **默认数据源**：脚本会创建一个名为"主数据库"的默认数据源，需要根据实际环境修改配置

---

数据库表创建完成后，可以：

1. ✅ 启动后端 API 服务
2. ✅ 使用 Swagger 测试 API
3. ✅ 开发前端管理页面
4. ✅ 添加测试数据

---

**创建时间：** 2026-09-14
**版本：** 1.0
**维护者：** 开发团队