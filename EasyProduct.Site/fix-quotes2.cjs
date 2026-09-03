const fs=require('fs');
const p='src/components/layout/MobileNavDrawer.vue';
let c=fs.readFileSync(p,'utf8');
// Fix all remaining issues - use string replacement
c=c.replace("from '\\'vue-i18n","from 'vue-i18n");
c=c.replace(/{ path: '\\''\\//g,"{ path: \"/");
c=c.replace(/titleKey: '\\'common\.nav\./g,"titleKey: 'common.nav.");
c=c.replace(/emit\\('\\'close'\\'\\)/g,"emit('close')");
c=c.replace("@use '\\'@/assets","@use '@/assets");
c=c.replace(/'\\]'\\''/g,"']");
fs.writeFileSync(p,c);
console.log('Fixed MobileNavDrawer.vue (2)');
