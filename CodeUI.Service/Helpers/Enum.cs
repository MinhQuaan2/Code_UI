using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CodeUI.Service.Helpers
{
    public class Enum

    {
        public enum RegexEnum
        {
            [Display(Name = "/^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$/gm")]
            Phone = 1,
        }

        public enum ElementStatusEnum
        {
            DRAFT = 0,
            PENDING = 1,
            APPROVED = 2,
            REJECTED = 3
        }

        public enum RedisSetUpType
        {
            GET = 1,
            SET = 2,
            DELETE = 3
        }
        public enum SystemRoleTypeEnum
        {
            [Display(Name = "CodeUI System Admin")]
            SystemAdmin = 1,
            [Display(Name = "Free Creator")]
            FreeCreator = 2,
            [Display(Name = "Pro Creator")]
            ProCreator = 3,
            [Display(Name = "Moderator")]
            Moderator = 4,
            [Display(Name = "ProPlus Creator")]
            ProPlusCreator = 5
        }    
        public enum ReportStatusEnum
        {
            PENDING = 1,
            APPROVED = 2,
            REJECTED = 3,
            SOLVED = 4,
            PROCESS = 5,
            SEND_TO_ADMIN = 6
        }
        public enum ReportTypeEnum
        {
            ACCOUNT = 1,
            ELEMENT =2
        }
        public enum ReportElementReasonEnum
        {
            [Display(Name = "Low Quality")]
            LOW_QUALITY = 1,
            [Display(Name = "Code Plagiarism")]
            CODE_PLAGIARISM = 2,
            [Display(Name = "Misleading Information")]
            MISLEADING_INFORMATION = 3,
            [Display(Name = "Harmful Code")]
            HARMFUL_CODE = 4,
            [Display(Name = "Harassment")]
            HARASSMENT = 5,
            [Display(Name = "Plaintext Passwords")]
            PLAINTEXT_PASSWORDS = 6,
            [Display(Name = "Performance Concerns")]
            PERFORMANCE_CONCERNS = 7,
            [Display(Name = "Sensitive Comments")]
            SENSITIVE_COMMENTS = 8,
            [Display(Name = "Non-Coding Content")]
            NON_CODING_CONTENT = 9,
            [Display(Name = "Other")]
            OTHER = 10
        }

        public enum ReportAccountReasonEnum
        {
            SPAM_OR_PHISING = 1,
            IMPERSONATION = 2,
            BULLYING = 3,
            HATE_SPEECH = 4,
            OTHER = 5
        }
        public enum FavoriteResponseActionEnum
        {
            SAVE_FAVORITE = 1,
            DELETE_FAVORITE = 2
        }
        public enum LikeResponseActionEnum
        {
            LIKE = 1,
            DISLIKE = 2
        }
        public enum AcceptReportEnum
        {
            [Display(Name = "Your report is accepted. The element has been banned!")]
            ACCEPT_REPORT = 1,
            [Display(Name = "The element has been banned!")]
            SKIPPED = 2,
            [Display(Name = "This report is sent to admin. Waiting ...")]
            SEND_TO_ADMIN = 3
        }
        public enum RequestStatusEnum
        {
            [EnumMember(Value = "Available")]
            AVAILABLE = 1,
            [EnumMember(Value = "Canceled")]
            CANCELED = 2,
            [EnumMember(Value = "Processing")]
            PROCESSING = 3,
            [EnumMember(Value = "Completed")]
            COMPLETED = 4,
            [EnumMember(Value = "All")]
            ALL = 5,
            [EnumMember(Value = "Submitted")]
            SUBMITTED = 6,
            [EnumMember(Value = "Banned")]
            BANNED
        }
        public enum SortingOption
        {
            [EnumMember(Value = "Default")]
            Default = 1,

            [EnumMember(Value = "Ascending")]
            Ascending = 2,

            [EnumMember(Value = "Descending")]
            Descending = 3
        }
        public enum FulfillmentStatusEnum
        {
            PROCESSING = 1,
            REJECTED = 2,
            APPROVED = 3,
            CANCELED = 4,
            COMPLETED = 5
        }
        public enum PointTransactionTypeEnum
        {
            [Display(Name = "Reward completion")]
            REWARD_COMPLETION = 1,
            [Display(Name = "Deposit compensation")]
            DEPOSIT_COMPENSATION = 2,
            [Display(Name = "Package buy")]
            PACKAGE_BUY = 3,
            [Display(Name = "Cancel request")]
            CANCEL_REQUEST = 4,
            [Display(Name = "Request acceptance fee")]
            ACCEPT_REQUEST_FEE = 5,
            [Display(Name = "Create request")]
            CREATE_REQUEST = 6,
            [Display(Name = "Report compensation")]
            REPORT_COMPENSATION = 7
        }
        public enum PackageNameEnum
        {
            [Display (Name = "Pro")]
            PRO = 3,
            [Display (Name = "Pro+")]
            PRO_PLUS = 5
        }
        public enum AcceptReportResponseEnum
        {
            [Display(Name = "Fulfillment is suitable with request description")]
            SUITABLE_FULFILLMENT = 1
        }
    }
}