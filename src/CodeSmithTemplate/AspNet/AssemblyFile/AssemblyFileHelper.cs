using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeSmithTemplate.AspNet.AssemblyFile
{
    public static class AssemblyFileHelper
    {
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
    }
}