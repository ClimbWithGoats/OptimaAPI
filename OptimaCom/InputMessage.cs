using System;

namespace OptimaCom
{
    public class InputMessage
    {
        public Guid Guid { get; set; }
        public string ObjectType { get; set; }
        public PipeLineAlias MethodName { get; set; }
        public OperationType OperationType { get; set; }
        public string Json { get; set; }
        public string Message { get; set; }
    }
}
