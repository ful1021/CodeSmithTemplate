using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSmithTemplate
{
    public class ClassNames
    {
        public string PkName { get; set; }
        public string AppServiceName { get; set; }
        public string GetAllInputName { get; set; }
        public string DtoName { get; set; }
        public string QueryDtoName { get; set; }
        public string CreateOrUpdateInputName { get; set; }
        public string CreateInputName { get; set; }
        public string UpdateInputName { get; set; }

        public string ApplicationDllFile { get; set; }
        public string CoreDllFile { get; set; }
        public string PermissionPrefix { get; set; }
        public string AppServicePermissionPrefix { get; set; }
        public string VueWebPermissionPrefix { get; set; }
        public string VueWebPageName { get; set; }
    }
}
