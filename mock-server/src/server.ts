// src/server.ts
import express from 'express'
import cors from 'cors'
import { ok } from './helpers/envelope.js'
import { resetAll } from './helpers/registry.js'
import { adminGuard, appGuard } from './helpers/auth.js'

const PORT = 7700
const app = express()
app.use(cors())
app.use(express.json())

// 模块 mock 状态表（与 README 一致；新增模块在此登记）
const MOCK_STATUS = [
  { zone: 'admin', module: 'Basic（认证/菜单/字典骨架）', status: 'pending', backendPhase: 'P1' },
  { zone: 'site', module: '官网内容（首页聚合骨架）', status: 'pending', backendPhase: 'P2' },
  { zone: 'app', module: '商城（会员登录骨架）', status: 'pending', backendPhase: 'P3' },
]

app.get('/__mock/status', (_req, res) => res.json(ok(MOCK_STATUS)))
app.post('/__mock/reset', (_req, res) => {
  resetAll()
  res.json(ok(null, '已重置'))
})

import { adminAuthRouter } from './routes/admin/auth.js'
import { adminMenuRouter } from './routes/admin/menu.js'
import { adminDictRouter } from './routes/admin/dict.js'
import { adminUserRouter } from './routes/admin/user.js'
import { adminDeptRouter } from './routes/admin/dept.js'
import { adminRoleRouter } from './routes/admin/role.js'
import { adminFileRouter } from './routes/admin/file.js'
import { adminConfigRouter } from './routes/admin/config.js'
import { adminDesktopRouter } from './routes/admin/desktop.js'
import { adminProfileRouter } from './routes/admin/profile.js'
import { adminAnnouncementRouter } from './routes/admin/announcement.js'
import { adminSiteNewsRouter } from './routes/admin/site-news.js'
import { adminSiteCategoryRouter } from './routes/admin/site-category.js'
import { adminSiteBannerRouter } from './routes/admin/site-banner.js'
import { adminSiteVideoRouter } from './routes/admin/site-video.js'
import { adminSiteDownloadRouter } from './routes/admin/site-download.js'
import { adminSiteAboutRouter } from './routes/admin/site-about.js'
import { adminSiteInquiryRouter } from './routes/admin/site-inquiry.js'
import { adminSiteContactRouter } from './routes/admin/site-contact.js'
import { adminProductCategoryRouter } from './routes/admin/product-category.js'
import { adminProductSpuRouter } from './routes/admin/product-spu.js'
import { adminProductChannelRouter } from './routes/admin/product-channel.js'
import { adminMallMemberRouter } from './routes/admin/mall-member.js'
import { adminMallLevelRouter } from './routes/admin/mall-level.js'
import { adminMallPointsRouter } from './routes/admin/mall-points.js'
import { adminMallCouponRouter } from './routes/admin/mall-coupon.js'
import { adminMallOrderRouter } from './routes/admin/mall-order.js'
import { adminMallPaymentRouter } from './routes/admin/mall-payment.js'
import { adminCrmCustomerRouter } from './routes/admin/crm-customer.js'
import { adminCrmSupplierRouter } from './routes/admin/crm-supplier.js'
import { adminCrmCurrencyRouter } from './routes/admin/crm-currency.js'
import { adminCrmTaxRateRouter } from './routes/admin/crm-tax-rate.js'
import { adminCrmSalesOrderRouter } from './routes/admin/crm-sales-order.js'
import { adminCrmPurchaseOrderRouter } from './routes/admin/crm-purchase-order.js'
import { adminCrmWarehouseRouter } from './routes/admin/crm-warehouse.js'
import { adminCrmStockRouter } from './routes/admin/crm-stock.js'
import { adminCrmStockRecordRouter } from './routes/admin/crm-stock-record.js'
import { adminCrmStockCheckRouter } from './routes/admin/crm-stock-check.js'
import { adminCrmStockAlertRouter } from './routes/admin/crm-stock-alert.js'
import { adminCrmInvoiceRouter } from './routes/admin/crm-invoice.js'
import { adminCrmPaymentRouter } from './routes/admin/crm-payment.js'
import { adminCrmArapRouter } from './routes/admin/crm-arap.js'
import { adminCrmFixedAssetRouter } from './routes/admin/crm-fixed-asset.js'
import { siteHomeRouter } from './routes/site/home.js'
import { siteProductRouter } from './routes/site/product.js'
import { siteCategoryRouter } from './routes/site/category.js'
import { siteNewsRouter } from './routes/site/news.js'
import { siteVideoRouter } from './routes/site/video.js'
import { siteDownloadRouter } from './routes/site/download.js'
import { siteAboutRouter } from './routes/site/about.js'
import { siteContactRouter } from './routes/site/contact.js'
import { siteInquiryRouter } from './routes/site/inquiry.js'
import { siteAnnouncementRouter } from './routes/site/announcement.js'
import { appAuthRouter } from './routes/app/auth.js'
import { appAnnouncementRouter } from './routes/app/announcement.js'
import { i18nRouter } from './routes/i18n.js'

app.use('/api/admin', adminGuard, adminAuthRouter, adminMenuRouter, adminDictRouter, adminUserRouter, adminDeptRouter, adminRoleRouter, adminFileRouter, adminConfigRouter, adminDesktopRouter, adminProfileRouter, adminAnnouncementRouter, adminSiteNewsRouter, adminSiteCategoryRouter, adminSiteBannerRouter, adminSiteVideoRouter, adminSiteDownloadRouter, adminSiteAboutRouter, adminSiteInquiryRouter, adminSiteContactRouter, adminProductCategoryRouter, adminProductSpuRouter, adminProductChannelRouter, adminMallMemberRouter, adminMallLevelRouter, adminMallPointsRouter, adminMallCouponRouter, adminMallOrderRouter, adminMallPaymentRouter, adminCrmCustomerRouter, adminCrmSupplierRouter, adminCrmCurrencyRouter, adminCrmTaxRateRouter, adminCrmSalesOrderRouter, adminCrmPurchaseOrderRouter, adminCrmWarehouseRouter, adminCrmStockRouter, adminCrmStockRecordRouter, adminCrmStockCheckRouter, adminCrmStockAlertRouter, adminCrmInvoiceRouter, adminCrmPaymentRouter, adminCrmArapRouter, adminCrmFixedAssetRouter)
app.use('/api/site', siteHomeRouter, siteProductRouter, siteCategoryRouter, siteNewsRouter, siteVideoRouter, siteDownloadRouter, siteAboutRouter, siteContactRouter, siteInquiryRouter, siteAnnouncementRouter)
app.use('/api/app', appGuard, appAuthRouter, appAnnouncementRouter)
app.use('/api/i18n', i18nRouter)

app.listen(PORT, () => {
  // eslint-disable-next-line no-console
  console.log(`[mock-server] running at http://localhost:${PORT}`)
})

export { adminGuard, appGuard }
