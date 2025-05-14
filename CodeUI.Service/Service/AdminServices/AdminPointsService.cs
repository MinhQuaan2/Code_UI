using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.PointsResponses;
using CodeUI.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.Service.AdminServices
{
    public interface IAdminPointsService
    {
        Task<BaseResponseViewModel<DashboardPointsResponse>> GetCreatorPointsDashboard(int month, int year);

        Task<BaseResponseViewModel<DashboardAdminPointsResponse>> GetAdminPointsDashboard(int month, int year);
    }
    public class AdminPointsService : IAdminPointsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminPointsService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseViewModel<DashboardAdminPointsResponse>> GetAdminPointsDashboard(int month, int year)
        {
            try
            {
                var transactions = _unitOfWork.Repository<AdminPoint>().GetAll().ToList();

                if (year < 2020 || year > 2030)
                {
                    throw new ErrorResponse(400, 4001, "Year was not valid!");
                }
                if (month < 1 || month > 12)
                {
                    throw new ErrorResponse(400, 4002, "Month was not valid!");
                }

                var result = new DashboardAdminPointsResponse()
                {
                    Time = $"{month}/{year}",
                    Diff = transactions.Where(x => x.Timestamp.Month == (month - 1) && x.Timestamp.Year == year).Sum(x => x.Amount),
                    AdminPointList = _mapper.Map<List<AdminPoint>,List<AdminPointsResponse>>(transactions.Where(x => x.Timestamp.Month == month && x.Timestamp.Year == year).ToList())
                };

                return new BaseResponseViewModel<DashboardAdminPointsResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    },
                    Data = result
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<DashboardPointsResponse>> GetCreatorPointsDashboard(int month,int year)
        {
            try
            {
                var transactions = _unitOfWork.Repository<PointsTransaction>().GetAll().ToList();

                if(year < 2020 || year > 2030)
                {
                    throw new ErrorResponse(400, 4001, "Year was not valid!");
                }
                if(month < 1 || month > 12)
                {
                    throw new ErrorResponse(400, 4002, "Month was not valid!");
                }

                var result = new DashboardPointsResponse()
                {
                    Time = $"{month}/{year}",
                    Diff = transactions.Where(x => x.Timestamp.Month == (month - 1) && x.Timestamp.Year == year).Sum(x => x.Amount),
                    CreatorPointList = _mapper.Map<List<PointsTransaction>, List<CreatorPointsResponse>>(transactions.Where(x => x.Timestamp.Month == month && x.Timestamp.Year == year).ToList())
                };

                return new BaseResponseViewModel<DashboardPointsResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        ErrorCode = 0,
                        Message = "Success!",
                        Success = true
                    },
                    Data = result
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
