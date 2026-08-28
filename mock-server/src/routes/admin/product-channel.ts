// src/routes/admin/product-channel.ts
import { Router } from 'express'
import { ok, fail, paginate } from '../../helpers/envelope.js'
import { isoTime } from '../../helpers/id.js'
import { CHANNELS, SPUS, type ChannelType } from '../../data/product.js'

export const adminProductChannelRouter = Router()

const CHANNEL_TYPES: ChannelType[] = ['site', 'miniapp', 'b2b']

adminProductChannelRouter.get('/product/channel/list', (req, res) => {
  const pageIndex = Number(req.query.pageIndex ?? 1)
  const pageSize = Number(req.query.pageSize ?? 10)
  const channel = req.query.channel as string | undefined
  const published = req.query.published as string | undefined
  const spuName = req.query.spuName as string | undefined

  const rows = SPUS.map(spu => {
    const spuChannels = CHANNELS.filter(c => c.spuId === spu.id)
    return {
      spuId: spu.id,
      spuName: spu.name,
      spuCode: spu.code,
      mainImage: spu.mainImage,
      channels: CHANNEL_TYPES.map(ch => {
        const found = spuChannels.find(c => c.channel === ch)
        return {
          id: found?.id || '',
          channel: ch,
          published: found?.published ?? false,
          sort: found?.sort ?? 0,
        }
      }),
    }
  })

  let filtered = [...rows]
  if (spuName) filtered = filtered.filter(r => r.spuName.includes(spuName))
  if (channel) {
    filtered = filtered.filter(r => {
      const ch = r.channels.find(c => c.channel === channel)
      return ch && ch.published
    })
  }
  if (published !== undefined) {
    const isPublished = published === 'true'
    filtered = filtered.filter(r => r.channels.some(c => c.published === isPublished))
  }

  res.json(ok(paginate(filtered, pageIndex, pageSize)))
})

adminProductChannelRouter.put('/product/channel/toggle', (req, res) => {
  const { spuId, channel, published } = req.body
  if (!spuId || !channel) { res.json(fail('spuId and channel required')); return }

  let record = CHANNELS.find(c => c.spuId === spuId && c.channel === channel)
  if (!record) {
    CHANNELS.push({
      id: `${spuId}-${channel}`,
      spuId,
      channel,
      published: !!published,
      sort: 0,
      createdAt: isoTime(),
      updatedAt: isoTime(),
    })
  } else {
    record.published = !!published
    record.updatedAt = isoTime()
  }
  res.json(ok(null, 'updated'))
})

adminProductChannelRouter.put('/product/channel/sort', (req, res) => {
  const { spuId, channel, sort } = req.body
  if (!spuId || !channel) { res.json(fail('spuId and channel required')); return }

  let record = CHANNELS.find(c => c.spuId === spuId && c.channel === channel)
  if (!record) {
    CHANNELS.push({
      id: `${spuId}-${channel}`,
      spuId,
      channel,
      published: false,
      sort: sort ?? 0,
      createdAt: isoTime(),
      updatedAt: isoTime(),
    })
  } else {
    record.sort = sort ?? 0
    record.updatedAt = isoTime()
  }
  res.json(ok(null, 'updated'))
})
