using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using static CodeUI.Service.Helpers.Enum;

namespace CodeUI.Service.Utilities
{
    public static class EnumUtils
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()!
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }
            return displayName;
        }

        /// <summary>
        /// Return true when enum not include enumName
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public static bool CheckValidEnums(Type type, string enumName)
        {
            return Enum.GetNames(type)
                .SingleOrDefault(x => x.Equals(enumName)) == null;
        }

        /// <summary>
        /// Return true when enum not include enumName
        /// </summary>
        /// <param name="type"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool CheckValidEnums(Type type, int enumValue)
        {
            return Enum.IsDefined(type, enumValue);
        }
        public static string GetReasonAsString(ReportElementReasonEnum reason)
        {
            var fieldInfo = reason.GetType().GetField(reason.ToString());
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute != null)
            {
                return displayAttribute.Name;
            }

            return reason.ToString(); // Fallback to the enum name if no Display attribute is found.
        }
    }
}
