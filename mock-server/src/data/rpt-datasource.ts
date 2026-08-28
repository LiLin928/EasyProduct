// src/data/rpt-datasource.ts
// 报表数据源 seed

import { guid, isoTime } from '../helpers/id.js'

export interface Datasource {
  id: string
  name: string
  type: 'mysql' | 'postgresql' | 'sqlserver' | 'oracle'
  host: string
  port: number
  database: string
  username: string
  password: string
  status: 'connected' | 'disconnected' | 'error'
  remark: string
  createdAt: string
  updatedAt: string
}

export const DATASOURCES: Datasource[] = [
  {
    id: guid(),
    name: '主业务库',
    type: 'mysql',
    host: '192.168.1.100',
    port: 3306,
    database: 'easyproduct',
    username: 'root',
    password: '********',
    status: 'connected',
    remark: '核心业务数据库',
    createdAt: isoTime(-30),
    updatedAt: isoTime(-2),
  },
  {
    id: guid(),
    name: '报表分析库',
    type: 'postgresql',
    host: '192.168.1.101',
    port: 5432,
    database: 'report_db',
    username: 'report_user',
    password: '********',
    status: 'connected',
    remark: '只读分析库，用于报表查询',
    createdAt: isoTime(-25),
    updatedAt: isoTime(-5),
  },
  {
    id: guid(),
    name: '历史归档库',
    type: 'sqlserver',
    host: '192.168.1.102',
    port: 1433,
    database: 'archive_db',
    username: 'sa',
    password: '********',
    status: 'disconnected',
    remark: '历史数据归档，按需连接',
    createdAt: isoTime(-20),
    updatedAt: isoTime(-15),
  },
  {
    id: guid(),
    name: 'ERP同步库',
    type: 'oracle',
    host: '192.168.1.103',
    port: 1521,
    database: 'orcl',
    username: 'erp_sync',
    password: '********',
    status: 'error',
    remark: 'ERP数据同步源，当前连接异常',
    createdAt: isoTime(-15),
    updatedAt: isoTime(-1),
  },
  {
    id: guid(),
    name: '日志数据库',
    type: 'mysql',
    host: '192.168.1.104',
    port: 3306,
    database: 'logs',
    username: 'log_user',
    password: '********',
    status: 'connected',
    remark: '系统日志收集库',
    createdAt: isoTime(-10),
    updatedAt: isoTime(-3),
  },
  {
    id: guid(),
    name: '测试环境库',
    type: 'mysql',
    host: '127.0.0.1',
    port: 3306,
    database: 'easyproduct_test',
    username: 'root',
    password: '********',
    status: 'connected',
    remark: '开发测试环境',
    createdAt: isoTime(-5),
    updatedAt: isoTime(-1),
  },
]
