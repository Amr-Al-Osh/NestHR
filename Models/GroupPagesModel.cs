

namespace NestHR.Models
{
    public class GroupPagesModel
    {
        public int GroupPageId { get; set; }

        public int GroupNum { get; set; }

        public string? GroupNameAr { get; set; }

        public string? GroupNameEng { get; set; }

        public int? MasterGroup { get; set; }

        public bool? IsShow { get; set; }

        public int? OrderGroup { get; set; }

        public virtual List<PageModel> PagesList { get; set; } = new List<PageModel>();

    }
}
