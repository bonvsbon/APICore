using System;

namespace APICore.Common
{
    public static class ConditionParams
    {
        public static Object hasOrNull(this Object val)
        {
            if(val == null || val.Equals(""))
            {
                val = DBNull.Value;
            }
            if (val is DateTime && ((DateTime)val).Equals(DateTime.MinValue))
            {
                return DBNull.Value;
            }

            return val;
        }
    }
}