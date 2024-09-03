using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.AppSettings;
using System.Data;
using TechTalk.SpecFlow;
namespace BsiPlaywrightPoc.Helpers
{
    public class ExecuteDbQueriesHelper
    {
        private readonly ScenarioContext _scenarioContext;
        private AppSettings _appSettings;

        public ExecuteDbQueriesHelper(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public DataTable QueryStandardDb(string sapId) => StandardDbConnectionString().SqlExecute(SelectQueryForStandards(sapId))!;

        public DataTable QueryOrderStateFromSapIntegrationDb(string orderNumber) => SapIntegrationDbConnectionString().SqlExecute(SelectQueryForOrderStateFromSapIntegrationDb(orderNumber))!;

        public DataTable QueryOrdersFromDwhDb(string orderNumber) => DwhDbConnectionString().SqlExecute(SelectQueryForOrdersFromDwhDb(orderNumber))!;

        private static string SelectQueryForOrdersFromDwhDb(string orderNumber) => $"SELECT* FROM[dbo].[Orders] WHERE Number = '{orderNumber}';";

        private static string SelectQueryForOrderStateFromSapIntegrationDb(string orderNumber) => $"SELECT* FROM[dbo].[OrderState] WHERE ShopifyOrderNumber = '{orderNumber}';";

        private string SelectQueryForStandardsOfSpecificModuleCodeAndPublisher(string sapId, string publisher, string code) =>
            "SELECT DISTINCT" +
            "s.SapId, s.Title, s.Publisher, m.Code " +
            "FROM [dbo].Standards s" +
            "INNER JOIN [dbo].Modules m ON s.AssetId = m.AssetId " +
            "WHERE s.SapId IN ( " +
            "SELECT SapId " +
            "FROM [dbo].Standards " +
            "INNER JOIN [dbo].Modules ON [dbo].Standards.AssetId = [dbo].Modules.AssetId " +
            $"WHERE SapId = '{sapId}' AND Publisher = '{publisher}' AND m.Code = '{code}' " +
            "GROUP BY SapId " +
            "HAVING COUNT(DISTINCT [dbo].Modules.Code) > 1);";

        private string SelectQueryForDocumentEligibilityConfiguration(string sapId) =>
            "SELECT TOP(1) [SapId], CONVERT(VARCHAR(100), [InsertDate], 21) AS InsertDate, [EligibleForShop], [EligibleForSubscriptions]  " +
            "FROM[document].[Eligibility_Configuration] " +
            $"WHERE SapId = '{sapId}' ORDER BY InsertDate DESC";

        private string SelectQueryForMaxDateTime() => "SELECT CONVERT(VARCHAR (100), MAX(InsertDate), 21) as MaxDate FROM [document].[Eligibility_Configuration]";

        private string SelectQueryForStandards(string sapId) => $"SELECT* FROM[dbo].[Standards] WHERE SapId = '{sapId}'";

        private string InsertQueryForDocumentEligibilityConfiguration(string sapId, string maxDateTime) => $"INSERT INTO[document].[Eligibility_Configuration] VALUES('{sapId}', '{maxDateTime}', 1, 1)";

        private string StandardDbConnectionString()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();
            return
                $"Server={_appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={_appSettings.DbConnectionDetails.StandardDb};User ID={_appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={_appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
        }

        private string SapIntegrationDbConnectionString()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();
            return
                $"Server={_appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={_appSettings.DbConnectionDetails.SapIntegrationDb};User ID={_appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={_appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
        }

        private string DwhDbConnectionString()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();
            return
                $"Server={_appSettings.DbConnectionDetails.DbCredentials!.ServerName!};Database={_appSettings.DbConnectionDetails.DwhDb};User ID={_appSettings.DbConnectionDetails.DbCredentials.DatabaseUser};Password={_appSettings.DbConnectionDetails.DbCredentials.DatabasePassword};Trusted_Connection=False;Encrypt=True;";
        }
    }
}
