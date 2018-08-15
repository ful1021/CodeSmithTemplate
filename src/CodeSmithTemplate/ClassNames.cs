using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeSmithTemplate
{
    public class ClassNames
    {
        public string PkName { get; set; }
        public string PkType { get; set; }


        public string AppServiceName { get; set; }
        public string MgmtAppServiceName { get; set; }
        public string BaseAppServiceName { get; set; }

        public string RepositoryName { get; set; }

        public string GetAllInputName { get; set; }
        public string DtoName { get; set; }
        public string QueryDtoName { get; set; }
        public string CreateOrUpdateInputName { get; set; }
        public string CreateInputName { get; set; }
        public string UpdateInputName { get; set; }


        public string ApplicationDllFile { get; set; }
        public string CoreDllFile { get; set; }



        public string CompanyName { get; set; }
        public string ProjectTemplateName { get; set; }

        public string ApplicationAssemblyName { get; set; }
        public string CoreAssemblyName { get; set; }

        public string PermissionModuleName { get; set; }

        public string EntityName { get; set; }
        public string EntitySummary { get; set; }
        public string EntityNamespace { get; set; }
        public string EntityDirectoryName { get; set; }


        public string PermissionPrefix { get; set; }
        public string AppServicePermissionPrefix { get; set; }
        public string VueWebPermissionPrefix { get; set; }



        public string VueWebPageName { get; set; }
        public string WebControllerName { get; set; }


        public PropertyInfo[] EntityColumns { get; set; }
        public PropertyInfo[] DtoColumns { get; set; }
        public PropertyInfo[] GetAllInputColumns { get; set; }
        public PropertyInfo[] EntityDateTimeProps { get; set; }
    }
}
