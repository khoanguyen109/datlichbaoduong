using System;
using NHibernate;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using MaintenanceSchedule.Data.Vienauto;
using MaintenanceSchedule.Library.NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaintenanceSchedule.Tests.Mapping
{
    [TestClass]
    public class NhibernateTest
    {
        private readonly string factoryKey = "nhibernate.current_factory_key";

        public NhibernateTest()
        {
            SessionFactory.Instance.Initialize(factoryKey, "VienautoConnectionString", typeof(ManufacturerMap).Assembly);
        }

        [TestMethod]
        public void EntityMappingTest()
        {
            using (var session = SessionFactory.Instance.OpenSession())
            {
                var errors = new List<Exception>();
                var metaDataClasses = SessionFactory.Instance.GetSessionFactory(factoryKey).GetAllClassMetadata();

                foreach (KeyValuePair<string, NHibernate.Metadata.IClassMetadata> objClass in metaDataClasses)
                {
                    var entity = objClass.Value.GetMappedClass(EntityMode.Poco);
                    try
                    {
                        session.CreateCriteria(entity).SetMaxResults(0).List();
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }
                Assert.AreEqual(0, errors.Count);
            }
        }
    }
}
