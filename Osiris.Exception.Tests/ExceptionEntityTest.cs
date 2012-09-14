using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inmeta.Exception.Service.Common;
using System.Runtime.Serialization;

namespace Inmeta.Exception.Tests
{
    [TestClass]
    public class ExceptionEntityTest
    {
        [TestMethod]
        public void TestDataContractSerialization()
        {
            var entity = new ExceptionEntity
            {
                ApplicationName = "AppName",
                ChangeSet = "Changeset",
                Comment = "Comment",
                ExceptionMessage = "ExMessage",
                ExceptionTitle = "ExTitle",
                ExceptionType = "ExType",
                Reporter = "Reporter",
                StackTrace = "StackTrace",
                TheClass = "Class",
                TheMethod = "Method",
                TheSource = "Source",
                Username = "username",
                Version = "1.0.0.0"
            };

            string str = entity.GetSerialized();

            var serializer = new DataContractSerializer(typeof(ExceptionEntity));
            var reader = new StringReader(str);
            var res = serializer.ReadObject(new XmlTextReader(reader)) as ExceptionEntity;
            
            Assert.IsNotNull(res);
            Assert.AreEqual(entity.Version, res.Version);
            Assert.AreEqual(entity.Username, res.Username);
            Assert.AreEqual(entity.TheSource, res.TheSource);
            Assert.AreEqual(entity.TheMethod, res.TheMethod);
            Assert.AreEqual(entity.TheClass, res.TheClass);
            Assert.AreEqual(entity.StackTrace, res.StackTrace);
            Assert.AreEqual(entity.Reporter, res.Reporter);
            Assert.AreEqual(entity.ExceptionType, res.ExceptionType);
            Assert.AreEqual(entity.ExceptionTitle, res.ExceptionTitle);
            Assert.AreEqual(entity.ExceptionMessage, res.ExceptionMessage);
            Assert.AreEqual(entity.Comment, res.Comment);
            Assert.AreEqual(entity.ChangeSet, res.ChangeSet);
            Assert.AreEqual(entity.ApplicationName, res.ApplicationName);
        }
    }
}
