import { post } from '@/utils/request'

interface ContactForm {
  companyName: string
  contactName: string
  phone: string
  email: string
  content: string
}

export const submitContact = (data: ContactForm) =>
  post<null>('/api/site/contact', data)
