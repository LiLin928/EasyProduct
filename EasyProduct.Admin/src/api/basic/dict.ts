import { get } from '@/utils/request'
import type { DictItem } from '@/types/basic'

/** 按字典类型取字典项 */
export const getDictData = (typeCode: string) =>
  get<DictItem[]>('/api/admin/basic/dict-data', { typeCode })
