# Site 端（官网）模块 API 文档

> 来源: `D:\4-MyProject\EasyProduct\mock-server\src\routes\site`
> 基础路径: `/api/site`

---

## 目录

1. [实体定义](#实体定义)
2. [API 接口](#api-接口)
   - [关于我们](#关于我们)
   - [公告管理](#公告管理)
   - [分类管理](#分类管理)
   - [联系我们](#联系我们)
   - [下载中心](#下载中心)
   - [首页管理](#首页管理)
   - [询价管理](#询价管理)
   - [新闻资讯](#新闻资讯)
   - [产品展示](#产品展示)
   - [视频中心](#视频中心)

---

## 实体定义

### SiteAbout (关于单页)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 单页ID | GUID |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| content | string | 内容(中文) | HTML 格式 |
| contentEn | string | 内容(英文) | HTML 格式 |
| updatedAt | string | 更新时间 | ISO 8601 |

### ContactInfo (联系信息)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| address | string | 地址(中文) | - |
| addressEn | string | 地址(英文) | - |
| phone | string | 电话 | - |
| email | string | 邮箱 | - |
| workingHours | string | 工作时间(中文) | - |
| workingHoursEn | string | 工作时间(英文) | - |

### Announcement (公告)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 公告ID | GUID |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| summary | string | 摘要(中文) | - |
| summaryEn | string | 摘要(英文) | - |
| content | string | 内容(中文) | HTML 格式 |
| contentEn | string | 内容(英文) | HTML 格式 |
| level | enum | 级别 | `'normal' | 'important' | 'urgent'` |
| isTop | boolean | 是否置顶 | - |
| publishTime | string | 发布时间 | ISO 8601 |
| attachments | Attachment[] | 附件列表 | 见下表 |

**Attachment 附件结构:**

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| name | string | 文件名 | - |
| url | string | 文件URL | - |
| size | number | 文件大小 | 字节 |

### ProductCategory (产品分类)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 分类ID | GUID |
| name | string | 名称(中文) | - |
| nameEn | string | 名称(英文) | - |
| parentId | string | 父级ID | 一级分类为`'0'` |
| sort | number | 排序号 | - |
| status | enum | 状态 | `'enabled' | 'disabled'` |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### SiteNews (新闻)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 新闻ID | GUID |
| categoryId | string | 分类ID | - |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| summary | string | 摘要(中文) | - |
| summaryEn | string | 摘要(英文) | - |
| content | string | 内容(中文) | HTML 格式 |
| contentEn | string | 内容(英文) | HTML 格式 |
| coverImage | string | 封面图URL | - |
| isTop | boolean | 是否置顶 | - |
| viewCount | number | 浏览次数 | - |
| status | enum | 状态 | `'draft' | 'published'` |
| publishTime | string | 发布时间 | ISO 8601 |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### SiteBanner (首页轮播图)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 轮播图ID | GUID |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| imageUrl | string | 图片URL | - |
| link | string | 跳转链接 | - |
| sort | number | 排序号 | - |
| status | enum | 状态 | `'enabled' | 'disabled'` |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### VideoCategory (视频分类)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 分类ID | - |
| name | string | 名称(中文) | - |
| nameEn | string | 名称(英文) | - |
| sort | number | 排序号 | - |

### SiteVideo (视频)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 视频ID | GUID |
| categoryId | string | 分类ID | - |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| coverImage | string | 封面图URL | - |
| videoUrl | string | 视频URL | - |
| duration | number | 时长(秒) | - |
| viewCount | number | 播放次数 | - |
| status | enum | 状态 | `'draft' | 'published'` |
| sort | number | 排序号 | - |
| publishTime | string | 发布时间 | ISO 8601 |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### DownloadCategory (下载分类)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 分类ID | - |
| name | string | 名称(中文) | - |
| nameEn | string | 名称(英文) | - |
| sort | number | 排序号 | - |

### SiteDownload (下载资源)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 下载ID | GUID |
| categoryId | string | 分类ID | - |
| title | string | 标题(中文) | - |
| titleEn | string | 标题(英文) | - |
| fileUrl | string | 文件URL | - |
| fileSize | number | 文件大小 | 字节 |
| downloadCount | number | 下载次数 | - |
| status | enum | 状态 | `'draft' | 'published'` |
| sort | number | 排序号 | - |
| publishTime | string | 发布时间 | ISO 8601 |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

### Inquiry (询价单)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 询价ID | GUID |
| companyName | string | 公司名称 | - |
| contactName | string | 联系人姓名 | - |
| phone | string | 联系电话 | - |
| email | string | 联系邮箱 | - |
| items | InquiryItem[] | 询价产品列表 | 见下表 |
| status | enum | 状态 | `'pending' | 'processing' | 'completed' | 'converted'` |
| remark | string | 备注 | - |
| createdAt | string | 创建时间 | ISO 8601 |
| updatedAt | string | 更新时间 | ISO 8601 |

**InquiryItem 询价产品项:**

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| productId | string | 产品ID | - |
| productName | string | 产品名称 | - |
| quantity | number | 数量 | - |
| unit | string | 单位 | - |
| remark | string | 备注 | - |

### Product (产品)

| 字段名 | 类型 | 说明 | 备注 |
|--------|------|------|------|
| id | string | 产品ID | GUID |
| categoryId | string | 分类ID | - |
| code | string | 产品编码 | - |
| name | string | 名称(中文) | - |
| nameEn | string | 名称(英文) | - |
| summary | string | 摘要(中文) | - |
| summaryEn | string | 摘要(英文) | - |
| coverImage | string | 封面图URL | - |
| images | string[] | 图片列表 | - |
| price | number | 价格 | - |
| unit | string | 单位 | - |
| specs | string | 规格参数 | JSON字符串 |
| status | string | 状态 | - |
| createdAt | string | 创建时间 | ISO 8601 |

---

## API 接口

### 关于我们

#### 获取关于单页信息

- **URL**: `/api/site/about/detail`
- **Method**: `GET`
- **描述**: 获取关于我们单页内容
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": SiteAbout,
  "timestamp": 1234567890
}
```

#### 获取联系信息

- **URL**: `/api/site/contact/info`
- **Method**: `GET`
- **描述**: 获取公司联系信息
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": ContactInfo,
  "timestamp": 1234567890
}
```

---

### 公告管理

#### 获取公告列表

- **URL**: `/api/site/announcement/list`
- **Method**: `GET`
- **描述**: 获取已发布公告列表（官网公开）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `keyword` (query): 关键词搜索（可选，匹配标题）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [Announcement],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

#### 获取公告详情

- **URL**: `/api/site/announcement/:id`
- **Method**: `GET`
- **描述**: 获取公告详情（仅返回已发布公告）
- **请求参数**:
  - `id` (path): 公告ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Announcement,
  "timestamp": 1234567890
}
```

---

### 分类管理

#### 获取分类列表

- **URL**: `/api/site/category/list`
- **Method**: `GET`
- **描述**: 获取产品分类树（一级分类）
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [ProductCategory],
  "timestamp": 1234567890
}
```

---

### 联系我们

#### 提交联系表单

- **URL**: `/api/site/contact`
- **Method**: `POST`
- **描述**: 提交联系/留言信息（带IP限流：60秒内最多3次）
- **请求参数**:
```json
{
  "name": "string",      // 姓名
  "phone": "string",     // 电话
  "email": "string",     // 邮箱
  "content": "string",   // 内容
  "company": "string"    // 公司（可选）
}
```
- **响应结构**:
```json
// 成功
{
  "code": 200,
  "message": "提交成功",
  "data": null,
  "timestamp": 1234567890
}

// 限流
{
  "code": 429,
  "message": "提交过于频繁，请稍后再试",
  "data": null,
  "timestamp": 1234567890
}
```

---

### 下载中心

#### 获取下载分类列表

- **URL**: `/api/site/download-category/list`
- **Method**: `GET`
- **描述**: 获取下载资源分类列表
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [DownloadCategory],
  "timestamp": 1234567890
}
```

#### 获取下载列表

- **URL**: `/api/site/download/list`
- **Method**: `GET`
- **描述**: 获取下载资源列表（支持分类筛选）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `categoryId` (query): 分类ID（可选）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [SiteDownload],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

---

### 首页管理

#### 获取轮播图列表

- **URL**: `/api/site/banner/list`
- **Method**: `GET`
- **描述**: 获取首页轮播图列表
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [SiteBanner],
  "timestamp": 1234567890
}
```

#### 获取新闻列表(首页)

- **URL**: `/api/site/news/list`
- **Method**: `GET`
- **描述**: 获取新闻列表（首页展示用，返回简化字段）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `keyword` (query): 关键词搜索（可选，匹配标题）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [{
      "id": "string",
      "categoryId": "string",
      "title": "string",
      "titleEn": "string",
      "summary": "string",
      "coverImage": "string",
      "isTop": true,
      "viewCount": 100,
      "publishTime": "string"
    }],
    "total": 50
  },
  "timestamp": 1234567890
}
```

---

### 询价管理

#### 提交询价

- **URL**: `/api/site/inquiry`
- **Method**: `POST`
- **描述**: 提交产品询价
- **请求参数**:
```json
{
  "companyName": "string",    // 公司名称
  "contactName": "string",    // 联系人姓名
  "phone": "string",          // 联系电话
  "email": "string",          // 联系邮箱
  "items": [                  // 询价产品列表
    {
      "productId": "string",
      "productName": "string",
      "quantity": 1,
      "unit": "string",
      "remark": "string"
    }
  ]
}
```
- **响应结构**:
```json
{
  "code": 200,
  "message": "询价提交成功",
  "data": Inquiry,
  "timestamp": 1234567890
}
```

#### 查询询价单

- **URL**: `/api/site/inquiry/:id`
- **Method**: `GET`
- **描述**: 根据ID查询询价单详情
- **请求参数**:
  - `id` (path): 询价单ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Inquiry,
  "timestamp": 1234567890
}
```

---

### 新闻资讯

#### 获取新闻列表

- **URL**: `/api/site/news/list`
- **Method**: `GET`
- **描述**: 获取新闻列表（完整字段）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `keyword` (query): 关键词搜索（可选，匹配标题）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [SiteNews],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

#### 获取新闻详情

- **URL**: `/api/site/news/:id`
- **Method**: `GET`
- **描述**: 获取新闻详情
- **请求参数**:
  - `id` (path): 新闻ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": SiteNews,
  "timestamp": 1234567890
}
```

---

### 产品展示

#### 获取产品列表

- **URL**: `/api/site/product/list`
- **Method**: `GET`
- **描述**: 获取产品列表（支持分类筛选、关键词搜索）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认10
  - `categoryId` (query): 分类ID（可选）
  - `keyword` (query): 关键词搜索（可选，匹配名称/编码）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [Product],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

#### 获取产品详情

- **URL**: `/api/site/product/:id`
- **Method**: `GET`
- **描述**: 获取产品详情
- **请求参数**:
  - `id` (path): 产品ID
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": Product,
  "timestamp": 1234567890
}
```

---

### 视频中心

#### 获取视频分类列表

- **URL**: `/api/site/video-category/list`
- **Method**: `GET`
- **描述**: 获取视频分类列表
- **请求参数**: 无
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": [VideoCategory],
  "timestamp": 1234567890
}
```

#### 获取视频列表

- **URL**: `/api/site/video/list`
- **Method**: `GET`
- **描述**: 获取视频列表（支持分类筛选）
- **请求参数**:
  - `pageIndex` (query): 页码，默认1
  - `pageSize` (query): 每页条数，默认9
  - `categoryId` (query): 分类ID（可选）
- **响应结构**:
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [SiteVideo],
    "total": 50,
    "pageIndex": 1,
    "pageSize": 9,
    "totalPages": 6,
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

---

## 通用响应格式

所有接口统一返回格式:

```json
{
  "code": 200,           // 状态码: 200成功; 400参数错误; 401未授权; 403无权限; 404不存在; 429限流; 500服务错误
  "message": "操作成功", // 提示信息
  "data": {},            // 业务数据
  "timestamp": 1234567890 // 时间戳
}
```

## 分页数据结构

分页列表响应:

```json
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "list": [],        // 数据列表
    "total": 50,       // 总条数
    "pageIndex": 1,    // 当前页码
    "pageSize": 10,    // 每页条数
    "totalPages": 5,   // 总页数
    "hasNextPage": true,
    "hasPrevPage": false
  },
  "timestamp": 1234567890
}
```

---

## 限流说明

**联系表单提交** (`POST /api/site/contact`) 有IP限流保护:
- 同一IP 60秒内最多提交3次
- 超过限制返回 `429` 状态码

---

*文档生成时间: 2026-09-07*
*来源: mock-server/src/routes/site*
