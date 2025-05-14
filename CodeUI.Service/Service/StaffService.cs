using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Request.StaffRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.StaffResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Helpers;
using CodeUI.Service.Utilities;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;
using Azure.Core;

namespace CodeUI.Service.Service
{
    public interface IStaffService
    {
        Task<BaseResponseViewModel<StaffResponse>> Login(string username, string password);
        Task ForgotPassword(string ID);
        Task<BaseResponseViewModel<bool>> UpdatePassword(string id, string password, string oldPassword = null);
    }
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration; 

        public StaffService(IUnitOfWork unitOfWork, IMapper mapper,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task ForgotPassword(string ID)
        {
            try
            {
                var staffEmail = _unitOfWork.Repository<StaffAccount>().GetAll().FirstOrDefault(x => x.Id == Guid.Parse(ID)).Email;

                var filename = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "mailtemplate.txt");

                MailMessage message = new MailMessage();
                message.Subject = "CodeUI - Reset Password Code";
                message.Body = File.ReadAllText(filename);
                message.IsBodyHtml = true;
                message.To.Add(staffEmail);

                string host = _configuration.GetValue<string>("Smtp:Server", "smtp.gmail.com");
                int port = _configuration.GetValue<int>("Smtp:Port", 587);
                string fromAddress = _configuration.GetValue<string>("Smtp:FromAddress", "codeui.node@gmail.com");

                message.From = new MailAddress(fromAddress);

                using (var smtpClient = new SmtpClient(host, port))
                {
                    await smtpClient.SendMailAsync(message);
                }

            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<StaffResponse>> Login(string username, string password)
        {
            try
            {
                var staff = _unitOfWork.Repository<StaffAccount>().GetAll()
                                       .FirstOrDefault(x => x.Username == username && x.Password == password);
                #region Validate 
                if (staff == null)
                {
                    throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND,
                        StaffErrorEnum.NOT_FOUND.GetDisplayName());
                }else if(staff.IsActive == false)
                {
                    throw new ErrorResponse(400, (int)StaffErrorEnum.INACTIVE,
                        StaffErrorEnum.INACTIVE.GetDisplayName());
                }
                #endregion

                string role = ((SystemRoleTypeEnum)staff.RoleId).ToString();

                string accessToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(staff.Username) ? "" : staff.Username,
                                                                      new string[] { role },
                                                                      staff.Id, _configuration);

                var result = new StaffResponse()
                {
                    Username = username,
                    Token = accessToken
                };

                return new BaseResponseViewModel<StaffResponse>
                {
                    Data = result,
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    }
                };
            }
            catch(ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<bool>> UpdatePassword(string id,string password,string oldPassword = null)
        {
            var staff = _unitOfWork.Repository<StaffAccount>().GetAll()
                                       .FirstOrDefault(x => x.Id == Guid.Parse(id));



            #region Validate 
            if(oldPassword!= null)
            {
                if(staff.Password != oldPassword)
                {
                    throw new ErrorResponse(400, (int)StaffErrorEnum.OLD_PASSWORD_NOT_MATCH,
                        StaffErrorEnum.OLD_PASSWORD_NOT_MATCH.GetDisplayName());
                }
            }

            if (staff == null)
            {
                throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND,
                    StaffErrorEnum.NOT_FOUND.GetDisplayName());
            }
            else if (staff.IsActive == false)
            {
                throw new ErrorResponse(400, (int)StaffErrorEnum.INACTIVE,
                    StaffErrorEnum.INACTIVE.GetDisplayName());
            }
            #endregion

            staff.Password = password;
            staff.UpdateDate = DateTime.Now;
            await _unitOfWork.Repository<StaffAccount>().UpdateDetached(staff);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<bool>
            {
                Data = true,
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                }
            };
        }
    }
}
