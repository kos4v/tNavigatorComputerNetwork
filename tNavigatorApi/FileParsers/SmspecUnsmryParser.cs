namespace tNavigatorLauncher.FileParsers
{
    public partial class NavigatorFileController
    {
        public string SmspecUnsmryParser(LauncherConfig config)
        {
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent
            {
                {
                    new StreamContent(File.OpenRead(config.SmspecPath)), "smsspec_file",
                    Path.GetFileName(config.SmspecPath)
                },
                {
                    new StreamContent(File.OpenRead(config.UnsmryPath)), "unsmry_file",
                    Path.GetFileName(config.UnsmryPath)
                }
            };

            var response = client.PostAsync(config.ConverterSmspecUnsmryUrl, content).Result;

            if (!response.IsSuccessStatusCode)
                throw new ArgumentException(
                    $"Ошибка при отправке файлов. Статус: \n{response.StatusCode}\n{response.Content}");

            string responseContent = response.Content.ReadAsStringAsync().Result;
            return responseContent;
        }
    }
}