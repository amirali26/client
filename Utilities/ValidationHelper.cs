using System;

namespace client.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidDate(string date)
        {
            return DateTime.TryParse(date, out DateTime result);
        }
        public static readonly string EmailRegex =
            @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";

        public static readonly string PhoneRegex = "^[0-9]*$";
    }
}