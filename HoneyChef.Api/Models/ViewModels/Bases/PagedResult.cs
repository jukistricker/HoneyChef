using IOITCore.Repositories.Interfaces.Bases;

namespace IOITCore.Models.ViewModels.Bases
{
    public class PagedResult<TDataResult> : IPagedResult<TDataResult>
    {
        public List<TDataResult> Results { get; set; } = new List<TDataResult>();
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
    }
}
