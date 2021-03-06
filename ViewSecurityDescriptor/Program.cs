﻿//  Copyright 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet;
using NtApiDotNet.Win32;
using System;
using System.Windows.Forms;

namespace ViewSecurityDescriptor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    MessageBox.Show("Usage: ViewSecurityDescriptor.exe (handle [--readonly]|Name SDDL NtType)", "Usage", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (args.Length < 3)
                    {
                        var handle = new SafeKernelObjectHandle(new IntPtr(int.Parse(args[0])), true);
                        bool read_only = args.Length > 1 ? args[1].Equals("--readonly") : false;
                        using (var obj = NtGeneric.FromHandle(handle))
                        {
                            SecurityUtils.EditSecurity(IntPtr.Zero, obj, obj.Name, read_only);
                        }
                    }
                    else
                    {
                        SecurityDescriptor sd = new SecurityDescriptor(args[1]);
                        NtType type = NtType.GetTypeByName(args[2], false);
                        if (type == null)
                        {
                            throw new ArgumentException(string.Format("Unknown NT type {0}", args[2]));
                        }
                        SecurityUtils.EditSecurity(IntPtr.Zero, args[0], sd, type);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
