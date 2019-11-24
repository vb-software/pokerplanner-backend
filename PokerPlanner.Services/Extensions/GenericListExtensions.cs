using System.Collections.Generic;

namespace PokerPlanner.Services.Extensions
{
    public static class GenericListExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return true;
            }

            return false;
        }
    }
}