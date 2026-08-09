import { t } from '../../utils/i18n'

Page({
  data: { title: '' },
  onShow() {
    this.setData({ title: t('common.tab.profile') })
  },
})
