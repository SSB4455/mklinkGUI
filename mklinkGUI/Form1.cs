using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
			VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
			folderBrowserDialog.Description = "选择源目录";
			folderBrowserDialog.UseDescriptionForTitle = true;

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				string selectedFolder = folderBrowserDialog.SelectedPath;
				//MessageBox.Show("您选择的网络文件夹是：" + selectedFolder);
				// 在这里使用 selectedFolder，进行您需要的操作
				textBox1.Text = selectedFolder;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
			folderBrowserDialog.Description = "选择要映射到的本地目录";
			folderBrowserDialog.UseDescriptionForTitle = true;

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				string selectedFolder = folderBrowserDialog.SelectedPath;
				//MessageBox.Show("You selected: " + selectedFolder);
				// 在这里使用 selectedFolder，进行您需要的操作
				textBox2.Text = selectedFolder;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(textBox1.Text))
			{
				MessageBox.Show("“" + textBox1.Text + "”目录不存在");
				return;
			}
			if (!Directory.Exists(textBox2.Text))
			{
				MessageBox.Show("“" + textBox2.Text + "”目录不存在");
				return;
			}
			string dirName = new DirectoryInfo(textBox1.Text).Name;
			string output = RunCmd("mklink /d " + textBox2.Text + Path.DirectorySeparatorChar + dirName + " " + textBox1.Text);
			label3.Text = output;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			//label3.Text = RunCmd("mklink /d E:\\test22 E:\\Users");
			if (!Directory.Exists(textBox1.Text))
			{
				MessageBox.Show("“" + textBox1.Text + "”目录不存在");
				return;
			}
			if (!Directory.Exists(textBox2.Text))
			{
				MessageBox.Show("“" + textBox2.Text + "”目录不存在");
				return;
			}
			string dirName = new DirectoryInfo(textBox1.Text).Name;
			string output = RunCmd("mklink /j " + textBox2.Text + Path.DirectorySeparatorChar + dirName + " " + textBox1.Text);
			label3.Text = output;
		}

		string RunCmd(string cmd)
		{
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

			return SplitCmdCurrentPath(output) + error;
		}

		string SplitCmdCurrentPath(string output)
		{
			string path = System.Environment.CurrentDirectory;
			int index1 = output.IndexOf(path, 0) + path.Length;
			int index2 = output.IndexOf(path, index1);
			return output.Substring(index1, index2 - index1);
		}
	}
}
