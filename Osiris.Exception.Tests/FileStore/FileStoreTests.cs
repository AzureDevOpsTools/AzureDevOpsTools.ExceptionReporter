using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fasterflect;
using Inmeta.Exception.Service.Common.Stores.FileStore;
using Inmeta.Exception.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inmeta.Exception.Tests.FileStore
{
    [TestClass]
    public class FileStoreTests
    {
        [TestMethod]
        public void FileStore_EnsureExceptionFileIsRemovedAfterPop()
        {
            for (var i = 0; i < 10; i++)
            {
                var ex = new Inmeta.Exception.Service.Common.ExceptionEntity()
                             {
                                 ApplicationName =
                                     ExceptionTestConstants.RndStrLength(50) + "<?xml version_" +
                                     ExceptionTestConstants.RndStrLength(10),
                                 ChangeSet = ExceptionTestConstants.RndStrLength(10),
                                 Comment = ExceptionTestConstants.RndStrLength(10),
                                 ExceptionMessage = ExceptionTestConstants.RndStrLength(10),
                                 ExceptionTitle = ExceptionTestConstants.RndStrLength(10),
                                 ExceptionType = ExceptionTestConstants.RndStrLength(10),
                                 Reporter = ExceptionTestConstants.RndStrLength(10),
                                 StackTrace = ExceptionTestConstants.RndStrLength(10),
                                 TheClass = ExceptionTestConstants.RndStrLength(10),
                                 TheMethod = ExceptionTestConstants.RndStrLength(10),
                                 TheSource = ExceptionTestConstants.RndStrLength(10),
                                 Username = ExceptionTestConstants.RndStrLength(10),
                                 Version = ExceptionTestConstants.RndStrLength(10)
                             };

                new Service.Common.Stores.FileStore.FileStore().SaveException(ex);
            }

            var ent = new Service.Common.Stores.FileStore.FileStore().PopExceptions();

            //check if log folder is empty 
            Assert.IsFalse(File.Exists(FileStore_Accessor.ExceptionsFileName), "File exists after popExceptions, After PopExceptions should log file be moved to previous = " + FileStore_Accessor.ExceptionsFileName);
            
            //previous log file is moved to previous folder.
            Assert.IsTrue(File.Exists(FileStore_Accessor.PreviousExceptionsFileName), "Previous do not exist, After PopExceptions should log file be moved to previous folder = "  + FileStore_Accessor.PreviousExceptionsFileName);
        }

        [TestMethod]
        public void FileStore_SaveOneExceptionToFile()
        {
            //clean up old exceptions 
            new Service.Common.Stores.FileStore.FileStore().PopExceptions();
            
            var ex = new Inmeta.Exception.Service.Common.ExceptionEntity()
            {
                ApplicationName = ExceptionTestConstants.RndStrLength(50) + "<?xml version_" + ExceptionTestConstants.RndStrLength(10),
                ChangeSet = ExceptionTestConstants.RndStrLength(10),
                Comment = ExceptionTestConstants.RndStrLength(10),
                ExceptionMessage = ExceptionTestConstants.RndStrLength(10),
                ExceptionTitle = ExceptionTestConstants.RndStrLength(10),
                ExceptionType = ExceptionTestConstants.RndStrLength(10),
                Reporter = ExceptionTestConstants.RndStrLength(10),
                StackTrace = ExceptionTestConstants.RndStrLength(10),
                TheClass = ExceptionTestConstants.RndStrLength(10),
                TheMethod = ExceptionTestConstants.RndStrLength(10),
                TheSource = ExceptionTestConstants.RndStrLength(10),
                Username = ExceptionTestConstants.RndStrLength(10),
                Version = ExceptionTestConstants.RndStrLength(10)
            };
            
            new Service.Common.Stores.FileStore.FileStore().SaveException(ex);


            var ent = new Service.Common.Stores.FileStore.FileStore().PopExceptions()[0];
            //ensure values are the same.
            ent.GetType().GetProperties().ToList().ForEach(
               (prop) =>
               Assert.IsTrue(ent.GetPropertyValue(prop.Name).ToString() == ex.GetPropertyValue(prop.Name).ToString()
               || ent.Comment.Contains(ex.Comment), "Property " + prop.Name + " does not equal org after loaded from file store")
               ); 
        }

        [TestMethod]
        public void FileStore_RestorInvalidXML()
        {
            //clean up old exceptions 
            new Service.Common.Stores.FileStore.FileStore().PopExceptions();

            var ex = new Inmeta.Exception.Service.Common.ExceptionEntity()
            {
                ApplicationName = ExceptionTestConstants.RndStrLength(50) + "<?xml version_" + ExceptionTestConstants.RndStrLength(10),
                ChangeSet = ExceptionTestConstants.RndStrLength(10),
                Comment = ExceptionTestConstants.RndStrLength(10),
                ExceptionMessage = ExceptionTestConstants.RndStrLength(10),
                ExceptionTitle = ExceptionTestConstants.RndStrLength(10),
                ExceptionType = ExceptionTestConstants.RndStrLength(10),
                Reporter = ExceptionTestConstants.RndStrLength(10),
                StackTrace = ExceptionTestConstants.RndStrLength(10),
                TheClass = ExceptionTestConstants.RndStrLength(10),
                TheMethod = ExceptionTestConstants.RndStrLength(10),
                TheSource = ExceptionTestConstants.RndStrLength(10),
                Username = ExceptionTestConstants.RndStrLength(10),
                Version = ExceptionTestConstants.RndStrLength(10)
            };

            new Service.Common.Stores.FileStore.FileStore().SaveException(ex);

            var modified = "";
            //load file and remove some stuff
            using (var stream = File.OpenText(FileStore_Accessor.ExceptionsFileName))
            {
                modified = stream.ReadToEnd().Replace("xml version", "sdfds");
            }

            //rewrite file 
            using (var rewrite = File.OpenWrite(FileStore_Accessor.ExceptionsFileName))
            {
                var bytes = ASCIIEncoding.Default.GetBytes(modified);
                rewrite.Write(bytes, 0, bytes.Length);
            }

            Assert.IsTrue( new Service.Common.Stores.FileStore.FileStore().PopExceptions().Length == 0);
        }


        [TestMethod]
        public void FileStore_LogFileIsToBig()
        {
            new Service.Common.Stores.FileStore.FileStore().PopExceptions();

            var ex = new Inmeta.Exception.Service.Common.ExceptionEntity()
            {
                ApplicationName = ExceptionTestConstants.RndStrLength(1000) + "<?xml version_" + ExceptionTestConstants.RndStrLength(10),
                ChangeSet = ExceptionTestConstants.RndStrLength(1000),
                Comment = ExceptionTestConstants.RndStrLength(1000),
                ExceptionMessage = ExceptionTestConstants.RndStrLength(1000),
                ExceptionTitle = ExceptionTestConstants.RndStrLength(1000),
                ExceptionType = ExceptionTestConstants.RndStrLength(1000),
                Reporter = ExceptionTestConstants.RndStrLength(1000),
                StackTrace = ExceptionTestConstants.RndStrLength(1000),
                TheClass = ExceptionTestConstants.RndStrLength(1000),
                TheMethod = ExceptionTestConstants.RndStrLength(1000),
                TheSource = ExceptionTestConstants.RndStrLength(1000),
                Username = ExceptionTestConstants.RndStrLength(1000),
                Version = ExceptionTestConstants.RndStrLength(1000)
            };

            new Service.Common.Stores.FileStore.FileStore().SaveException(ex);

            //0.01 MB = 10K
            FileStore_Accessor._maxFileSize = 0.01F;

            ex = new Inmeta.Exception.Service.Common.ExceptionEntity()
            {
                ApplicationName = ExceptionTestConstants.RndStrLength(1000) + "<?xml version_" + ExceptionTestConstants.RndStrLength(10),
                ChangeSet = ExceptionTestConstants.RndStrLength(1000),
                Comment = ExceptionTestConstants.RndStrLength(1000),
                ExceptionMessage = ExceptionTestConstants.RndStrLength(1000),
                ExceptionTitle = ExceptionTestConstants.RndStrLength(1000),
                ExceptionType = ExceptionTestConstants.RndStrLength(1000),
                Reporter = ExceptionTestConstants.RndStrLength(1000),
                StackTrace = ExceptionTestConstants.RndStrLength(1000),
                TheClass = ExceptionTestConstants.RndStrLength(1000),
                TheMethod = ExceptionTestConstants.RndStrLength(1000),
                TheSource = ExceptionTestConstants.RndStrLength(1000),
                Username = ExceptionTestConstants.RndStrLength(1000),
                Version = ExceptionTestConstants.RndStrLength(1000)
            };

            try
            {
                new Service.Common.Stores.FileStore.FileStore().SaveException(ex);
                Assert.Fail("Exception should have exceeded max file size");
            }
            catch (FileLoadException)
            {

            }

            Assert.IsTrue(new Service.Common.Stores.FileStore.FileStore().PopExceptions().Length == 1, "Only first exception should be stored, second should never be stored since maxfilesize is exceeded.");
        }
    }
}
