using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        public ResponseAPI ResponseModel { get; set; }

        public BaseService(IHttpClientFactory client)
        {
            _httpClient = client;
            ResponseModel = new ResponseAPI();
        }

        public void Dispose()
        {

        }

        public async Task<ResponseAPI> SendAsync(RequestAPI request)
        {
            try
            {
                var client = _httpClient.CreateClient("MangoAPI");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("Accept", "application/json");
                requestMessage.RequestUri = new Uri(request.Url);
                client.DefaultRequestHeaders.Clear();
                if (request.File != null)
                {
                    // Handle case having files
                    var multipartFormData = new MultipartFormDataContent();
                    multipartFormData.Add(new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8,"application/json"));
                    multipartFormData.Add(new StreamContent(request.File.OpenReadStream()), "file", request.File.FileName);
                    requestMessage.Content = multipartFormData;
                }
                else if (request.Data != null)
                {
                    // Handle normal cases
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8,
                                    "application/json");
                }

                switch (request.Method)
                {
                    case SD.ApiType.POST:
                        requestMessage.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        requestMessage.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        requestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        requestMessage.Method = HttpMethod.Get;
                        break;
                }
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                string responseContentJson = await responseMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ResponseAPI>(responseContentJson);
                if (res != null && res.IsSuccess)
                {
                    ResponseModel.IsSuccess = res.IsSuccess;
                    ResponseModel.StatusCode = res.StatusCode;
                    ResponseModel.Result = res.Result;
                    return ResponseModel;
                }
                throw new Exception(responseContentJson);
            }
            catch (Exception ex)
            {
                ResponseModel.IsSuccess = false;
                ResponseModel.ErrorMessages.Add(ex.Message);
                ResponseModel.DisplayMessage = "Error";
                ResponseModel.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return ResponseModel;
        }
    }
}
