using AutoMapper;
using AutoMapper.QueryableExtensions;
using CodeUI.Service.Exceptions;
using Castle.Core.Resource;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.Attributes;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using static CodeUI.Service.Helpers.ErrorEnum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static CodeUI.Service.Helpers.Enum;
using CodeUI.Service.DTO.Response.ElementResponses;
using Microsoft.Identity.Client;
using System.Text.Json;

namespace CodeUI.Service.Service
{
    public interface IElementService
    {
        Task<BaseResponseViewModel<ElementResponse>> CreateElement(CreateElementRequest request);
        Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetElements(SimpleElementResponse filter, PagingRequest paging, string? accountID);
        Task<BaseResponseViewModel<SimpleElementResponse>> UpdateElement(int id, string accountID, UpdateElementRequest request);
        Task<BaseResponseViewModel<ElementResponse>> GetElementByID(int id, string accountID);

        //Task<List<ElementResponse>> UpdateElementStatuses(UpdateStatusRequest elementStatuses);
        Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetRandomFreeElements(SimpleElementResponse filter, PagingRequest paging, string? accountID);
        Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetFavoriteElements(SimpleElementResponse filter, PagingRequest paging, string accountID);
        Task<BaseResponseViewModel<ElementResponse>> PostElement(int id,string accountID);
        Task<BaseResponseViewModel<ElementResponse>> DeleteElement(int id,string accountID);

    }
    public class ElementService : IElementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ElementService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork= unitOfWork;
            _mapper= mapper;
        }

        public async Task<BaseResponseViewModel<ElementResponse>> CreateElement(CreateElementRequest request)
        {
            try
            { 
                var category = _unitOfWork.Repository<Category>().GetAll().FirstOrDefault(x => x.Name == request.CategoryName);
                var element = _mapper.Map<Element>(request);
                element.CategoryId = category.Id;
                element.Category = category;
                element.CreateDate = DateTime.Now;
                element.UpdateDate = DateTime.Now;
                element.IsActive = true;
                element.Status = ElementStatusEnum.DRAFT.ToString().ToUpper();

                await _unitOfWork.Repository<Element>().InsertAsync(element);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ElementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ElementResponse>(element),
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetElements(SimpleElementResponse filter, PagingRequest paging, string? accountID = null)
        {
            try
            {
                var elements = await  _unitOfWork.Repository<Element>().GetAll()
                                                                .Where(x => x.IsActive == true)
                                                                .OrderByDescending(x => x.CreateDate)
                                                                .ProjectTo<SimpleElementResponse>(_mapper.ConfigurationProvider)
                                                                .ToListAsync();
                                                                
                if (accountID != null)
                {
                    for(int i = 0; i < elements.Count; i++)
                    {
                        elements[i] = await CheckLikeAndFavorite(elements[i],accountID);
                    }
                }

                if (filter.LikeCount != null)
                {
                    elements = elements.OrderByDescending(x => x.LikeCount).ToList();
                }
                if (filter.CommentCount != null)
                {
                    elements = elements.OrderByDescending(x => x.CommentCount).ToList();
                }
                if (filter.Favorites != null)
                {
                    elements = elements.OrderByDescending(x => x.Favorites).ToList();
                }

                var result = elements.AsQueryable()
                                     .DynamicFilter(filter)
                                     .DynamicSort(filter)
                                     .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<SimpleElementResponse>()
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<SimpleElementResponse>> UpdateElement(int id, string accountID, UpdateElementRequest request)
        {
            try
            {
                var element = _unitOfWork.Repository<Element>().GetAll().FirstOrDefault(x => x.Id == id && x.IsActive == true);
                var category = _unitOfWork.Repository<Category>().GetAll().FirstOrDefault(x => x.Name == request.CategoryName && x.IsActive == true);

                #region Validating update element request

                if (element == null)
                {
                    throw new ErrorResponse(400, (int) ElementErrorEnum.NOT_FOUND, 
                        ElementErrorEnum.NOT_FOUND.GetDisplayName());
                }

                if(element.OwnerId != Guid.Parse(accountID)) 
                {
                    throw new ErrorResponse(403, (int) ElementErrorEnum.FORBIDDEN, 
                        ElementErrorEnum.FORBIDDEN.GetDisplayName());
                }

                if(category == null)
                {
                    throw new ErrorResponse(404, (int)CategoryErrorEnum.NOT_FOUND,
                        CategoryErrorEnum.NOT_FOUND.GetDisplayName());
                }
                #endregion

                element.Title = request.Title;
                element.Description = request.Description;
                element.UpdateDate = DateTime.Now;
                element.Category = category;


                await _unitOfWork.Repository<Element>().UpdateDetached(element);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<SimpleElementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<SimpleElementResponse>(element)
            };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<ElementResponse>> GetElementByID(int id, string? accountID = null)
        {
            try
            {
                var element = _unitOfWork.Repository<Element>().GetAll().FirstOrDefault(x => x.Id == id && x.IsActive == true);

                if(element == null)
                {
                    throw new ErrorResponse(404, (int)ElementErrorEnum.NOT_FOUND, 
                        ElementErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var elementResponse = _mapper.Map<SimpleElementResponse>(element);

                if (accountID != null)
                {
                    elementResponse = await CheckLikeAndFavorite(elementResponse, accountID);
                }

                return new BaseResponseViewModel<ElementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ElementResponse>(elementResponse)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetRandomFreeElements(SimpleElementResponse filter, PagingRequest paging, string? accountID = null)
        {
            try
            {
                #region Check date for randomizing with pattern

                var filename = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "random.json");
                var jsonString = File.ReadAllText(filename);
                var dayGuid = "";
                var nightGuid = "";

                Dictionary<string,string> jsonValues = JsonSerializer.Deserialize<Dictionary<string,string>>(jsonString);

                //var randomHash = 1;
                var isDaytime = DateTime.Now.Hour <= 12;
                var currentDate = jsonValues.GetValueOrDefault("currentDate");

                if(isDaytime)
                {
                    dayGuid = jsonValues.GetValueOrDefault("day");
                    if (dayGuid == "" || currentDate != DateTime.Now.Date.ToString())
                    {
                        dayGuid = Guid.NewGuid().ToString();
                        jsonValues["day"] = dayGuid;
                        currentDate= DateTime.Now.Date.ToString();
                    }
                }
                else
                {
                    nightGuid = jsonValues.GetValueOrDefault("night");
                    if(nightGuid == "" || currentDate != DateTime.Now.Date.ToString())
                    {
                        nightGuid = Guid.NewGuid().ToString();
                        jsonValues["night"] = nightGuid;
                        currentDate= DateTime.Now.Date.ToString();
                    }
                }

                jsonValues["currentDate"] = currentDate;

                string json = JsonSerializer.Serialize(jsonValues);
                File.WriteAllText(filename, json);

                #endregion

                var randomSeed = isDaytime ? dayGuid : nightGuid;
                var elements = _unitOfWork.Repository<Element>().GetAll()
                                                                .Where(x => x.Owner.RoleId == 2 && x.Status == ElementStatusEnum.APPROVED.ToString() && x.IsActive == true)
                                                                .ToList()
                                                                .Shuffle(randomSeed)
                                                                .ProjectTo<SimpleElementResponse>(_mapper.ConfigurationProvider)
                                                                .ToList();

                if (accountID != null)
                {
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        elements[i] = await CheckLikeAndFavorite(elements[i], accountID);
                    }
                }

                var result = elements.AsQueryable()
                                     .DynamicFilter(filter)
                                     .DynamicSort(filter)
                                     .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<SimpleElementResponse>()
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
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
        
        public async Task<BaseResponsePagingViewModel<SimpleElementResponse>> GetFavoriteElements(SimpleElementResponse filter, PagingRequest paging, string accountID = null)
        {
            try
            {
                var elements = _unitOfWork.Repository<Element>().GetAll()
                                                                .Where(x => x.Favorites.Any(f => f.AccountId == Guid.Parse(accountID) && f.ElementId == x.Id))
                                                                .ProjectTo<SimpleElementResponse>(_mapper.ConfigurationProvider)
                                                                .ToList();

                if (accountID != null)
                {
                    for (int i = 0; i < elements.Count(); i++)
                    {
                        elements[i] = await CheckLikeAndFavorite(elements[i], accountID);
                    }
                }

                var result = elements.AsQueryable()
                                     .DynamicFilter(filter)
                                     .DynamicSort(filter)
                                     .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<SimpleElementResponse>()
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
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<ElementResponse>> PostElement(int id,string accountID)
        {
            try
            {
                var element = await _unitOfWork.Repository<Element>().GetAll().FirstOrDefaultAsync(x => x.Id == id);

                #region Validation

                if (accountID != element.OwnerId.ToString())
                {
                    throw new ErrorResponse(403, (int)ElementErrorEnum.FORBIDDEN, ElementErrorEnum.FORBIDDEN.GetDisplayName());
                }

                if( !(element.Status == ElementStatusEnum.DRAFT.ToString() || element.Status == ElementStatusEnum.REJECTED.ToString() ))
                {
                    throw new ErrorResponse(400, (int)ElementErrorEnum.INVALID_STATUS, ElementErrorEnum.INVALID_STATUS.GetDisplayName());
                }

                #endregion

                element.UpdateDate = DateTime.Now;
                element.Status = ElementStatusEnum.PENDING.ToString();

                await _unitOfWork.Repository<Element>().UpdateDetached(element);

                var postRequest = new PostRequest()
                {
                    ElementId= id,
                    IssuedDate = DateTime.Now,
                    Status = ElementStatusEnum.PENDING.ToString()
                };

                await _unitOfWork.Repository<PostRequest>().InsertAsync(postRequest);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ElementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ElementResponse>(element)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }
        
        public async Task<BaseResponseViewModel<ElementResponse>> DeleteElement(int id,string accountID = "")
        {
            try
            {
                var element = await _unitOfWork.Repository<Element>().GetAll().FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true );

                #region Validate owner for element
                //if (element.OwnerId != Guid.Parse(accountID))
                //{
                //    throw new ErrorResponse(403, (int)ElementErrorEnum.FORBIDDEN, ElementErrorEnum.FORBIDDEN.GetDisplayName());
                //}
                #endregion

                element.UpdateDate = DateTime.Now;
                element.IsActive = false;

                await _unitOfWork.Repository<Element>().UpdateDetached(element);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<ElementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ElementResponse>(element)
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        private async Task<SimpleElementResponse> CheckLikeAndFavorite(SimpleElementResponse element, string accountID)
        {
            try
            {
                var liked = await _unitOfWork.Repository<LikeTable>().GetAll().FirstOrDefaultAsync(x => x.ElementId == element.Id && x.AccountId == Guid.Parse(accountID));
                var favorited = await _unitOfWork.Repository<Favorite>().GetAll().FirstOrDefaultAsync(x => x.ElementId == element.Id && x.AccountId == Guid.Parse(accountID));

                element.IsLiked = liked == null ? false : true;
                element.IsFavorite = favorited == null ? false : true;

                return element;
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
