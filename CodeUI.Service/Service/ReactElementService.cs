using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response.PaymentResponse;
using CodeUI.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using CodeUI.Data.Entity;
using AutoMapper;
using CodeUI.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using CodeUI.Service.DTO.Request.ReactRequest;
using System.Security.Principal;
using Google.Rpc;
using System.Net.NetworkInformation;
using AutoMapper.QueryableExtensions;
using CodeUI.Service.Utilities;
using CodeUI.Service.Attributes;
using static CodeUI.Service.Helpers.ErrorEnum;
using CodeUI.Service.Exceptions;
using CodeUI.Service.DTO.Response.ReactElementResponses;

namespace CodeUI.Service.Service
{
    public interface IReactElementService
    {
        Task<BaseResponseViewModel<LikeResponse>> likeElement(int ElementId, Guid AccountId);
        Task<BaseResponseViewModel<CommentResponse>> createComment(int ElementId, Guid AccountId, CommentRequest request);
        Task<BaseResponseViewModel<FavoriteResponse>> saveFavorite(int ElementId, Guid AccountId);
        Task<BaseResponseViewModel<CommentResponse>> replyComment(int CommentId, Guid AccountId, CommentRequest request);
        Task<BaseResponsePagingViewModel<CommentResponse>> getCommentsByElementId(int ElementId, PagingRequest paging);
        Task<BaseResponseViewModel<CommentResponse>> editComment(int CommentId, Guid AccountId, CommentRequest request);
        Task<BaseResponseViewModel<CommentResponse>> deleteComment(int CommentId, Guid AccountId);
    }
    public class ReactElementService : IReactElementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TimeZoneInfo _timeZoneInfo;
        public ReactElementService(IUnitOfWork unitOfWork, IMapper mapper, TimeZoneInfo timeZoneInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeZoneInfo = timeZoneInfo;
        }
        public async Task<BaseResponseViewModel<LikeResponse>> likeElement(int ElementId, Guid AccountId)
        {
            try
            {
                LikeResponse response = null;
                var account = await _unitOfWork.Repository<Account>().GetAll()
                        .Where(x => x.Id == AccountId)
                        .FirstOrDefaultAsync();
                var element = await _unitOfWork.Repository<Element>().GetAll()
                        .Where(x => x.Id == ElementId)
                        .FirstOrDefaultAsync();
                var existedLike = await _unitOfWork.Repository<LikeTable>().GetAll()
                        .Where(x => x.AccountId == AccountId && x.ElementId == ElementId)
                        .FirstOrDefaultAsync();

                DateTime timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);

                if (existedLike == null)
                {
                    LikeTable like = new LikeTable()
                    {
                        AccountId = AccountId,
                        ElementId = ElementId,
                        Account = account,
                        Element = element,
                    };
                    await _unitOfWork.Repository<LikeTable>().InsertAsync(like);

                    await _unitOfWork.CommitAsync();
                    response = _mapper.Map<LikeResponse>(like);
                    response.Action = Helpers.Enum.LikeResponseActionEnum.LIKE.ToString();
                }
                else
                {
                    _unitOfWork.Repository<LikeTable>().Delete(existedLike); // Assuming you have a Delete method
                    await _unitOfWork.CommitAsync();
                    response = _mapper.Map<LikeResponse>(existedLike);
                    response.Action = Helpers.Enum.LikeResponseActionEnum.DISLIKE.ToString();
                }

                if (response == null)
                {
                    throw new ErrorResponse(500, (int)ReactElementErrorEnum.LIKE_FAILED,
                        ReactElementErrorEnum.LIKE_FAILED.GetDisplayName());
                }

                return new BaseResponseViewModel<LikeResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<CommentResponse>> createComment(int ElementId, Guid AccountId, CommentRequest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                        .Where(x => x.Id == AccountId)
                        .FirstOrDefaultAsync();
                var element = await _unitOfWork.Repository<Element>().GetAll()
                        .Where(x => x.Id == ElementId)
                        .FirstOrDefaultAsync();
                var comment = _mapper.Map<Comment>(request);
                comment.Account = account;
                comment.Element = element;
                comment.AccountId = AccountId;
                comment.ElementId = ElementId;
                comment.Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
                await _unitOfWork.Repository<Comment>().InsertAsync(comment);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<CommentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CommentResponse>(comment)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<FavoriteResponse>> saveFavorite(int ElementId, Guid AccountId)
        {
            try
            {
                FavoriteResponse response = null;
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(x => x.Id == AccountId)
                            .FirstOrDefaultAsync();
                var element = await _unitOfWork.Repository<Element>().GetAll()
                            .Where(x => x.Id == ElementId)
                            .FirstOrDefaultAsync();
                var existingFavorite = await _unitOfWork.Repository<Favorite>().GetAll()
                            .Where (x => x.AccountId == AccountId && x.ElementId == ElementId)
                            .FirstOrDefaultAsync();
                if (existingFavorite == null) {
                    Favorite favorite = new Favorite()
                    {
                        Account = account,
                        Element = element,
                        AccountId = AccountId,
                        ElementId = ElementId,
                        Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo),
                    };
                    await _unitOfWork.Repository<Favorite>().InsertAsync(favorite);

                    await _unitOfWork.CommitAsync();
                    response = _mapper.Map<FavoriteResponse>(favorite);
                    response.Action = Helpers.Enum.FavoriteResponseActionEnum.SAVE_FAVORITE.ToString();
                }
                else 
                {
                    _unitOfWork.Repository<Favorite>().Delete(existingFavorite); // Assuming you have a Delete method
                    await _unitOfWork.CommitAsync();
                    response = _mapper.Map<FavoriteResponse>(existingFavorite);
                    response.Action = Helpers.Enum.FavoriteResponseActionEnum.DELETE_FAVORITE.ToString();
                }
                if(response == null)
                {
                    throw new ErrorResponse(500, (int)ReactElementErrorEnum.FAVORITE_FAILED,
                        ReactElementErrorEnum.FAVORITE_FAILED.GetDisplayName());
                }
                return new BaseResponseViewModel<FavoriteResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = response
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<CommentResponse>> replyComment(int CommentId, Guid AccountId, CommentRequest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                        .Where(x => x.Id == AccountId)
                        .FirstOrDefaultAsync();
                var comment = await _unitOfWork.Repository<Comment>().GetAll()
                        .Where(x => x.Id == CommentId)
                        .FirstOrDefaultAsync();
                if (comment == null)
                {
                    // Handle the case where the comment doesn't exist.
                    return new BaseResponseViewModel<CommentResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Comment with Id" + CommentId + "not found",
                            Success = false,
                            ErrorCode = 404 // Not Found
                        },
                        Data = null
                    };
                }
                if (comment.RootComment != null)
                {
                    comment = comment.RootComment;
                }
                var element = await _unitOfWork.Repository<Element>().GetAll()
                        .Where(x => x.Id == comment.ElementId)
                        .FirstOrDefaultAsync();
                if (element == null)
                {
                    // Handle the case where the comment doesn't exist.
                    return new BaseResponseViewModel<CommentResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Element not found",
                            Success = false,
                            ErrorCode = 404 // Not Found
                        },
                        Data = null
                    };
                }
                var exComment = _mapper.Map<Comment>(request);
                exComment.Account = account;
                exComment.Element = element;
                exComment.AccountId = AccountId;
                exComment.ElementId = element.Id;
                exComment.RootCommentId = CommentId;
                exComment.Timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);

                await _unitOfWork.Repository<Comment>().InsertAsync(exComment);

                comment.InverseRootComment.Add(exComment);

                await _unitOfWork.Repository<Comment>().UpdateDetached(comment);

                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<CommentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CommentResponse>(comment)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<CommentResponse>> getCommentsByElementId(int ElementId, PagingRequest paging)
        {
            try
            {
                var comments = _unitOfWork.Repository<Comment>().GetAll()
                        .Where(x => x.ElementId == ElementId && x.RootCommentId == null)
                        .OrderByDescending(x => x.Timestamp)
                        .ProjectTo<CommentResponse>(_mapper.ConfigurationProvider)
                        .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                //var comments = _unitOfWork.Repository<Comment>()
                //    .GetAll()
                //    .Where(x => x.ElementId == ElementId && x.RootCommentId == null)
                //    .OrderByDescending(x => x.Timestamp) // Sort top-level comments by Timestamp in descending order
                //    .ProjectTo<CommentResponse>(_mapper.ConfigurationProvider)
                //    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                //var sortedComments = comments.Item2.ToList();

                //// Sort child comments by Timestamp in ascending order
                //foreach (var comment in sortedComments)
                //{
                //    comment.InverseRootComment = comment.InverseRootComment.OrderBy(child => child.Timestamp).ToList();
                //}

                return new BaseResponsePagingViewModel<CommentResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = comments.Item1
                    },
                    Data = comments.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<BaseResponseViewModel<CommentResponse>> editComment(int CommentId, Guid AccountId, CommentRequest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(x => x.Id == AccountId)
                            .FirstOrDefaultAsync();
                var comment = await _unitOfWork.Repository<Comment>().GetAll()
                        .Where(x => x.Id == CommentId)
                        .FirstOrDefaultAsync();
                if (comment.AccountId != AccountId)
                {
                    return new BaseResponseViewModel<CommentResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "You don't have permission to edit",
                            Success = false,
                            ErrorCode = 403 // Forbidden
                        },
                        Data = null
                    };
                }
                comment.CommentContent = request.CommentContent;
                await _unitOfWork.Repository<Comment>().UpdateDetached(comment);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<CommentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CommentResponse>(comment)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<CommentResponse>> deleteComment(int CommentId, Guid AccountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                                .Where(x => x.Id == AccountId)
                                .FirstOrDefaultAsync();
                var returnComment = await _unitOfWork.Repository<Comment>().GetAll()
                            .Where(x => x.Id == CommentId)
                            .FirstOrDefaultAsync();
                if (returnComment.AccountId != AccountId)
                {
                    return new BaseResponseViewModel<CommentResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "You don't have permission to edit",
                            Success = false,
                            ErrorCode = 403 // Forbidden
                        },
                        Data = null
                    };
                }
                var comments = await _unitOfWork.Repository<Comment>().GetAll()
                            .Where(x => x.Id == CommentId || x.RootCommentId == CommentId)
                            .ToListAsync();
                if (comments != null && comments.Any())
                {
                    // Remove the comment from the database.
                    foreach (var comment in comments)
                    {
                        _unitOfWork.Repository<Comment>().Delete(comment);
                    }
                    await _unitOfWork.CommitAsync();
                }
                return new BaseResponseViewModel<CommentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<CommentResponse>(returnComment)
                };
            }
            catch(Exception ex) 
            {
                throw;
            }
        }
    }
}
