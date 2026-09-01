const fs = require('fs');
const path = 'D:\\\\4-MyProject\\\\EasyProduct\\\\EasyProduct.Admin\\\\src\\\\views\\\\workflow\\\\publish\\\\index.vue';

let content = fs.readFileSync(path, 'utf-8');

content = content.replace(
  'router.push(\\\\/workflow/designer\\\\/ + row.id)',
  'router.push(\/workflow/designer/\\)'
);

content = content.replace(
