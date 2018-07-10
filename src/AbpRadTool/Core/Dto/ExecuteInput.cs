namespace AbpRadTool.Core.Dto
{
    public class ExecuteInput
    {
        /// <summary>
        /// 方法名 集合
        /// </summary>
        public string[] MethodNames { get; set; }

        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAsync { get; set; }
    }
}