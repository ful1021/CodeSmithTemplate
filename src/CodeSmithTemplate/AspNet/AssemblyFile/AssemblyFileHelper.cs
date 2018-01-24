using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSmithTemplate.AspNet.AssemblyFile
{
    public class AssemblyFileHelper
    {
        #region GetSummary
        public static Dictionary<PropertyInfo, string> GetPropertiesSummary(PropertyInfo[] props)
        {
            Dictionary<PropertyInfo, string> dict = new Dictionary<PropertyInfo, string>();
            foreach (var item in props)
            {
                dict[item] = GetPropertySummary(item);
            }
            return dict;
        }

        /// <summary>
        /// 得到反射字段 注释
        /// </summary>
        /// <returns></returns>
        public static string GetPropertySummary(PropertyInfo item)
        {
            string name = item.Name;
            var textInfo = blqw.Reflection.CSCommentReader.Create(item);
            var text = textInfo != null ? textInfo.Summary : "";
            if (name == "CreationTime")
            {
                text = "创建时间";
            }
            else if (name == "CreatorUserId")
            {
                text = "创建人";
            }
            else if (name == "LastModificationTime")
            {
                text = "上次修改时间";
            }
            else if (name == "LastModifierUserId")
            {
                text = "上次修改人";
            }

            return text;
        }

        /// <summary>
        /// 获取类注释
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetClassSummary(Type type)
        {
            if (type != null)
            {
                var info = blqw.Reflection.CSCommentReader.Create(type);
                if (info != null)
                {
                    return info.Summary.Replace("\r", "").Replace("\n", "");
                }
            }
            return "";
        }
        #endregion

        #region Assembly

        /// <summary>
        /// 根据dll文件，获取得到 Assembly
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string dllFile)
        {
            //byte[] filedata = System.IO.File.ReadAllBytes(dllFile);
            //Assembly assembly = Assembly.Load(filedata);
            //LoadFrom 会使文件 占用不释放
            Assembly assembly = Assembly.LoadFrom(dllFile);
            return assembly;
        }

        /// <summary>
        /// 获取反射的Assembly 类型
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetAssemblyType(string dllFile, string className)
        {
            Assembly assembly = GetAssembly(dllFile);
            var type = assembly.GetTypes().FirstOrDefault(a => a.Name == className);
            return type;
        }

        #endregion Assembly

        #region 列

        public static PropertyInfo[] GetPropertiesByDll(string dllFile, string className)
        {
            var type = GetAssemblyType(dllFile, className);
            return GetProperties(type);
        }

        /// <summary>
        /// 反射获取PropertyInfo集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(Type type)
        {
            if (type == null)
            {
                return new PropertyInfo[0];
            }
            PropertyInfo[] propertyinfo = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return propertyinfo;
        }

        /// <summary>
        /// 默认Dto列
        /// </summary>
        /// <param name="entityColumns"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetDtoProperties(PropertyInfo[] entityColumns)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in entityColumns)
            {
                if (CommonCode.IsIn(col.Name, "Id"))
                {
                    continue;
                }
                if (CommonCode.IsAbpCreationAudited(col))
                {
                    continue;
                }
                if (CommonCode.IsList(col))
                {
                    continue;
                }
                list.Add(col);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 所有枚举列
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetEnumProperties(PropertyInfo[] props)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in props)
            {
                if (col.PropertyType.IsEnum)
                {
                    list.Add(col);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 所有枚举列
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetDateTimeProperties(PropertyInfo[] props)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in props)
            {
                if (GetCSharpNullableTypeByProp(col).Contains("DateTime"))
                {
                    list.Add(col);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// GetAllInput 输入参数 列
        /// </summary>
        /// <param name="entityColumns"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetAllInputColumns(PropertyInfo[] entityColumns)
        {
            var dtos = GetDtoProperties(entityColumns);
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var col in dtos)
            {
                if (CommonCode.IsAbpValueObject(col))
                {
                    continue;
                }
                list.Add(col);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 得到反射字段 类型
        /// </summary>
        /// <returns></returns>
        public static string GetPropertyType(Type type, string propertyName = "Id")
        {
            var props = GetProperties(type);
            var propInfo = props.FirstOrDefault(a => a.Name == propertyName);
            if (propInfo != null)
            {
                return GetCSharpTypeByProp(propInfo);
            }
            return "";
        }

        /// <summary>
        /// 得到反射字段 类型
        /// </summary>
        /// <returns></returns>
        public static string GetPropertyDefaultValueString(Type type, string propertyName = "Id")
        {
            var props = GetProperties(type);
            var propInfo = props.FirstOrDefault(a => a.Name == propertyName);
            if (propInfo != null)
            {
                var ctype = GetCSharpTypeByProp(propInfo);
                if (ctype.Equals("Guid", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "Guid.Empty";
                }
                else
                {
                    return "0";
                }
            }
            return "";
        }

        /// <summary>
        /// 判断当前表，是否存在列name
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="name">列名字字符串</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="type">列在C#中的类型字符串</param>
        /// <returns></returns>
        public static bool IsExistsColByProp(PropertyInfo[] entityColumns, string name, bool ignoreCase = false, string type = null)
        {
            var b = false;
            foreach (var col in entityColumns)
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
                    b = GetCSharpTypeByProp(col).Trim().Equals(type.Trim(), StringComparison.OrdinalIgnoreCase);
                }
                if (b)
                {
                    break;
                }
            }
            return b;
        }

        #endregion 列

        #region 类型处理

        /// <summary>
        /// 根据列得到转换为C#后，可空类型字符串 如：int?
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static string GetCSharpNullableTypeByProp(PropertyInfo prop)
        {
            var propTypeFullName = prop.PropertyType.FullName;
            var type = GetCSharpTypeByPropertyTypeFullName(propTypeFullName);
            if (type.Contains("Nullable"))
            {
                if (propTypeFullName.Contains("Int32"))
                {
                    return "int?";
                }
                else if (propTypeFullName.Contains("Int64"))
                {
                    return "long?";
                }
                else if (propTypeFullName.Contains("DateTime"))
                {
                    return "DateTime?";
                }
                else if (propTypeFullName.Contains("Boolean"))
                {
                    return "bool?";
                }
                else if (propTypeFullName.Contains("Guid"))
                {
                    return "Guid?";
                }
            }
            return type;
        }

        /// <summary>
        /// 根据列得到转换为C#后类型字符串
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static string GetCSharpTypeByProp(PropertyInfo prop)
        {
            return GetCSharpTypeByPropertyTypeFullName(prop.PropertyType.FullName);
        }

        /// <summary>
        /// 根据列得到转换为C#后类型字符串
        /// </summary>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static string GetCSharpTypeByPropertyTypeFullName(string typeName)
        {
            switch (typeName)
            {
                case "System.AnsiString":
                case "System.AnsiStringFixedLength": return "string";
                case "System.Binary":
                case "System.Byte[]": return "byte[]";
                case "System.Boolean": return "bool";
                case "System.Byte": return "byte";
                case "System.Currency": return "decimal";
                case "System.Date":
                case "System.DateTime":
                case "System.DateTime2":
                case "System.DateTimeOffset": return "DateTime";
                case "System.Decimal": return "decimal";
                case "System.Double": return "double";
                case "System.Guid": return "Guid";
                case "System.Int16": return "short";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "System.Object": return "object";
                case "System.SByte": return "sbyte";
                case "System.Single": return "float";
                case "System.String":
                case "System.StringFixedLength": return "string";
                case "System.Time": return "TimeSpan";
                case "System.UInt16": return "ushort";
                case "System.UInt32": return "uint";
                case "System.UInt64": return "ulong";
                default:
                    return typeName;
            }
        }

        public static string GetCSharpAppServiceInputType(PropertyInfo prop)
        {
            var propTypeFullName = prop.PropertyType.FullName;
            var type = GetCSharpNullableTypeByProp(prop);
            if (type != "string" && !CommonCode.IsAbpValueObject(prop) && !type.Contains("[]"))
            {
                if (!type.EndsWith("?"))
                {
                    return type + "?";
                }
            }
            return type;
        }

        #endregion 类型处理
    }
}