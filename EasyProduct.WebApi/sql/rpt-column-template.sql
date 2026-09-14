-- 报表列模板表
CREATE TABLE `rpt_column_template` (
  `id` char(36) NOT NULL COMMENT '主键ID',
  `name` varchar(100) NOT NULL COMMENT '模板名称',
  `field` varchar(50) NOT NULL COMMENT '字段名',
  `type` varchar(20) NOT NULL DEFAULT 'string' COMMENT '字段类型：string/number/date/currency',
  `width` int NOT NULL DEFAULT 150 COMMENT '列宽',
  `format` varchar(50) DEFAULT NULL COMMENT '格式化规则',
  `sortable` int NOT NULL DEFAULT 1 COMMENT '是否可排序：0-否，1-是',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注说明',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` varchar(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` int NOT NULL DEFAULT 0 COMMENT '是否删除：0-否，1-是',
  PRIMARY KEY (`id`),
  KEY `idx_name` (`name`),
  KEY `idx_type` (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表列模板表';

-- 插入示例列模板
INSERT INTO `rpt_column_template` (`id`, `name`, `field`, `type`, `width`, `format`, `sortable`, `remark`, `create_by`) VALUES
('template-date', '日期列', 'date', 'date', 120, 'yyyy-MM-dd', 1, '日期格式化模板', 'system'),
('template-currency', '金额列', 'amount', 'currency', 120, '#,##0.00', 1, '金额格式化模板', 'system'),
('template-number', '数量列', 'quantity', 'number', 100, '#,##0', 1, '数字格式化模板', 'system');