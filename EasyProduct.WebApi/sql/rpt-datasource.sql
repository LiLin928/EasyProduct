-- 报表数据源表
CREATE TABLE `rpt_datasource` (
  `id` char(36) NOT NULL COMMENT '主键ID',
  `name` varchar(100) NOT NULL COMMENT '数据源名称',
  `type` varchar(20) NOT NULL COMMENT '数据源类型：mysql/postgresql/sqlserver/oracle',
  `host` varchar(100) NOT NULL COMMENT '主机地址',
  `port` int NOT NULL COMMENT '端口号',
  `database` varchar(100) NOT NULL COMMENT '数据库名称',
  `username` varchar(50) NOT NULL COMMENT '用户名',
  `password` varchar(500) NOT NULL COMMENT '密码（加密存储）',
  `status` int NOT NULL DEFAULT 2 COMMENT '连接状态：1-已连接，2-连接错误',
  `last_test_time` datetime DEFAULT NULL COMMENT '最后测试时间',
  `error_message` varchar(500) DEFAULT NULL COMMENT '错误信息',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注说明',
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` varchar(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` int NOT NULL DEFAULT 0 COMMENT '是否删除：0-否，1-是',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_name` (`name`),
  KEY `idx_type` (`type`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表数据源表';

-- 插入默认数据源（主库）
INSERT INTO `rpt_datasource` (`id`, `name`, `type`, `host`, `port`, `database`, `username`, `password`, `status`, `remark`, `create_by`) VALUES
('default-datasource-id', '主数据库', 'mysql', 'localhost', 3306, 'easyproduct', 'root', '', 1, '默认主数据库连接', 'system');