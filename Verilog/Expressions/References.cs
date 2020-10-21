﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class TaskReference : Primary
    {
        public string TaskName { get; protected set; }
        public string ModuleName { get; protected set; }

        protected TaskReference()
        {
        }

        private System.WeakReference<Task> taskReferenceRef;
        public Task Task
        {
            get
            {
                Task ret;
                if (!taskReferenceRef.TryGetTarget(out ret)) return null;
                return ret;
            }
            protected set
            {
                taskReferenceRef = new WeakReference<Task>(value);
            }
        }

        public static TaskReference ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            TaskReference ret = new TaskReference();
            ret.TaskName = word.Text;
            ret.ModuleName = nameSpace.Module.Name;
            word.Color(CodeDrawStyle.ColorType.Identifier);
            word.MoveNext();
            if (nameSpace.Module.Tasks.ContainsKey(ret.TaskName))
            {
                ret.Task = nameSpace.Module.Tasks[ret.TaskName];
            }

            return ret;
        }
    }

    public class NameSpaceReference : Primary
    {
        public string Name { get; protected set; }

        private System.WeakReference<NameSpace> nameSpaceRef;
        public NameSpace NameSpace
        {
            get
            {
                NameSpace ret;
                if (!nameSpaceRef.TryGetTarget(out ret)) return null;
                return ret;
            }
            protected set
            {
                nameSpaceRef = new WeakReference<NameSpace>(value);
            }
        }

        public NameSpaceReference(NameSpace nameSpace)
        {
            Name = nameSpace.Name;
            NameSpace = nameSpace;
        }
    }

    public class ModuleInstanceReference : Primary
    {
        ModuleItems.ModuleInstantiation moduleInstantiation;
        public ModuleInstanceReference(ModuleItems.ModuleInstantiation moduleInstantiation)
        {
            this.moduleInstantiation = moduleInstantiation;
        }
    }



}
