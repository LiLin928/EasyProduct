const { chromium } = require('playwright');

const urls = [
  { name: 'news', url: 'http://localhost:5174/news' },
  { name: 'products', url: 'http://localhost:5174/products' },
  { name: 'videos', url: 'http://localhost:5174/videos' },
  { name: 'downloads', url: 'http://localhost:5174/downloads' }
];

async function extractStyles(page, pageName) {
  const styles = await page.evaluate((name) => {
    const data = { pageName: name, url: window.location.href };
    
    // 页面头部区域样式
    const header = document.querySelector('.page-header, .header, [class*="header"], [class*="banner"]');
    if (header) {
      const headerStyles = window.getComputedStyle(header);
      data.header = {
        selector: header.className || header.tagName,
        backgroundColor: headerStyles.backgroundColor,
        color: headerStyles.color,
        padding: headerStyles.padding,
        height: headerStyles.height,
        minHeight: headerStyles.minHeight
      };
    }
    
    // 页面标题样式
    const title = document.querySelector('h1, .page-title, [class*="title"], .el-page-header__title');
    if (title) {
      const titleStyles = window.getComputedStyle(title);
      data.title = {
        selector: title.className || title.tagName,
        text: title.textContent?.substring(0, 50),
        color: titleStyles.color,
        fontSize: titleStyles.fontSize,
        fontWeight: titleStyles.fontWeight
      };
    }
    
    // 搜索框样式
    const searchInput = document.querySelector('.search-input, input[type="search"], .el-input__inner, [class*="search"] input');
    if (searchInput) {
      const searchStyles = window.getComputedStyle(searchInput);
      data.searchInput = {
        selector: searchInput.className || searchInput.tagName,
        backgroundColor: searchStyles.backgroundColor,
        border: searchStyles.border,
        borderRadius: searchStyles.borderRadius,
        height: searchStyles.height
      };
    }
    
    // 卡片样式
    const cards = document.querySelectorAll('.el-card, [class*="card"], .news-item, .product-item');
    if (cards.length > 0) {
      const cardStyles = window.getComputedStyle(cards[0]);
      data.card = {
        count: cards.length,
        selector: cards[0].className,
        backgroundColor: cardStyles.backgroundColor,
        border: cardStyles.border,
        borderRadius: cardStyles.borderRadius,
        boxShadow: cardStyles.boxShadow
      };
    }
    
    // 主色调提取
    const primaryBtns = document.querySelectorAll('.el-button--primary');
    if (primaryBtns.length > 0) {
      const btnStyle = window.getComputedStyle(primaryBtns[0]);
      data.primaryColor = btnStyle.backgroundColor;
    }
    
    // 页面背景色
    const bodyStyles = window.getComputedStyle(document.body);
    data.pageBackground = bodyStyles.backgroundColor;
    
    // 布局容器
    const container = document.querySelector('.container, .page-container, main');
    if (container) {
      const containerStyles = window.getComputedStyle(container);
      data.container = {
        selector: container.className,
        maxWidth: containerStyles.maxWidth,
        padding: containerStyles.padding
      };
    }
    
    return data;
  }, pageName);
  
  return styles;
}

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
  const page = await context.newPage();
  
  const results = {};
  
  for (const urlInfo of urls) {
    try {
      console.log('Analyzing:', urlInfo.url);
      await page.goto(urlInfo.url, { waitUntil: 'networkidle', timeout: 30000 });
      await page.waitForTimeout(2000);
      
      const styles = await extractStyles(page, urlInfo.name);
      
      // 截图保存
      const screenshotPath = `D:/4-MyProject/EasyProduct/page_${urlInfo.name}_screenshot.png`;
      await page.screenshot({ path: screenshotPath, fullPage: true });
      console.log('Screenshot saved:', screenshotPath);
      
      results[urlInfo.name] = { success: true, data: styles, screenshot: screenshotPath };
    } catch (error) {
      console.error('Error with', urlInfo.name, ':', error.message);
      results[urlInfo.name] = { success: false, error: error.message };
    }
  }
  
  await browser.close();
  console.log('\n===RESULTS===');
  console.log(JSON.stringify(results, null, 2));
})();
