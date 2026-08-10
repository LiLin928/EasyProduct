import { get } from '@/utils/request'
import type { About, ContactInfo } from '@/types/site'

export const getAboutDetail = () =>
  get<About>('/api/site/about/detail')

export const getContactInfo = () =>
  get<ContactInfo>('/api/site/contact/info')
