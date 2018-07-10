using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using AbpRadTool.Core.Dto;
using EnvDTE;
using EnvDTE80;

namespace AbpRadTool.Core
{
    public class AddNewServiceMethodHelper : HelperBase
    {
        private const string ErrMessage = "Please run in the class that implements IApplicationService interface.";
        private const string InterfaceName = "Abp.Application.Services.IApplicationService";
        private readonly DTE2 _dte;
        private CodeClass _serviceClass;
        private CodeInterface _serviceInterface;
        private Document _document;

        public AddNewServiceMethodHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
        }

        public override bool CanExecute(ExecuteInput input)
        {
            _document = _dte.ActiveDocument;
            if (_document == null || _document.ProjectItem == null || _document.ProjectItem.FileCodeModel == null)
            {
                MessageBox(ErrMessage);
                return false;
            }

            _serviceClass = GetClass(_document.ProjectItem.FileCodeModel.CodeElements);
            if (_serviceClass == null)
            {
                MessageBox(ErrMessage);
                return false;
            }

            _serviceInterface = GetServiceInterface(_serviceClass as CodeElement);
            if (_serviceInterface == null)
            {
                MessageBox(ErrMessage);
                return false;
            }
            return true;
        }

        public override void Execute(ExecuteInput input)
        {
            foreach (string name in input.MethodNames)
            {
                var splitName = name.Split('|');
                if (splitName != null && splitName.Length > 0)
                {
                    string methodName = splitName[0];
                    string doccoment = "";
                    if (splitName.Length > 1)
                    {
                        doccoment = splitName[1];
                    }
                    try
                    {
                        AddMethodToClass(_serviceClass, methodName, doccoment, input.IsAsync);
                        AddMethodToInterface(_serviceInterface, methodName, doccoment, input.IsAsync);
                        CreateDtoFiles(_document, methodName, doccoment);
                    }
                    catch (Exception e)
                    {
                        MessageBox("Generation failed.\r\nMethod name: {0}\r\nException: {1}", MessageBoxButton.OK, MessageBoxImage.Exclamation, name, e.Message);
                    }
                }
            }
        }

        #region 生成方法定义

        #region 生成方法定义到类中

        /// <summary>
        /// 生成方法定义到类中
        /// </summary>
        /// <param name="serviceClass"></param>
        /// <param name="methodName"></param>
        /// <param name="async"></param>
        private void AddMethodToClass(CodeClass serviceClass, string methodName, string doccoment, bool async)
        {
            var returnName = string.Format(async ? "Task<{0}>" : "{0}", GetDtoClassName(methodName, DtoType.Output));
            var function = serviceClass.AddFunction(methodName, vsCMFunction.vsCMFunctionFunction, returnName, -1, vsCMAccess.vsCMAccessPublic);
            function.AddParameter("input", GetDtoClassName(methodName, DtoType.Input));
            function.Comment = doccoment;
            if (async)
            {
                function.StartPoint.CreateEditPoint().ReplaceText(6, "public async", (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
            }
            function.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint().ReplaceText(0, "throw new System.NotImplementedException();", (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
        }

        #endregion 生成方法定义到类中

        #region 生成方法定义到接口

        /// <summary>
        /// 生成方法定义到接口
        /// </summary>
        /// <param name="serviceInterface"></param>
        /// <param name="name"></param>
        /// <param name="async"></param>
        private void AddMethodToInterface(CodeInterface serviceInterface, string methodName, string doccoment, bool async)
        {
            var returnName = string.Format(async ? "Task<{0}>" : "{0}", GetDtoClassName(methodName, DtoType.Output));
            var function = serviceInterface.AddFunction(methodName, vsCMFunction.vsCMFunctionFunction, returnName, -1);
            function.AddParameter("input", GetDtoClassName(methodName, DtoType.Input));
            function.Comment = doccoment;
        }

        #endregion 生成方法定义到接口

        #endregion 生成方法定义

        #region 私有帮助方法

        #region 获取当前类

        /// <summary>
        /// 获取当前类
        /// </summary>
        /// <param name="codeElements"></param>
        /// <returns></returns>
        private CodeClass GetClass(CodeElements codeElements)
        {
            var elements = codeElements.Cast<CodeElement>().ToList();
            var result = elements.FirstOrDefault(codeElement => codeElement.Kind == vsCMElement.vsCMElementClass) as CodeClass;
            if (result != null)
            {
                return result;
            }
            foreach (var codeElement in elements)
            {
                result = GetClass(codeElement.Children);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion 获取当前类

        #region 获取当前类对应接口

        /// <summary>
        /// 获取当前类对应接口
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static CodeInterface GetServiceInterface(CodeElement element)
        {
            CodeElements elements;
            if (element.Kind == vsCMElement.vsCMElementClass)
            {
                elements = (element as CodeClass).ImplementedInterfaces;
            }
            else if (element.Kind == vsCMElement.vsCMElementInterface)
            {
                elements = (element as CodeInterface).Bases;
            }
            else
            {
                throw new ArgumentException("The parameter element is not Class nor Interface");
            }
            var baseInterfaces = elements.Cast<CodeElement>()
                .Where(codeElement => codeElement.Kind == vsCMElement.vsCMElementInterface)
                .Cast<CodeInterface>()
                .ToList();

            var result = baseInterfaces.FirstOrDefault(codeInterface => codeInterface.FullName == InterfaceName);
            if (result != null)
            {
                return element as CodeInterface;
            }
            foreach (var baseInterface in baseInterfaces)
            {
                result = GetServiceInterface(baseInterface as CodeElement);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion 获取当前类对应接口

        #region 创建Dto类文件

        /// <summary>
        /// 创建Dto类文件
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        private void CreateDtoFiles(Document document, string methodName, string doccoment)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            //string templateFile = Path.Combine(Path.GetDirectoryName(location), "Templates", "Dto.txt");
            //string template = File.ReadAllText(templateFile);

            var parentItem = document.ProjectItem.Collection.Parent as ProjectItem;
            var appServicesFolder = parentItem.Name;
            var dtoFolder = parentItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault(item => item.Name == "Dto");
            if (dtoFolder == null)
            {
                dtoFolder = parentItem.ProjectItems.AddFolder("Dto");
            }

            string nameSpace = GetNameSpace(document);
            string path = Path.GetTempPath();
            Directory.CreateDirectory(path);
            foreach (var str in new[] { "Input", "Output" })
            {
                var dtoclassName = GetDtoClassName(methodName, str);

                string templateFile = Path.Combine(Path.GetDirectoryName(location), "Templates", str + ".txt");
                string template = File.ReadAllText(templateFile);
                string content = string.Format(template, nameSpace, dtoclassName, doccoment);
                string file = Path.Combine(path, dtoclassName + ".cs");
                try
                {
                    File.WriteAllText(file, content);
                    dtoFolder.ProjectItems.AddFromFileCopy(file);
                }
                catch (Exception e)
                {
                    MessageBox(e.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                finally
                {
                    File.Delete(file);
                }
            }
        }

        #endregion 创建Dto类文件

        #region 获取Dto类名

        private string GetDtoClassName(string methodName, DtoType type)
        {
            return GetDtoClassName(methodName, type.ToString());
        }

        private string GetDtoClassName(string methodName, string type)
        {
            var serviceName = _serviceClass.Name.Replace("AppService", "");
            //var appServicesFolder = (_document.ProjectItem.Collection.Parent as ProjectItem).Name;
            string dtoClassName = string.Format("{0}{1}{2}", serviceName, methodName, type);
            return dtoClassName;
        }

        #endregion 获取Dto类名

        #region 获取当前命名空间

        private string GetNameSpace(Document document)
        {
            return document.ProjectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().FullName;
        }

        #endregion 获取当前命名空间

        #endregion 私有帮助方法
    }

    public enum DtoType
    {
        Input,
        Output
    }
}