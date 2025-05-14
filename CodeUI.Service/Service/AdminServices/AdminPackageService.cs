using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.Service.AdminServices
{
    public interface IAdminPackageService
    {
        Task<BaseResponseViewModel<PackageDashboardResponse>> GetDashboardPackage(int month, int year);
    }
    public class AdminPackageService : IAdminPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminPackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseViewModel<PackageDashboardResponse>> GetDashboardPackage(int month,int year)
        {
            try
            {
                //if(month > DateTime.Now.Month)
                //{
                //    throw new ErrorResponse(400, 4001, "The month is invalid!");
                //}
                var transactions = _unitOfWork.Repository<Transaction>().GetAll();
                var totalRevenue = transactions.Where(x => x.Date.Month == month && x.Date.Year == year).Sum(x => x.Amount);
                var revenureDiff = transactions.Where(x => x.Date.Month == month-1 && x.Date.Year == year).Sum(x => x.Amount);

                return new BaseResponseViewModel<PackageDashboardResponse>
                {
                    Status = new StatusViewModel
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    },
                    Data = new PackageDashboardResponse
                    {
                        Month = month,
                        RevenueDiff= totalRevenue - revenureDiff,
                        TotalRevenue= totalRevenue
                    }
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
