using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;
using CodeSmith.Engine;
using SchemaExplorer;

namespace CodeSmithTemplate
{
    /// <summary>
    /// 公共代码帮助类  由于CodeSmith 不支持多个类继承，因此只有所有代码都写在这一个类中，继承CodeTemplate
    /// </summary>
    public class CommonCode : CodeTemplate
    {
        //[Category("01.命名")]
        //public ClassNames N { get; set; }

        #region 设置公共属性

        private string _outputDirectory = "";

        /// <summary>
        /// 输出根目录路径
        /// </summary>
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Optional]
        [NotChecked]
        [Category("01.输出目录")]
        [Description("输出文件的根目录")]
        [DefaultValue("")]
        public string OutputDirectory
        {
            get
            {
                return _outputDirectory.Trim();
            }
            set
            {
                _outputDirectory = value;
                if (_outputDirectory.EndsWith("\\") == false)
                {
                    _outputDirectory += "\\";
                }
            }
        }

        private string _creator = "付亮";

        /// <summary>
        /// 创建人
        /// </summary>
        [Category("其它选项")]
        public string Creator
        {
            get { return _creator.Trim(); }
            set { _creator = value; }
        }

        #endregion 设置公共属性

        #region 字符串扩展方法

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamel(string value)
        {
            return StringUtil.ToCamelCase(value);
        }

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFirstLetterCamel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return value[0].ToString().ToLower() + value.Substring(1);
        }

        /// <summary>
        /// 转换为PascalCase命名法,第一个单词首字母大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPascal(string value)
        {
            return StringUtil.ToPascalCase(value);
        }

        /// <summary>
        /// 转换为单数形式:如Users 生成 User
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSingular(string value)
        {
            return StringUtil.ToSingular(value);
        }

        /// <summary>
        /// 转换为复数形式:如User 生成 Users
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPlural(string value)
        {
            return StringUtil.ToPlural(value);
        }

        /// <summary>
        /// 判断字符串是否为数据库关键字段，如果是加[]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeDelimitedIdentifier"></param>
        /// <returns></returns>
        public static string GetDelimitedIdentifier(string source, bool includeDelimitedIdentifier)
        {
            if ((string.IsNullOrEmpty(source) || source.StartsWith("[")) || source.EndsWith("]"))
            {
                return source;
            }
            List<string> list2 = new List<string> { " ", "'", "~", "!", "%", "^", "&", "(", ")", "-", "{", "}", ".", @"\", "`" };
            List<string> wordList = list2;
            if (!StringUtil.ContainsString(source, wordList) && !includeDelimitedIdentifier)
            {
                return source;
            }
            return string.Format("[{0}]", source);
        }




        #endregion 字符串扩展方法

        #region 生成文件

        /// <summary>
        /// 根据模板名称得到模板
        /// </summary>
        /// <param name="TemplateName"></param>
        /// <returns></returns>
        public CodeTemplate GetCodeTemplate(string TemplateName)
        {
            CodeTemplateCompiler compiler = new CodeTemplateCompiler(this.CodeTemplateInfo.DirectoryName + TemplateName);

            CodeTemplate template = null;

            compiler.CodeTemplateInfo.ToString();
            compiler.Compile();
            if (compiler.Errors.Count == 0)
            {
                template = compiler.CreateInstance();
            }
            else
            {
                System.Text.StringBuilder errorMessage = new System.Text.StringBuilder();
                for (int i = 0; i < compiler.Errors.Count; i++)
                {
                    errorMessage.Append(compiler.Errors[i].ToString()).Append("\r\n");
                }
                throw new ApplicationException(errorMessage.ToString());
            }

            //复制属性
            if (template != null)
            {
                CopyPropertiesTo(template);
            }

            return template;
        }

        /// <summary>
        /// 通用根据模板生成文件方法
        /// </summary>
        public void RenderToFileByTables(bool isRender, string templatePath, string directory, TableSchemaCollection tables, Func<TableSchema, string> className = null)
        {
            if (isRender)
            {
                if (directory.IndexOf("{dbname}") >= 0)
                {
                    directory = directory.Replace("{dbname}", tables[0].Database.Name);
                }
                //载入子模板
                CodeTemplate template = GetCodeTemplate(templatePath);
                foreach (var tab in tables)
                {
                    //CopyPropertiesTo(template);
                    template.SetProperty("SourceTable", tab);

                    RenderToFile(directory, className, template, tab);
                }
                Response.WriteLine(templatePath + "代码生成完毕！");
            }
        }

        /// <summary>
        /// 输出其它模块内容
        /// 需要在头部写命令注册：
        /// <%@ Register Template="Abp.VueSpaWeb/BuildMenu.cst" Name="BuildMenuTemplate" MergeProperties="True" %>
        /// 调用：
        /// RenderOtherTemplate<BuildMenuTemplate>(BuildMenu);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RenderOtherTemplateResponse<T>() where T : CodeTemplate, new()
        {
            T sm = new T();
            this.CopyPropertiesTo(sm);
            sm.Render(this.Response);
        }

        /// <summary>
        /// 输出其它模块内容
        /// </summary>
        /// <param name="isRender"></param>
        /// <param name="templatePath"></param>
        public void RenderOtherTemplate(bool isRender, string templatePath)
        {
            if (isRender)
            {
                CodeTemplate template = GetCodeTemplate(templatePath);
                template.Render(this.Response);
            }
        }

        /// <summary>
        /// 通用根据模板生成文件方法
        /// </summary>
        /// <param name="isRender"></param>
        /// <param name="templatePath"></param>
        /// <param name="directory"></param>
        /// <param name="Tables"></param>
        /// <param name="fileName"></param>
        public void RenderToFileByTablesFixFileName(bool isRender, string templatePath, string directory, TableSchemaCollection Tables, string fileName = null)
        {
            if (isRender)
            {
                if (directory.IndexOf("{dbname}") >= 0)
                {
                    directory = directory.Replace("{dbname}", Tables[0].Database.Name);
                }
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = Tables[0].Database.Name;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(fileName) && fileName.IndexOf("{dbname}") >= 0)
                    {
                        fileName = fileName.Replace("{dbname}", Tables[0].Database.Name);
                    }
                }
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                //载入子模板
                CodeTemplate template = GetCodeTemplate(templatePath);

                //CopyPropertiesTo(template);

                template.SetProperty("Tables", Tables);

                template.RenderToFile(directory + fileName, true);

                Response.WriteLine(templatePath + "代码生成完毕！");
            }
        }

        /// <summary>
        /// 通用根据模板生成文件方法
        /// </summary>
        public void RenderToFile(bool isRender, string templatePath, string directory, string currclassName)
        {
            if (isRender)
            {
                //载入子模板
                CodeTemplate template = GetCodeTemplate(templatePath);
                //CopyPropertiesTo(template);

                RenderToFile(directory, currclassName, template);

                Response.WriteLine(templatePath + "代码生成完毕！");
            }
        }

        private void RenderToFile(string directory, Func<TableSchema, string> className, CodeTemplate template, TableSchema tab)
        {
            if (directory.IndexOf("{tablename}") >= 0)
            {
                directory = directory.Replace("{tablename}", tab.Name);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            }
            string currclassName = null;
            if (className == null)
            {
                currclassName = tab.Name;
            }
            else
            {
                currclassName = className(tab);
            }
            //if (!string.IsNullOrWhiteSpace(splitTableName) && currclassName.IndexOf("{tablename_split_last}") >= 0)
            //{
            //    var temps = tab.Name.Split(splitTableName.ToArray());
            //    if (temps != null && temps.Length > 1)
            //    {
            //        currclassName = currclassName.Replace("{tablename_split_last}", ToSingular(temps.LastOrDefault()));
            //    }
            //}
            if (currclassName.IndexOf(".") > 0)
            {
                template.RenderToFile(directory + currclassName, true);
            }
            else
            {
                template.RenderToFile(directory + currclassName + ".cs", true);
            }
        }

        private void RenderToFile(string directory, string currclassName, CodeTemplate template)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (currclassName.IndexOf(".") > 0)
            {
                template.RenderToFile(Path.Combine(directory, currclassName), true);
            }
            else
            {
                template.RenderToFile(Path.Combine(directory, currclassName) + ".cs", true);
            }
        }

        #endregion 生成文件

        #region Helper方法

        /// <summary>
        /// 判断item是否存在list中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsIn<T>(T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 判断item是否存在list中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsIn(string item, params string[] list)
        {
            return list.Any(a => item.Equals(a, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime? TryToDateTimeOrNull(object str)
        {
            DateTime? result = null;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                DateTime tmp;
                if (DateTime.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime TryToDateTime(object str, string defaultResult = null)
        {
            var result = TryToDateTimeOrNull(str);
            if (result == null && !string.IsNullOrWhiteSpace(defaultResult))
            {
                DateTime tmp;
                if (DateTime.TryParse(defaultResult, out tmp))
                {
                    result = tmp;
                }
            }
            return result ?? DateTime.MinValue;
        }

        /// <summary>
        /// 对象 转换为 decimal 类型
        /// </summary>
        public static decimal TryToDecimal(object str, decimal defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                decimal tmp;
                if (decimal.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 int 类型
        /// </summary>
        public static int TryToInt(object str, int defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                if (int.TryParse(str.ToString(), out int tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 string 类型
        /// </summary>
        public static string TryToString(object source)
        {
            if (source != null)
            {
                return source.ToString();
            }
            return "";
        }
        #endregion Helper方法

        /// <summary>
        /// App 类型
        /// </summary>
        public enum AppServiceType
        {
            混合应用,
            纯管理后台应用
        }
    }
}