﻿using System;
using System.Windows.Forms;

namespace PK
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Forms.Authorization form = new Forms.Authorization();
            if (form.ShowDialog() == DialogResult.OK)
                Application.Run(new Forms.Main(form.UsersRole, form.UsersLogin));

            Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Приложение продолжит работу. Сообщите администратору о возникновении ошибки.", "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.log", true))
                writer.Write("\r\n\r\n" + DateTime.Now.ToString() + "\r\n" + e.Exception.ToString());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show("Приложение будет закрыто.", "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.log", true))
                    writer.Write("\r\n\r\n" + DateTime.Now.ToString() + " CRITICAL ERROR \r\n" + e.ExceptionObject.ToString());
            }
        }
    }
}
