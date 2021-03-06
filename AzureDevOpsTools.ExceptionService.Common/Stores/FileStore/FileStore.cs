﻿//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Xml.Serialization;
//using AzureDevOpsTools.Exception.Common;
//using Microsoft.Win32;

//namespace AzureDevOpsTools.ExceptionService.Common.Stores.FileStore
//{
//    public class FileStore
//    {
//        private const string FileName = "Exceptions.txt";
//        private const string PathExtension = @"AzureDevOps\Exceptions\";
//        private static readonly byte[] NewLineBytes;
//        private readonly object fileLockObject = new object();
        
//        //in MBs
//        public static float MaxFileSize { get; set; } = 10;

//        static FileStore()
//        {
//            NewLineBytes = UTF8Encoding.Default.GetBytes(System.Environment.NewLine);

//            //try to parse max size from application config.
//            float.TryParse(ConfigurationManager.AppSettings["ExceptionFileSizeInMB"], out var mfs);
//            MaxFileSize = mfs;
//            if (MaxFileSize < 10.0)
//                MaxFileSize = 10;
//        }

//        public void SaveException(ExceptionEntity exception)
//        {
//            var ser = new XmlSerializer(typeof(ExceptionEntity));
//            lock (fileLockObject)
//            {
//                using (
//                    var file = File.Open(ExceptionsFileName, FileMode.Append, FileAccess.Write, FileShare.None)
//                    )
//                {
//                    if (file.Length > MaxFileSize * 1000000)
//                    {
//                        throw new FileLoadException("The log file at " + ExceptionsFileName + " has exceeded max size of " + MaxFileSize + " MB. Exception will be discarded.");
//                    }
//                    //write xml 
//                    ser.Serialize(file, exception);

//                    //write a line break for readability
//                    file.Write(NewLineBytes, 0, NewLineBytes.Count());
//                }
//            }
//        }

//        public ExceptionEntity[] PopExceptions()
//        {
//            //no new exceptions. return empty list.
//            if (!File.Exists(ExceptionsFileName))
//                return new ExceptionEntity[]{};

//            var result = ParseExcpetions(ExceptionsFileName);
               
//            //save all failed entities into failed folder.
//            result.Item2.ForEach(MoveToFailedFolder);

//            try
//            {
//                //move old log file to previous folder
                
//                //First delete old previous file
//                File.Delete(OldPreviousExceptionsFileName);

//                //move previous to old previous
//                if (File.Exists(PreviousExceptionsFileName))
//                    File.Move(PreviousExceptionsFileName, OldPreviousExceptionsFileName);

//                //move current to previous
//                if (File.Exists(ExceptionsFileName))
//                    File.Move(ExceptionsFileName, PreviousExceptionsFileName);
//            }
//            catch (System.Exception ex)
//            {
//                ServiceLog.Error(
//                    $"Failed to clean up old exceptions. Due to exception: {System.Environment.NewLine}{ex}");
//            }

//            //all successfull entities
//            return result.Item1.ToArray();
//        }

//        public ExceptionEntity[] PopExceptionsWaitAck(string key)
//        {
//            //no new exceptions. return empty list.
//            if (!File.Exists(ExceptionsFileName))
//                return new ExceptionEntity[] { };

//            var result = ParseExcpetions(ExceptionsFileName);

//            //save all failed entities into failed folder.
//            if (result.Item2.Any())
//                result.Item2.ForEach(item => MoveToFailedFolder(item, key));

//            try
//            {
//                if (result.Item1.Any())
//                {
//                    //Smth to ack - move current to temporary
//                    File.Move(ExceptionsFileName, TempFileName(key));
//                }
//                else
//                {
//                    //Nothing to ack - rearrange files now

//                    //First delete old previous file
//                    File.Delete(OldPreviousExceptionsFileName);

//                    //move previous to old previous
//                    if (File.Exists(PreviousExceptionsFileName))
//                        File.Move(PreviousExceptionsFileName, OldPreviousExceptionsFileName);

//                    //move current to previous
//                    if (File.Exists(ExceptionsFileName))
//                        File.Move(ExceptionsFileName, PreviousExceptionsFileName);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                ServiceLog.Error("Failed to clean up old exceptions. Due to exception: " + System.Environment.NewLine + ex);
//            }

//            //all successfull entities
//            return result.Item1.ToArray();
//        }

//        public bool Ack(string key)
//        {
//            var res = true;
//            try
//            {

//                if (Directory.Exists(GetFolder(GetFailedExceptionFolder(key))))
//                {
//                    ServiceLog.Error("Ack delivery recieved, failed exception for session found." + System.Environment.NewLine);
//                    res = false;
//                }

//                if (File.Exists(TempFileName(key)))
//                {
//                    //First delete old previous file
//                    File.Delete(OldPreviousExceptionsFileName);

//                    //move previous to old previous
//                    if (File.Exists(PreviousExceptionsFileName))
//                        File.Move(PreviousExceptionsFileName, OldPreviousExceptionsFileName);

//                    //move current to previous
//                    if (File.Exists(TempFileName(key)))
//                        File.Move(TempFileName(key), PreviousExceptionsFileName);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                ServiceLog.Error("Failed to clean up old exceptions. Due to exception: " + System.Environment.NewLine + ex);
//            }
//            return res;
//        }

//        public Tuple<List<ExceptionEntity>, List<string>> ParseExcpetions(string filename)
//        {
//            //no namespace
//            var ser = new XmlSerializer(typeof(ExceptionEntity));

//            var exceptions = new List<ExceptionEntity>();
//            var failed = new List<string>();
//            lock (fileLockObject)
//            {
//                string log;

//                //get string from file
//                if (!ReadExceptionFile(filename, out log))
//                    return new Tuple<List<ExceptionEntity>, List<string>>(exceptions, failed);

//                //simple split strategy 
//                string splitPart = "<?xml";
                
//                //If some property in the ExceptionEntity contains the split argument is not a problem since XmlSerializer will escape the result.
//                var entities = log.Split(new string[] { splitPart },
//                                            StringSplitOptions.RemoveEmptyEntries);

//                //deserialize and add to list.
//                exceptions.AddRange(
//                    entities.Select(entity =>
//                    {
//                        try
//                        {
//                            //prepend with splitpart to generate valid xml.
//                            entity = splitPart + entity;

//                            //remove exceptions.maritimesim.com from namespace. This could be normalizing by using of exception entities, but for now quick fix.
//                            entity = entity.Replace("http://exceptions.maritimesim.com/", "");
                            
//                            var reader = new StringReader(entity.Trim());

//                            return ser.Deserialize(reader) as ExceptionEntity;
//                        }
//                        catch (InvalidOperationException ex)
//                        {              
//                            ServiceLog.Error("Failed to deserialized exceptions: File moved to failed folder. " 
//                                + System.Environment.NewLine + "Content: " 
//                                + System.Environment.NewLine + entity+ ex);
//                            failed.Add(entity);
//                        }
//                        return null;
//                    })
//                    //do not add if deserialize failed => exceptionentity == null
//                        .Where((exceptionEntResult) => exceptionEntResult != null)
//                    );

//                return new Tuple<List<ExceptionEntity>, List<string>>(exceptions, failed);
//            }
//        }

//        private static void MoveToFailedFolder(string errornousXml)
//        {
//            try
//            {

//                using (var stream = File.CreateText(FailedExceptionsFileName))
//                {
//                    stream.Write(errornousXml);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                //failed to move erronous xml to failed exceptions.
//                ServiceLog.Error("Failed to move erronous XML to failed file = " + FailedExceptionsFileName + ex);
//            }
//        }

//        private static void MoveToFailedFolder(string errornousXml, string key)
//        {
//            try
//            {
//                CleanFailedExceptions();
//                using (var stream = File.AppendText(GetFailedExceptionFileNameByKey(key)))
//                {
//                    stream.Write(errornousXml);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                //failed to move erronous xml to failed exceptions.
//                ServiceLog.Error("Failed to move erronous XML to failed file = " + FailedExceptionsFileName+ ex);
//            }
//        }

//        private static bool ReadExceptionFile(string filename, out string log)
//        {
//            log = "";
//            try
//            {
//                 if (File.Exists(filename))
//                    using (var file = File.OpenText(filename))
//                    {
//                        log = file.ReadToEnd();
//                        file.Close();
//                    }
//            }
//            catch (UnauthorizedAccessException)
//            {
//                //failed to read exceptions probably due to issues with file access.
//                ServiceLog.Warning("Tried to read exceptions file with result access denied. File = " +
//                                           filename);
//            }

//            return log.Length > 0;
//        }

//        private static string FailedExceptionsFileName => GetPath(Path.Combine(PathExtension, "failed"));

//        private static string GetFailedExceptionFileNameByKey(string key)
//        {
//            return GetPath(GetFailedExceptionFolder(key));
//        }

//        private static string GetFailedExceptionFolder(string key)
//        {
//            return Path.Combine(Path.Combine(PathExtension, "failed"), key);
//        }

//        private static void CleanFailedExceptions()
//        {
//            try
//            {
//                var dir = Directory.GetDirectories(GetFolder(Path.Combine(PathExtension, "failed"))).
//                    OrderByDescending(d => new DirectoryInfo(d).CreationTime);

//                int limit = 3;
//                for (int i = limit; i < dir.Count(); i++)
//                {
//                    Directory.Delete(dir.ElementAt(limit), true);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                ServiceLog.Error("Failed to clean up failed files folder : "+ ex);
//            }
//        }

//        private static string OldPreviousExceptionsFileName => Path.ChangeExtension(GetPath(Path.Combine(PathExtension, "old")), "old");

//        public static string PreviousExceptionsFileName => GetPath(Path.Combine(PathExtension, "old"));

//        public static string ExceptionsFileName => GetPath(PathExtension);

//        private static string TempFileName(string key)
//        {
//            string filePath= GetPath(PathExtension);
//            string path = Path.GetDirectoryName(filePath);
//            return Path.Combine(path, key);
//        }

//        private static string GetPath(string extension)
//        {
//            var path = GetFolder(extension);

//            //ensure path exists
//            Directory.CreateDirectory(path);
                    
//            //append filename 
//            path = System.IO.Path.Combine(path, FileName);

//            return path;
//        }

//        private static string GetFolder(string extension)
//        {
//            var localMachine = Registry.LocalMachine;
//            const string keypath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";

//            //default location to 
//            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

//            //override with Registry settings if available.
//            var key = localMachine.OpenSubKey(keypath);

//            if (key?.GetValue("Common AppData") != null)
//                path = key.GetValue("Common AppData").ToString();
            

//            path = System.IO.Path.Combine(path, extension);

//            return path;
//        }
//    }
//}
