namespace Shopping.DemoApp.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class EnumUtil
    {
        public static bool IsDefined<T>(int value)
        {
            var values = Enum.GetValues(typeof(T)).Cast<int>().OrderBy(x => x);

            return values.Contains(value);
        }
    }
}
