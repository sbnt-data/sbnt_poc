using Blazored.LocalStorage;
using Newtonsoft.Json;
using PMS.Web.Client.Helper;
using PMS.Web.Client.Models;
using PMS.Web.Client.Models.Common;
using PMS.Web.Client.Models.Unit;
using PMS.Web.Client.Services.Contracts.CommonContracts;
using PMS.Web.Client.Services.Contracts.MasterContracts;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PMS.Web.Client.Services.HttpClientServices.MasterHttpClient
{
    public class JobClassHttpClient : IJobClassService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly ITokenService _tokenService;
        public bool refreshRequired = true;
        public JobClassHttpClient(HttpClient httpClient, ILocalStorageService localStorageService, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _tokenService = tokenService;
        }
        /// <summary>
        /// Method/Event   : GETALLJOBCLASS
        /// Author         : Anaswara.s
        /// Created On     : 01-APR-2024
        /// DB Procedures  : 
        /// Other Fn calls : 
        /// Comments       : This method asynchronously retrieves job class data, either from local storage
        ///                  if it's not expired or from an API endpoint, updating the local storage and setting 
        ///                  an expiration time  if necessary.
        /// ================================================================================================
        ///   Last Modified By  ||    Date        ||    Purpose/Reason

        /// ================================================================================================

        ///                     ||                ||    

        /// ================================================================================================        	 
        /// </summary>
        public async Task<IEnumerable<GetJobClassModel>> GetAllJobClass()
        {
            var token = await _tokenService.GetToken();

            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", $"{token}");
            }
            if (await _localStorageService.ContainKeyAsync(LocalStorageConstants.JobClassDataExpirationKey))
            {
                var jobclassExpiration = await _localStorageService.GetItemAsync<DateTime>(LocalStorageConstants.JobClassDataExpirationKey);
                if (jobclassExpiration > DateTime.Now)
                {
                    refreshRequired = false;
                }
            }
            if (!refreshRequired && await _localStorageService.ContainKeyAsync(LocalStorageConstants.JobClassDataExpirationKey))
            {

                var data= await _localStorageService.GetItemAsync<IEnumerable<GetJobClassModel>>(LocalStorageConstants.JobClassDataKey);
                if(data != null)
                {
                    return data;
                }

            }
            var jobclasslist = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<GetJobClassModel>>
            (await _httpClient.GetStreamAsync($"/api/JobClass/"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            await _localStorageService.SetItemAsync(LocalStorageConstants.JobClassDataKey, jobclasslist);
            await _localStorageService.SetItemAsync(LocalStorageConstants.JobClassDataExpirationKey, DateTime.Now.AddMinutes(10));
            return jobclasslist;

        }
        /// <summary>
        /// Method/Event   : AddJobClass
        /// Author         : Anaswara.s
        /// Created On     : 01-APR-2024
        /// DB Procedures  : 
        /// Other Fn calls : 
        /// Comments       : This method asynchronously adds a new job class by sending a POST request to a specified API endpoint,
        /// ================================================================================================
        ///   Last Modified By  ||    Date        ||    Purpose/Reason

        /// ================================================================================================

        ///                     ||                ||    

        /// ================================================================================================        	 

        /// </summary>
        public async Task<CreateJobClassResponse> AddJobClass(CreateJobClass jobClassModel)
        {
            var token = await _tokenService.GetToken();
            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", $"{token}");
            }

            var addjobclass = new StringContent(System.Text.Json.JsonSerializer.Serialize(jobClassModel), Encoding.UTF8, "application/json");
            var jobresponse = await _httpClient.PostAsync("/api/JobClass", addjobclass);

            var jobclassResponseJson = await jobresponse.Content.ReadAsStringAsync();
            var jobclassResponse = JsonConvert.DeserializeObject<CreateJobClassResponse>(jobclassResponseJson);
            if (jobresponse.IsSuccessStatusCode)
            {
                var jobclassData = await _localStorageService.GetItemAsync<List<GetJobClassModel>>(LocalStorageConstants.JobClassDataKey);
                if (jobclassResponse != null)
                {
                    var newJobclass = new GetJobClassModel()
                    {
                        Id = jobclassResponse.jobClassjc.Id,
                        Name = jobClassModel.Name,
                        ShowJobAnalysis = jobClassModel.ShowJobAnalysis,
                        ShowUnplanned = jobClassModel.ShowUnplanned,
                        SortOrder = jobClassModel.SortOrder,
                        ConcurId= jobclassResponse.jobClassjc.ConcurId,
                    };
                    jobclassData.Add(newJobclass);
                    await _localStorageService.SetItemAsync(LocalStorageConstants.JobClassDataKey, jobclassData);
                }
            }
            if (jobresponse == null) jobclassResponse = new CreateJobClassResponse();
            return jobclassResponse;

        }

        /// <summary>
        /// Method/Event   : UpdateJobClass
        /// Author         : Anaswara.s
        /// Created On     :  01-APR-2024
        /// DB Procedures  : 
        /// Other Fn calls : 
        /// Comments       : This method asynchronously updates a job class by sending a PUT request to the API endpoint with the updated data,
        ///                  then updates the local storage with the modified job class if the operation is successful. 
        ///    Last Modified By  ||    Date        ||    Purpose/Reason

        /// ================================================================================================

        ///                     ||                ||    

        /// ================================================================================================        	 
        /// </summary>
        public async Task<UpdateJobClassGroupResponseModel> UpdateJobClass(UpdateJobClass jobClassModel)
        {
            try
            {
                var token = await _tokenService.GetToken();
                if (token != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var updateJobclass = new StringContent(System.Text.Json.JsonSerializer.Serialize(jobClassModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("/api/JobClass/", updateJobclass);
                var jobClassData = await response.Content.ReadAsStringAsync();
                var jobClassResponse = JsonConvert.DeserializeObject<UpdateJobClassGroupResponseModel>(jobClassData);

                if (response.IsSuccessStatusCode)
                {
                    var existingJobClassData = await _localStorageService.GetItemAsync<List<GetJobClassModel>>(LocalStorageConstants.JobClassDataKey);
                    if (existingJobClassData != null)
                    {
                        var updatedJobClassData = new GetJobClassModel
                        {
                            Id = jobClassModel.Id,
                            Name = jobClassModel.Name,
                            ShowJobAnalysis = jobClassModel.ShowJobAnalysis,
                            ShowUnplanned = jobClassModel.ShowUnplanned,
                            SortOrder = jobClassModel.SortOrder,
                            ConcurId = jobClassResponse.ConcurId,

                        };

                        var jobClassIndex = existingJobClassData.FindIndex(jbcs => jbcs.Id == jobClassModel.Id);
                        if (jobClassIndex != -1)
                        {
                            existingJobClassData[jobClassIndex] = updatedJobClassData;
                            existingJobClassData[jobClassIndex].ConcurId = jobClassResponse.ConcurId;
                            await _localStorageService.SetItemAsync(LocalStorageConstants.JobClassDataKey, existingJobClassData);
                        }
                    }
                }
                if (jobClassResponse.ValidationErrors?.Count > 0)
                {
                    if (jobClassResponse.ValidationErrors.Any(error => error == "COMERR102" || error == "JCERR105"))
                    {
                        await _localStorageService.RemoveItemAsync(LocalStorageConstants.JobClassDataKey);
                    }
                }
                return jobClassResponse;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw;
            }
            
        }

        /// <summary>
        /// Method/Event   : DELETEJobClass
        /// Author         : Anaswara.s
        /// Created On     : 01-APR-2024
        /// DB Procedures  : 
        /// Other Fn calls : 
        /// Comments       : This method asynchronously deletes a job class by sending a DELETE request to the API endpoint with the specified job class ID, /// ================================================================================================
        ///   Last Modified By  ||    Date        ||    Purpose/Reason

        /// ================================================================================================

        ///                     ||                ||    

        /// ================================================================================================        	 
        /// </summary>

        
        public async Task<DeleteJobClassResponseModel> DeleteJobClass(decimal Id)
        {
            var jobclassResponse = await _httpClient.DeleteAsync($"/api/JobClass/{Id}");
            var JobData = await jobclassResponse.Content.ReadAsStringAsync();
            var JobResponse = JsonConvert.DeserializeObject<DeleteJobClassResponseModel>(JobData);
            Console.WriteLine(jobclassResponse.IsSuccessStatusCode.ToString());
            var jobclassExistingData = await _localStorageService.GetItemAsync<List<GetJobClassModel>>(LocalStorageConstants.JobClassDataKey);
            if (jobclassResponse.IsSuccessStatusCode)
            {
               // var jobclassExistingData = await _localStorageService.GetItemAsync<List<GetJobClassModel>>(LocalStorageConstants.JobClassDataKey);
                if (jobclassExistingData != null)
                {
                    var jobclassIndex = jobclassExistingData.FindIndex(jbcs => jbcs.Id == Id);
                    if (jobclassIndex != -1)
                    {
                        jobclassExistingData.RemoveAt(jobclassIndex);
                        await _localStorageService.SetItemAsync(LocalStorageConstants.JobClassDataKey, jobclassExistingData);
                    }
                }
            }
           
            else if (JobResponse?.ValidationErrors?.Count > 0 && jobclassExistingData != null)
            {
                if (JobResponse.ValidationErrors.Any(error => error == "JCERR105"))
                {
                    //await _localStorageService.RemoveItemAsync(LocalStorageConstants.JobClassDataKey);
                    var index = jobclassExistingData.FindIndex(p => p.Id == Id);
                    if (index != -1)
                    {
                        jobclassExistingData.RemoveAt(index);
                        await _localStorageService.SetItemAsync(LocalStorageConstants.UnitDataKey, jobclassExistingData);
                    }
                }
            }
            var jobclassData = await jobclassResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeleteJobClassResponseModel>(jobclassData);
        }

    }
}
