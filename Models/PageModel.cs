namespace NestHR.Models
{
    public class PageModel
    {
        public int PageNum { get; set; }

        public string? PageNameAr { get; set; }

        public string? PageNameEng { get; set; }

        public int PageGroup { get; set; }

        public string? Url { get; set; }

        public string? Icon { get; set; }

        public int? OrderPage { get; set; }

        public bool? IsShow { get; set; }
    }
}
