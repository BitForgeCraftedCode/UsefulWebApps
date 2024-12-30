using System.Xml.Linq;

namespace UsefulWebApps.Helpers.Weather
{
    public static class ManageAPIKey
    {
        private static string appDirectory = Directory.GetCurrentDirectory();
        public static string GetAPIKey()
        {
            XDocument xmlDoc = new XDocument();
            string docApiKey = string.Empty;
            try
            {
                xmlDoc = XDocument.Load($"{appDirectory}/Helpers/Weather/APIKEY.xml");
                docApiKey = xmlDoc.Element("ApiKey").Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to get api key from xml");
                Console.WriteLine(e);
            }
            return docApiKey;
        }
    }
}
