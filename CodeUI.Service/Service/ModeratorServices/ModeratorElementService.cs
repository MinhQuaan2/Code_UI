using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.API.Controllers.Moderator
{
    public interface IModeratorElementService
    {
        Task<BaseResponsePagingViewModel<ElementResponse>> GetPendingElements(ElementResponse filter, PagingRequest paging);

        Task<List<ElementResponse>> UpdateElementStatuses(UpdateStatusRequest elementStatuses);

        Task<BaseResponseViewModel<ElementResponse>> ApproveElement(int id, bool isApproved);
        Task DeleteInactiveElements();
    }

    public class ModeratorElementService : IModeratorElementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly TimeZoneInfo _timeZoneInfo;

        public ModeratorElementService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _timeZoneInfo = timeZoneInfo;
        }

        public async Task<BaseResponsePagingViewModel<ElementResponse>> GetPendingElements(ElementResponse filter, PagingRequest paging)
        {
            try
            {
                var elements = _unitOfWork.Repository<Element>().GetAll()
                                                                .Where(x => x.Status == "PENDING" && x.IsActive == true)
                                                                .ProjectTo<ElementResponse>(_mapper.ConfigurationProvider)
                                                                .DynamicSort(filter)
                                                                .DynamicSort(filter)
                                                                .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<ElementResponse>
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page= paging.Page,
                        Size= paging.PageSize,
                        Total = elements.Item1
                    },
                    Data = elements.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<List<ElementResponse>> UpdateElementStatuses(UpdateStatusRequest elementStatuses)
        {
            try
            {
                var elements = await _unitOfWork.Repository<Element>().GetAll().ToListAsync();

                foreach (var elementID in elementStatuses.ElementStatuses.Keys)
                {
                    var singleElement = elements.FirstOrDefault(x => x.Id == elementID);

                    #region validation
                    if (singleElement == null)
                    {
                        throw new ErrorResponse(400, (int)ElementErrorEnum.NOT_FOUND, ElementErrorEnum.NOT_FOUND.GetDisplayName());
                    }
                    if (elementStatuses.ElementStatuses.GetValueOrDefault(elementID) == null)
                    {
                        throw new ErrorResponse(400, (int)ElementErrorEnum.INVALID_STATUS, ElementErrorEnum.INVALID_STATUS.GetDisplayName());
                    }
                    #endregion

                    var postRequest = new PostRequest();
                    postRequest.ElementId = elementID;
                    postRequest.IssuedDate = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                    postRequest.Status = elementStatuses.ElementStatuses.GetValueOrDefault(elementID);
                    postRequest.ModeratorId = Guid.Parse(elementStatuses.ModID);

                    singleElement.Status = elementStatuses.ElementStatuses.GetValueOrDefault(elementID);
                    await _unitOfWork.Repository<Element>().UpdateDetached(singleElement);
                }

                await _unitOfWork.CommitAsync();

                var result = _mapper.Map<List<Element>, List<ElementResponse>>(elements.Where(el => elementStatuses.ElementStatuses.Keys.Any(x => x == el.Id)).ToList());

                return result;
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        //public async Task<Result<Branch>> ApproveElement(int id, bool isApproved)
        //{
        //    try
        //    {
        //        var element = _unitOfWork.Repository<Element>().GetAll().FirstOrDefault(x => x.Id == id);
        //        if (element == null)
        //        {
        //            throw new ErrorResponse(404, (int)ElementErrorEnum.NOT_FOUND, ElementErrorEnum.NOT_FOUND.GetDisplayName());
        //        }

        //        element.Status = isApproved ? ElementStatusEnum.APPROVED.ToString() : ElementStatusEnum.REJECTED.ToString();

        //        await _unitOfWork.Repository<Element>().UpdateDetached(element);
        //        await _unitOfWork.CommitAsync();

        //        return new BaseResponseViewModel<ElementResponse>
        //        {
        //            Status =
        //            {
        //                Success = true,
        //                Message = "Success!",
        //                ErrorCode = 0
        //            },
        //            Data = _mapper.Map<ElementResponse>(element)
        //        };
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task DeleteInactiveElements()
        {
            try
            {
                var inactiveElements = _unitOfWork.Repository<Element>().GetAll().Where(x => x.IsActive == false);
                if(inactiveElements == null)
                {
                    throw new ErrorResponse(404, (int)ElementErrorEnum.NO_INACTIVE, ElementErrorEnum.NO_INACTIVE.GetDisplayName());
                }
                _unitOfWork.Repository<Element>().DeleteRange(inactiveElements);
                await _unitOfWork.CommitAsync();
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        Task<BaseResponseViewModel<ElementResponse>> IModeratorElementService.ApproveElement(int id, bool isApproved)
        {
            throw new NotImplementedException();
        }
    }
}
