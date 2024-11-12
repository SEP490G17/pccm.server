namespace Application.SpecParams
{
    /// <summary>
    ///  Các biến query được truyền lên trên url
    /// 
    public class BaseSpecParam
    {
        private const int MaxPageSize = 50; // hằng số max page size
        public int Skip { get; set; } = 0; // Số bản ghi bỏ qua khi query
        private int _pageSize = 99; // số lượng bản ghi trong 1 trang

        public int PageSize // getter setter
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Sort { get; set; } // sort theo cái gì

        private string _search; // search theo cáo gì
        public string Search // getter setter
        {
            get => _search;
            set => _search = value.ToLower();
        }

    }
}