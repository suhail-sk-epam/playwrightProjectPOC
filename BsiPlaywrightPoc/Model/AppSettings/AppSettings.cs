namespace BsiPlaywrightPoc.Model.AppSettings
{
    public class AppSettings
    {
        public DbConnectionDetails DbConnectionDetails { get; set; }
        public KnowledgeBasicAuth KnowledgeBasicAuth { get; set; }
        public ApiBasicAuth ApiBasicAuth { get; set; }
        public BsiKnowledge BsiKnowledge { get; set; }
        public SubscriptionApi SubscriptionApi { get; set; }
        public MiddlewareApi MiddlewareApi { get; set; }
        public IngestionApi IngestionApi { get; set; }
        public User User { get; set; }
        public RuntimeSettings RuntimeSettings { get; set; }
    }
}
