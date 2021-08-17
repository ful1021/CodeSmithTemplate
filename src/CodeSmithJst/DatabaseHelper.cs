using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeSmith.Engine;
using SchemaExplorer;

namespace CodeSmithJst
{
    public class DatabaseHelper
    {
        #region 处理Table

        /// <summary>
        /// 得到[dbo].
        /// </summary>
        /// <param name="Table"></param>
        /// <returns></returns>
        public string GetTableOwner(TableSchema Table)
        {
            StringBuilder sb = new StringBuilder();
            if (Table.Owner.Length > 0)
            {
                sb.AppendFormat("[{0}].", Table.Owner);
            }
            else
            {
                return "";
            }
            return sb.ToString();
        }

        /// <summary>
        /// 得到表注释
        /// </summary>
        /// <param name="Table"></param>
        /// <returns></returns>
        public string GetTableDescription(TableSchema Table)
        {
            return Table.Description;
        }

        #endregion 处理Table

        #region 处理主键

        /// <summary>
        /// 得到主键的名字
        /// </summary>
        /// <returns></returns>
        public string GetPKName(TableSchema table)
        {
            return table.PrimaryKey.MemberColumns[0].Name;
        }

        /// <summary>
        /// 得到主键在C#中的类型
        /// </summary>
        /// <returns></returns>
        public string GetPKCSharpType(TableSchema table)
        {
            return GetCSharpTypeByDb(table.PrimaryKey.MemberColumns[0]);
        }

        /// <summary>
        /// 得到主键在SQL中的类型
        /// </summary>
        /// <returns></returns>
        public string GetPKSqlDbType(TableSchema table)
        {
            return GetSqlDbType(table.PrimaryKey.MemberColumns[0]);
        }

        /// <summary>
        /// 得到主键的长度
        /// </summary>
        /// <returns></returns>
        public int GetPKSize(TableSchema table)
        {
            return table.PrimaryKey.MemberColumns[0].Size;
        }

        #endregion 处理主键

        #region 处理列

        /// <summary>
        /// 判断当前列，是否名字为参数name，在C#中的类型是否为参数type
        /// </summary>
        /// <param name="col"></param>
        /// <param name="name">列名字字符串</param>
        /// <param name="type">列在C#中的类型字符串</param>
        /// <returns></returns>
        public bool IsEqualsCol(ColumnSchema col, string name, string type = null)
        {
            var nameEqual = col.Name.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(type))
            {
                return nameEqual;
            }
            return nameEqual && GetCSharpTypeByDb(col).Trim().Equals(type.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断当前表，是否存在列name
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="name">列名字字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="type">列在C#中的类型字符串</param>
        /// <returns></returns>
        public bool IsExistsColDb(TableSchema tab, string name, bool ignoreCase = false, string type = null)
        {
            var b = false;
            foreach (var col in tab.Columns)
            {
                if (ignoreCase)
                {
                    if (col.Name.Trim().ToLower() == name.Trim().ToLower())
                    {
                        b = true;
                    }
                }
                else
                {
                    if (col.Name.Trim() == name.Trim())
                    {
                        b = true;
                    }
                }
                if (b && !string.IsNullOrWhiteSpace(type))
                {
                    b = GetCSharpTypeByDb(col).Trim().Equals(type.Trim(), StringComparison.OrdinalIgnoreCase);
                }
                if (b)
                {
                    break;
                }
            }
            return b;
        }

        /// <summary>
        /// 是否为标识符，不支持Access
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsIdentity(ColumnSchema col)
        {
            return (bool)col.ExtendedProperties["CS_IsIdentity"].Value;
        }

        #endregion 处理列

        #region 类型处理

        /// <summary>
        /// 根据列得到转换为C#后，可空类型字符串 如：int?
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetCSharpNullableTypeByDb(ColumnSchema column)
        {
            if (column.Name.EndsWith("TypeCode")) return column.Name;
            switch (column.DataType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength: return "string";
                case DbType.Binary: return "byte[]";
                case DbType.Boolean:
                    if (column.AllowDBNull) { return "bool?"; }
                    else { return "bool"; }
                case DbType.Byte:
                    if (column.AllowDBNull) { return "byte?"; }
                    else { return "byte"; }
                case DbType.Currency:
                    if (column.AllowDBNull) { return "decimal?"; }
                    else { return "decimal"; }
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    if (column.AllowDBNull) { return "DateTime?"; }
                    else { return "DateTime"; }
                case DbType.Decimal:
                    if (column.AllowDBNull) { return "decimal?"; }
                    else { return "decimal"; }
                case DbType.Double:
                    if (column.AllowDBNull) { return "double?"; }
                    else { return "double"; }
                case DbType.Guid:
                    if (column.AllowDBNull) { return "Guid?"; }
                    else { return "Guid"; }
                case DbType.Int16:
                    if (column.AllowDBNull) { return "short?"; }
                    else { return "short"; }
                case DbType.Int32:
                    if (column.AllowDBNull) { return "int?"; }
                    else { return "int"; }
                case DbType.Int64:
                    if (column.AllowDBNull) { return "long?"; }
                    else { return "long"; }
                case DbType.Object: return "object";
                case DbType.SByte:
                    if (column.AllowDBNull) { return "sbyte?"; }
                    else { return "sbyte"; }
                case DbType.Single:
                    if (column.AllowDBNull) { return "float?"; }
                    else { return "float"; }
                case DbType.String:
                case DbType.StringFixedLength: return "string";
                case DbType.Time:
                    if (column.AllowDBNull) { return "TimeSpan?"; }
                    else { return "TimeSpan"; }
                case DbType.UInt16:
                    if (column.AllowDBNull) { return "ushort?"; }
                    else { return "ushort"; }
                case DbType.UInt32:
                    if (column.AllowDBNull) { return "uint?"; }
                    else { return "uint"; }
                case DbType.UInt64:
                    if (column.AllowDBNull) { return "ulong?"; }
                    else { return "ulong"; }
                case DbType.VarNumeric:
                    if (column.Scale > 0)
                    {
                        if (column.AllowDBNull) { return "double?"; }
                        else { return "double"; }
                    }
                    else if (column.Scale == 0)
                    {
                        if (column.AllowDBNull) { return "int?"; }
                        else { return "int"; }
                    }
                    else
                    {
                        if (column.AllowDBNull) { return "decimal?"; }
                        else { return "decimal"; }
                    }
                case DbType.Xml: return "Xml";
                default:
                    return column.SystemType.ToString().Substring("System.".Length);
            }
        }

        /// <summary>
        /// 根据列得到转换为C#后类型字符串
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetCSharpTypeByDb(ColumnSchema column)
        {
            //if (column.Name.EndsWith("TypeCode")) return column.Name;
            //获取oracle数据库的属性是NativeType属性，它是一个string类型的结果，将会是比如clob,number,timestamp(6),这样的，
            //由于oracle数据库一般用number类型，不用int类型，因此当自定义类转换为具体类型时，要参考ColumnSchema对象的Precision, Scale, Size来判断了。
            //其中scale是精确到小数点后的位数，如number(6,2),则scale属性为2，size是用于计算Nvarchar2这种类型的长度，
            //如nvarchar2(100)，则size属性为100，precision为小数点前精确位数，如number(6,2),则precision属性为6，但是timestamp这几个属性没有值，
            //它是直接从NativeType里看的出来时间长度，可能为3,6,9，默认为6，NativeType值将为timestamp(6)。
            switch (column.DataType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength: return "string";
                case DbType.Binary: return "byte[]";
                case DbType.Boolean: return "bool";
                case DbType.Byte: return "byte";
                case DbType.Currency: return "decimal";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset: return "DateTime";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "double";
                case DbType.Guid: return "Guid";
                case DbType.Int16: return "short";
                case DbType.Int32: return "int";
                case DbType.Int64: return "long";
                case DbType.Object: return "object";
                case DbType.SByte: return "sbyte";
                case DbType.Single: return "float";
                case DbType.String:
                case DbType.StringFixedLength: return "string";
                case DbType.Time: return "TimeSpan";
                case DbType.UInt16: return "ushort";
                case DbType.UInt32: return "uint";
                case DbType.UInt64: return "ulong";
                case DbType.VarNumeric:
                    if (column.Scale > 0)
                    {
                        return "double";
                    }
                    else if (column.Scale == 0)
                    {
                        return "int";
                    }
                    else
                    {
                        return "decimal";
                    }
                case DbType.Xml: return "Xml";
                default:
                    return column.SystemType.ToString().Substring("System.".Length);
            }
        }

        /// <summary>
        /// 根据列得到转换为Sql数据库中的类型
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetSqlDbType(ColumnSchema column)
        {
            switch (column.NativeType)
            {
                case "bigint": return "BigInt";
                case "binary": return "Binary";
                case "bit": return "Bit";
                case "char": return "Char";
                case "datetime": return "DateTime";
                case "decimal": return "Decimal";
                case "float": return "Float";
                case "image": return "Image";
                case "int": return "Int";
                case "money": return "Money";
                case "nchar": return "NChar";
                case "ntext": return "NText";
                case "numeric": return "Decimal";
                case "nvarchar": return "NVarChar";
                case "real": return "Real";
                case "smalldatetime": return "SmallDateTime";
                case "smallint": return "SmallInt";
                case "smallmoney": return "SmallMoney";
                case "sql_variant": return "Variant";
                case "sysname": return "NChar";
                case "text": return "Text";
                case "timestamp": return "Timestamp";
                case "tinyint": return "TinyInt";
                case "uniqueidentifier": return "UniqueIdentifier";
                case "varbinary": return "VarBinary";
                case "varchar": return "VarChar";
                default: return "__UNKNOWN__" + column.NativeType;
            }
        }

        /// <summary>
        /// 根据列得到转换为MySql数据库中的类型
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetMySqlDbType(ColumnSchema column)
        {
            switch (column.NativeType)
            {
                case "bigint": return "BigInt";
                case "binary": return "Binary";
                case "bit": return "Bit";
                case "char": return "Char";
                case "date": return "Date";
                case "datetime": return "DateTime";
                case "decimal": return "Decimal";
                case "float": return "Float";
                case "image": return "Image";
                case "int": return "Int32";
                case "money": return "Money";
                case "nchar": return "NChar";
                case "ntext": return "NText";
                case "numeric": return "Decimal";
                case "nvarchar": return "NVarChar";
                case "real": return "Real";
                case "smalldatetime": return "SmallDateTime";
                case "smallint": return "SmallInt";
                case "smallmoney": return "SmallMoney";
                case "sql_variant": return "Variant";
                case "sysname": return "NChar";
                case "text": return "Text";
                case "timestamp": return "Timestamp";
                case "tinyint": return "TinyInt";
                case "uniqueidentifier": return "UniqueIdentifier";
                case "varbinary": return "VarBinary";
                case "varchar": return "VarChar";
                case "blob": return "Blob";
                case "double": return "Double";
                default: return "__UNKNOWN__" + column.NativeType;
            }
        }

        /// <summary>
        /// 根据列得到转换为Java后类型字符串（暂不完善）
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetJavaType(ColumnSchema column)
        {
            if (GetPKName(column.Table).Trim() == column.Name)
            {
                return "Long";
            }
            if (column.Name.EndsWith("TypeCode")) return column.Name;
            switch (column.DataType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength: return "String";
                case DbType.Binary: return "byte[]";
                case DbType.Boolean: return "boolean";
                case DbType.Byte: return "byte";
                case DbType.Currency: return "decimal";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset: return "java.util.Date";
                case DbType.Decimal: return "decimal";
                case DbType.Double: return "Double";
                case DbType.Guid: return "Guid";
                case DbType.Int16: return "short";
                case DbType.Int32: return "Integer";
                case DbType.Int64: return "Long";
                case DbType.Object: return "object";
                case DbType.SByte: return "sbyte";
                case DbType.Single: return "float";
                case DbType.String:
                case DbType.StringFixedLength: return "String";
                case DbType.Time: return "TimeSpan";
                case DbType.UInt16: return "ushort";
                case DbType.UInt32: return "uint";
                case DbType.UInt64: return "ulong";
                case DbType.VarNumeric:
                    if (column.Scale > 0)
                    {
                        return "Double";
                    }
                    else if (column.Scale == 0)
                    {
                        return "Integer";
                    }
                    else
                    {
                        return "decimal";
                    }
                case DbType.Xml: return "Xml";
                default:
                    return column.SystemType.ToString().Substring("System.".Length);
            }
        }

        /// <summary>
        /// 根据列名判断该列类型是否为数据库类型
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IsUserDefinedType(DataObjectBase column)
        {
            switch (column.NativeType.Trim().ToLower())
            {
                case "bigint":
                case "binary":
                case "bit":
                case "char":
                case "date":
                case "datetime":
                case "datetime2":
                case "time":
                case "decimal":
                case "float":
                case "image":
                case "int":
                case "money":
                case "nchar":
                case "ntext":
                case "numeric":
                case "nvarchar":
                case "real":
                case "smalldatetime":
                case "smallint":
                case "smallmoney":
                case "sql_variant":
                case "sysname":
                case "text":
                case "timestamp":
                case "tinyint":
                case "uniqueidentifier":
                case "varbinary":
                case "xml":
                case "varchar":
                    return false;
            }
            return true;
        }

        #endregion 类型处理
    }
}
