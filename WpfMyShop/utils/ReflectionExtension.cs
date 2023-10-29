using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyShop.utils
{
    public static class ReflectionExtension
    {
        public static void CopyProperties(this object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            var sourceProps = source.GetType().GetProperties();

            foreach (var sourceProp in sourceProps)
            {
                if (!sourceProp.CanRead) continue;

                var targetProp = targetType.GetProperty(sourceProp.Name);
                if (targetProp == null) continue;
                if (!targetProp.CanWrite) continue;
                if (targetProp.GetSetMethod(true) != null
                    && targetProp.GetSetMethod(true)!.IsPrivate) continue;
                if ((targetProp.GetSetMethod()!.Attributes
                    & System.Reflection.MethodAttributes.Static) != 0) continue;
                if (!targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType)) continue;

                // Pass all tests, let's assign
                targetProp.SetValue(target, sourceProp.GetValue(source, null), null);
            }
        }
    }
}
