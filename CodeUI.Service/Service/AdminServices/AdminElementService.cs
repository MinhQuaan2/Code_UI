using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeUI.Service.Attributes;
using CodeUI.Service.Utilities;

namespace CodeUI.Service.Service.AdminServices
{
    public interface IAdminElementService
    {
        public Task<DashboardElementResponse> GetDashboardElements(ElementResponse filter, PagingRequest paging);
    }
    public class AdminElementService : IAdminElementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminElementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DashboardElementResponse> GetDashboardElements(ElementResponse filter, PagingRequest paging)
        {
            try
            {
                #region Get start and end of week

                var startDate = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
                var endDate = DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek);

                #endregion

                var elements = _unitOfWork.Repository<Element>().GetAll();
                var thisWeekElement = elements.Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate);
                var lastWeekElement = elements.Where(x => x.CreateDate >= startDate.AddDays(-7) && x.CreateDate <= endDate.AddDays(-7));
                float percent = (float)thisWeekElement.ToList().Count() / ((float)lastWeekElement.ToList().Count() >= 0 ? lastWeekElement.ToList().Count() : 1);

                percent = percent > 1 ? 1 - percent : -1 * percent;

                var result = thisWeekElement.ProjectTo<ElementResponse>(_mapper.ConfigurationProvider)
                                            .DynamicFilter(filter)
                                            .DynamicSort(filter)
                                            .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                var elementList = new BaseResponsePagingViewModel<ElementResponse>
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };

                return new DashboardElementResponse
                {
                    elementList = elementList,
                    percent = $"{percent}"
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
