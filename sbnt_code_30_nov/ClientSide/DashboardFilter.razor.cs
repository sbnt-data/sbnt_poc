using Microsoft.AspNetCore.Components;
using PMS.Web.Client.Models.Common;
using PMS.Web.Client.Services.Contracts.MasterContracts;
using PMS.Web.Client.Models;
using PMS.Web.Client.Models.DashboardModels;
using PMS.Web.Client.Services.Contracts.JobPlanningContracts;
using PMS.Web.Client.Models.JobPlanningModels;
using PMS.Web.Client.Models.Status;
using PMS.Web.Client.Services.Contracts.CommonContracts;
using System.Globalization;
using PMS.Web.Client.Services.Contracts.EquipmentContracts;
using Blazored.SessionStorage;
using PMS.Web.Client.Helper;
using System.Resources;
using PMS.Web.Client.Shared.ResourceFiles;
using PMS.Web.Client.Services.Contracts.DashboardContracts;
using static PMS.Web.Client.Models.JobType;



namespace PMS.Web.Client.Pages.Dashboard
{
    public partial class DashboardFilter
    {

        [Inject] public ISessionStorageService SessionStorage { get; set; } = default!;
        [Inject] public IEquipmentService EquipmentHttpClient { get; set; } = default!;
        [Inject] private IPositionService PositionService { get; set; } = default!;
        [Inject] private IJobClassService JobClassService { get; set; } = default!;
        [Inject] private IJobSearchService JobTagService { get; set; } = default!;
        [Inject] private IJobTypeService JobTypeService { get; set; } = default!;
        [Inject] private IJobSearchService JobSearchService { get; set; } = default!;
        [Inject] public IJobPlanningService JobPlanService { get; set; } = default!;
        [Inject] public IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] public IVesselDashboard VesselDashboardService { get; set; } = default!;

        [Parameter] public bool FilterVisible { get; set; }
        [Parameter] public EventCallback<DashBoradRequestModelInput> OnShowDashBoardData { get; set; }

        public List<DashBoardparametersInput>? VesselDashBoardParameters { get; set; } = new List<DashBoardparametersInput>();
        public DateTime? FromDate { get; set; } = DateTime.Today;
        public DateTime? ToDate { get; set; } = DateTime.Today;
        public List<GetPositionModel> PositionData { get; set; } = default!;
        public List<ResponsiblePositionModelOutput> ResPositionData { get; set; } = default!;
        public List<GetJobClassModel> JobClassData { get; set; } = default!;
        public List<SearchTagModel> JobTagData { get; set; } = default!;
        public List<PMS.Web.Client.Models.JobType.GetJobType> JobTypeData { get; set; } = default!;
        public List<JobSearchModel> JoTitleData { get; set; } = default!;
        public List<StatusMasterDto> PriorityData { get; set; } = default!;
        public List<StatusMasterDto> SafetyData { get; set; } = default!;
        public List<StatusMasterDto> JobStatusData { get; set; } = default!;
        public List<StatusMasterDto> JobCategoryData { get; set; } = default!;
        public List<Department> DepartmentData { get; set; } = default!;
        public bool LoaderVisible { get; set; } = false;
        public DashboardFilterModel DashBaoardFilterData { get; set; } = new DashboardFilterModel();
        public static ResourceManager ResourceManager = new ResourceManager(typeof(Resources));
        public string settingValueString { get; set; }
        public string DateFormat { get; set; } = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        public bool EnableVessel { get; set; } = true;

        public List<RadioModel> JobFor { get; set; } = new List<RadioModel>()
        {
            new RadioModel() { RadioText = ResourceManager.GetString("rsAllPendingJobs"), RadioCode = "0" },
            new RadioModel() { RadioText = ResourceManager.GetString("rsJobForTheWeek"), RadioCode = "1" },
            new RadioModel() { RadioText = ResourceManager.GetString("rsJobForTheMonth"), RadioCode = "2" } ,
            new RadioModel() { RadioText = ResourceManager.GetString("rsOverdueJobs"), RadioCode = "3" },
            new RadioModel() { RadioText = ResourceManager.GetString("rsDrydockJobs"), RadioCode = "4" },
        };

        private void OnRadioGroupChange(string newValue)
        {
            DashBaoardFilterData.SelectedJobFor = newValue;
        }
        private async Task OnClickClose()
        {
           FilterVisible = false;
        }
        private void OnClear()
        {
            DashBaoardFilterData = new DashboardFilterModel();
            FromDate = null;
            ToDate = null;
            VesselDashBoardParameters.Clear();
            SessionStorage.RemoveItemAsync(SessionStorageConstants.DashBoardFilterData);
        }
        protected override async Task OnInitializedAsync()
        {
            await PopulateCombo();
            var storedFilterData = await SessionStorage.GetItemAsync<DashBoradRequestModelInput>(SessionStorageConstants.DashBoardFilterData);
            if (storedFilterData != null)
            {

                DashBaoardFilterData = new DashboardFilterModel
                {
                    SelectedJobFor = storedFilterData.JobDue,
                    ClassData = storedFilterData.IsClassItem,
                    RiskAssessments = storedFilterData.IsRiskAssessment,
                    FromDate = storedFilterData.FromDate,
                    ToDate = storedFilterData.ToDate,
                    SelectedVesselList = storedFilterData.DashBoardparameters?.Where(p => p.Type == "VSL").Select(p => p.Id).ToList(),
                    SelectedJobCategories = storedFilterData.DashBoardparameters?.Where(p => p.Type == "CAT").Select(p => p.Id).ToList(),
                    SelectedPositions = storedFilterData.DashBoardparameters?.Where(p => p.Type == "POS").Select(p => p.Id).ToList(),
                    SelectedJobTypes = storedFilterData.DashBoardparameters?.Where(p => p.Type == "TYP").Select(p => p.Id).ToList(),
                    SelectedSafetyLevels = storedFilterData.DashBoardparameters?.Where(p => p.Type == "SAF").Select(p => p.Id).ToList(),
                    SelectedDepartments = storedFilterData.DashBoardparameters?.Where(p => p.Type == "DEP").Select(p => p.Id).ToList(),
                    SelectedJobClassess = storedFilterData.DashBoardparameters?.Where(p => p.Type == "CLS").Select(p => p.Id).ToList(),
                    SelectedTags = storedFilterData.DashBoardparameters?.Where(p => p.Type == "TAG").Select(p => p.Id).ToList(),
                    SelectedPrioritys = storedFilterData.DashBoardparameters?.Where(p => p.Type == "PRT").Select(p => p.Id).ToList(),
                    SelectedJobStatus = storedFilterData.DashBoardparameters?.Where(p => p.Type == "STA").Select(p => p.Id).ToList()
                };

                VesselDashBoardParameters = storedFilterData.DashBoardparameters ?? new List<DashBoardparametersInput>();
            }
            else
            {
                await LoadTransactionDate();
            }
        }
        private async Task PopulateCombo()
        {
            try
            {

                var jobTagIdValue = new JobTagIdValue();
                jobTagIdValue.JobTagIds.Add(-1);

                var positionTask = PositionService.GetAllPositionDetails();
                var respositionTask = VesselDashboardService.GetResponsiblePosition();
                var jobClassTask = JobClassService.GetAllJobClass();
                var jobTagTask = JobTagService.GetAllSearchTag(-1);
                var jobTypeTask = JobTypeService.GetJobType();
                var joTitleTask = JobSearchService.GetAllJobSearch(jobTagIdValue);
                var departmentTask = DepartmentService.GetDepartmentList();
                var jobCategoryTask = JobPlanService.GetStatusByStatusTypeHardCode("JC");
                var safetyTask = JobPlanService.GetStatusByStatusTypeHardCode("SF");
                var priorityTask = JobPlanService.GetStatusByStatusTypeHardCode("PR");
                var jobStatusTask = JobPlanService.GetStatusByStatusTypeHardCode("JO");

                await Task.WhenAll(
                                    positionTask, jobClassTask, jobTagTask, jobTypeTask, joTitleTask,
                                     departmentTask);
                
                PositionData = (await positionTask)?.ToList() ?? new List<GetPositionModel>();
                ResPositionData = (await respositionTask)?.ToList() ?? new List<ResponsiblePositionModelOutput>();
                JobClassData = (await jobClassTask)?.ToList() ?? new List<GetJobClassModel>();
                JobTagData = (await jobTagTask)?.ToList() ?? new List<SearchTagModel>();
                JobTypeData = (await jobTypeTask)?.ToList() ?? new List<PMS.Web.Client.Models.JobType.GetJobType>();
                JoTitleData = (await joTitleTask)?.ToList() ?? new List<JobSearchModel>();
                DepartmentData = (await departmentTask)?.ToList() ?? new List<Department>();
                JobCategoryData = (await jobCategoryTask)?.ToList() ?? new List<StatusMasterDto>();
                SafetyData = (await safetyTask)?.ToList() ?? new List<StatusMasterDto>();
                PriorityData = (await priorityTask)?.ToList() ?? new List<StatusMasterDto>();
                JobStatusData = (await jobStatusTask)?.ToList() ?? new List<StatusMasterDto>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task SelectedVesselChangedHandler(List<decimal> VesselId)
        {
            DashBaoardFilterData.SelectedVesselList = VesselId;
            DashBoardParameter(VesselId, "VSL");
        }
        public void OnSelectedJobCategoriesChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedJobCategories = Values;

            DashBoardParameter(Values, "CAT");
        }
        public void OnSelectedPositionsChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedPositions = Values;

            DashBoardParameter(Values, "POS");
        }
        public void OnSelectedJobTypesChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedJobTypes = Values;

            DashBoardParameter(Values, "TYP");
        }
        public void OnSafetyLevelChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedSafetyLevels = Values;

            DashBoardParameter(Values, "SAF");
        }
        public void OnSelectedDepartmentsChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedDepartments = Values;

            DashBoardParameter(Values, "DEP");
        }
        public void OnSelectedClassChanged(bool Values)
        {
            DashBaoardFilterData.ClassData = Values;
        }
        public void OnSelectedRiskChanged(bool Values)
        {
            DashBaoardFilterData.RiskAssessments = Values;
        }
        public void OnSelectedJobClassessChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedJobClassess = Values;

            DashBoardParameter(Values, "CLS");
        }
        public void OnSelectedTagsChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedTags = Values;

            DashBoardParameter(Values, "TAG");
        }
        public void OnSelectedPrioritysChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedPrioritys = Values;

            DashBoardParameter(Values, "PRT");
        }
        public void OnSelectedJobStatusChanged(List<decimal> Values)
        {
            DashBaoardFilterData.SelectedJobStatus = Values;

            DashBoardParameter(Values, "STA");
        }
        private void DashBoardParameter(List<decimal> Values, string HardCode)
        {
            try
            {
                if (VesselDashBoardParameters != null)
                {
                    VesselDashBoardParameters.RemoveAll(param => param.Type == HardCode);
                }
                foreach (decimal value in Values)
                {
                    VesselDashBoardParameters.Add(new DashBoardparametersInput { Id = value, Type = HardCode });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task ShowDashBoardData()
        {
            try
            {
                LoaderVisible = true;
                await Task.Run(async () =>
                {
                    DashBaoardFilterData.SelectedVesselGroupList = await SessionStorage.GetItemAsync<List<decimal>>(SessionStorageConstants.SelectedVesselGroups);
                    if (DashBaoardFilterData.SelectedVesselGroupList.Count > 0)
                    {
                        DashBoardParameter(DashBaoardFilterData.SelectedVesselGroupList, "GRP");
                    }
                    var DashBoardFilterData = new DashBoradRequestModelInput
                    {
                        JobDue = DashBaoardFilterData.SelectedJobFor,
                        IsClassItem = DashBaoardFilterData.ClassData,
                        IsRiskAssessment = DashBaoardFilterData.RiskAssessments,
                        FromDate = DashBaoardFilterData.FromDate,
                        ToDate = DashBaoardFilterData.ToDate,
                        DashBoardparameters = VesselDashBoardParameters

                    };
                    await SessionStorage.SetItemAsync(SessionStorageConstants.DashBoardFilterData, DashBoardFilterData);
                    await OnShowDashBoardData.InvokeAsync(DashBoardFilterData);

                    FilterVisible = false;
                });
                LoaderVisible = false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task LoadTransactionDate()
        {
            try
            {
                var userSettings = await EquipmentHttpClient.GetUserSettingsAsync();
                ToDate = DateTime.Today;

                if (userSettings != null)
                {
                    settingValueString = userSettings.FirstOrDefault(u => u.HardCode == "REP_FROM_DATE")?.SettingValue;

                    if (DateTime.TryParse(settingValueString, out DateTime parsedDate))
                    {
                        FromDate = parsedDate;
                    }
                    else
                    {
                        settingValueString = userSettings.FirstOrDefault(u => u.HardCode == "REP_FROM_TODAY_DAY")?.SettingValue;
                        if (int.TryParse(settingValueString, out int days))
                        {
                            FromDate = DateTime.Today.AddDays(-days);
                        }
                    }
                }
                if (string.IsNullOrEmpty(settingValueString))
                {
                    FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
