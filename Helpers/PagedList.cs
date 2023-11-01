using Azure;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Helpers
{
    public class PagedList
    {

        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int Start {get; set;}
        public int  Limit { get; set; }
        public int Count { get; set; }
        public PagedList(int currentpage, int count, int limit)
        {
            Limit = limit;
            CurrentPage = currentpage;
            TotalPage = (int) Math.Ceiling(count / (double) limit);
            Start = (currentpage - 1) * limit;
            Count = count;
        }
    }
}