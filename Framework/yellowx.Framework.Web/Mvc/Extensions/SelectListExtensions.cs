using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Web.Mvc.Extensions
{
    public static class SelectListExtensions
    {
        public static List<SelectListItem> EnumToSelectList<TSource>(this TSource e, string unselectedText = null, TSource firstInList = default(TSource), params TSource[] exclude)
            where TSource : struct
        {
            return new TSource?(e).NullableEnumToSelectList(unselectedText, firstInList, exclude);
        }

        private static SelectListItem EnumToSelectListItem<TSource>(TSource? e, FieldInfo enumValue) where TSource : struct
        {
            var friendlyNameAttribute =
                            enumValue.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                .Cast<System.ComponentModel.DescriptionAttribute>()
                                .FirstOrDefault();

            var value = enumValue.GetValue(e);

            return new SelectListItem
            {
                Selected = value.Equals(e),
                Text = friendlyNameAttribute == null ? value.ToString().PutSpacesInPascalCase() : friendlyNameAttribute.Description,
                Value = value.ToString()
            };
        }

        public static List<SelectListItem> NullableEnumToSelectList<TSource>(this TSource? e, string unselectedText = null, TSource firstInList = default(TSource), params TSource[] exclude)
            where TSource : struct
        {
            var type = typeof(TSource);
            var enumMembers = type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => !f.Name.IsIn(exclude.Select(ex => ex.ToString())));

            int startPos = -1;
            var enumOptionsEnd = enumMembers.TakeWhile(n =>
            {
                startPos++;
                return !n.GetValue(e).Equals(firstInList);
            })
            .ToArray();

            var enumOptions = enumMembers
                .Skip(startPos)
                .Union(enumOptionsEnd)
                .Select(n => EnumToSelectListItem(e, n));

            if (unselectedText != null)
            {
                enumOptions = new[] { new SelectListItem() { Value = "", Text = unselectedText } }
                    .Union(enumOptions);
            }

            return enumOptions.ToList();
        }
    }
}
