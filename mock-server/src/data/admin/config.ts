// src/data/admin/config.ts
import { guid, isoTime } from '../../helpers/id.js'

export interface SystemConfig {
  id: string
  key: string
  label: string
  value: string
  type: 'string' | 'number' | 'boolean'
  remark: string
  createdAt: string
  updatedAt: string
}

const now = () => isoTime()

export const SYSTEM_CONFIGS: SystemConfig[] = [
  {
    id: guid(),
    key: 'site_name',
    label: '站点名称',
    value: 'EasyProduct 管理系统',
    type: 'string',
    remark: '浏览器标签与登录页标题',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'site_icp',
    label: '备案号',
    value: '粤ICP备00000000号',
    type: 'string',
    remark: '官网底部备案号',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'upload_max_size',
    label: '上传大小上限(MB)',
    value: '2',
    type: 'number',
    remark: '单文件上传大小上限',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'register_enabled',
    label: '允许会员注册',
    value: 'true',
    type: 'boolean',
    remark: '小程序会员注册开关',
    createdAt: now(),
    updatedAt: now(),
  },
  {
    id: guid(),
    key: 'customer_service_phone',
    label: '客服电话',
    value: '400-000-0000',
    type: 'string',
    remark: '官网客服联系电话',
    createdAt: now(),
    updatedAt: now(),
  },
]