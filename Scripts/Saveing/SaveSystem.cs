using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Debugging;
using UnityEngine;

namespace Saveing
{

    /// <summary>
    /// Used for saving, loading any serializable object to save within Unity.
    /// Uses the Application.persistentDataPath as path and stores the objects as .bin files.
    /// 
    /// inspired by https://www.youtube.com/watch?v=XOjd_qU2Ido&t=238s
    /// 
    /// 27.10.2021 - Bl4ck?
    /// </summary>
    public static class SaveSystem<T>
    {
        public const string FILE_EXTENSION = ".bin";

        public static string SubFolderFullPath { get => path + "/" + SubFolder; }

        public static string FileName { get => typeof(T).Name; }

        public static string SubFolder { get=> FileName + "s"; }

        private static string path = Application.persistentDataPath;
        private static string name = nameof(SaveSystem<T>); 
        private static EScriptGroup scriptGroup = EScriptGroup.SaveSystem;
        private static BinaryFormatter formatter = new BinaryFormatter();

        #region Saving -----------------------------------------------------------------
        public static void TrySaving(T _dataToSafe)
        {
            DebugManager.Output(scriptGroup, name, "Saving data which is " + _dataToSafe.ToString());
            Save(_dataToSafe, path, FileName);
        }

        public static void TrySaving(T _dataToSafe, string _file)
        {
            DebugManager.Output(scriptGroup, name, "Saving data which is " + _dataToSafe.ToString());
            Save(_dataToSafe, path, _file);
        }

        /// <summary>
        /// Try to save a file with a desired name.
        /// </summary>
        /// <param name="_file">File to load</param>
        public static void TrySavingSubFolder(T _dataToSafe, string _file)
        {
            DebugManager.Output(scriptGroup, name, "Saving data which is " + _dataToSafe.ToString());
            if (!Directory.Exists(path + "/" + SubFolder))
                Directory.CreateDirectory(path + "/" + SubFolder);
            Save(_dataToSafe, path + "/" + SubFolder + "/", _file);
        }

        public static void TrySavingTexture(Texture2D _texture, string _filePath, string _fileName)
        {
            DebugManager.Output(scriptGroup, name, "Saving texture which is " + _texture.ToString());
            byte[] byteStream = _texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(_filePath + "/" + _fileName + ".png", byteStream);
        }

        /// <summary>
        /// Saves some data using BinaryFormatter.
        /// </summary>
        /// <param name="_dataToSafe">The desired data to safe</param>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">The name used for saving</param>
        private static void Save(T _dataToSafe, string _filePath, string _fileName)
        {
            string path = _filePath + "/" + _fileName + FILE_EXTENSION;

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, _dataToSafe);
                stream.Close();
            }
        }
        #endregion -----------------------------------------------------------------

        #region Loading -----------------------------------------------------------------
        public static T TryLoading() => TryLoading(path, FileName + FILE_EXTENSION);
        
        public static T TestLoad(string _path)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(_path, FileMode.Open))
            {
                T data = (T)formatter.Deserialize(stream);
                stream.Close();
                return data;
            }
        } 

        public static Dictionary<string,T> TryLoadAll()
        {
            Dictionary<string, T> files = new Dictionary<string, T>();

            if (!Directory.Exists(path + "/" + SubFolder))
                return files;

            foreach(var file in Directory.EnumerateFiles(path + "/" + SubFolder))
            {
                
                FileInfo fileInfo = new FileInfo(file.Replace("\\", "/"));
                try
                {
                    T data = TryLoading(fileInfo.DirectoryName, fileInfo.Name);
                    if (data != null)
                        files.Add(fileInfo.FullName, data);
                }
                catch (Exception)
                {
                    DebugManager.OutputWarning(scriptGroup, name, "File: " + fileInfo.Name + " could not be loaded.");
                }
            }

            return files;
        }

        public static T TryLoadingSubFolder(string _fileName)
        {
            return TryLoading(path + "/" + SubFolder, _fileName + FILE_EXTENSION);
        }

        public static T TryLoadingSubFolderExtension(string _fileName)
        {
            return TryLoading(path + "/" + SubFolder, _fileName);
        }

        /// <summary>
        /// Try to load a file with a desired name.
        /// </summary>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">Name of the file</param>
        /// <returns>An object of containing the file</returns>
        private static T TryLoading(string _filePath, string _fileNameWithExtenstion)
        {
            T data = default;
            if (!ExistsWithExtension(_filePath, _fileNameWithExtenstion))
                throw new FileNotFoundException("No SaveFile found at " + _fileNameWithExtenstion + "!");
            
            data = Load(_filePath, _fileNameWithExtenstion);
            DebugManager.Output(scriptGroup, name, "Loading file " + _fileNameWithExtenstion +
                " with data which is:  " + data);

            if (data == null)
                throw new FileLoadException("Failed to load file " + _fileNameWithExtenstion + "!");

            return data;
        }

        private static T Load(string _filePath, string _fileNameWithExtenstion)
        {
            string path = _filePath + "/" + _fileNameWithExtenstion;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                T data = (T)formatter.Deserialize(stream);
                stream.Close();
                return data;
            }
        }
        #endregion -----------------------------------------------------------------

        #region Deleting -----------------------------------------------------------------
        /// <summary>
        /// Tries to delete a file.
        /// </summary>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">The desired filename</param>
        public static void TryDeleting(string _fileName)
        {
            Delete(path, _fileName);
        }

        /// <summary>
        /// Deletes a specific file.
        /// </summary>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">Name of the file</param>
        private static void Delete(string _filePath, string _fileName)
        {
            string path = _filePath + "/" + _fileName + ".bin";
            if (File.Exists(path))
            {
                DebugManager.Output(scriptGroup, name, "Deleting " + path);
                File.Delete(path);
            }
            else
                throw new FileNotFoundException("No SaveFile found at " + _filePath + "!");
        }
        #endregion -----------------------------------------------------------------

        #region Clarifictaion -----------------------------------------------------------------
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">Name of the file</param>
        /// <returns>True if it exists</returns>
        public static bool ExistsSubFolder(string _fileName) => 
            File.Exists(path + "/" + SubFolder + "/" + _fileName + FILE_EXTENSION);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <returns>True if it exists</returns>
        public static bool Exists() => File.Exists(path + "/" + FileName + FILE_EXTENSION);
        
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="_filePath">Path of the file</param>
        /// <param name="_fileName">Name of the file</param>
        /// <returns>True if it exists</returns>
        private static bool ExistsWithExtension(string _filePath, string _fileNameWithExtension) =>
            File.Exists(_filePath + "/" + _fileNameWithExtension);

        #endregion -----------------------------------------------------------------
    }
}
