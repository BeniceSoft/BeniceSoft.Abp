namespace BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions;

public static class DynamicQueryConstants
{
    /// <summary>
    /// 操作符
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// 等于
        /// </summary>
        public const string Equal = "equal";

        /// <summary>
        /// 不等于
        /// </summary>
        public const string NotEqual = "not_equal";

        /// <summary>
        /// 大于
        /// </summary>
        public const string GreaterThan = "greater_than";

        /// <summary>
        /// 大于等于
        /// </summary>
        public const string GreaterThanOrEqual = "greater_than_or_equal";

        /// <summary>
        /// 小于
        /// </summary>
        public const string LessThan = "less_than";

        /// <summary>
        /// 小于等于
        /// </summary>
        public const string LessThanOrEqual = "less_than_or_equal";

        /// <summary>
        /// 包含
        /// </summary>
        public const string Contains = "contains";

        /// <summary>
        /// 开头是
        /// </summary>
        public const string StartsWith = "starts_with";

        /// <summary>
        /// 结尾是
        /// </summary>
        public const string EndsWith = "ends_with";

        /// <summary>
        /// 不包含
        /// </summary>
        public const string NotContains = "not_contains";

        /// <summary>
        /// 介于
        /// </summary>
        public const string Between = "between";

        /// <summary>
        /// 在
        /// </summary>
        public const string In = "in";

        /// <summary>
        /// 不在
        /// </summary>
        public const string NotIn = "not_in";
    }

    /// <summary>
    /// 类型
    /// </summary>
    public static class TypeNames
    {
        public const string Integer = "integer";

        public const string Double = "double";

        public const string String = "string";

        public const string Date = "date";

        public const string DateTime = "datetime";

        public const string Boolean = "boolean";

        public const string Guid = "guid";
    }

    /// <summary>
    /// 关系
    /// </summary>
    public static class Relations
    {
        public const string And = "and";
        public const string Or = "or";
    }
}