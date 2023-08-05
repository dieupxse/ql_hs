namespace QL_HS.Base
{
    public class ActionResultMetaDto
    {
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }
        public int RowPerPage { get; set; }
        public int CurrentPage { get; set; }
        public object Query { get; set; }
    }
}
