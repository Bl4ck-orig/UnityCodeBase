using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Debugging
{

    public class DebugManager : MonoBehaviour
    {
        public static Action SaveLog { get; set; }


        [SerializeField] private bool activateDebugOutput;
        [Header("Exclude")]
        [SerializeField] private List<string> excludeClasses;
        [SerializeField] private List<EScriptGroup> excludedScriptGroups;
        [Header("Include")]
        [SerializeField] private bool onlyUseIncludedClasses;
        [SerializeField] private List<string> includeClasses;
        [SerializeField] private List<EScriptGroup> includeScriptGroups;

        private string log = "Log created " + System.DateTime.Now + "\n\n";
        private TextWriter tw;
        private static DebugManager instance;

        #region Initialization -----------------------------------------------------------------
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            
            instance = this;

            SaveLog += OnSaveLog;

            SetUpVariables();
        }

        private void SetUpVariables()
        {
            if (excludeClasses == null)
                excludeClasses = new List<string>();

            if (includeClasses == null)
                includeClasses = new List<string>();

            if (excludedScriptGroups == null)
                excludedScriptGroups = new List<EScriptGroup>();

            if (includeScriptGroups == null)
                includeScriptGroups = new List<EScriptGroup>();
        }

        /// <summary>
        /// Subscribes the log function to the applications logging services.
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }
        #endregion -----------------------------------------------------------------

        private static bool IsDebuggingActive() => instance != null && instance.activateDebugOutput;

        private bool ShouldPrint(EScriptGroup _scriptGroup, string _sender)
        {
            if (excludedScriptGroups.Contains(_scriptGroup))
                return false;

            if (excludeClasses.Contains(_sender))
                return false;

            if (onlyUseIncludedClasses && !(includeClasses.Contains(_sender) || includeScriptGroups.Contains(_scriptGroup)))
                return false;

            return true;
        }

        #region Print Normal -----------------------------------------------------------------
        /// <summary>
        /// Prints the desired output if there is an instance of the class and it is said to do ouputs.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void Output(EScriptGroup _scriptGroup, string _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            if (!instance.ShouldPrint(_scriptGroup, _sender))
                return;
      
            Debug.Log(Time.frameCount + " | <" + _sender + "> " + _output);
        }

        /// <summary>
        /// Prints the desired output if there is an instance of the class and it is said to do ouputs.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void Output(EScriptGroup _scriptGroup, System.Object _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            string sender = _sender.GetType().ToString();

            if (!instance.ShouldPrint(_scriptGroup, sender))
                return;

            Debug.Log(Time.frameCount + " | <" + _sender + "> " + _output);
        }

        /// <summary>
        /// Prints the desired output more detailed, if the class is an it is said to do ouputs.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void OutputDetailed(EScriptGroup _scriptGroup, System.Object _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            string sender = _sender.GetType().ToString();

            if (!instance.ShouldPrint(_scriptGroup, sender))
                return;

            Debug.Log(Time.frameCount + " | <" + _sender.ToString() + "> " + _output);        
        }
        #endregion -----------------------------------------------------------------

        #region Print Warning -----------------------------------------------------------------
        /// <summary>
        /// Prints the desired output if there is an instance of the class and it is said to do ouputs as a warning.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void OutputWarning(EScriptGroup _scriptGroup, string _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            if (!instance.ShouldPrint(_scriptGroup, _sender))
                return;

            Debug.LogWarning(Time.frameCount + " | <" + _sender + "> " + _output);
        }


        /// <summary>
        /// Prints the desired output if there is an instance of the class and it is said to do ouputs as a warning.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void OutputWarning(EScriptGroup _scriptGroup, System.Object _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            string sender = _sender.GetType().ToString();

            if (!instance.ShouldPrint(_scriptGroup, sender))
                return;

            Debug.LogWarning(Time.frameCount + " | <" + sender + "> " + _output);        
        }

        /// <summary>
        /// Prints the desired output as a warning more detailed, if the class is an it is said to do ouputs.
        /// </summary>
        /// <param name="_scriptGroup">The type of the script</param>
        /// <param name="_sender">The send of the output</param>
        /// <param name="_output">The desired output</param>
        public static void OutputWarningDetailed(EScriptGroup _scriptGroup, System.Object _sender, string _output)
        {
            if (!IsDebuggingActive())
                return;

            string sender = _sender.GetType().ToString();

            if (!instance.ShouldPrint(_scriptGroup, sender))
                return;

            Debug.LogWarning(Time.frameCount + " | <" + _sender.ToString() + "> " + _output);
        
        }
        #endregion -----------------------------------------------------------------

        #region Logging -----------------------------------------------------------------
        /// <summary>
        /// Logs any output to the console in a string
        /// </summary>
        /// <param name="_logstring">Text to be logged</param>
        /// <param name="_stackTrace">Stacktrace of the message</param>
        /// <param name="_type">Type of the log</param>
        private void Log(string _logstring, string _stackTrace, LogType _type)
        {
            // Warnings are ignored
            if (_type == LogType.Warning)
                return;
            log += "[" + System.DateTime.Now + "] " + _logstring + "\n";
            // In case of an error or exception the stacktrace is saved
            if (_type == LogType.Error || _type == LogType.Exception)
                log += _stackTrace + "\n";
        }

        /// <summary>
        /// Saves the report to file.
        /// </summary>
        private void OnSaveLog()
        {
            string filename = Application.dataPath + "/BugReport-" + GetHashString(System.DateTime.Now.ToString()) + ".txt";
            if (File.Exists(filename))
                File.Delete(filename);
            tw = new StreamWriter(filename, true);
            tw.WriteLine(log);
            log = "Buglog created " + System.DateTime.Now + "\n\n";
            tw.Close();
        }

        /// <summary>
        /// Computes a hash.
        /// by https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        /// </summary>
        /// <param name="inputString">String to be hashed</param>
        /// <returns>Hash of string as byte array</returns>
        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Computes a hash.
        /// by https://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        /// </summary>
        /// <param name="inputString">String to be hashed</param>
        /// <returns>Hash of string</returns>
        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        #endregion -----------------------------------------------------------------

        #region CleanUp -----------------------------------------------------------------
        private void OnDestroy() => SaveLog -= OnSaveLog;

        private void OnDisable() => Application.logMessageReceived -= Log;
        #endregion -----------------------------------------------------------------

    }
}
