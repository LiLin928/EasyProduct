# CSS 样式指南

## 概述

本文档定义 EasyProduct Admin 管理系统的 CSS 样式规范，包括设计令牌、变量定义和最佳实践。

## 1. 设计令牌 (Design Tokens)

### 1.1 颜色系统

#### 主色 (Primary)

```css
:root {
  --color-primary: #409EFF;
  --color-primary-light: #66B1FF;
  --color-primary-dark: #337ECC;
  --color-primary-disabled: #A0CFFF;
  --color-primary-light-9: #ECF5FF;
}
```

#### 辅助色 (Secondary)

```css
:root {
  --color-success: #67C23A;
  --color-warning: #E6A23C;
  --color-danger: #F56C6C;
  --color-info: #909399;
}
```

#### 中性色 (Neutral)

```css
:root {
  /* 文字 */
  --text-primary: #303133;
  --text-regular: #606266;
  --text-secondary: #909399;
  --text-placeholder: #C0C4CC;
  
  /* 边框 */
  --border-base: #DCDFE6;
  --border-light: #E4E7ED;
  --border-lighter: #EBEEF5;
  --border-extra-light: #F2F6FC;
  
  /* 背景 */
  --bg-base: #FFFFFF;
  --bg-page: #F5F7FA;
  --bg-overlay: rgba(0, 0, 0, 0.5);
}
```

#### 侧边栏专用色

```css
:root {
  --sidebar-bg: #304156;
  --sidebar-text: #BFCBD9;
  --sidebar-active: #409EFF;
  --sidebar-hover: #263445;
  --submenu-bg: #1F2D3D;
}
```

### 1.2 间距系统

```css
:root {
  --space-xs: 4px;
  --space-sm: 8px;
  --space-md: 16px;
  --space-lg: 24px;
  --space-xl: 32px;
  --space-2xl: 48px;
}
```

### 1.3 圆角系统

```css
:root {
  --radius-sm: 4px;
  --radius-md: 8px;   /* 主要圆角 */
  --radius-lg: 16px;
  --radius-full: 9999px;
}
```

### 1.4 阴影系统

```css
:root {
  --shadow-sm: 0 2px 4px rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 8px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 8px 16px rgba(0, 0, 0, 0.15);
  --shadow-sidebar: 0 4px 12px rgba(0, 0, 0, 0.15);
}
```

### 1.5 字体系统

```css
:root {
  --font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 
                 'Helvetica Neue', Arial, sans-serif;
  
  --font-size-xs: 12px;
  --font-size-sm: 13px;
  --font-size-base: 14px;
  --font-size-md: 16px;
  --font-size-lg: 18px;
  --font-size-xl: 20px;
  
  --font-weight-normal: 400;
  --font-weight-medium: 500;
  --font-weight-bold: 600;
}
```

## 2. 过渡动画参数

### 2.1 标准过渡

```css
:root {
  /* 快速过渡 - 悬停效果 */
  --transition-fast: 150ms ease;
  
  /* 标准过渡 - 状态变化 */
  --transition-base: 250ms ease-in-out;
  
  /* 慢速过渡 - 展开/收起 */
  --transition-slow: 300ms cubic-bezier(0.4, 0, 0.2, 1);
  
  /* 弹性过渡 - 弹窗、下拉 */
  --transition-bounce: 300ms cubic-bezier(0.34, 1.56, 0.64, 1);
}
```

### 2.2 组件特定过渡

```css
/* 页签 */
.tab-transition {
  transition: all 250ms ease-in-out;
}

/* 侧边栏 */
.sidebar-transition {
  transition: width 300ms cubic-bezier(0.4, 0, 0.2, 1);
}

/* 子菜单 */
.submenu-transition {
  transition: height 300ms ease-in-out, opacity 250ms ease-in-out;
}

/* 按钮 */
.button-transition {
  transition: background-color 150ms ease, 
              border-color 150ms ease, 
              color 150ms ease;
}
```

## 3. 组件样式规范

### 3.1 页签组件

```scss
// Tab Bar
.tab-bar {
  height: 40px;
  background: var(--bg-base);
  border-bottom: 1px solid var(--border-light);
  padding: 4px;
  display: flex;
  align-items: center;
}

// Tab Item
.tab-item {
  height: 32px;
  padding: 8px 16px;
  border-radius: var(--radius-md); // 8px
  display: flex;
  align-items: center;
  gap: var(--space-xs); // 4px
  cursor: pointer;
  transition: var(--transition-base);
  
  // 默认状态
  color: var(--text-regular);
  background: transparent;
  
  // 悬停状态
  &:hover {
    background: var(--bg-page);
    color: var(--color-primary);
  }
  
  // 激活状态
  &.active {
    background: var(--color-primary);
    color: #FFFFFF;
    font-weight: var(--font-weight-medium);
  }
  
  // 图标
  .tab-icon {
    width: 14px;
    height: 14px;
  }
  
  // 关闭按钮
  .tab-close {
    width: 14px;
    height: 14px;
    border-radius: 50%;
    opacity: 0;
    transition: var(--transition-fast);
    
    &:hover {
      background: var(--color-danger);
      color: #FFFFFF;
    }
  }
  
  &:hover .tab-close {
    opacity: 1;
  }
}

// 新建按钮
.tab-add {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--text-secondary);
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--bg-page);
    color: var(--color-primary);
  }
}
```

### 3.2 侧边栏组件

```scss
// Sidebar Container
.sidebar {
  width: 220px;
  height: 100vh;
  background: var(--sidebar-bg);
  transition: var(--sidebar-transition);
  
  &.collapsed {
    width: 64px;
  }
}

// Menu Item
.menu-item {
  height: 48px;
  display: flex;
  align-items: center;
  padding: 0 var(--space-md);
  color: var(--sidebar-text);
  cursor: pointer;
  position: relative;
  transition: var(--transition-fast);
  
  &:hover {
    background: var(--sidebar-hover);
    color: #FFFFFF;
  }
  
  // 激活指示器
  &.active::before {
    content: '';
    position: absolute;
    left: 0;
    top: 50%;
    transform: translateY(-50%);
    width: 3px;
    height: 24px;
    background: var(--sidebar-active);
    border-radius: 0 3px 3px 0;
  }
  
  // 图标区域
  .menu-icon {
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    
    .collapsed & {
      width: 64px;
    }
  }
  
  // 文字区域
  .menu-text {
    margin-left: var(--space-sm);
    white-space: nowrap;
    overflow: hidden;
    
    .collapsed & {
      display: none;
    }
  }
}

// 子菜单
.submenu {
  background: var(--submenu-bg);
  overflow: hidden;
  transition: var(--submenu-transition);
  
  .submenu-item {
    height: 40px;
    padding-left: calc(24px + 24px); // 图标区域 + 缩进
    display: flex;
    align-items: center;
    color: var(--sidebar-text);
    cursor: pointer;
    transition: var(--transition-fast);
    
    &:hover {
      color: #FFFFFF;
    }
    
    &.active {
      color: var(--sidebar-active);
    }
  }
}

// 展开/收起按钮
.collapse-btn {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--sidebar-text);
  cursor: pointer;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  transition: var(--transition-fast);
  
  &:hover {
    color: #FFFFFF;
    background: var(--sidebar-hover);
  }
}
```

## 4. 响应式断点

```css
:root {
  --breakpoint-sm: 576px;
  --breakpoint-md: 768px;
  --breakpoint-lg: 992px;
  --breakpoint-xl: 1200px;
  --breakpoint-2xl: 1400px;
}

/* SCSS Mixins */
@mixin respond-to($breakpoint) {
  @if $breakpoint == sm {
    @media (min-width: var(--breakpoint-sm)) { @content; }
  }
  @if $breakpoint == md {
    @media (min-width: var(--breakpoint-md)) { @content; }
  }
  @if $breakpoint == lg {
    @media (min-width: var(--breakpoint-lg)) { @content; }
  }
  @if $breakpoint == xl {
    @media (min-width: var(--breakpoint-xl)) { @content; }
  }
}
```

## 5. 最佳实践

### 5.1 命名规范

- 使用 BEM 命名法: `.block__element--modifier`
- 组件名使用小写和连字符: `.tab-bar`, `.menu-item`
- 状态类使用前缀: `.is-active`, `.is-disabled`, `.is-collapsed`

### 5.2 变量使用

```scss
// ✅ 推荐: 使用 CSS 变量
.tab-item {
  background: var(--color-primary);
  border-radius: var(--radius-md);
  transition: var(--transition-base);
}

// ❌ 不推荐: 硬编码
.tab-item {
  background: #409EFF;
  border-radius: 8px;
  transition: all 0.25s ease;
}
```

### 5.3 z-index 层级

```css
:root {
  --z-dropdown: 100;
  --z-sticky: 200;
  --z-fixed: 300;
  --z-modal-backdrop: 400;
  --z-modal: 500;
  --z-popover: 600;
  --z-tooltip: 700;
}
```

### 5.4 性能优化

```css
/* 使用 transform 和 opacity 进行动画 */
.animate {
  will-change: transform, opacity;
  transform: translateZ(0);
}

/* 避免频繁改变的属性 */
.avoid {
  /* 避免在动画中使用 */
  width: ...;
  height: ...;
  left: ...;
  top: ...;
}
```

## 6. Element Plus 变量覆盖

```scss
// element-variables.scss

/* 主色 */
$--color-primary: #409EFF;
$--color-success: #67C23A;
$--color-warning: #E6A23C;
$--color-danger: #F56C6C;

/* 边框 */
$--border-color-base: #DCDFE6;
$--border-color-light: #E4E7ED;

/* 字体 */
$--font-path: '~element-ui/lib/theme-chalk/fonts';

/* 导入 Element Plus */
@import '~element-ui/packages/theme-chalk/src/index';
```

---
*文档版本: 1.0*
*更新日期: 2026-09-01*
