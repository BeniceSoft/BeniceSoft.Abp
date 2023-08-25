namespace BeniceSoft.Abp.Auth.Core.Models;

public class RowPermissionCondition
{
    /// <summary>
    /// �Ƿ񳬼�����Ա
    /// </summary>
    public bool IsDataSuperAdmin { get; set; }

    /// <summary>
    /// �߼���
    /// and or
    /// </summary>
    public string LogicalOperator { get; set; }

    /// <summary>
    /// �ֶ���
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// ������
    /// </summary>
    public string Operator { get; set; }

    /// <summary>
    /// ֵ
    /// </summary>
    public List<string> Values { get; set; }
}