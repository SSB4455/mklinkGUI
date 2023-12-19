using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace mklinkGUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button != null)
			{
				VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
				folderBrowserDialog.Reset();
				folderBrowserDialog.Description = "选择源目录";
				folderBrowserDialog.UseDescriptionForTitle = true;
				folderBrowserDialog.SelectedPath = @"\\" + button.Text + Path.DirectorySeparatorChar;

				// 创建一个新的线程来发送按键
				Thread keyPressThread = new Thread(new ThreadStart(SendEnterKey));
				keyPressThread.Start();
				
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					string selectedFolder = folderBrowserDialog.SelectedPath;
					//MessageBox.Show("您选择的网络文件夹是：" + selectedFolder);
					Console.WriteLine("selectedFolder: " + selectedFolder);

					LinkCmd2(selectedFolder);
				}
			}
		}

		private void textBox1_OnKeyDownHandler(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				LinkCmd2(textBox1.Text);
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			LinkCmd2(textBox1.Text);
		}

		private void LinkCmd2(string sourceFolder)
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				MessageBox.Show("请选择源目录", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "选择要映射到的本地目录";
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				string targetFolder = folderBrowserDialog.SelectedPath;
				//MessageBox.Show("You selected: " + selectedFolder);

				if (!targetFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					targetFolder += Path.DirectorySeparatorChar;
				}
				targetFolder += GetFolderName(sourceFolder);
				string outputError = RunCmd("mklink /d " + targetFolder + " " + sourceFolder);
				if (!string.IsNullOrEmpty(outputError))
				{
					MessageBox.Show(outputError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				Process.Start(targetFolder);
			}
		}

		string GetFolderName(string pathStr)
		{
			string folderName = "";
			try {
				if (Directory.Exists(pathStr))
				{
					folderName = new DirectoryInfo(pathStr).Name;
				}
			}
			catch { }

			if (string.IsNullOrEmpty(folderName))
			{
				folderName = pathStr;
				int startIndex = pathStr.LastIndexOf(Path.DirectorySeparatorChar);
				int endIndex = startIndex;
				if (pathStr.Split(Path.DirectorySeparatorChar).Length > 2 && endIndex > 0)
				{
					startIndex = pathStr.LastIndexOf(Path.DirectorySeparatorChar, endIndex - 1) + 1;
				}
				else
				{
					endIndex = pathStr.Length - 1;
				}
				folderName = pathStr.Substring(startIndex, endIndex - startIndex);
			}
			folderName = folderName.Replace(":", "").Replace(";", "").Replace(Path.DirectorySeparatorChar, '-');
			return folderName;
		}

		string RunCmd(string cmd)
		{
			Console.WriteLine("RunCmd: " + cmd);
			// 创建进程对象并设置要运行的命令及参数
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";   // 指定要运行的命令为 cmd.exe（Windows 系统）或者 bash（Linux/MacOS 系统）
			process.StartInfo.UseShellExecute = false; //此处必须为false否则引发异常
			process.StartInfo.RedirectStandardInput = true; //标准输入
			process.StartInfo.RedirectStandardOutput = true; //标准输出
			process.StartInfo.RedirectStandardError = true;

			process.StartInfo.Verb = "runas";	//使用管理员权限

			//不显示命令行窗口界面
			//process.StartInfo.CreateNoWindow = true;
			//process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			//string output = "";

			// 开始运行进程
			process.Start();

			/*process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.OutputDataReceived += (sender, args) =>
			{
				Console.WriteLine("OutputDataReceived: " + args.Data);
				output += "\n" + args.Data;
			};
			process.ErrorDataReceived += (sender, args) =>
			{
				Console.WriteLine("ErrorDataReceived: " + args.Data);
				output += "\n" + args.Data;
			};*/

			process.StandardInput.WriteLine(cmd);
			process.StandardInput.AutoFlush = true;
			//process.StandardInput.Flush();
			process.StandardInput.WriteLine("exit");
			//process.StandardInput.Flush();

			// 获取输出信息
			string output = process.StandardOutput.ReadToEnd();
			Console.WriteLine("output: " + output);
			string error = process.StandardError.ReadToEnd();
			Console.WriteLine("error:" + error);

			// 等待进程结束
			process.WaitForExit();
			process.Close();

			//return SplitCmdCurrentPath(output) + error;
			return error;
		}

		string SplitCmdCurrentPath(string output)
		{
			string path = System.Environment.CurrentDirectory;
			int index1 = output.IndexOf(path, 0) + path.Length;
			int index2 = output.IndexOf(path, index1);
			return output.Substring(index1, index2 - index1);
		}

		private void SendEnterKey()
		{
			// 等待一段时间后发送 Enter 键
			Thread.Sleep(100);
			SendKeys.SendWait("{ENTER}");
		}
	}
}
