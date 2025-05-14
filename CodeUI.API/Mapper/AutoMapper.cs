using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Service.DTO.Request.AccountRequest;
using CodeUI.Service.DTO.Request.CategoryRequest;
using CodeUI.Service.DTO.Request.ElementRequest;
using CodeUI.Service.DTO.Request.PackageRequest;
using CodeUI.Service.DTO.Request.ProfileRequest;
using CodeUI.Service.DTO.Request.ReactRequest;
using CodeUI.Service.DTO.Response.AccountResponses;
using CodeUI.Service.DTO.Response.CategoryResponses;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.DTO.Request.ReportRequest;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.FollowResponse;
using CodeUI.Service.DTO.Response.PackageResponse;
using CodeUI.Service.DTO.Response.ProfileResponses;
using CodeUI.Service.DTO.Response.ReactElementResponse;
using CodeUI.Service.DTO.Response.ReportResponse;
using CodeUI.Service.DTO.Response.ReactElementResponses;
using static CodeUI.Service.Helpers.Enum;
using ServiceStack.Text;
using CodeUI.Service.DTO.Response.PackageResponses;
using CodeUI.Service.DTO.Request.StaffRequest;
using CodeUI.Service.DTO.Response.StaffResponses;
using CodeUI.Service.DTO.Response.DonationResponse;
using CodeUI.Service.DTO.Request.DonationRequest;
using CodeUI.Service.DTO.Response.PaymentResponse;
using CodeUI.Service.DTO.Response.PaymentResponses;
using CodeUI.Service.DTO.Request.RequestRequest;
using CodeUI.Service.DTO.Response.RequestResponse;
using CodeUI.Service.DTO.Response.PointsResponses;

namespace CodeUI.API.Mapper
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            #region Mapping Template
            //CreateMap<CurrentObject, DestinationObject>().ReverseMap();
            #endregion

            #region Account
            CreateMap<Account, AccountResponse>()
                .ForMember(x => x.ProfileResponse, opt => opt.MapFrom(dst => dst.Profile))
                .ForMember(x => x.Followers, opt => opt.MapFrom(dst => dst.FollowFollowers))
                .ForMember(x => x.Followings, opt => opt.MapFrom(dst => dst.FollowFollowNavigations))
                .ForMember(x => x.Role, opt => opt.MapFrom(dst => dst.Role.Name))
                .ReverseMap();
            CreateMap<Account, AdminAccountResponse>()
                .ForMember(x => x.ProfileResponse, opt => opt.MapFrom(dst => dst.Profile))
                .ForMember(x => x.TotalElement, opt => opt.MapFrom(dst => dst.Elements.Count))
                .ForMember(x => x.Role, opt => opt.MapFrom(dst => dst.Role.Name))
                .ReverseMap();
            CreateMap<Account, MultipleAccountResponse>()
                .ForMember(x => x.ProfileResponse, opt => opt.MapFrom(dst => dst.Profile))
                .ForMember(x => x.Role, opt => opt.MapFrom(dst => dst.Role.Name))
                .ReverseMap();
            CreateMap<CreateAccountRequest, Account>()
                .ForMember(x => x.Profile, opt => opt.MapFrom(dst => dst.CreateProfile))
                .ReverseMap();
            CreateMap<Account, LoginRequest>().ReverseMap();
            #endregion

            #region Profile
            CreateMap<Data.Entity.Profile, UpdateProfileRequest>()
                .ForMember(x => x.Phone, opt => opt.MapFrom(src => src.Account.Phone))
                .ReverseMap();
            CreateMap<Data.Entity.Profile, ProfileResponse>()
                .ForMember(x => x.Username, opt => opt.MapFrom(src => src.Account.Username))
                .ForMember(x => x.Phone, opt => opt.MapFrom(src => src.Account.Phone))
                .ForMember(x => x.TotalFollower, opt => opt.MapFrom(src => src.Account.FollowFollowers.Count))
                .ForMember(x => x.TotalFollowing, opt => opt.MapFrom(src => src.Account.FollowFollowNavigations.Count))
                .ForMember(x => x.TotalApprovedElement, opt => opt.MapFrom(src => src.Account.Elements.Count(e => e.Status == ElementStatusEnum.APPROVED.ToString() && e.IsActive == true)))
                .ReverseMap();
            CreateMap<Data.Entity.Profile, CreateProfileRequest>().ReverseMap();
            #endregion

            #region Element
            CreateMap<Element, ElementResponse>()
                .ForMember(x => x.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))    
                .ForMember(x => x.LikeCount, opt => opt.MapFrom(src => src.LikeTables.Count))    
                .ForMember(x => x.Favorites, opt => opt.MapFrom(src => src.Favorites.Count))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(x => x.OwnerUsername, opt => opt.MapFrom(src => src.Owner.Username))
                .ForMember(x => x.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(x => x.ProfileResponse, opt => opt.MapFrom(src => src.Owner.Profile))
                .ReverseMap();
            CreateMap<Element, SimpleElementResponse>()
                .ForMember(x => x.CommentCount, opt => opt.MapFrom(src => src.Comments.Count))    
                .ForMember(x => x.LikeCount, opt => opt.MapFrom(src => src.LikeTables.Count))    
                .ForMember(x => x.Favorites, opt => opt.MapFrom(src => src.Favorites.Count))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(x => x.OwnerUsername, opt => opt.MapFrom(src => src.Owner.Username))
                .ForMember(x => x.ProfileResponse, opt => opt.MapFrom(src => src.Owner.Profile))
                .ReverseMap();
            CreateMap<ElementResponse, SimpleElementResponse>().ReverseMap();
            CreateMap<Element, CreateElementRequest>().ReverseMap();
            CreateMap<Element, UpdateElementRequest>().ReverseMap();
            #endregion

            #region Category

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Category,QueryCategoryRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            #endregion

            #region React
            CreateMap<CommentRequest, Comment>().ReverseMap();
            CreateMap<Data.Entity.Profile, ProfileReturn>(); // Map Profile to ProfileReturn
            CreateMap<Account, AccountReturn>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile)); // Map Account to AccountReturn, including the Profile
            CreateMap<Comment, CommentResponse>()
                .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account)); // Map Comment to CommentResponse, including the Account
            CreateMap<Favorite, FavoriteResponse>();
            CreateMap<LikeTable, LikeResponse>();
            #endregion

            #region Package
            CreateMap<PackageRequest, Package>().ReverseMap();
            CreateMap<FeatureRequest, Feature>().ReverseMap();
            CreateMap<Package, PackageResponse>()
                .ForMember(dest => dest.TotalFeature, opt => opt.MapFrom(src => src.PackageFeatures.Count()))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.PackageFeatures.Select(pf => new FeatureResponse
                {
                    Id = pf.Feature.Id,
                    Name = pf.Feature.Name,
                    Description = pf.Feature.Description,
                    Config = new ConfigResponse
                    {
                        ConfigName = pf.Config,
                        Number = pf.Number.ToString() ?? null, // Set default value if Number is null
                        Unit = pf.Unit
                    }
                }).ToList()));

            CreateMap<Feature, FeatureResponse>();
            CreateMap<PackageFeature, ConfigResponse>()
                .ForMember(dest => dest.ConfigName, opt => opt.MapFrom(src => src.Config));
            CreateMap<PremiumNote, BuyPackageResponse>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Account))
                .ForMember(dest => dest.Package, opt => opt.MapFrom(src => src.Package));
            CreateMap<Account, CustomerResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Package, PackageBoughtResponse>();
            //CreateMap<PackageRequest, Package>().ReverseMap();
            //CreateMap<Package, PackageResponse>()
            //    .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.PackageFeatures.Select(pf => pf.Feature)));
            ////CreateMap<PackageFeature, FeatureResponse>();
            //CreateMap<Feature, FeatureResponse>();
            //    //.ForMember(dest => dest.Config, opt => opt.MapFrom(src => src.PackageFeatures));
            //CreateMap<PackageFeature, ConfigResponse>();

            #endregion

            #region Follow
            CreateMap<Follow, FollowResponse>().ReverseMap();
            CreateMap<Data.Entity.Profile, FollowProfileResponse>();
            CreateMap<Account, FollowAccountResponse>()
                .ForMember(dest => dest.FollowProfileResponse, opt => opt.MapFrom(src => src.Profile));

            //CreateMap<List<Account>, List<FollowProfileResponse>>();
            #endregion

            #region Report
            CreateMap<ReportRequest, Report>()
                .ForMember(dest => dest.ReportImages, opt => opt.MapFrom(src => src.ReportImages));
            CreateMap<ReportImageRequest, ReportImage>(); // Add this mapping configuration
            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.ReportImages, opt => opt.MapFrom(src => src.ReportImages));
            CreateMap<ReportImage, ReportImageResponse>();
            CreateMap<Element, ModElementReportResponse>()
                .ForMember(dest => dest.ElementId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Owner.Username))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.TotalReport, opt => opt.MapFrom(src => src.Reports.Count(f => f.Status == ReportStatusEnum.PENDING.ToString())));
            CreateMap<Report, ReportResultResponse>()
                .ForMember(dest => dest.ReporterEmail, opt => opt.MapFrom(src => src.Reporter.Email))
                .ForMember(dest => dest.IsReportedEmail, opt => opt.MapFrom(src => src.Element.Owner.Email));
            CreateMap<Account, ModAccountReportResponse>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TotalReport, opt => opt.MapFrom(src => src.ReportTargetAccounts.Count(f => f.Status == ReportStatusEnum.PENDING.ToString())));
            CreateMap<FulfillmentReportRequest, FulfillmentReport>();
            CreateMap<FulfillmentReport, FulfillmentReportResponse>()
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Fulfillment.RequestId));
            CreateMap<FulfillmentReport, ModFulfillmentReportResponse>()
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.Username));
            CreateMap<FulfillmentReport, ReportResultResponse>()
                .ForMember(dest => dest.ReporterEmail, opt => opt.MapFrom(src => src.Reporter.Email));
            CreateMap<Fulfillment, FulfillmentDetailResponse>();
            #endregion

            #region Staff
            CreateMap<CreateStaffRequest, StaffAccount>().ReverseMap();
            CreateMap<UpdateStaffRequest, StaffAccount>().ReverseMap();
            CreateMap<StaffResponse, StaffAccount>().ReverseMap();
            #endregion

            #region Donation
            CreateMap<DonationPackage, DonationPackageResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.DonationPackageDetail.ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.DonationPackageDetail.Price));
            CreateMap<DonationPackageRequest, DonationPackage>()
                .ForMember(dest => dest.DonationPackageDetail, opt => opt.MapFrom(src => src.DetailRequest));
            CreateMap<DonationPackageDetailRequest, DonationPackageDetail>();
            CreateMap<DonationBenefitRequest, DonationBenefit>();
            CreateMap<DonationBenefit, DonationBenefitResponse>();
            CreateMap<DonationPackageDetail, DonationPackageDetailResponse>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.DonationPackage.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DonationPackage.Description))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.DonationDetailBenefits.Select(db => new DonationBenefitResponse
                {
                    Id = db.DonationBenefit.Id,
                    Title = db.DonationBenefit.Title,
                    Description = db.DonationBenefit.Description
                }).ToList()));
            //CreateMap<DonationPackageUpdateRequest, DonationPackage>()
            //    .ForMember(dest => dest.DonationPackageDetail.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
            #endregion

            #region Payment
            CreateMap<Transaction, PaymentConfirmResponse>();
            CreateMap<Transaction, PaymentHistoryResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account.Username));
            CreateMap<PointsTransaction, TransactionResponse>();
            #endregion

            #region Request
            CreateMap<RequestRequest, Request>();
            CreateMap<Request, RequestResponse>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.CreateByNavigation.Username))
                .ForMember(dest => dest.RequesterEmail, opt => opt.MapFrom(src => src.CreateByNavigation.Email))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiveByNavigation.Username))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReceiverEmail, opt => opt.MapFrom(src => src.ReceiveByNavigation.Email))
                .ReverseMap();
            CreateMap<Request, Fulfillment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore mapping for the 'Id' property
            CreateMap<Fulfillment, FulfillmentResponse>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Request.CreateByNavigation.Username))
                .ForMember(dest => dest.RequesterEmail, opt => opt.MapFrom(src => src.Request.CreateByNavigation.Email));
            CreateMap<Request, RequestListResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.requesterId, opt => opt.MapFrom(src => src.CreateByNavigation.Id));
            
            CreateMap<Request, RequestResponse>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.CreateByNavigation.Username))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiveByNavigation.Username))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap();
            CreateMap<Request, RequestFulfillmentResponse>()
                .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.CreateByNavigation.Username))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.ReceiveByNavigation.Username))
                .ForMember(dest => dest.RequesterEmail, opt => opt.MapFrom(src => src.CreateByNavigation.Email))
                .ForMember(dest => dest.ReceiverEmail, opt => opt.MapFrom(src => src.ReceiveByNavigation.Email))
                .ReverseMap();
            CreateMap<Fulfillment, FulfillResponse>();
            #endregion

            #region Points
            CreateMap<PointsTransaction, CreatorPointsResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account.Username))
                .ReverseMap();
            CreateMap<AdminPoint, AdminPointsResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account.Username))
                .ReverseMap();
            #endregion
        }
    }
}
