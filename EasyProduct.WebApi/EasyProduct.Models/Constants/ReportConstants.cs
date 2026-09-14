using System.ComponentModel;

namespace EasyProduct.Models.Constants
{
    /// <summary>
    /// 报表模块常量
    /// </summary>
    public static class ReportConstants
    {
        /// <summary>
        /// 数据源状态
        /// </summary>
        public static class DatasourceStatus
        {
            /// <summary>已连接</summary>
            [Description("已连接")]
            public const int Connected = 1;

            /// <summary>连接错误</summary>
            [Description("连接错误")]
            public const int Error = 2;
        }

        /// <summary>
        /// 报表状态
        /// </summary>
        public static class ReportStatus
        {
            /// <summary>草稿</summary>
            [Description("草稿")]
            public const int Draft = 1;

            /// <summary>已发布</summary>
            [Description("已发布")]
            public const int Published = 2;

            /// <summary>已归档</summary>
            [Description("已归档")]
            public const int Archived = 3;
        }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public static class DatasourceType
        {
            /// <summary>MySQL 数据库</summary>
            [Description("MySQL")]
            public const string MySql = "mysql";

            /// <summary>PostgreSQL 数据库</summary>
            [Description("PostgreSQL")]
            public const string PostgreSql = "postgresql";

            /// <summary>SQL Server 数据库</summary>
            [Description("SQL Server")]
            public const string SqlServer = "sqlserver";

            /// <summary>Oracle 数据库</summary>
            [Description("Oracle")]
            public const string Oracle = "oracle";
        }

        /// <summary>
        /// 图表类型
        /// </summary>
        public static class ChartType
        {
            /// <summary>表格</summary>
            [Description("表格")]
            public const string Table = "table";

            /// <summary>折线图</summary>
            [Description("折线图")]
            public const string Line = "line";

            /// <summary>柱状图</summary>
            [Description("柱状图")]
            public const string Bar = "bar";

            /// <summary>饼图</summary>
            [Description("饼图")]
            public const string Pie = "pie";
        }
    }
}