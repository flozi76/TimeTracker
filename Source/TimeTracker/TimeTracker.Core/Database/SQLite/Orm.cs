namespace TimeTracker.Core.Database.SQLite
{
    using System;
    using System.Reflection;

    public static class Orm
    {
        public const int DefaultMaxStringLength = 140;

        public static string SqlDecl(TableMapping.Column p)
        {
            string decl = "\"" + p.Name + "\" " + SqlType(p) + " ";

            if (p.IsPK)
            {
                decl += "primary key ";
            }
            if (p.IsAutoInc)
            {
                decl += "autoincrement ";
            }
            if (!p.IsNullable)
            {
                decl += "not null ";
            }
            if (!string.IsNullOrEmpty(p.Collation))
            {
                decl += "collate " + p.Collation + " ";
            }

            return decl;
        }

        public static string SqlType(TableMapping.Column p)
        {
            var clrType = p.ColumnType;
            if (clrType == typeof(Boolean) || clrType == typeof(Byte) || clrType == typeof(UInt16) || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32))
            {
                return "integer";
            }
            else if (clrType == typeof(UInt32) || clrType == typeof(Int64))
            {
                return "bigint";
            }
            else if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal))
            {
                return "float";
            }
            else if (clrType == typeof(String))
            {
                int len = p.MaxStringLength;
                return "varchar(" + len + ")";
            }
            else if (clrType == typeof(DateTime))
            {
                return "datetime";
            }
            else if (clrType.IsEnum)
            {
                return "integer";
            }
            else if (clrType == typeof(byte[]))
            {
                return "blob";
            }
            else
            {
                throw new NotSupportedException("Don't know about " + clrType);
            }
        }

        public static bool IsPK(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            return attrs.Length > 0;
        }

        public static string Collation(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(CollationAttribute), true);
            if (attrs.Length > 0)
            {
                return ((CollationAttribute)attrs[0]).Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsAutoInc(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(AutoIncrementAttribute), true);
            return attrs.Length > 0;
        }

        public static bool IsIndexed(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(IndexedAttribute), true);
            return attrs.Length > 0;
        }

        public static int MaxStringLength(PropertyInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(MaxLengthAttribute), true);
            if (attrs.Length > 0)
            {
                return ((MaxLengthAttribute)attrs[0]).Value;
            }
            else
            {
                return DefaultMaxStringLength;
            }
        }
    }
}