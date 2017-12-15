using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SM64DSe.Templates
{
    class CommonTemplate
    {
        public static string ReplaceHTMLChars(string str)
        {
            return str.Replace("&", "&amp;")
                      .Replace("<", "&lt;")
                      .Replace(">", "&gt;");
        }

        public static string ShowFolderDialog(string defaultStr)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = System.IO.Path.GetDirectoryName(Program.m_ROMPath);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
                return defaultStr;
        }

        public static bool ValidateObjectName(string name, string field)
        {
            if(new Regex(@"[<>]").IsMatch(name))
            {
                MessageBox.Show("\"" + name + "\" is not a valid " + field +
                    ". No angle brackets, please");
                return false;
            }
            return true;
        }
        public static bool ValidateIdentifier(string name, string field)
        {
            if(!new Regex(@"\A\w+\Z", RegexOptions.ECMAScript).IsMatch(name) ||
                new Regex(@"(__|\A(_[A-Z]|\d))", RegexOptions.ECMAScript).IsMatch(name))
            {
                MessageBox.Show("\"" + name + "\" is not a valid " + field +
                    ". It must contain only letters, underscores, and numbers; cannot start with " +
                    "a number or an underscore followed by a capital letter; and cannot have " +
                    "two underscores in a row.", "Bad Identifier");
                return false;
            }
            return true;
        }

        public static string FlagsToString(uint flags)
        {
            char[] str = new char[35];
            for(int i = 0; i < 35; ++i)
            {
                if (i % 9 == 8)
                    str[i] = ' ';
                else
                    str[i] = (flags & 1 << ((i * 8 + 8) / 9)) != 0 ? '1' : '0';
            }
            return new string(str);
        }
        public static void ValidateFlags(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text.Replace(" ", "");
            if (text.Any(x => x != '0' && x != '1'))
            {
                e.Cancel = true;
                return;
            }
            textBox.Text = text.Substring(0x00, 0x08) + " " +
                           text.Substring(0x08, 0x08) + " " +
                           text.Substring(0x10, 0x08) + " " +
                           text.Substring(0x18, 0x08);
            uint runningFlags = 0;
            for (int i = 0; i < 32; ++i)
                if (text[i] == '1')
                    runningFlags |= (uint)(1 << i);
            textBox.Tag = runningFlags;
        }

        public static int CountIndent(string line, int pos)
        {
            return pos + 3 * line.Substring(0, pos).Count(x => x == '\t');
        }

        private enum IfState
        {
            FALSE = 0,
            TRUE = 1,
            DONE = 2, //false and won't be true
        };

        public static void DocumentObject(string name, string codeName, int category,
            ushort objectID, ushort actorID, string description, string bankReq)
        {
            string fakeInternalName =
                new Regex("([a-z])([A-Z])").Replace(codeName, "$1_$2").ToUpper();
            using (FileStream objList = new FileStream("obj_list.txt", FileMode.Open),
                              objData = new FileStream("objectdb.xml", FileMode.Open))
            {
                StreamReader objListR = new StreamReader(objList),
                             objDataR = new StreamReader(objData);
                StreamWriter objListW = new StreamWriter(objList),
                             objDataW = new StreamWriter(objData);

                List<string> objListLines = new List<string>();
                while (!objListR.EndOfStream) { objListLines.Add(objListR.ReadLine()); }

                while (objectID >= objListLines.Count)
                    objListLines.Add("");
                objListLines[objectID] = string.Format(
                    "0x{0:x} == {1} (0x{2:x})", objectID, fakeInternalName, actorID);

                objList.SetLength(0);
                objListLines.ForEach(x => objListW.WriteLine(x));
                objListW.Flush();

                List<string> objDataLines = new List<string>();
                while (!objDataR.EndOfStream) { objDataLines.Add(objDataR.ReadLine()); }

                Regex openRegex = new Regex(@"\A\s*<\s*object\s+id\s*=\s*'" + objectID + @"'\s*>\s*\z");
                Regex closeRegex = new Regex(@"\A\s*</\s*object\s*>\s*\z");
                Regex finalRegex = new Regex(@"\A\s*</\s*database\s*>\s*\z");
                int whereToInsert = -1;
                for (int i = 0; i < objDataLines.Count; ++i)
                    if (openRegex.IsMatch(objDataLines[i]))
                    {
                        whereToInsert = i;
                        //delete old documentation if it exists
                        for (int j = i; j < objDataLines.Count; ++j)
                            if (closeRegex.IsMatch(objDataLines[j]))
                            {
                                objDataLines.RemoveRange(i, j - i + 1);
                                break;
                            }
                        break;
                    }

                if (whereToInsert == -1)
                    for (int i = objDataLines.Count - 1; i >= 0; --i)
                        if (finalRegex.IsMatch(objDataLines[i]))
                        {
                            whereToInsert = i;
                            objDataLines.Insert(i, "");
                            break;
                        }

                objDataLines.Insert(whereToInsert, string.Format(
                    "\t<object id='{0}'>" + Environment.NewLine +
                    "\t\t<category>{1}</category>" + Environment.NewLine +
                    "\t\t<name>{2}</name>" + Environment.NewLine +
                    "\t\t<internalname>{3}</internalname>" + Environment.NewLine +
                    "\t\t<actorid>{4}</actorid>" + Environment.NewLine +
                    "\t\t<description>{5}</description>" + Environment.NewLine +
                    "\t\t<bankreq>{6}</bankreq>" + Environment.NewLine +
                    "\t</object>",
                    objectID,
                    category,
                    ReplaceHTMLChars(name),
                    fakeInternalName,
                    actorID,
                    ReplaceHTMLChars(description),
                    bankReq));

                objData.SetLength(0);
                objDataLines.ForEach(x => objDataW.WriteLine(x));
                objDataW.Flush();
            }

            ObjectDatabase.Load();
        }

        public static bool FillTemplate(List<string> sourceLines, Dictionary<string, string> defs)
        {
            Regex subsRegex = new Regex(@"\(_[A-Z]\w*\)");
            List<IfState> ifStates = new List<IfState>();
            ifStates.Add(IfState.TRUE);
            for(int i = 0; i < sourceLines.Count; ++i)
            {
                string str = sourceLines[i];

                string str2 = str.TrimStart();
                if (str2.StartsWith("#") || (ifStates.Last() == IfState.TRUE))
                    while (subsRegex.IsMatch(str))
                    {
                        Match match = subsRegex.Match(str);
                        string identifier = str.Substring(match.Index + 1, match.Length - 2);
                        string repl;
                        if (!defs.ContainsKey(identifier))
                        {
                            MessageBox.Show("Warning: The key \"" + identifier + "\" is undefined.");
                            repl = "DUD";
                        }
                        else
                            repl = defs[identifier];
                        if (subsRegex.IsMatch(repl))
                        {
                            MessageBox.Show("Uh oh, infinite recursion detected!");
                            return false;
                        }

                        repl = repl.Replace(Environment.NewLine, Environment.NewLine + new string(' ', CountIndent(str, match.Index)));
                        str = str.Substring(0, match.Index) +
                              repl +
                              str.Substring(match.Index + match.Length);
                    }
                
                if(str2.StartsWith("#"))
                {
                    Match match;
                    if((match = new Regex(@"\A\#ifn?deftmpl ").Match(str2)).Value != "")
                    {
                        string identifier = str2.Substring(match.Value.Length);
                        ifStates.Add(defs.ContainsKey(identifier) == match.Value.Contains("ifn")
                            ? IfState.FALSE : IfState.TRUE);
                        str = "_Dud";
                    }
                    else if ((match = new Regex(@"\A\#elifn?deftmpl ").Match(str2)).Value != "")
                    {
                        string identifier = str2.Substring(match.Value.Length);
                        if (ifStates.Last() == IfState.TRUE)
                            ifStates[ifStates.Count - 1] = IfState.DONE;
                        if (ifStates.Last() == IfState.FALSE && 
                                defs.ContainsKey(identifier) == match.Value.Contains("ifn"))
                            ifStates[ifStates.Count - 1] = IfState.TRUE;
                        str = "_Dud";
                    }
                    else if ((match = new Regex(@"\A\#elsetmpl").Match(str2)).Value != "")
                    {
                        if (ifStates.Last() == IfState.TRUE)
                            ifStates[ifStates.Count - 1] = IfState.DONE;
                        if (ifStates.Last() == IfState.FALSE)
                            ifStates[ifStates.Count - 1] = IfState.TRUE;
                        str = "_Dud";
                    }
                    else if ((match = new Regex(@"\A\#endiftmpl").Match(str2)).Value != "")
                    {
                        ifStates.RemoveAt(ifStates.Count - 1);
                        str = "_Dud";
                    }
                    else if ((match = new Regex(@"\A\#definetmpl ").Match(str2)).Value != "")
                    {
                        string str3 = str2.Substring(match.Value.Length);
                        Match idMatch = new Regex(@"\w+ ?", RegexOptions.ECMAScript).Match(str3);
                        string identifier = idMatch.Value.TrimEnd();
                        string value = str3.Substring(idMatch.Value.Length);
                        defs[identifier] = value;
                        str = "_Dud";
                    }
                }

                if (ifStates.Last() != IfState.TRUE)
                    str = "_Dud";

                sourceLines[i] = str;
            }

            sourceLines.RemoveAll(x => x == "_Dud");
            return true;
        }
    }
}
