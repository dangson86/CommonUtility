using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonUtility.Extentions
{
    public static class CommonExt
    {  
        public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            if (property == null)
                return null;

            var type = typeof(T);
            if (property.IsDefined(type, true))
            {
                var attrs = property.GetCustomAttribute(type, true);

                return attrs as T;
            }

            return null;
        }


        public static bool IsWithinDate(this DateTime time, DateTime begin, DateTime end)
        {
            var beginDate = begin.DayOfYear;
            var endDate = end.DayOfYear;

            if (endDate < beginDate)
            {
                beginDate = end.DayOfYear;
                endDate = begin.DayOfYear;
            }

            return beginDate <= time.DayOfYear && time.DayOfYear <= endDate;
        }
        public static bool IsWithin(this DateTime time, DateTime begin, DateTime end)
        {
            if (end < begin)
            {
                var temp = begin;
                begin = end;
                end = temp;
            }
            return begin <= time && begin <= end;
        }

        public static List<string> GetEnumNames<TEnum>() where TEnum : struct
        {
            return Enum.GetNames(typeof(TEnum)).ToList();
        }
        public static TEnum? TryParseEnum<TEnum>(string enumString) where TEnum : struct
        {
            TEnum result;
            if (Enum.TryParse<TEnum>(enumString, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            if (type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
        public static TValue GetAttributeValue<TAttribute, TValue>(this PropertyInfo propInfo, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            if (propInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
        {
            var tType = typeof(TAttribute);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(tType, true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetTypesWithAttributeValue<TAttribute>(this Assembly assembly, Func<TAttribute, bool> valueSelector) where TAttribute : Attribute
        {
            var types = GetTypesWithAttribute<TAttribute>(assembly);

            foreach (Type type in types)
            {
                var attribute = type.GetCustomAttribute<TAttribute>(true);
                if (attribute != null)
                {
                    if (valueSelector(attribute))
                    {
                        yield return type;
                    }
                }
            }
        }


        public static bool IgnoreCaseContain(this string a, string b)
        {
            try
            {
                a = a?.ToUpper();
                b = b?.ToUpper();
                return a.Contains(b);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IgnoreCaseEqual(this string a, string b)
        {
            try
            {
                return string.Compare(a, b, true) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Attr GetAttr<Attr>(this Enum input) where Attr : Attribute
        {
            if (input != null)
            {
                var type = input.GetType();
                var m = type.GetMember(input.ToString()).FirstOrDefault();
                if (m != null)
                {
                    var att = m.GetCustomAttributes(typeof(Attr), true).FirstOrDefault() as Attr;
                    return att;
                }
            }
            return null;
        }
        public static T GetAttValue<T, Attr>(this Enum input, Func<Attr, T> func) where Attr : Attribute
        {
            if (input != null)
            {
                var att = GetAttr<Attr>(input);
                if (att != null)
                {
                    return func.Invoke(att);
                }
            }

            return default(T);
        }
        public static bool TryToEnum<TEnum>(this string value, out TEnum? result) where TEnum : struct
        {
            if (Enum.TryParse(value, out TEnum parseResult))
            {
                result = parseResult;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public static TEnum? TryToEnum<TEnum>(this string value) where TEnum : struct
        {
            if (Enum.TryParse(value, out TEnum parseResult))
            {
                return parseResult;
            }
            else
            {
                return null;
            }
        }

        public static EnumT? ToEnumByAttribute<EnumT, AttributeT>(this string value, Func<AttributeT, string> expresstion) where AttributeT : Attribute where EnumT : struct
        {
            return EnumLookUp<EnumT, AttributeT>(value, expresstion);
        }


        public static EnumT? EnumLookUp<EnumT, AttributeT>(string value, Func<AttributeT, string> expresstion) where AttributeT : Attribute where EnumT : struct
        {
            Type enumType = typeof(EnumT);
            Type attributeType = typeof(AttributeT);

            //if can parse return parse value
            var enumResult = TryToEnum<EnumT>(value);
            if (enumResult != null)
                return enumResult;

            //else lookup value by attribute
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                MemberInfo info = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
                AttributeT att = info?.GetCustomAttribute<AttributeT>();
                if (att != null)
                {
                    var valueList = expresstion(att)?.Split(';', '|', ',');
                    if (valueList.Contains(value, StringComparer.OrdinalIgnoreCase))
                    {
                        return (EnumT)enumValue;
                    }
                    else
                    {
                    }
                }
            }
            return default(EnumT?);
        }



        public static ICollection<PropertyInfo> GetPropertiesWithAttr<AttrType>(this Type type) where AttrType : Attribute
        {
            ICollection<PropertyInfo> result = new List<PropertyInfo>();
            if (type != null && type.IsClass)
            {
                result = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(AttrType))).ToList();
            }
            return result;
        }
        public static AttrType GetAttr<AttrType>(this PropertyInfo property) where AttrType : Attribute
        {
            if (property != null)
            {
                return property.GetCustomAttribute<AttrType>();
            }
            return null;
        }

        public static T? ConvertTo<T>(this string input, Func<string, T?> convertLogic) where T : struct
        {
            if (string.IsNullOrEmpty(input))
                return null;
            else
            {
                return convertLogic(input);
            }
        }
    }
}
