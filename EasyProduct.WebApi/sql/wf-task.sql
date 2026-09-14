-- 流程任务表
CREATE TABLE `wf_task` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `instance_id` CHAR(36) NOT NULL COMMENT '流程实例ID',
  `node_id` VARCHAR(50) NOT NULL COMMENT '节点ID',
  `node_name` VARCHAR(100) NOT NULL COMMENT '节点名称',
  `assignee_id` CHAR(36) NOT NULL COMMENT '受托人ID',
  `assignee_name` VARCHAR(50) NOT NULL COMMENT '受托人姓名',
  `status` INT NOT NULL DEFAULT 0 COMMENT '任务状态：0-待办、1-已通过、2-已拒绝、3-已委托、4-已取消',
  `comment` TEXT COMMENT '审批意见',
  `completed_at` DATETIME COMMENT '完成时间',
  `delegator_id` CHAR(36) COMMENT '委托人ID',
  `delegator_name` VARCHAR(50) COMMENT '委托人姓名',
  `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0-否、1-是',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` DATETIME COMMENT '更新时间',
  `created_by` VARCHAR(36) COMMENT '创建人ID',
  `updated_by` VARCHAR(36) COMMENT '更新人ID',
  PRIMARY KEY (`id`),
  KEY `idx_instance_id` (`instance_id`),
  KEY `idx_assignee_id` (`assignee_id`),
  KEY `idx_status` (`status`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='流程任务表';
