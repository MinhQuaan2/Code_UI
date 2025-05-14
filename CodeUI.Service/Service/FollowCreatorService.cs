using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Request;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.FollowResponse;
using CodeUI.Service.DTO.Response.FollowResponses;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service
{
    public interface IFollowCreatorService
    {
        Task<BaseResponseViewModel<FollowResponse>> followCreator(string username, Guid followerId);
        Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollower(PagingRequest paging, Guid accountId);
        Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowing(PagingRequest paging, Guid accountId);
        Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowingByUsername(PagingRequest paging, string username);
        Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowerByUsername(PagingRequest paging, string username);
        Task<BaseResponseViewModel<FollowListResponse>> getAllFollowByUsername(string username);
    }
    public class FollowCreatorService : IFollowCreatorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FollowCreatorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<BaseResponseViewModel<FollowResponse>> followCreator(string username, Guid followerId)
        {
            try
            {
                var follower = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Id == followerId)
                            .FirstOrDefaultAsync();
                var followed = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Username == username)
                            .FirstOrDefaultAsync();
                if(followed == null) 
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName() );
                }
                if (followed.Id == followerId || followed.RoleId == (int)SystemRoleTypeEnum.Moderator || followed.RoleId == (int)SystemRoleTypeEnum.SystemAdmin)
                {
                    throw new ErrorResponse(400, (int)FollowErrorEnum.FOLLOW_UNABLE,
                        FollowErrorEnum.FOLLOW_UNABLE.GetDisplayName());
                }// xet xem Id cua nguoi follow va nguoi dc follow co trung khong hoac xet xem role cua nguoi duoc follow co phai admin k?

                var existingFollow = await _unitOfWork.Repository<Follow>().GetAll()
                        .Where(f => f.FollowId == followed.Id && f.FollowerId == followerId)
                        .FirstOrDefaultAsync();
                if (existingFollow != null)
                {
                    _unitOfWork.Repository<Follow>().Delete(existingFollow);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<FollowResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Unfollow sucessfully",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<FollowResponse>(existingFollow)
                    };
                }
                else
                {
                    Follow follow = new Follow()
                    {
                        Follower = follower,
                        FollowerId = followerId,
                        FollowId = followed.Id,
                        FollowNavigation = followed
                    };
                    await _unitOfWork.Repository<Follow>().InsertAsync(follow);
                    await _unitOfWork.CommitAsync();
                    return new BaseResponseViewModel<FollowResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Follow successfully",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<FollowResponse>(follow)
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollower(PagingRequest paging, Guid accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Id == accountId)
                            .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var followerIdList = await _unitOfWork.Repository<Follow>().GetAll()
                            .Where(f => f.FollowId == accountId)
                            .Select(f => f.FollowerId)
                            .ToListAsync();
                var followerList = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => followerIdList.Contains(a.Id))
                    .ToListAsync();
                var followerResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followerList);

                // Create a tuple with metadata and data
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = followerResponseList.Count // Update this with the correct count
                    },
                    followerResponseList.ToList()
                );

                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<FollowAccountResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowing(PagingRequest paging, Guid accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Id == accountId)
                            .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var followerIdList = await _unitOfWork.Repository<Follow>().GetAll()
                            .Where(f => f.FollowerId == accountId)
                            .Select(f => f.FollowId)
                            .ToListAsync();
                var followerList = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => followerIdList.Contains(a.Id))
                    .ToListAsync();
                var followerResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followerList);

                // Create a tuple with metadata and data
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = followerResponseList.Count // Update this with the correct count
                    },
                    followerResponseList.ToList()
                );

                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<FollowAccountResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowerByUsername(PagingRequest paging, string username)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Username == username)
                            .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var followerIdList = await _unitOfWork.Repository<Follow>().GetAll()
                            .Where(f => f.FollowId == account.Id)
                            .Select(f => f.FollowerId)
                            .ToListAsync();
                var followerList = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => followerIdList.Contains(a.Id))
                    .ToListAsync();
                var followerResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followerList);

                // Create a tuple with metadata and data
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = followerResponseList.Count // Update this with the correct count
                    },
                    followerResponseList.ToList()
                );

                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<FollowAccountResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<FollowAccountResponse>> getFollowingByUsername(PagingRequest paging, string username)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                            .Where(a => a.Username == username)
                            .FirstOrDefaultAsync();
                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                        AccountErrorEnum.NOT_FOUND.GetDisplayName());
                }
                var followingIdList = await _unitOfWork.Repository<Follow>().GetAll()
                            .Where(f => f.FollowerId == account.Id)
                            .Select(f => f.FollowId)
                            .ToListAsync();
                var followingList = await _unitOfWork.Repository<Account>().GetAll()
                    .Where(a => followingIdList.Contains(a.Id))
                    .ToListAsync();
                var followingResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followingList);

                // Create a tuple with metadata and data
                var result = (
                    new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = followingResponseList.Count // Update this with the correct count
                    },
                    followingResponseList.ToList()
                );

                // Return the tuple as part of BaseResponsePagingViewModel
                return new BaseResponsePagingViewModel<FollowAccountResponse>()
                {
                    Metadata = result.Item1,
                    Data = result.Item2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<FollowListResponse>> getAllFollowByUsername(string username)
        {
            var accountList = _unitOfWork.Repository<Account>().GetAll();
                            
            var account = await accountList.Where(a => a.Username == username).FirstOrDefaultAsync();
            if (account == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnum.NOT_FOUND,
                    AccountErrorEnum.NOT_FOUND.GetDisplayName());
            }
            //follower list
            var followList = _unitOfWork.Repository<Follow>().GetAll();
            var followerIdList = await followList
                            .Where(f => f.FollowId == account.Id)
                            .Select(f => f.FollowerId)
                            .ToListAsync();
            var followerList = await accountList
                .Where(a => followerIdList.Contains(a.Id))
                .ToListAsync();
            var followerResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followerList);
            //following list
            var followingIdList = await followList
                            .Where(f => f.FollowerId == account.Id)
                            .Select(f => f.FollowId)
                            .ToListAsync();
            var followingList = await accountList
                .Where(a => followingIdList.Contains(a.Id))
                .ToListAsync();
            var followingResponseList = _mapper.Map<List<Account>, List<FollowAccountResponse>>(followingList);

            var response = new FollowListResponse
            {
                Followers = followerResponseList,
                Following = followingResponseList
            };
            return new BaseResponseViewModel<FollowListResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Get All Follow Successfully",
                    Success = true,
                    ErrorCode = 0
                },
                Data = response
            };
        }
    }
}
