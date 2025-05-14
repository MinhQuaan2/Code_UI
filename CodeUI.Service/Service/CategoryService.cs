using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Resource;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.CategoryRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.CategoryResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface ICategoryService
    {
        Task<BaseResponseViewModel<QueryCategoryRequest>> CreateCategory(QueryCategoryRequest request);
        Task<BaseResponsePagingViewModel<QueryCategoryRequest>> GetCategories(QueryCategoryRequest filter, PagingRequest paging);
        Task<CategoryResponse> GetCategory(int? id, string? name);
        Task<CategoryResponse> DeleteCategory(string? name);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<BaseResponseViewModel<QueryCategoryRequest>> CreateCategory(QueryCategoryRequest request)
        {
            try
            {
                var checkCategory = await _unitOfWork.Repository<Category>().GetAll().FirstOrDefaultAsync(x => x.Name == request.Name);

                if(checkCategory != null)
                {
                    throw new ErrorResponse(400, (int)CategoryErrorEnum.ALREADY_EXIST, CategoryErrorEnum.ALREADY_EXIST.GetDisplayName());
                }

                await _unitOfWork.Repository<Category>().InsertAsync(_mapper.Map<Category>(request));
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<QueryCategoryRequest>
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = request
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<QueryCategoryRequest>> GetCategories(QueryCategoryRequest filter, PagingRequest paging)
        {
            try
            {
                var result = _unitOfWork.Repository<Category>().GetAll()
                                                               .ProjectTo<QueryCategoryRequest>(_mapper.ConfigurationProvider)
                                                               .DynamicFilter(filter)
                                                               .DynamicSort(filter)
                                                               .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<QueryCategoryRequest>
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = result.Item1
                    },
                    Data = result.Item2.ToList()
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<CategoryResponse> GetCategory(int? id = null, string? name = null)
        {
            try
            {
                var cateList = _unitOfWork.Repository<Category>().GetAll();
                var result = new Category();
                if (id == null && name != null)
                {
                    result = await cateList.FirstOrDefaultAsync(c => c.Name == name);
                }
                else if(name == null && id != null) 
                {
                    result = await cateList.FirstOrDefaultAsync(c => c.Id == id);
                }
                else
                {
                    throw new ErrorResponse(400, (int)CategoryErrorEnum.INVALID_FILTER, CategoryErrorEnum.INVALID_FILTER.GetDisplayName());
                }

                if(result == null)
                {
                    throw new ErrorResponse(404, (int)CategoryErrorEnum.NOT_FOUND, CategoryErrorEnum.NOT_FOUND.GetDisplayName());
                }

                return _mapper.Map<CategoryResponse>(result);
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        
        public async Task<CategoryResponse> DeleteCategory(string? name = null)
        {
            try
            {
                var cateList = _unitOfWork.Repository<Category>().GetAll();
                var result = new Category();
                if (name != null)
                {
                    result = await cateList.FirstOrDefaultAsync(c => c.Name == name && c.IsActive == true);
                }
                else
                {
                    throw new ErrorResponse(400, (int)CategoryErrorEnum.INVALID_FILTER, CategoryErrorEnum.INVALID_FILTER.GetDisplayName());
                }

                if(result == null)
                {
                    throw new ErrorResponse(404, (int)CategoryErrorEnum.NOT_FOUND, CategoryErrorEnum.NOT_FOUND.GetDisplayName());
                }

                result.IsActive = false;
                await _unitOfWork.Repository<Category>().UpdateDetached(result);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<CategoryResponse>(result);
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
