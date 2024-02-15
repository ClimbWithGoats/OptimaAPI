using System;

namespace OptimaCom
{
    public class OutputMessage
    {
        public Guid GuidResult { get; set; }
        public string Result { get; set; }
        public string Errors { get; set; }
        public string Time { get => DateTime.Now.ToString("s"); }

    }
}
