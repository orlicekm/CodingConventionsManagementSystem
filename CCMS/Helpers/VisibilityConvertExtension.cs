namespace CCMS.Helpers
{
    public static class VisibilityConvertExtension
    {
        public static bool ToVisible(this string convertedValue)
        {
            return string.IsNullOrEmpty(convertedValue);
        }

        public static bool ToVisible(this int convertedValue)
        {
            return convertedValue == 0;
        }
    }
}