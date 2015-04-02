using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace SqlLibrary
{
    public enum SqlDateFormat : byte { YMD, DMY }

    public static class SqlHelper
    {

        public static SqlDateFormat DateFormat = SqlDateFormat.YMD;

        const int SQLLastMillisecondInSecond = 997;


        private static StringBuilder AppendDateOrTime(this StringBuilder @this, char token, params int[] parts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0) @this.Append(token);
                if (parts[i] < 10) @this.Append('0');

                @this.Append(parts[i]);
            }

            return @this;
        }


        public static StringBuilder AppendDateTime(this StringBuilder @this, DateTime t)
        {
            if (t == DateTime.MinValue)
            {
                return @this.Append("null");
            }


            @this.Append("N'"); switch (DateFormat)
            {
                case SqlDateFormat.DMY: @this.AppendDateOrTime('-', t.Day, t.Month, t.Year); break;
                default: @this.AppendDateOrTime('-', t.Year, t.Month, t.Day); break;
            }



            @this.Append(' ').AppendDateOrTime(':', t.Hour, t.Minute, t.Second).Append('.');

            if (t.Millisecond > SQLLastMillisecondInSecond) @this.Append(SQLLastMillisecondInSecond);
            else
            {
                if (t.Millisecond < 10) @this.Append('0');
                if (t.Millisecond < 100) @this.Append('0');
                @this.Append(t.Millisecond);
            }
            return @this.Append('\'');


        }



        public static StringBuilder AppendArray(this StringBuilder @this, Type elementType, Array array)
        {
            if (elementType == typeof(byte))
            {
                @this.Append('0').Append('x');

                foreach (byte item in array)
                {
                    @this.Append(item.ToString("x2"));
                }

                return @this;
            }

            @this.Append('(');

            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    @this.Append(',');
                }

                @this.AppendConstant(elementType, array.GetValue(i));
            }

            return @this.Append(')');
        }


        public static SqlDbType GetDbType(this Type @this)
        {


            if (@this == typeof(byte[]))
            {
                return SqlDbType.Image;
            }


            if (@this == typeof(Guid))
            {
                return SqlDbType.UniqueIdentifier;
            }


            switch (Type.GetTypeCode(@this))
            {
                case TypeCode.Boolean : return SqlDbType.Bit;
                case TypeCode.Byte    : return SqlDbType.Binary;
                case TypeCode.Char    : return SqlDbType.Char;
                case TypeCode.DateTime: return SqlDbType.DateTime;
                case TypeCode.String  : return SqlDbType.NVarChar;
                case TypeCode.Decimal : return SqlDbType.Decimal;
                case TypeCode.Double  : return SqlDbType.Float;
                case TypeCode.Int16   : return SqlDbType.SmallInt;
                case TypeCode.Int32   : return SqlDbType.Int;
                case TypeCode.Int64   : return SqlDbType.BigInt;
                case TypeCode.SByte   : return SqlDbType.Binary;
                case TypeCode.Single  : return SqlDbType.Real;
                case TypeCode.UInt16  : return SqlDbType.SmallInt;
                case TypeCode.UInt32  : return SqlDbType.Int;
                case TypeCode.UInt64  : return SqlDbType.BigInt;
                case TypeCode.Empty   :
                case TypeCode.DBNull  : 
                default               :return  SqlDbType.Variant;

            }
        }


        public static StringBuilder AppendConstant(this StringBuilder @this, Type type, object value)
        {
            if (value == null)
            {
                return @this.Append("null");
            }


            type = value.GetType();


            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return @this.Append(true.Equals(value) ? 1 : 0);
                case TypeCode.Byte: return @this.Append(((byte)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Char: return @this.Append("N'").Append(value.Equals('\'') ? "''" : value.ToString()).Append('\'');
                case TypeCode.DateTime: return @this.AppendDateTime((DateTime)value);
                case TypeCode.String: return @this.Append("N'").Append(value.ToString().Replace("'", "''")).Append('\'');
                case TypeCode.Decimal: return @this.Append(((decimal)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Double: return @this.Append(((double)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Int16: return @this.Append(((short)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Int32: return @this.Append(((int)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Int64: return @this.Append(((long)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.SByte: return @this.Append(((sbyte)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Single: return @this.Append(((float)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.UInt16: return @this.Append(((ushort)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.UInt32: return @this.Append(((uint)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.UInt64: return @this.Append(((ulong)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Empty:
                case TypeCode.DBNull: return @this.Append("null");
                default:
                    if (type.IsArray)
                    {
                        return @this.AppendArray(type.GetElementType(), value as Array);
                    }

                    if (type == typeof(Guid))
                    {
                        return @this.Append('\'').Append(value).Append('\'');
                    }
                   

                    return @this.Append(@this);

            }
        }
    }
}