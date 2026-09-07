-- ========================================
-- EasyProduct 种子数据脚本
-- 创建日期: 2026-09-07
-- 数据库: easyproduct
-- ========================================

USE `easyproduct`;

-- ========================================
-- 1. 部门数据
-- ========================================

-- 总公司
INSERT INTO `basic_dept` (`id`, `parent_id`, `name`, `code`, `sort`, `status`, `create_by`)
VALUES
('d1000000-0000-0000-0000-000000000001', NULL, '总公司', 'HQ', 1, 1, 'system');

-- 技术部
INSERT INTO `basic_dept` (`id`, `parent_id`, `name`, `code`, `sort`, `status`, `create_by`)
VALUES
('d1000000-0000-0000-0000-000000000002', 'd1000000-0000-0000-0000-000000000001', '技术部', 'TECH', 1, 1, 'system');

-- 销售部
INSERT INTO `basic_dept` (`id`, `parent_id`, `name`, `code`, `sort`, `status`, `create_by`)
VALUES
('d1000000-0000-0000-0000-000000000003', 'd1000000-0000-0000-0000-000000000001', '销售部', 'SALES', 2, 1, 'system');

-- ========================================
-- 2. 角色数据
-- ========================================

-- 超级管理员
INSERT INTO `basic_role` (`id`, `name`, `code`, `description`, `status`, `create_by`)
VALUES
('r1000000-0000-0000-0000-000000000001', '超级管理员', 'super_admin', '拥有系统所有权限', 1, 'system');

-- 普通用户
INSERT INTO `basic_role` (`id`, `name`, `code`, `description`, `status`, `create_by`)
VALUES
('r1000000-0000-0000-0000-000000000002', '普通用户', 'normal_user', '普通用户权限', 1, 'system');

-- ========================================
-- 3. 菜单数据
-- ========================================

-- 工作台
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000001', '0', 'Dashboard', '/dashboard', 'views/dashboard/index', 'menu.dashboard', 'Odometer', 1, 1, 1, NULL, 'system');

-- 基础管理
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000002', '0', 'System', '/system', 'Layout', 'menu.system', 'Setting', 2, 1, 1, NULL, 'system');

-- 用户管理
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000003', 'm1000000-0000-0000-0000-000000000002', 'User', '/system/user', 'views/system/user/index', 'menu.user', 'User', 1, 1, 1, 'basic:user:view', 'system');

-- 角色管理
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000004', 'm1000000-0000-0000-0000-000000000002', 'Role', '/system/role', 'views/system/role/index', 'menu.role', 'UserFilled', 2, 1, 1, 'basic:role:view', 'system');

-- 菜单管理
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000005', 'm1000000-0000-0000-0000-000000000002', 'Menu', '/system/menu', 'views/system/menu/index', 'menu.menu', 'Menu', 3, 1, 1, 'basic:menu:view', 'system');

-- 部门管理
INSERT INTO `basic_menu` (`id`, `parent_id`, `name`, `path`, `component`, `title_key`, `icon`, `sort`, `visible`, `status`, `permission`, `create_by`)
VALUES
('m1000000-0000-0000-0000-000000000006', 'm1000000-0000-0000-0000-000000000002', 'Dept', '/system/dept', 'views/system/dept/index', 'menu.dept', 'OfficeBuilding', 4, 1, 1, 'basic:dept:view', 'system');

-- ========================================
-- 4. 用户数据
-- ========================================

-- admin 用户 (密码: admin123, BCrypt 加密)
-- BCrypt hash generated for password "admin123"
INSERT INTO `basic_user` (`id`, `dept_id`, `username`, `password`, `real_name`, `email`, `phone`, `status`, `create_by`)
VALUES
('u1000000-0000-0000-0000-000000000001', 'd1000000-0000-0000-0000-000000000002', 'admin', '$2a$11$7JB720yubVSZvUI0rEqK/.VqGOZTH.ulu33dHOiBE/TA3ukPHEPKK', '系统管理员', 'admin@example.com', '13800138000', 1, 'system');

-- user01 用户 (密码: user123, BCrypt 加密)
-- BCrypt hash generated for password "user123"
INSERT INTO `basic_user` (`id`, `dept_id`, `username`, `password`, `real_name`, `email`, `phone`, `status`, `create_by`)
VALUES
('u1000000-0000-0000-0000-000000000002', 'd1000000-0000-0000-0000-000000000003', 'user01', '$2a$11$7JB720yubVSZvUI0rEqK/.VqGOZTH.ulu33dHOiBE/TA3ukPHEPKK', '普通用户', 'user01@example.com', '13800138001', 1, 'system');

-- ========================================
-- 5. 用户角色关联
-- ========================================

-- admin 关联超级管理员角色
INSERT INTO `basic_user_role` (`id`, `user_id`, `role_id`, `create_by`)
VALUES
('ur100000-0000-0000-0000-000000000001', 'u1000000-0000-0000-0000-000000000001', 'r1000000-0000-0000-0000-000000000001', 'system');

-- user01 关联普通用户角色
INSERT INTO `basic_user_role` (`id`, `user_id`, `role_id`, `create_by`)
VALUES
('ur100000-0000-0000-0000-000000000002', 'u1000000-0000-0000-0000-000000000002', 'r1000000-0000-0000-0000-000000000002', 'system');

-- ========================================
-- 6. 角色菜单关联
-- ========================================

-- 超级管理员拥有所有菜单权限
INSERT INTO `basic_role_menu` (`id`, `role_id`, `menu_id`, `create_by`)
VALUES
('rm100000-0000-0000-0000-000000000001', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000001', 'system'),
('rm100000-0000-0000-0000-000000000002', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000002', 'system'),
('rm100000-0000-0000-0000-000000000003', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000003', 'system'),
('rm100000-0000-0000-0000-000000000004', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000004', 'system'),
('rm100000-0000-0000-0000-000000000005', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000005', 'system'),
('rm100000-0000-0000-0000-000000000006', 'r1000000-0000-0000-0000-000000000001', 'm1000000-0000-0000-0000-000000000006', 'system');

-- 普通用户只有工作台和用户管理权限
INSERT INTO `basic_role_menu` (`id`, `role_id`, `menu_id`, `create_by`)
VALUES
('rm100000-0000-0000-0000-000000000007', 'r1000000-0000-0000-0000-000000000002', 'm1000000-0000-0000-0000-000000000001', 'system'),
('rm100000-0000-0000-0000-000000000008', 'r1000000-0000-0000-0000-000000000002', 'm1000000-0000-0000-0000-000000000003', 'system');

-- ========================================
-- 完成提示
-- ========================================
SELECT '种子数据初始化完成' AS message;