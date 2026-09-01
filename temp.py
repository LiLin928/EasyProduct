import re 
path=r'EasyProduct.Admin/src/views/workflow/publish/index.vue' 
with open(path,'r',encoding='utf-8') as f: 
    c=f.read() 
if 'handleDesign(row)' not in c: 
