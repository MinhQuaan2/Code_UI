using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeUI.Service.Helpers
{
    public class ErrorEnum
    {
        public enum ElementErrorEnum
        {
            [Display(Name = "Invalid target!")]
            INVALID_TARGET = 4001,
            [Display(Name = "Invalid status!")]
            INVALID_STATUS = 4002,

            [Display(Name = "This account is not the owner of this element!")]
            FORBIDDEN = 4031,

            [Display(Name = "Element not found!")]
            NOT_FOUND = 4041,
            [Display(Name = "No inactive elements!")]
            NO_INACTIVE = 4042,
        }

        public enum ProfileErrorEnum
        {
            [Display(Name = "Invalid date of birth!")]
            INVALID_DOB = 4001,
            [Display(Name = "Wallet balance can not be negative!")]
            NEGATIVE_WALLET = 4002,
            [Display(Name = "Invalid phone number format!")]
            INVALID_PHONE_NUMBER = 4003,
            [Display(Name = "Username already exist!")]
            USERNAME_ALREADY_EXIST = 4004,
            [Display(Name = "Username can only be update every 30 days!")]
            UPDATED_RECENTLY = 4005,

            [Display(Name = "Profile not found!")]
            NOT_FOUND = 4041
        }

        public enum CategoryErrorEnum
        {
            [Display(Name = "Category not found!")]
            NOT_FOUND = 4041,

            [Display(Name = "Category already exist!")]
            ALREADY_EXIST = 4001,
            [Display(Name = "Invalid filter input!")]
            INVALID_FILTER = 4002
        }
        
        public enum AccountErrorEnum
        {
            [Display(Name = "Account not found!")]
            NOT_FOUND = 4041
        }

        public enum PaymentErrorEnum
        {
            [Display(Name = "An error occurred during payment process!")]
            ERROR_OCCURRED = 4001,
            [Display(Name = "This transaction has already existed!")]
            TRANSACTION_DUPLICATE = 4002
        }
        public enum PackageErrorEnum
        {
            [Display(Name = "Package not found!")]
            PACKAGE_NOT_FOUND = 4040,
            [Display(Name = "Feature not found!")]
            FEATURE_NOT_FOUND = 4041,
            [Display(Name = "Package Feature not found!")]
            PACKAGE_FEATURE_NOT_FOUND = 4042,
            [Display(Name = "Not enough money!!!")]
            NOT_ENOUGH_MONEY = 4001,
            [Display(Name = "You have bought this one bro???")]
            ALREADY_BUY = 4002,
            [Display(Name = "Inactive feature cannot be added to package")]
            INACTIVE_FEATURE = 4003
        }
        public enum FollowErrorEnum
        {
            [Display(Name = "Follow not found!")]
            FOLLOW_NOT_FOUND =4040,
            [Display(Name = "Follow is unable")]
            FOLLOW_UNABLE = 4041
        }
        public enum  ReportErrorEnum
        {
            [Display(Name = "Report target is null")]
            TARGET_NULL = 4001,
            [Display(Name = "Report not found!")]
            NOT_FOUND = 4041
        }
        public enum ReactElementErrorEnum
        {
            [Display(Name = "Favorite failed")]
            FAVORITE_FAILED = 5001,
            [Display(Name = "Like failed")]
            LIKE_FAILED = 5002
        }
        public enum StaffErrorEnum
        {
            [Display(Name = "Staff account not found!")]
            NOT_FOUND = 4041,

            [Display(Name = "This account is inactive!")]
            INACTIVE = 4001,
            [Display(Name = "This account already exist!")]
            DUPLICATE = 4002,
            [Display(Name = "Old password does not match!")]
            OLD_PASSWORD_NOT_MATCH = 4003,
        }
        public enum DonationPackageErrorEnum
        {
            [Display(Name = "Donation package not found!")]
            DONATION_PACKAGE_NOT_FOUND = 4041,
            [Display(Name = "Donation benefit not found!")]
            DONATION_BENEFIT_NOT_FOUND =4042
        }
        public enum RequestErrorEnum
        {
            [Display(Name = "Create request error")]
            CREATE_FAILED = 5001,
            [Display(Name = "Wallet is not enough")]
            LACK_MONEY = 4001,
            [Display(Name = "Request not found!")]
            REQUEST_NOT_FOUND = 4041,
            [Display(Name = "You don't have permission")]
            FORBIDDEN = 4031,
            [Display(Name = "This request isn't available")]
            NOT_AVAILABLE = 4002,
            [Display(Name = "Creator cannot accept/ reject the request")]
            RECIPIENT_REQUEST = 4003,
            [Display(Name = "This creator is not recipient")]
            NOT_RECIPIENT = 4004,
            [Display(Name = "Fulfillment not found!")]
            FULFILLMENT_NOT_FOUND = 4042,
            [Display(Name = "Confusion between fulfillment and request")]
            CONFUSION_FULFILLMENT = 4004,
            [Display(Name = "Fulfillment is not completed")]
            FULFILLEMNT_NOT_COMPLETE = 4005
        }
        public enum RoleErrorEnum
        {
            [Display(Name = "Role not found")]
            NOT_FOUND = 4041
        }
    }
}
