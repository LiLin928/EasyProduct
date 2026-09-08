-- ========================================
-- EasyProduct 数据库初始化脚本
-- 创建日期: 2026-09-07
-- 数据库: easyproduct
-- 字符集: utf8mb4
-- ========================================

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `easyproduct`
DEFAULT CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE `easyproduct`;

-- ========================================
-- Basic 模块表
-- ========================================

-- ----------------------------
-- 1. 部门表 (basic_dept)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dept`;
CREATE TABLE `basic_dept` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT NULL COMMENT '父部门ID',
  `name` VARCHAR(50) NOT NULL COMMENT '部门名称',
  `code` VARCHAR(50) DEFAULT NULL COMMENT '部门编码',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='部门表';

-- ----------------------------
-- 2. 用户表 (basic_user)
-- ----------------------------
DROP TABLE IF EXISTS `basic_user`;
CREATE TABLE `basic_user` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dept_id` CHAR(36) DEFAULT NULL COMMENT '部门ID',
  `username` VARCHAR(50) NOT NULL COMMENT '用户名',
  `password` VARCHAR(255) NOT NULL COMMENT '密码（BCrypt加密）',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `avatar` VARCHAR(255) DEFAULT NULL COMMENT '头像URL',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_username` (`username`),
  KEY `idx_dept_id` (`dept_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- ----------------------------
-- 3. 角色表 (basic_role)
-- ----------------------------
DROP TABLE IF EXISTS `basic_role`;
CREATE TABLE `basic_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `name` VARCHAR(50) NOT NULL COMMENT '角色名称',
  `code` VARCHAR(50) NOT NULL COMMENT '角色编码',
  `description` VARCHAR(255) DEFAULT NULL COMMENT '描述',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色表';

-- ----------------------------
-- 4. 用户角色关联表 (basic_user_role)
-- ----------------------------
DROP TABLE IF EXISTS `basic_user_role`;
CREATE TABLE `basic_user_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `user_id` CHAR(36) NOT NULL COMMENT '用户ID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_role` (`user_id`, `role_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户角色关联表';

-- ----------------------------
-- 5. 菜单表 (basic_menu)
-- ----------------------------
DROP TABLE IF EXISTS `basic_menu`;
CREATE TABLE `basic_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT '0' COMMENT '父菜单ID（0表示根菜单）',
  `name` VARCHAR(50) NOT NULL COMMENT '菜单名称（路由name）',
  `path` VARCHAR(255) DEFAULT NULL COMMENT '路由路径',
  `component` VARCHAR(255) DEFAULT NULL COMMENT '组件路径',
  `title_key` VARCHAR(100) DEFAULT NULL COMMENT '标题i18n key',
  `icon` VARCHAR(50) DEFAULT NULL COMMENT '图标名称',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `visible` INT DEFAULT 1 COMMENT '是否可见：0=否，1=是',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `permission` VARCHAR(100) DEFAULT NULL COMMENT '权限标识',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_status` (`status`),
  KEY `idx_visible` (`visible`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='菜单表';

-- ----------------------------
-- 6. 角色菜单关联表 (basic_role_menu)
-- ----------------------------
DROP TABLE IF EXISTS `basic_role_menu`;
CREATE TABLE `basic_role_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `menu_id` CHAR(36) NOT NULL COMMENT '菜单ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_role_menu` (`role_id`, `menu_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_menu_id` (`menu_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色菜单关联表';

-- ========================================
-- Ops 模块表
-- ========================================

-- ----------------------------
-- 7. 操作日志表 (ops_operate_log)
-- ----------------------------
DROP TABLE IF EXISTS `ops_operate_log`;
CREATE TABLE `ops_operate_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `module` VARCHAR(50) DEFAULT NULL COMMENT '模块名称',
  `target` VARCHAR(100) DEFAULT NULL COMMENT '操作对象',
  `action` VARCHAR(50) DEFAULT NULL COMMENT '操作动作',
  `method` VARCHAR(10) DEFAULT NULL COMMENT 'HTTP方法',
  `url` VARCHAR(500) DEFAULT NULL COMMENT '请求URL',
  `params` TEXT COMMENT '请求参数',
  `ip` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
  `user_agent` VARCHAR(500) DEFAULT NULL COMMENT '用户代理',
  `user_id` CHAR(36) DEFAULT NULL COMMENT '用户ID',
  `user_name` VARCHAR(50) DEFAULT NULL COMMENT '用户名',
  `status` INT DEFAULT 1 COMMENT '操作状态：0=失败，1=成功',
  `error_msg` TEXT COMMENT '错误信息',
  `duration` INT DEFAULT NULL COMMENT '耗时（毫秒）',
  `trace_id` VARCHAR(50) DEFAULT NULL COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_module` (`module`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作日志表';

-- ----------------------------
-- 8. 登录日志表 (ops_login_log)
-- ----------------------------
DROP TABLE IF EXISTS `ops_login_log`;
CREATE TABLE `ops_login_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `user_id` CHAR(36) DEFAULT NULL COMMENT '用户ID',
  `user_name` VARCHAR(50) DEFAULT NULL COMMENT '用户名',
  `login_type` VARCHAR(20) DEFAULT NULL COMMENT '登录类型：login/logout',
  `ip` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
  `location` VARCHAR(100) DEFAULT NULL COMMENT '登录地点',
  `browser` VARCHAR(100) DEFAULT NULL COMMENT '浏览器',
  `os` VARCHAR(100) DEFAULT NULL COMMENT '操作系统',
  `status` INT DEFAULT 1 COMMENT '登录状态：0=失败，1=成功',
  `error_msg` TEXT COMMENT '错误信息',
  `trace_id` VARCHAR(50) DEFAULT NULL COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='登录日志表';

-- ========================================
-- Basic 扩展表（字典、公告、系统参数）
-- ========================================

-- ----------------------------
-- 9. 字典类型表 (basic_dict_type)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dict_type`;
CREATE TABLE `basic_dict_type` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dict_name` VARCHAR(100) NOT NULL COMMENT '字典名称',
  `dict_type` VARCHAR(100) NOT NULL COMMENT '字典类型（唯一标识）',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_dict_type` (`dict_type`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='字典类型表';

-- ----------------------------
-- 10. 字典数据表 (basic_dict_data)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dict_data`;
CREATE TABLE `basic_dict_data` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dict_type` VARCHAR(100) NOT NULL COMMENT '字典类型',
  `dict_label` VARCHAR(100) NOT NULL COMMENT '字典标签',
  `dict_value` VARCHAR(100) NOT NULL COMMENT '字典值',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_dict_type` (`dict_type`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='字典数据表';

-- ----------------------------
-- 11. 公告表 (basic_notice)
-- ----------------------------
DROP TABLE IF EXISTS `basic_notice`;
CREATE TABLE `basic_notice` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `notice_title` VARCHAR(200) NOT NULL COMMENT '公告标题',
  `notice_content` TEXT NOT NULL COMMENT '公告内容（富文本）',
  `notice_type` INT NOT NULL DEFAULT 1 COMMENT '公告类型：1=通知，2=公告',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `top_flag` INT DEFAULT 0 COMMENT '是否置顶：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_notice_type` (`notice_type`),
  KEY `idx_status` (`status`),
  KEY `idx_top_flag` (`top_flag`),
  KEY `idx_publish_time` (`publish_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='公告表';

-- ----------------------------
-- 12. 系统参数表 (basic_config)
-- ----------------------------
DROP TABLE IF EXISTS `basic_config`;
CREATE TABLE `basic_config` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `config_name` VARCHAR(100) NOT NULL COMMENT '参数名称',
  `config_key` VARCHAR(100) NOT NULL COMMENT '参数键名',
  `config_value` VARCHAR(500) NOT NULL COMMENT '参数键值',
  `config_type` INT DEFAULT 1 COMMENT '系统内置：0=否，1=是',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_config_key` (`config_key`),
  KEY `idx_config_type` (`config_type`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='系统参数表';

-- ========================================
-- 完成提示
-- ========================================
SELECT '数据库初始化完成' AS message;