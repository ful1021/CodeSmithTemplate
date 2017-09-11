using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSmithTemplate.AspNet.AssemblyFile
{
    public static class AssemblyFileHelper
    {
        public static Dictionary<string, string> GetPropertiesSummary(string dllFile, string className, bool isFirstLetterCamel = true)
        {
            var type = CommonCode.GetAssemblyType(dllFile, className);
            var props = CommonCode.GetProperties(type).ToList();
            return GetPropertiesSummary(props, isFirstLetterCamel);
        }

        public static Dictionary<string, string> GetPropertiesSummary(List<PropertyInfo> props, bool isFirstLetterCamel = true)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
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
    }
}