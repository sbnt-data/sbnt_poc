// using AutoMapper;
// using Dapper;
// using Microsoft.EntityFrameworkCore;
// using PMS.Application.Features.JobSearch.Queries.GetJobSearchListByFilter;
// using PMS.Contracts.Persistence;
// using PMS.Domain.Entities;
// using PMS.Domain.Models;
// using PMS.Domain.Models.JobOrder;
// using PMS.Domain.Models.JobPlan;
// using PMS.Persistence.DataAccessLayer;
// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace PMS.Persistence.Repositories
// {
//     public class JobSearchRepository : CommonBaseRepository, IJobSearchRepository
//     {
//         private readonly DataAccess _dataAccess;
//         public JobSearchRepository(ShipmateTechnicalContext context, IMapper mapper, DataAccess dataAccess) : base(context, mapper)
//         {
//             _dataAccess = dataAccess;
//         }
//         public async Task<List<JobSearchModelOutput>> GetAllJobSearch(decimal clientId, JobTagIdValueModelOutput inputModel)
//         {
//             try
//             {
//                 DynamicParameters parameters = new DynamicParameters();
//                 var idlist = new List<TabletypeJobTagIdValueModel>();
//                 foreach (var id in inputModel.JobTagIds)
//                 {
//                     idlist.Add(new TabletypeJobTagIdValueModel { JobTagId=id});
//                 }
//                 parameters.Add("@ClientId", clientId, DbType.Decimal);
//                 parameters.Add("@JobTagIds", HelperClass.ToDataTable(idlist).AsTableValuedParameter("PM.tvp_JobTagId"));
                
//                 _dataAccess.EnsureConnectionOpen();
 
//                 var jobSearchData = await _dataAccess.QueryAsync<JobSearchModelOutput>("[PM].[usp_GetJobSearchData]", parameters);
//                 var jobSearchPopulate = jobSearchData.ListResultset;
//                 _dataAccess.EnsureConnectionClosed();


//                 return jobSearchPopulate;
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception(ex.ToString());
//             }
//         }

        

//         public async Task<List<SearchTagModelOutput>> GetAllJobTagByVesselId(decimal clientId, decimal  vesselId)
//         {

//             try
//             {

//                 DynamicParameters parameters = new DynamicParameters();
//                 parameters.Add("@ClientId", clientId, DbType.Decimal);
//                 parameters.Add("@VesselId", vesselId, DbType.Decimal);
//                 _dataAccess.EnsureConnectionOpen();
//                 var returnDapper = await _dataAccess.QueryAsync<SearchTagModelOutput>("[PM].[usp_GetJobTagByVesselId]", parameters);
//                 var JobTagData = returnDapper.ListResultset;
//                 _dataAccess.EnsureConnectionClosed();
//                 return JobTagData;

//             }
//             catch (Exception ex)
//             {
//                 throw new Exception(ex.ToString());
//             }
//             return null;
//         }
//     }
// }

// -----------------------------------------------------
// Observations & Updated Code

// Connection Management:

// Methods _dataAccess.EnsureConnectionOpen() and _dataAccess.EnsureConnectionClosed() are manually handling the connection. If _dataAccess doesn't manage the lifecycle properly or an exception occurs before closing, it can lead to a disposed connection issue (ObjectDisposedException).
// Exception Handling:

// Wrapping the exception (throw new Exception(ex.ToString())) hides the original stack trace and makes debugging harder. It's better to rethrow the original exception (throw;) or include the original exception as an inner exception.
// Returned Data:

// Variables like jobSearchData.ListResultset and returnDapper.ListResultset are being used directly. If ListResultset is null, this could trigger a NullReferenceException.
// Parameter Management:

// Dynamic parameters are being used correctly, but additional validation could ensure inputModel.JobTagIds is not null or empty.

using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PMS.Application.Features.JobSearch.Queries.GetJobSearchListByFilter;
using PMS.Contracts.Persistence;
using PMS.Domain.Entities;
using PMS.Domain.Models;
using PMS.Domain.Models.JobOrder;
using PMS.Domain.Models.JobPlan;
using PMS.Persistence.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Persistence.Repositories
{
    public class JobSearchRepository : CommonBaseRepository, IJobSearchRepository
    {
        private readonly DataAccess _dataAccess;

        public JobSearchRepository(ShipmateTechnicalContext context, IMapper mapper, DataAccess dataAccess)
            : base(context, mapper)
        {
            _dataAccess = dataAccess;
        }

        public async Task<List<JobSearchModelOutput>> GetAllJobSearch(decimal clientId, JobTagIdValueModelOutput inputModel)
        {
            if (inputModel?.JobTagIds == null || !inputModel.JobTagIds.Any())
                throw new ArgumentException("JobTagIds cannot be null or empty.");

            try
            {
                var idlist = inputModel.JobTagIds
                    .Select(id => new TabletypeJobTagIdValueModel { JobTagId = id })
                    .ToList();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ClientId", clientId, DbType.Decimal);
                parameters.Add("@JobTagIds", HelperClass.ToDataTable(idlist).AsTableValuedParameter("PM.tvp_JobTagId"));

                using (_dataAccess.EnsureConnectionOpen())
                {
                    var jobSearchData = await _dataAccess.QueryAsync<JobSearchModelOutput>("[PM].[usp_GetJobSearchData]", parameters);
                    return jobSearchData?.ListResultset ?? new List<JobSearchModelOutput>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching job search data.", ex);
            }
        }

        public async Task<List<SearchTagModelOutput>> GetAllJobTagByVesselId(decimal clientId, decimal vesselId)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ClientId", clientId, DbType.Decimal);
                parameters.Add("@VesselId", vesselId, DbType.Decimal);

                using (_dataAccess.EnsureConnectionOpen())
                {
                    var returnDapper = await _dataAccess.QueryAsync<SearchTagModelOutput>("[PM].[usp_GetJobTagByVesselId]", parameters);
                    return returnDapper?.ListResultset ?? new List<SearchTagModelOutput>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching job tags by vessel ID.", ex);
            }
        }
    }
}
