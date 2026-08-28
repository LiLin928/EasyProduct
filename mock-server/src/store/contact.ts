// src/store/contact.ts
// [backend: Site 模块 | status: pending]
import Mock from 'mockjs'
import { guid, isoTime } from '../helpers/id.js'
import { registerReset } from '../helpers/registry.js'

export interface ContactMessage {
  id: string
  name: string
  company: string
  phone: string
  email: string
  subject: string
  message: string
  status: 'unread' | 'read' | 'archived'
  createdAt: string
  updatedAt: string
}

const messages: ContactMessage[] = Mock.mock({
  'list|8': [{
    id: '@guid',
    name: '@cname',
    company: '@ctitle(3,8)',
    phone: /^1[3-9]\d{9}$/,
    email: '@email',
    subject: '@ctitle(5,15)',
    message: '@cparagraph(2,4)',
    status: '@pick(["unread", "read", "archived"])',
    createdAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
    updatedAt: '@datetime("yyyy-MM-ddTHH:mm:ss")',
  }],
}).list.map((m: Record<string, unknown>) => ({
  ...m,
  id: guid(),
  createdAt: isoTime(),
  updatedAt: isoTime(),
})) as ContactMessage[]

function resetContactStore(): void {
  messages.length = 0
}

registerReset(resetContactStore)

export function listContacts(): ContactMessage[] {
  return messages
}

export function getContact(id: string): ContactMessage | undefined {
  return messages.find((m) => m.id === id)
}

export function createContact(data: Omit<ContactMessage, 'id' | 'status' | 'createdAt' | 'updatedAt'>): ContactMessage {
  const msg: ContactMessage = {
    ...data,
    id: guid(),
    status: 'unread',
    createdAt: isoTime(),
    updatedAt: isoTime(),
  }
  messages.unshift(msg)
  return msg
}

export function updateContactStatus(id: string, status: ContactMessage['status']): boolean {
  const msg = messages.find((m) => m.id === id)
  if (!msg) return false
  msg.status = status
  msg.updatedAt = isoTime()
  return true
}
