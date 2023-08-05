namespace QL_HS.Base
{
    public class ActionResultDto
    {
        public int ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public ActionResultMetaDto Meta { get; set; }
        public object Data { get; set; }
    }

    public class ActionResultDto<T>
    {
        public int ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public ActionResultMetaDto Meta { get; set; }
        public T Data { get; set; }
    }
}
