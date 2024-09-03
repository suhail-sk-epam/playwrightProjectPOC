namespace BsiPlaywrightPoc.Model
{
    public class Standard
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public string SapId { get; set; }
        public string Id { get; set; }
        public List<string> Files { get; set; }
        public List<string> RefIds { get; set; }
        public List<string> PrevVersions { get; set; }
        public string SeriesId { get; set; }
        public string TrackedChangesSapId { get; set; }
        public string ShopifyProductId { get; set; }
    }
}
