using Bogus;
using SqlSugar;
using EasyProduct.Models.Entitys.Mall;
using EasyProduct.Models.Entitys.Product;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Enums.Mall;

namespace EasyProduct.Tests.TestData;

/// <summary>
/// 测试数据工厂，简化测试数据创建
/// </summary>
/// <remarks>
/// 提供以下功能：
/// 1. 使用 Bogus 生成随机测试数据
/// 2. 支持创建各种实体对象
/// 3. 支持批量创建测试数据
/// 4. 提供常用的默认值和规则
///
/// 使用示例：
/// <code>
/// public class OrderServiceTests : TestBase
/// {
///     [Fact]
///     public async Task CreateOrder_ShouldReturnOrder()
///     {
///         // 创建测试会员
///         var member = _dataFactory.CreateMember();
///
///         // 创建测试订单
///         var order = _dataFactory.CreateOrder(member.Id.ToString());
///
///         // 插入数据库
///         await _db.Insertable(order).ExecuteCommandAsync();
///     }
/// }
/// </code>
/// </remarks>
public class TestDataFactory
{
    private readonly ISqlSugarClient _db;
    private readonly Faker _faker = new Faker("zh_CN");

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="db">SqlSugar 数据库客户端</param>
    public TestDataFactory(ISqlSugarClient db)
    {
        _db = db;
    }

    #region 会员相关

    /// <summary>
    /// 创建测试会员实体
    /// </summary>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>会员实体对象</returns>
    public Member CreateMember(Action<Member>? overrides = null)
    {
        var member = new Faker<Member>("zh_CN")
            .RuleFor(m => m.OpenId, f => f.Random.Guid().ToString())
            .RuleFor(m => m.Nickname, f => f.Name.FirstName())
            .RuleFor(m => m.Avatar, f => f.Internet.Avatar())
            .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("1##########"))
            .RuleFor(m => m.Email, f => f.Internet.Email())
            .RuleFor(m => m.RealName, f => f.Name.FullName())
            .RuleFor(m => m.Balance, f => f.Finance.Amount(0, 10000))
            .RuleFor(m => m.Points, f => f.Random.Int(0, 10000))
            .Generate();

        overrides?.Invoke(member);
        return member;
    }

    /// <summary>
    /// 批量创建测试会员实体
    /// </summary>
    /// <param name="count">创建数量</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>会员实体列表</returns>
    public List<Member> CreateMembers(int count, Action<Member>? overrides = null)
    {
        var members = new List<Member>();
        for (int i = 0; i < count; i++)
        {
            members.Add(CreateMember(overrides));
        }
        return members;
    }

    #endregion

    #region 订单相关

    /// <summary>
    /// 创建测试订单实体
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>订单实体对象</returns>
    public Order CreateOrder(string memberId, Action<Order>? overrides = null)
    {
        var order = new Faker<Order>("zh_CN")
            .RuleFor(o => o.OrderNo, f => GenerateOrderNo())
            .RuleFor(o => o.MemberId, memberId)
            .RuleFor(o => o.ReceiverName, f => f.Name.FullName())
            .RuleFor(o => o.ReceiverPhone, f => f.Phone.PhoneNumber("1##########"))
            .RuleFor(o => o.ReceiverAddress, f => f.Address.FullAddress())
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount(10, 10000))
            .RuleFor(o => o.FreightAmount, f => f.Finance.Amount(0, 50))
            .RuleFor(o => o.DiscountAmount, f => f.Finance.Amount(0, 100))
            .RuleFor(o => o.PayAmount, (f, o) => o.TotalAmount + o.FreightAmount - o.DiscountAmount)
            .RuleFor(o => o.Status, OrderStatus.PendingPayment)
            .RuleFor(o => o.Source, f => f.PickRandom(new[] { "cart", "direct" }))
            .Generate();

        overrides?.Invoke(order);
        return order;
    }

    /// <summary>
    /// 批量创建测试订单实体
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="count">创建数量</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>订单实体列表</returns>
    public List<Order> CreateOrders(string memberId, int count, Action<Order>? overrides = null)
    {
        var orders = new List<Order>();
        for (int i = 0; i < count; i++)
        {
            orders.Add(CreateOrder(memberId, overrides));
        }
        return orders;
    }

    #endregion

    #region 支付相关

    /// <summary>
    /// 创建测试支付记录实体
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="memberId">会员ID</param>
    /// <param name="amount">支付金额</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>支付记录实体对象</returns>
    public Payment CreatePayment(string orderId, string memberId, decimal amount, Action<Payment>? overrides = null)
    {
        var payment = new Faker<Payment>("zh_CN")
            .RuleFor(p => p.PaymentNo, f => GeneratePaymentNo())
            .RuleFor(p => p.OrderId, orderId)
            .RuleFor(p => p.MemberId, memberId)
            .RuleFor(p => p.Amount, amount)
            .RuleFor(p => p.PaymentMethod, PaymentMethod.WechatPay)
            .RuleFor(p => p.PaymentChannel, f => f.PickRandom(new[] { "jsapi", "h5", "native", "app" }))
            .RuleFor(p => p.Status, PaymentStatus.Pending)
            .Generate();

        overrides?.Invoke(payment);
        return payment;
    }

    /// <summary>
    /// 批量创建测试支付记录实体
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="memberId">会员ID</param>
    /// <param name="amount">支付金额</param>
    /// <param name="count">创建数量</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>支付记录实体列表</returns>
    public List<Payment> CreatePayments(string orderId, string memberId, decimal amount, int count, Action<Payment>? overrides = null)
    {
        var payments = new List<Payment>();
        for (int i = 0; i < count; i++)
        {
            payments.Add(CreatePayment(orderId, memberId, amount, overrides));
        }
        return payments;
    }

    #endregion

    #region 商品相关

    /// <summary>
    /// 创建测试商品分类实体
    /// </summary>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>商品分类实体对象</returns>
    public product_category CreateCategory(Action<product_category>? overrides = null)
    {
        var category = new Faker<product_category>("zh_CN")
            .RuleFor(c => c.CategoryName, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.ParentId, "0")
            .RuleFor(c => c.CategoryCode, f => f.Commerce.Ean13())
            .RuleFor(c => c.Sort, f => f.Random.Int(0, 100))
            .RuleFor(c => c.Icon, f => f.Image.PicsumUrl())
            .Generate();

        overrides?.Invoke(category);
        return category;
    }

    /// <summary>
    /// 创建测试商品SPU实体
    /// </summary>
    /// <param name="categoryId">分类ID（字符串格式）</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>商品SPU实体对象</returns>
    public product_spu CreateSpu(string categoryId, Action<product_spu>? overrides = null)
    {
        var spu = new Faker<product_spu>("zh_CN")
            .RuleFor(s => s.SpuName, f => f.Commerce.ProductName())
            .RuleFor(s => s.SpuCode, f => f.Commerce.Ean13())
            .RuleFor(s => s.CategoryId, categoryId)
            .RuleFor(s => s.Brand, f => f.Company.CompanyName())
            .RuleFor(s => s.MainImage, f => f.Image.PicsumUrl())
            .RuleFor(s => s.Images, f => f.Image.PicsumUrl())
            .RuleFor(s => s.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(s => s.Unit, f => f.PickRandom(new[] { "件", "个", "台", "套" }))
            .Generate();

        overrides?.Invoke(spu);
        return spu;
    }

    /// <summary>
    /// 创建测试商品SKU实体
    /// </summary>
    /// <param name="spuId">SPU ID（字符串格式）</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>商品SKU实体对象</returns>
    public product_sku CreateSku(string spuId, Action<product_sku>? overrides = null)
    {
        var sku = new Faker<product_sku>("zh_CN")
            .RuleFor(s => s.SpuId, spuId)
            .RuleFor(s => s.SkuName, f => f.Commerce.ProductName())
            .RuleFor(s => s.SkuCode, f => f.Commerce.Ean13())
            .RuleFor(s => s.Price, f => f.Finance.Amount(10, 1000))
            .RuleFor(s => s.Stock, f => f.Random.Int(0, 500))
            .RuleFor(s => s.SpecJson, f => $"[{{\"name\":\"颜色\",\"value\":\"{f.Commerce.Color()}\"}}]")
            .Generate();

        overrides?.Invoke(sku);
        return sku;
    }

    #endregion

    #region 购物车相关

    /// <summary>
    /// 创建测试购物车实体
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <param name="skuId">SKU ID（字符串格式）</param>
    /// <param name="quantity">数量</param>
    /// <param name="overrides">覆盖默认值的属性</param>
    /// <returns>购物车实体对象</returns>
    public Cart CreateCart(string memberId, string skuId, int quantity = 1, Action<Cart>? overrides = null)
    {
        var cart = new Faker<Cart>("zh_CN")
            .RuleFor(c => c.MemberId, memberId)
            .RuleFor(c => c.SkuId, skuId)
            .RuleFor(c => c.Quantity, quantity)
            .RuleFor(c => c.Selected, 1)
            .Generate();

        overrides?.Invoke(cart);
        return cart;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 生成订单编号
    /// </summary>
    /// <returns>订单编号</returns>
    private string GenerateOrderNo() => $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{_faker.Random.Int(1000, 9999)}";

    /// <summary>
    /// 生成支付单号
    /// </summary>
    /// <returns>支付单号</returns>
    private string GeneratePaymentNo() => $"PAY{DateTime.UtcNow:yyyyMMddHHmmss}{_faker.Random.Int(1000, 9999)}";

    #endregion
}