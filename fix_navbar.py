import codecs
with codecs.open('EasyProduct.Site/src/components/layout/AppNavbar.vue', 'r', encoding='utf-8') as f:
    c = f.read()
c = c.replace(chr(63)+chr(32)+chr(39)+'EN'+chr(39)+chr(32)+chr(58)+chr(32)+chr(39)+'中文'+chr(39), chr(63)+chr(32)+chr(116)+chr(40)+chr(39)+'common.locale.enUS'+chr(39)+chr(41)+chr(32)+chr(58)+chr(32)+chr(116)+chr(40)+chr(39)+'common.locale.zhCN'+chr(39)+chr(41))
with codecs.open('EasyProduct.Site/src/components/layout/AppNavbar.vue', 'w', encoding='utf-8') as f:
    f.write(c)
print('Done')
