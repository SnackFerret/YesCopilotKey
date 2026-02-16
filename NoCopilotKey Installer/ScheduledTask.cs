using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace NoCopilotKey_Installer
{
    public static class ScheduledTask
    {
        //static IRegisteredTask EnsureScheduledTask()
        //{
        //    string exeName = GetExeName();
        //    string taskName = "NoCopilotKey";
        //    string author = "Dan Weiss (www.dwedit.org)";
        //    string description = "Changes Copilot keyboard key into Right Ctrl key";
        //
        //    return CreateScheduledTask(exeName, taskName, author, description);
        //}

        static ITaskService _taskService;
        public static ITaskService GetTaskService()
        {
            try
            {
                if (_taskService == null)
                {
                    var typeOfTaskService = Type.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD"), false);
                    _taskService = (ITaskService)Activator.CreateInstance(typeOfTaskService);
                    _taskService.Connect(null, null, null, null);
                }
                return _taskService;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        static ITaskFolder _taskRootFolder;
        public static ITaskFolder GetTaskRootFolder()
        {
            try
            {
                if (_taskRootFolder == null)
                {
                    var taskService = GetTaskService();
                    _taskRootFolder = taskService.GetFolder("\\");
                }
                return _taskRootFolder;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IRegisteredTask GetScheduledTask(string taskName)
        {
            var taskRootFolder = GetTaskRootFolder();
            try
            {
                return taskRootFolder.GetTask(taskName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool RemoveScheduledTask(string taskName)
        {
            var taskRootFolder = GetTaskRootFolder();
            try
            {
                taskRootFolder.DeleteTask(taskName, 0);
                return true;
            }
            catch (Exception ex)
            {
                //FileNotFoundException
                //UnauthorizedAccessException
                return false;
            }
        }

        //static bool VerifyTaskSettingsOkay(IRegisteredTask registeredTask, string exePath)
        //{
        //    if (registeredTask == null) return false;
        //    try
        //    {
        //        var taskDefinition = registeredTask.Definition;
        //        var actions = taskDefinition.Actions;
        //        if (actions.Count != 1) return false;
        //        var action = actions[1];
        //        if (action.Type != _TASK_ACTION_TYPE.TASK_ACTION_EXEC) return false;
        //        var execAction = (IExecAction)action;
        //        if (execAction.Path != exePath) return false;
        //        var principal = taskDefinition.Principal;
        //        if (principal.RunLevel != _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST) return false;
        //        if (principal.UserId != null) return false;
        //        if (principal.GroupId != "Users") return false;
        //        var taskSettings = taskDefinition.Settings;
        //        if (taskSettings.AllowDemandStart != true) return false;
        //        if (taskSettings.AllowHardTerminate != true) return false;
        //        if (taskSettings.Compatibility != _TASK_COMPATIBILITY.TASK_COMPATIBILITY_V2_1) return false;
        //        if (taskSettings.DisallowStartIfOnBatteries != false) return false;
        //        if (taskSettings.Enabled != true) return false;
        //        if (taskSettings.RunOnlyIfIdle != false) return false;
        //        if (taskSettings.RunOnlyIfNetworkAvailable != false) return false;
        //        if (taskSettings.StopIfGoingOnBatteries != false) return false;
        //        if (taskSettings.MultipleInstances != _TASK_INSTANCES_POLICY.TASK_INSTANCES_STOP_EXISTING) return false;
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public static TaskScheduler.IRegisteredTask CreateScheduledTask(string exePath, string taskName, string author, string description)
        {
            const int TASK_CREATE_OR_UPDATE = 6;
            var taskService = GetTaskService();
            var rootFolder = GetTaskRootFolder();

            ////const string taskName = "NoCopilotKey";
            //var typeOfTaskService = Type.GetTypeFromCLSID(new Guid("0F87369F-A4E5-4CFC-BD3E-73E6154572DD"));
            //var taskService = (TaskScheduler.ITaskService)Activator.CreateInstance(typeOfTaskService);
            ////System.Runtime.InteropServices.VariantWrapper variantWrapper = new System.Runtime.InteropServices.VariantWrapper(null);
            //taskService.Connect(null, null, null, null);
            //var rootFolder = taskService.GetFolder("\\");

            TaskScheduler.IRegisteredTask registeredTask = null;
            //try
            //{
            //    registeredTask = rootFolder.GetTask(taskName);
            //}
            //catch (Exception ex)
            //{
            //    //FileNotFoundException
            //}
            if (registeredTask == null)
            {
                try
                {
                    var taskDefinition = taskService.NewTask(0);
                    var action = taskDefinition.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                    var execAction = (IExecAction)action;
                    execAction.Path = exePath;
                    var principal = taskDefinition.Principal;
                    principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
                    principal.GroupId = "Users";
                    var triggers = taskDefinition.Triggers;
                    var trigger = triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
                    var taskSettings = taskDefinition.Settings;
                    taskSettings.AllowDemandStart = true;
                    taskSettings.AllowHardTerminate = true;
                    taskSettings.Compatibility = TaskScheduler._TASK_COMPATIBILITY.TASK_COMPATIBILITY_V2_1;
                    taskSettings.DisallowStartIfOnBatteries = false;
                    taskSettings.Enabled = true;
                    taskSettings.RunOnlyIfIdle = false;
                    taskSettings.RunOnlyIfNetworkAvailable = false;
                    taskSettings.StopIfGoingOnBatteries = false;
                    taskSettings.ExecutionTimeLimit = "PT0S";
                    taskSettings.MultipleInstances = TaskScheduler._TASK_INSTANCES_POLICY.TASK_INSTANCES_STOP_EXISTING;
                    taskDefinition.RegistrationInfo.Author = author;
                    taskDefinition.RegistrationInfo.Description = description;
                    registeredTask = rootFolder.RegisterTaskDefinition(taskName, taskDefinition, TASK_CREATE_OR_UPDATE, null, null, TaskScheduler._TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, null);
                }
                catch (Exception ex)
                {
                    //UnauthorizedAccessException
                    return null;
                }
            }
            return registeredTask;
        }

    }
}
