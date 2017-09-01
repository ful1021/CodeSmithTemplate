using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSmithTemplate.AspNet.AssemblyFile
{
    public static class AssemblyFileHelper
    {
        /// <summary>
        /// 根据dll文件，获取得到 Assembly
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string dllFile)
        {
            //byte[] filedata = File.ReadAllBytes(dllFile);
            //Assembly assembly = Assembly.Load(filedata);
            //LoadFrom 会使文件 占用不释放
            Assembly assembly = Assembly.LoadFrom(dllFile);
            return assembly;
        }

        /// <summary>
        /// 根据dll文件，和类名 反射获取PropertyInfo集合
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties(string dllFile, string className)
        {
            System.Type type = GetAssemblyType(dllFile, className);
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
        /// 获取反射的Assembly 类型
        /// </summary>
        /// <param name="dllFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static System.Type GetAssemblyType(string dllFile, string className)
        {
            Assembly assembly = GetAssembly(dllFile);
            var type = assembly.GetTypes().FirstOrDefault(a => a.Name == className);
            return type;
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
                return CommonCode.GetCSharpType(propInfo);
            }
            return "";
        }

        public static Dictionary<string, string> GetPropertiesSummary(string dllFile, string className, bool isFirstLetterCamel = true)
        {
            var props = GetProperties(dllFile, className);
            Dictionary<string, string>
        dict = new Dictionary<string, string>();
            foreach (var item in props)
            {
                var name = item.Name;
                string text = GetPropertySummary(item);
                if (isFirstLetterCamel)
                {
                    name = CommonCode.ToFirstLetterCamel(name);
                }
                dict[name] = text;
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
        /// <param name="dllFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static string GetClassSummary(string dllFile, string className)
        {
            Assembly assembly = GetAssembly(dllFile);
            var type = assembly.GetTypes().FirstOrDefault(a => a.Name == className);
            return GetClassSummary(type);
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
                    return info.Summary;
                }
            }
            return "";
        }


        public static string GetCSharpNullType(PropertyInfo prop, string dllFile)
        {
            var propTypeFullName = prop.PropertyType.FullName;
            var type = CommonCode.GetCSharpNullType(prop); ;
            if (type.Contains("Nullable"))
            {

            }
            return type;
        }
    }
}