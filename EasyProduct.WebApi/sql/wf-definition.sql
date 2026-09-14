-- 流程定义表
CREATE TABLE `wf_definition` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `code` VARCHAR(50) NOT NULL COMMENT '流程编码',
  `name` VARCHAR(100) NOT NULL COMMENT '流程名称',
  `version` INT NOT NULL DEFAULT 1 COMMENT '版本号',
  `category` VARCHAR(50) DEFAULT '' COMMENT '流程分类',
  `description` TEXT COMMENT '流程描述',
  `nodes` TEXT NOT NULL COMMENT '流程节点（JSON）',
  `status` INT NOT NULL DEFAULT 0 COMMENT '流程状态：0-草稿、1-已发布、2-已归档',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-否、1-是',
  `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0-禁用、1-启用',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME COMMENT '更新时间',
  `created_by` VARCHAR(36) COMMENT '创建人ID',
  `updated_by` VARCHAR(36) COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_code` (`code`),
  KEY `idx_status` (`status`),
  KEY `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='流程定义表';
