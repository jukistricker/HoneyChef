namespace IOITCore.Models.Common
{
    public class FilteredPagination : BasePagination
    {
        [System.ComponentModel.DefaultValue("1=1")]
        public string? query { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string? select { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string? search { get; set; }
    }

    public class FilteredPaginationWithDate : BasePagination
    {
        [System.ComponentModel.DefaultValue("1=1")]
        public string? query { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string? select { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string? search { get; set; }

        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }

    }

    public class FilteredCommon : FilteredPagination
    {
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public int type { get; set; } = -1;
        [System.ComponentModel.DefaultValue(-1)]
        public int status { get; set; } = -1;
    }

}