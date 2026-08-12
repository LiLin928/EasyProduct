# 菜单管理页面重构方案

## 背景

参考 `D:\4-MyProject\EasyProject\PCWeb\src\views\basic\menu` 的设计，重构当前项目的菜单管理页面，使其更加友好和易用。

## 主要改进点

### 1. 树形表格替代树形组件

**当前实现：**
- 使用 `el-tree` 组件
- 操作按钮在节点内部

**改进方案：**
- 使用 `el-table` + `tree-props` 实现树形表格
- 操作按钮在独立的列中
- 更直观的数据展示

### 2. 搜索功能增强

**当前实现：**
- 无搜索功能

**改进方案：**
- 添加搜索表单（菜单名称、状态筛选）
- 实时过滤树形数据
- 支持递归过滤（父级匹配时保留所有子级）

### 3. 状态和可见性快捷切换

**当前实现：**
- 需要编辑弹窗才能修改状态和可见性

**改进方案：**
- 使用 Switch 开关直接在表格中切换
- 调用 API 立即保存修改
- 失败时自动恢复原值

### 4. 图标选择优化

**当前实现：**
- 手动输入图标名称

**改进方案：**
- 添加图标选择器弹窗
- 可视化选择 Element Plus 图标
- 预览效果

## 技术实现细节

### 树形表格配置

```vue
<el-table
  v-loading="loading"
  :data="filteredTableData"
  row-key="id"
  :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
  border
  default-expand-all
>
  <!-- 列定义 -->
</el-table>
```

### 搜索过滤逻辑

```typescript
// 过滤菜单树（递归）
const filterMenuTree = (menus: Menu[], params: { name?: string; status?: string }): Menu[] => {
  return menus.filter(menu => {
    const nameMatch = !params.name || menu.name.includes(params.name)
    const statusMatch = !params.status || menu.status === params.status

    // 递归过滤子菜单
    if (menu.children && menu.children.length > 0) {
      const filteredChildren = filterMenuTree(menu.children, params)
      return (nameMatch && statusMatch) || filteredChildren.length > 0
    }

    return nameMatch && statusMatch
  }).map(menu => ({
    ...menu,
    children: menu.children ? filterMenuTree(menu.children, params) : undefined,
  }))
}
```

### 状态切换实现

```typescript
const handleStatusChange = async (row: Menu) => {
  try {
    await updateMenu(row.id, { status: row.status })
    ElMessage.success('状态更新成功')
  } catch (error) {
    // 恢复原值
    row.status = row.status === 'enabled' ? 'disabled' : 'enabled'
  }
}
```

## 文件结构

```
src/views/basic/menu/
├── index.vue                    # 主页面（树形表格 + 搜索）
├── components/
│   ├── MenuFormDialog.vue        # 新增/编辑弹窗
│   └── IconSelectDialog.vue      # 图标选择器（可选）
```

## 待办事项

### 批次 1：基础重构（优先级 P0）
1. ✅ 将 `el-tree` 改为 `el-table` + `tree-props`
2. ✅ 添加搜索表单（菜单名称、状态）
3. ✅ 实现递归过滤逻辑
4. ✅ 添加状态和可见性的 Switch 开关

### 批次 2：增强功能（优先级 P1）
1. ⏳ 创建 `MenuFormDialog.vue` 弹窗组件
2. ⏳ 添加图标选择器（可视化选择）
3. ⏳ 添加父级菜单的 Cascader 选择器
4. ⏳ 优化表单验证规则

### 批次 3：完善细节（优先级 P2）
1. ⏳ 添加拖拽排序功能
2. ⏳ 添加批量操作（启用/禁用）
3. ⏳ 添加导出功能

## 对比表

| 功能 | 当前实现 | 改进后 | 优先级 |
|------|---------|--------|--------|
| 数据展示 | el-tree | el-table（树形表格） | P0 |
| 搜索功能 | 无 | SearchForm + 实时过滤 | P0 |
| 状态切换 | 编辑弹窗 | Switch 直接切换 | P0 |
| 可见性切换 | 编辑弹窗 | Switch 直接切换 | P0 |
| 图标选择 | 手动输入 | 可视化选择器 | P1 |
| 父级选择 | TreeSelect | Cascader 级联 | P1 |

## 预期效果

1. **更直观的数据展示**：表格形式更符合管理系统的使用习惯
2. **更快捷的操作**：状态和可见性无需打开弹窗即可修改
3. **更强大的搜索**：支持按名称和状态筛选，递归过滤
4. **更友好的表单**：图标可视化选择，父级级联选择

## 参考资源

- 参考项目：`D:\4-MyProject\EasyProject\PCWeb\src\views\basic\menu`
- Element Plus 文档：https://element-plus.org/zh-CN/component/table.html#树形数据与懒加载