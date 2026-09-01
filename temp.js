const fs = require('fs');  
const path = 'EasyProduct.Admin/src/views/workflow/publish/index.vue';  
let content = fs.readFileSync(path, 'utf-8');  
  
if (!content.includes('Á÷³ÌÉèÖÃ')) {  
