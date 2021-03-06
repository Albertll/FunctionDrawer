﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FunctionDrawer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DrawerMainView());
        }

        public static void ForEach<T>(this IEnumerable<T> objects, Action<T> action)
        {
            foreach (var obj in objects)
            {
                action(obj);
            }
        }
    }
}
