// utils/i18n.ts
import { getLocaleCache, setLocaleCache } from './storage'

export type MiniLocale = 'zh-CN' | 'en-US'

// 内置语言包（内嵌 JSON 内容）
const zhCommon = {
  "app": {
    "name": "EasyProduct"
  },
  "button": {
    "confirm": "确定",
    "cancel": "取消",
    "submit": "提交",
    "save": "保存",
    "delete": "删除",
    "edit": "编辑",
    "add": "添加",
    "buy": "立即购买",
    "addToCart": "加入购物车",
    "settle": "结算",
    "pay": "支付",
    "receive": "确认收货",
    "cancelOrder": "取消订单",
    "more": "更多",
    "viewMore": "查看更多"
  },
  "loading": "加载中…",
  "loadFailed": "加载失败",
  "tab": {
    "home": "首页",
    "category": "分类",
    "cart": "购物车",
    "profile": "我的"
  },
  "search": {
    "placeholder": "搜索商品"
  },
  "product": {
    "price": "价格",
    "originalPrice": "原价",
    "stock": "库存",
    "sales": "销量",
    "hot": "热销推荐",
    "new": "新品上市",
    "detail": "商品详情",
    "spec": "选择规格",
    "quantity": "数量",
    "addToCartSuccess": "已加入购物车",
    "noStock": "库存不足"
  },
  "cart": {
    "title": "购物车",
    "empty": "购物车是空的",
    "selectAll": "全选",
    "total": "合计",
    "settle": "结算",
    "deleteConfirm": "确定删除该商品吗？"
  },
  "order": {
    "title": "我的订单",
    "orderNo": "订单号",
    "createTime": "下单时间",
    "totalAmount": "订单金额",
    "status": {
      "pending": "待付款",
      "paid": "待发货",
      "shipped": "待收货",
      "completed": "已完成",
      "cancelled": "已取消",
      "refunded": "已退款"
    },
    "payNow": "立即支付",
    "cancel": "取消订单",
    "confirmReceive": "确认收货"
  },
  "address": {
    "title": "收货地址",
    "name": "收货人",
    "phone": "联系电话",
    "region": "所在地区",
    "detail": "详细地址",
    "default": "设为默认",
    "add": "新增地址",
    "edit": "编辑地址",
    "deleteConfirm": "确定删除该地址吗？"
  },
  "pay": {
    "title": "确认支付",
    "orderInfo": "订单信息",
    "payAmount": "支付金额",
    "payMethod": "支付方式",
    "wxPay": "微信支付",
    "paySuccess": "支付成功",
    "payFailed": "支付失败"
  },
  "profile": {
    "myOrders": "我的订单",
    "myAddress": "收货地址",
    "contactService": "联系客服",
    "settings": "设置",
    "logout": "退出登录"
  },
  "announcement": {
    "title": "公告中心",
    "detail": "公告详情",
    "type": {
      "system": "系统公告",
      "activity": "活动公告",
      "update": "更新公告"
    },
    "unread": "未读",
    "noData": "暂无公告",
    "publishedAt": "发布时间",
    "attachment": "附件",
    "download": "下载",
    "markRead": "标记已读",
    "pullRefresh": "下拉刷新",
    "loadMore": "上拉加载更多",
    "noMore": "没有更多了"
  }
}

const enCommon = {
  "app": {
    "name": "EasyProduct"
  },
  "button": {
    "confirm": "Confirm",
    "cancel": "Cancel",
    "submit": "Submit",
    "save": "Save",
    "delete": "Delete",
    "edit": "Edit",
    "add": "Add",
    "buy": "Buy Now",
    "addToCart": "Add to Cart",
    "settle": "Checkout",
    "pay": "Pay",
    "receive": "Confirm Receipt",
    "cancelOrder": "Cancel Order",
    "more": "More",
    "viewMore": "View More"
  },
  "loading": "Loading...",
  "loadFailed": "Failed to load",
  "tab": {
    "home": "Home",
    "category": "Category",
    "cart": "Cart",
    "profile": "Profile"
  },
  "search": {
    "placeholder": "Search products"
  },
  "product": {
    "price": "Price",
    "originalPrice": "Original Price",
    "stock": "Stock",
    "sales": "Sales",
    "hot": "Hot Products",
    "new": "New Arrivals",
    "detail": "Product Details",
    "spec": "Select Specification",
    "quantity": "Quantity",
    "addToCartSuccess": "Added to cart",
    "noStock": "Out of stock"
  },
  "cart": {
    "title": "Shopping Cart",
    "empty": "Your cart is empty",
    "selectAll": "Select All",
    "total": "Total",
    "settle": "Checkout",
    "deleteConfirm": "Are you sure to delete this item?"
  },
  "order": {
    "title": "My Orders",
    "orderNo": "Order No",
    "createTime": "Order Time",
    "totalAmount": "Total Amount",
    "status": {
      "pending": "Pending Payment",
      "paid": "Pending Shipment",
      "shipped": "Pending Receipt",
      "completed": "Completed",
      "cancelled": "Cancelled",
      "refunded": "Refunded"
    },
    "payNow": "Pay Now",
    "cancel": "Cancel Order",
    "confirmReceive": "Confirm Receipt"
  },
  "address": {
    "title": "Shipping Address",
    "name": "Recipient",
    "phone": "Phone",
    "region": "Region",
    "detail": "Detailed Address",
    "default": "Set as Default",
    "add": "Add Address",
    "edit": "Edit Address",
    "deleteConfirm": "Are you sure to delete this address?"
  },
  "pay": {
    "title": "Confirm Payment",
    "orderInfo": "Order Info",
    "payAmount": "Payment Amount",
    "payMethod": "Payment Method",
    "wxPay": "WeChat Pay",
    "paySuccess": "Payment Successful",
    "payFailed": "Payment Failed"
  },
  "profile": {
    "myOrders": "My Orders",
    "myAddress": "My Address",
    "contactService": "Contact Service",
    "settings": "Settings",
    "logout": "Logout"
  },
  "announcement": {
    "title": "Announcements",
    "detail": "Announcement Details",
    "type": {
      "system": "System",
      "activity": "Activity",
      "update": "Update"
    },
    "unread": "Unread",
    "noData": "No announcements",
    "publishedAt": "Published At",
    "attachment": "Attachment",
    "download": "Download",
    "markRead": "Mark as Read",
    "pullRefresh": "Pull to refresh",
    "loadMore": "Load more",
    "noMore": "No more data"
  }
}

const BASELINE: Record<MiniLocale, Record<string, unknown>> = {
  'zh-CN': { common: zhCommon },
  'en-US': { common: enCommon },
}

let currentLocale: MiniLocale = 'zh-CN'
let merged: Record<string, unknown> = {}

function deepMerge(target: Record<string, unknown>, source: Record<string, unknown>): Record<string, unknown> {
  for (const key of Object.keys(source)) {
    const sv = source[key]
    const tv = target[key]
    if (sv && typeof sv === 'object' && !Array.isArray(sv) && tv && typeof tv === 'object') {
      target[key] = deepMerge({ ...(tv as Record<string, unknown>) }, sv as Record<string, unknown>)
    } else {
      target[key] = sv
    }
  }
  return target
}

function lookup(obj: Record<string, unknown>, keyPath: string): string {
  const parts = keyPath.split('.')
  let cur: unknown = obj
  for (const p of parts) {
    if (!cur || typeof cur !== 'object') return keyPath
    cur = (cur as Record<string, unknown>)[p]
  }
  return typeof cur === 'string' ? cur : keyPath
}

export function getLocale(): MiniLocale {
  return currentLocale
}

/** 取文案：key 形如 common.button.confirm */
export function t(key: string): string {
  return lookup(merged, key)
}

/** 启动时调用：系统语言 → 缓存远程包 → 内置基线合并远程覆盖 */
export async function initI18n(): Promise<void> {
  const sysInfo = wx.getSystemInfoSync()
  const sys = sysInfo.language || 'zh_CN'
  currentLocale = sys.toLowerCase().includes('en') ? 'en-US' : 'zh-CN'
  merged = deepMerge({}, BASELINE[currentLocale])

  const cacheKey = 'remote:' + currentLocale
  const cached = getLocaleCache<Record<string, unknown>>(cacheKey)
  if (cached) merged = deepMerge(merged, cached)

  try {
    const remote = await new Promise<Record<string, unknown>>((resolve, reject) => {
      wx.request({
        // 语言包托管在 /api/i18n（非 /api/app 分区），mock/生产同源
        url: 'http://localhost:7700/api/i18n/' + currentLocale + '/common.json',
        success: (res) => resolve(res.data as Record<string, unknown>),
        fail: reject,
      })
    })
    merged = deepMerge(merged, { common: remote })
    setLocaleCache(cacheKey, { common: remote })
  } catch {
    // 离线：内置基线 + 上次缓存
  }
}
