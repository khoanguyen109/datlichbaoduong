using System;
using NUnit.Framework;
using yellowx.Framework.Web.Mvc.Extensions;

namespace yellowx.Framework.Tests.Web.Mvc.Extensions
{
    [TestFixture]
    public class SelectListExtensionTests
    {
        [Test]
        public void CanEnumToSelectList()
        {
            AnimalType? type = AnimalType.Ox;
            var selectList = type.NullableEnumToSelectList("Animal Type");
            selectList.ForEach(item =>
            {
                Console.WriteLine($"Text: {item.Text}, Value: {item.Value}");
            });
        }
    }

    public enum AnimalType
    {
        [System.ComponentModel.Description("Bò")]
        Ox,
        [System.ComponentModel.Description("Heo")]
        Pig,
        [System.ComponentModel.Description("Gà")]
        Chicken,
        [System.ComponentModel.Description("Dê")]
        Goat
    }
}
