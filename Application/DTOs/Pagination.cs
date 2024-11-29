namespace Application.DTOs
{
    public class Pagination<T> where T : class
    {
        public Pagination()
        {

        }
        public Pagination(int pageSize, int count, IReadOnlyList<T> data)
        {
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }

    public class PaginationNoti<T> : Pagination<T> where T : class
    {
        public int NumOfUnread { get; set; }
        public PaginationNoti():base()
        {
            
        }
        public PaginationNoti(int pageSize, int count, IReadOnlyList<T> data, int numOfUnread) : base(pageSize, count, data)
        {
            NumOfUnread = numOfUnread;
        }
    }
}