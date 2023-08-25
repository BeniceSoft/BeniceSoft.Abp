using System.Collections.Concurrent;
using System.Reflection;

namespace BeniceSoft.Abp.Extensions.Caching.Extensions;

public static class TypeExtensions
{
    private static readonly Type[] VoidTypes =
    {
        typeof(void),
        typeof(Task),
        typeof(ValueTask)
    };

    private static readonly ConcurrentDictionary<TypeInfo, bool> IsTaskOfTCache =
        new ConcurrentDictionary<TypeInfo, bool>();

    private static readonly ConcurrentDictionary<TypeInfo, bool> IsValueTaskOfTCache =
        new ConcurrentDictionary<TypeInfo, bool>();

    private static readonly Type VoidTaskResultType = Type.GetType("System.Threading.Tasks.VoidTaskResult", false)!;

    public static object GetDefaultValue(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        if (typeInfo.AsType() == typeof(void))
        {
            return null;
        }

        switch (Type.GetTypeCode(typeInfo.AsType()))
        {
            case TypeCode.Object:
            case TypeCode.DateTime:
                if (typeInfo.IsValueType)
                {
                    return Activator.CreateInstance(typeInfo.AsType());
                }
                else
                {
                    return null;
                }

            case TypeCode.Empty:
            case TypeCode.String:
                return null;

            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
                return 0;

            case TypeCode.Int64:
            case TypeCode.UInt64:
                return 0;

            case TypeCode.Single:
                return default(Single);

            case TypeCode.Double:
                return default(Double);

            case TypeCode.Decimal:
                return new Decimal(0);

            default:
                throw new InvalidOperationException("Code supposed to be unreachable.");
        }
    }

    public static bool IsTask(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return typeInfo.AsType() == typeof(Task);
    }

    public static bool IsTaskWithResult(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return IsTaskOfTCache.GetOrAdd(typeInfo,
            Info => Info.IsGenericType && typeof(Task).GetTypeInfo().IsAssignableFrom(Info));
    }

    public static bool IsValueTask(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return typeInfo.AsType() == typeof(ValueTask);
    }

    public static bool IsValueTaskWithResult(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return IsValueTaskOfTCache.GetOrAdd(typeInfo,
            Info => Info.IsGenericType && Info.GetGenericTypeDefinition() == typeof(ValueTask<>));
    }

    public static bool IsVoidType(this Type type)
    {
        return VoidTypes.Contains(type);
    }
    
    public static bool IsTaskWithVoidTaskResult(this TypeInfo typeInfo)
    {
        if (typeInfo == null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        return typeInfo.GenericTypeArguments?.Length > 0 && typeInfo.GenericTypeArguments[0] == VoidTaskResultType;
    }
}