using System;

namespace yellowx.Framework
{
    public class Assert
    {
        public static void CannotNull(object target, string message)
        {
            if (target == null)
                throw new Exception(message);
        }

        public static void IsNotTrue(bool @true, string message)
        {
            if (!@true)
                throw new Exception(message);
        }
    }
}
