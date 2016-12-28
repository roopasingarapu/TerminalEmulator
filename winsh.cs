using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace WP2
{
class Winsh{
static String promptDis = "WINSH>";
static Hashtable manCmd = new Hashtable();
static string uname = Environment.UserName.ToString();
static DirectoryInfo dirInfo;
Winsh()
{
Console.Clear();
Console.Title = "WINSH";
dirInfo= new DirectoryInfo(Directory.GetCurrentDirectory());
}

static void Main(string[] args)

{
Winsh win = new Winsh();

Console.WriteLine("Please type \"cmds\" at the prompt to check the list of available commands. \nUse \"man command\" to display help regarding the command.\n [] means optional");

manCmd.Add("man", "Display help for command\nUsage: man \"command\"");
manCmd.Add("ls", "Lists the files in the directory specified\nUsage: ls \"[path]\"");
manCmd.Add("more", "Displays the content that fits on the screen; use \"Enter\" to display more \nUsage: more \"filename\"");
manCmd.Add("cp", "Copies the source file to destination\nUsage: cp \"srcfile[s]\" \"destfile|destFolder\\\";\n Please note the \\ symbol for telling destination as a directory");
manCmd.Add("grep", "Searches for the given pattern within the files in the directory\nUsage: grep \"pattern\"");
manCmd.Add("cd", "Changes the directory to the path specified\nUsage: cd [\"path\"]; with no arguments defaults to initial directory");
manCmd.Add("date", "Display the current date\nUsage: date");
manCmd.Add("ps", "Display the current processes on the system\nUsage: ps");
manCmd.Add("pwd", "Display the current directory\nUsage: pwd");
manCmd.Add("rm", "Removes the directory or file specified\nUsage: rm \"file[s]\"");
manCmd.Add("wc", "Word cound utility\nUsage: wc \"file[s]\"");
manCmd.Add("clear", "Clears the screen\nUsage: clear");
manCmd.Add("cmds", "Displays commands implemented on the screen\nUsage: cmds");

Console.Write("@" + promptDis);
String inp = Console.ReadLine().Trim();
Regex pattern = new Regex("[ ]{1,}");
Regex rSpc = new Regex(" ");
inp = pattern.Replace(inp," ");
while (true)
{
if (inp.Trim().Equals("") || inp.Equals(null))
{
Console.Write("@" + promptDis);
Console.Write(promptDis);
inp = Console.ReadLine().Trim();
inp = pattern.Replace(inp, " ");
continue;
}
String origCmd;
origCmd=inp.Substring(0);
String[] inpSplt = rSpc.Split(origCmd);
inp = inpSplt[0].Substring(0).Trim();

try
{
switch (inp.Trim())
{
case "man":
win.manualC(inpSplt);
break;
case "ls":
win.lsFunc(inpSplt);
break;
case "cmds":
int i = 1;
foreach (DictionaryEntry de in manCmd)
{
Console.WriteLine("{0}.\t{1}", i++, de.Key.ToString());
}
break;
case "cd":
win.cgDir(inpSplt);
break;
case "pwd":
win.pwdFunc();
break;
case "more":
win.moreFunc(inpSplt);
break;
case "cp":
win.cpFunc(inpSplt);
break;
case "date":
win.dateFunc(inpSplt);
break;
case "grep":
win.grepFunc(inpSplt);
break;
case "wc":
win.wordCnt(inpSplt);
break;
case "ps":
win.psFunc(inpSplt);
break;
case "clear":
Console.Clear();
Console.WriteLine("Please type \"cmds\" at the prompt to check the list of available commands.");
Console.WriteLine("Use \"man command\" to display help regarding the command.\n [] means optional");
break;
case "rm":
win.rmvFn(inpSplt);
break;
case "exit":
return;
default:
Console.WriteLine("Please enter the command from list specified.");
break;
}
}
catch (Exception)
{
Console.Write("Please use man for usage");
}
Console.Write("@" + promptDis);
inp = Console.ReadLine().Trim();
inp = pattern.Replace(inp, " ");
}
}
static void grepDirectory(String gPattern, DirectoryInfo dir)
{
try
{
Regex rgex = new Regex(gPattern);
DirectoryInfo di = new DirectoryInfo(dir.FullName);
FileInfo[] fi = di.GetFiles("*.*");
for (int i = 0; i < fi.Length && fi[i].Length>0; i++)
{
FileStream fs = new FileStream(fi[i].FullName, FileMode.Open);
StreamReader sr = new StreamReader(fs);
string line = "";
int lineNum = 1;
line = sr.ReadLine();
int j = 0;
bool flag = false;
while (line != null)
{
String[] splt = rgex.Split(line);
if (splt.Length > 1)
{
flag = true;
if (j == 0)
{
Console.WriteLine("FileName : {0} Start", fi[i].FullName);
}
Console.Write("Line " + lineNum + ": ");
for (int s = 0; s < splt.Length; s++)
{
Console.Write(splt[s]);
if (s != splt.Length - 1)
{
Console.Write(rgex.ToString());
}
}
Console.WriteLine();
j++;
}
line = sr.ReadLine();
++lineNum;
if (line == null && flag)
{
Console.WriteLine("{0} End", fi[i].FullName);
}
}
sr.Dispose();
sr.Close();
fs.Close();
Console.WriteLine();
}
DirectoryInfo[] diSub = di.GetDirectories();
for (int dirNum = 0; dirNum < diSub.Length; dirNum++)
{
grepDirectory(gPattern, diSub[dirNum]);
}
}
catch (FileNotFoundException)
{
Console.WriteLine("Could not open file");
}
catch (Exception)
{
//Console.WriteLine("Please use man for usage");
}
}

void manualC(String[] ipCmd)
{
if (ipCmd.Length > 1)
{
int i = 1;
bool flg = false;
foreach (DictionaryEntry de in manCmd)
{
i++;
if (ipCmd[1].Equals(de.Key.ToString()))
{
Console.WriteLine(de.Value.ToString());
flg = true;
break;
}
}
if (!flg)
{
Console.WriteLine("Command not found.");
}
}
}
void pwdFunc()
{
try
{
DirectoryInfo di = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
Console.WriteLine(di.FullName);
}
catch (Exception)
{
Console.WriteLine("Unexpected Exception");
Console.WriteLine("\n");
Console.WriteLine(manCmd["pwd"].ToString());
}
}
void cgDir(String[] ipCmd)
{
if (ipCmd.Length >= 1)
{
try
{
if(ipCmd.Length==1){
Directory.SetCurrentDirectory(dirInfo.FullName);
}
else if (ipCmd.Length > 1 && !ipCmd[1].Equals(null) && !ipCmd[1].Equals(""))
{
if (Directory.Exists(ipCmd[1]))
Directory.SetCurrentDirectory(ipCmd[1]);
else if (ipCmd[1].Equals(".."))
{
string parent = Directory.GetCurrentDirectory();
DirectoryInfo parDirInf = new DirectoryInfo(parent);
parent = parent.Substring(0, parent.LastIndexOf("\\"));
Directory.SetCurrentDirectory(parent);
}
else if (ipCmd[1].Equals("."))
{
}
else
throw new DirectoryNotFoundException();
}
else
throw new DirectoryNotFoundException();
}
catch (DirectoryNotFoundException)
{
Console.WriteLine("Directory not found");
Console.WriteLine("\n");
Console.WriteLine(manCmd["cd"].ToString());
}
}
}
void dateFunc(String[] ipCmd)
{
if (ipCmd.Length == 1)
Console.WriteLine(System.DateTime.Now);
else
Console.WriteLine("man date for usage");
}

void psFunc(String[] ipCmd)
{
if (ipCmd.Length == 1)
{
System.Diagnostics.Process[] ps=System.Diagnostics.Process.GetProcesses();
int i = 0;
while (ps.Length > 0)
{
Console.WriteLine("PID: " + ps[i].Id + "\tProcess Name: " + ps[i].ProcessName );
i++;
}
}
else
Console.WriteLine("man ps for usage");
}
void cpFunc(String[] ipCmd)
{
DirectoryInfo di = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
if (ipCmd.Length > 2)
{
try
{
if (ipCmd.Length >= 3)
{
for (int i = 1; i < ipCmd.Length - 1; i++)
{
if (ipCmd[i].Equals(null) || ipCmd[i].Equals("") || !File.Exists(Path.Combine(di.FullName, ipCmd[i])))
{
throw new FileNotFoundException();
}
}
try
{
if (ipCmd.Length == 3 && !ipCmd[2].EndsWith("\\"))
{
if (!ipCmd[1].Equals(null) && !ipCmd[1].Equals("") && File.Exists(Path.Combine(di.FullName, ipCmd[1])))
File.Copy(Path.Combine(di.FullName, ipCmd[1]), Path.Combine(di.FullName, ipCmd[2]));
else if (!ipCmd[1].Equals(null) && !ipCmd[1].Equals("") && !File.Exists(Path.Combine(di.FullName, ipCmd[1])))
Console.WriteLine("Source File not found : " + Path.Combine(di.FullName, ipCmd[1]));
}
else if (ipCmd[ipCmd.Length - 1].EndsWith("\\"))
{
if (Directory.Exists(Path.Combine(di.FullName, ipCmd[ipCmd.Length - 1])))
{
for (int i = 1; i < ipCmd.Length - 1; i++)
{
if (!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && File.Exists(Path.Combine(di.FullName, ipCmd[i])))
File.Copy(Path.Combine(di.FullName, ipCmd[i]), Path.Combine(di.FullName, Path.Combine(ipCmd[ipCmd.Length - 1], ipCmd[i])));
else if (!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && !File.Exists(Path.Combine(di.FullName, ipCmd[i])))
Console.WriteLine("Source File not found : " + Path.Combine(di.FullName, ipCmd[i]));
}
}
else
throw new DirectoryNotFoundException();
}
else
{
}
}
catch (DirectoryNotFoundException)
{
if (ipCmd[ipCmd.Length - 1].EndsWith("\\"))
{
di.CreateSubdirectory(ipCmd[ipCmd.Length - 1]);
for (int i = 1; i < ipCmd.Length - 1; i++)
{
if (!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && File.Exists(Path.Combine(di.FullName, ipCmd[i])))
File.Copy(Path.Combine(di.FullName, ipCmd[i]), Path.Combine(di.FullName, Path.Combine(ipCmd[ipCmd.Length - 1], ipCmd[i])));
else if (!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && !File.Exists(Path.Combine(di.FullName, ipCmd[i])))
Console.WriteLine("Source File not found : " + Path.Combine(di.FullName, ipCmd[i]));
}
}
}
catch (ArgumentNullException)
{
Console.WriteLine("Arguments cannot be null");
}
catch (FileNotFoundException)
{
Console.WriteLine("Source file not found");
}
catch (Exception)
{
for (int i = 1; i < ipCmd.Length - 1; i++)
{
Console.Write("A file already exists with the given name. Do you want to Over-Write? Yes/No: ");
String conf = Console.ReadLine().Trim();
if (conf.ToLower().Equals("yes"))
{
if (!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && File.Exists(Path.Combine(di.FullName, ipCmd[i])))
{
if (!ipCmd[ipCmd.Length - 1].EndsWith("\\"))
File.Copy(Path.Combine(di.FullName, ipCmd[i]), Path.Combine(di.FullName, ipCmd[ipCmd.Length - 1]), true);
else if (ipCmd[ipCmd.Length - 1].EndsWith("\\"))
File.Copy(Path.Combine(di.FullName, ipCmd[i]), Path.Combine(di.FullName, Path.Combine(ipCmd[ipCmd.Length - 1], ipCmd[i])), true);
}
else if ((!ipCmd[i].Equals(null) && !ipCmd[i].Equals("") && !File.Exists(Path.Combine(di.FullName, ipCmd[i]))))
Console.WriteLine("Source File not found : " + Path.Combine(di.FullName, ipCmd[i]));
}
}
}
}
else
throw new Exception();
}
catch (FileNotFoundException)
{
Console.WriteLine("Source file not found");
}
catch (Exception)
{
Console.WriteLine("Illegal Arguments");
Console.WriteLine("\n");
Console.WriteLine(manCmd["cp"].ToString());
}
}
else
{
Console.WriteLine("Illegal Arguments");
Console.WriteLine("\n");
Console.WriteLine(manCmd["cp"].ToString());
}
}
void moreFunc(String[] ipCmd)
{
if (ipCmd.Length == 2)
{
try
{
FileStream fs = new FileStream(ipCmd[1], FileMode.Open);
StreamReader sr = new StreamReader(fs);
string line;
line = sr.ReadLine();
int numofLines = 1;
while (line != null)
{
++numofLines;
line = sr.ReadLine();
}
sr.Dispose();
sr.Close();
fs = new FileStream(ipCmd[1], FileMode.Open);
sr = new StreamReader(fs);
for (int i = 0; i < (numofLines > 10 ? 10 : numofLines); i++)
{
line = sr.ReadLine();
Console.WriteLine(line);
}
int curLines = (numofLines > 10 ? 10 : numofLines);
Console.WriteLine("{0}%", ((curLines * 100) / numofLines));
for (line = sr.ReadLine(); line != null; line = sr.ReadLine())
{
Console.ReadKey();
Console.WriteLine(line);
if (curLines == numofLines - 2)
++curLines;
Console.WriteLine("{0}%", (((++curLines) * 100) / numofLines));
}
sr.Dispose();
sr.Close();
}
catch (FileNotFoundException)
{
Console.WriteLine("Cannot open file");
}
}
else
{
Console.WriteLine("Improper Usage of more; \"man more\" for usage help");
}
}


void grepFunc(String[] ipCmd)
{
if (ipCmd.Length == 2)
{
try
{
String curDir = Directory.GetCurrentDirectory();
DirectoryInfo di = new DirectoryInfo(curDir);
grepDirectory(ipCmd[1], di);
}
catch (FileNotFoundException)
{
Console.WriteLine("Cannot open file");
}
}
else
Console.WriteLine("Improper Usage, Please try again.");
}
void rmvFn(String[] ipCmd)
{
if (ipCmd.Length > 1)
{
try
{
for (int i = 1; i < ipCmd.Length; i++)
{
if (ipCmd[i].Length > 0)
{
if (File.Exists(ipCmd[i]))
{
File.Delete(ipCmd[i]);
}
else if (!File.Exists(ipCmd[i]) && ipCmd[i].Length > 0)
Console.WriteLine(" File {0} does not exist ",ipCmd[i]);
}
else
Console.WriteLine("Improper Usage of rm; \"man rm\" for help");
}
}
catch (FileNotFoundException)
{
Console.WriteLine("Could not remove file");
}
catch (ArgumentException)
{
Console.WriteLine("Argument Error");
}
}
else
{
Console.Write("Improper Usage of rm; \"man rm\" for help");
}
}
void wordCnt(String[] ipCmd)
{
Regex rSpc = new Regex(" ");
DirectoryInfo di = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
if (ipCmd.Length > 1)
{
try
{
for (int i = 1; i < ipCmd.Length; i++)
{
DirectoryInfo curdD = new DirectoryInfo(Directory.GetCurrentDirectory());
if (ipCmd[i].Length>0 && File.Exists(Path.Combine(curdD.FullName, ipCmd[i])))
{
Console.WriteLine(" -{0} Start- ", ipCmd[i]);
FileStream fs = new FileStream(Path.Combine(curdD.FullName, ipCmd[i]), FileMode.Open);
StreamReader sr = new StreamReader(fs);
String line = sr.ReadLine();
string[] words = rSpc.Split(line);
int numofLines = 0;
int numWords = 0;
int numChars = 0;
while (line != null)
{
++numofLines;
words = rSpc.Split(line);
for (int wl = 0; wl < words.Length; wl++)
{
if (words[wl].Length > 0)
++numWords;
numChars += words[wl].Length;
}
numChars += (words.Length - 1);
line = sr.ReadLine();
}
Console.WriteLine("Lines: " + numofLines);
Console.WriteLine("Words: " + numWords);
Console.WriteLine("Chars: " + numChars);
Console.WriteLine(" -{0} End- ", ipCmd[i]);

sr.Dispose();
sr.Close();
fs.Close();
}
}
}
catch (FileNotFoundException)
{
Console.WriteLine("Could not open file");
}
catch (ArgumentException)
{
Console.WriteLine("Argument Error");
}
}
else
Console.WriteLine("Please man wc for proper usage");
}
void lsFunc(String[] ipCmd)
{
if (ipCmd.Length >= 1)
{
try
{
DirectoryInfo di = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
if (ipCmd.Length > 1 && !ipCmd[1].Equals(null) && !ipCmd[1].Equals(""))
{
di = new DirectoryInfo(ipCmd[1]);
}
Console.WriteLine("{0:15D}\t{1}\t{2}", "File/DirName", "Size", "LastModified");
foreach (FileInfo fi in di.GetFiles())
{
Console.WriteLine("{0:15D}\t{1}\t{2}", fi.Name, fi.Length, fi.LastWriteTime);
}
foreach (DirectoryInfo diint in di.GetDirectories())
{
Console.WriteLine("{0:15D}\t{1}\t{2}", diint.Name, "Dir", diint.LastWriteTime);
}
}
catch (ArgumentException)
{
Console.WriteLine("Insufficient Arguments");
Console.WriteLine("\n");
Console.WriteLine(manCmd["ls"].ToString());
}
catch (DirectoryNotFoundException)
{
Console.WriteLine("Cannot find the Directory");
Console.WriteLine("\n");
Console.WriteLine(manCmd["ls"].ToString());
}
catch (Exception)
{
Console.WriteLine("Unexpected Exception");
Console.WriteLine("\n");
Console.WriteLine(manCmd["ls"].ToString());
}
}
}
}
}