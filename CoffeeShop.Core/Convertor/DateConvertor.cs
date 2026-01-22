namespace CoffeeShop.Core.Convertor
{
    internal static class DateConvertor
    {
        public static string ToMonthName(this int month)
        {
            string[] months = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return (month > 0 && month <= 12) ? months[month - 1] : string.Empty;
        }
    }
}