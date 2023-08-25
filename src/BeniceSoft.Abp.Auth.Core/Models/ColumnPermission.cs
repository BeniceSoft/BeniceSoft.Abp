namespace BeniceSoft.Abp.Auth.Core.Models
{
    public class ColumnPermission
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列权限等级
        /// 0：隐藏  1：只读  2：读写
        /// </summary>
        public int ColumnAuthLevel { get; set; }

        /// <summary>
        /// 当前列是否显示
        /// </summary>
        public bool IsDisplay { get; set; }
    }
}
