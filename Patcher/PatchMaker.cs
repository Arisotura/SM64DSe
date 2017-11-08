/* 
 * Adopted from NSMBe's patch maker
 */ 

﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace SM64DSe.Patcher
{
    public class PatchMaker
    {
        DirectoryInfo romdir;
        uint m_CodeAddr;

        public PatchMaker(DirectoryInfo romdir, uint codeAddr)
        {
            this.romdir = romdir;
            m_CodeAddr = codeAddr;
        }
		
        public void compilePatch()
        {
            PatchCompiler.compilePatch(m_CodeAddr, romdir);
        }

        public void alignStream(Stream stream, int modulus)
        {
            byte[] zero = { 0x00 };
            while (stream.Position % modulus != 0)
                stream.Write(zero, 0, 1);
        }

        public static bool PatchToSupportBigASMHacks()
        {
            bool autorw = Program.m_ROM.CanRW();
            if (!autorw) Program.m_ROM.BeginRW();
            if (Program.m_ROM.Read32(0x6590) != 0) //the patch makes this not 0
            {
                if (!autorw) Program.m_ROM.EndRW();
                return true;
            }
            if (!autorw) Program.m_ROM.EndRW();

            if (MessageBox.Show("This requires the ROM to be further patched. " +
                "Continue with the patch?", "Table Shifting Patch", MessageBoxButtons.YesNo) == DialogResult.No)
                return false;

            if (!autorw) Program.m_ROM.BeginRW();
            NitroOverlay ov2 = new NitroOverlay(Program.m_ROM, 2);

            //Move the ACTOR_SPAWN_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x6590, Program.m_ROM.ReadBlock(0x90864, 0x61c));
            Program.m_ROM.WriteBlock(0x90864, new byte[0x61c]);

            //Adjust pointers
            Program.m_ROM.Write32(0x1a198, 0x02006590);

            //Move the OBJ_TO_ACTOR_TABLE so it can expand
            Program.m_ROM.WriteBlock(0x4b00, ov2.ReadBlock(0x0210cbf4 - ov2.GetRAMAddr(), 0x28c));
            ov2.WriteBlock(0x0210cbf4 - ov2.GetRAMAddr(), new byte[0x28c]);

            //Adjust pointers
            ov2.Write32(0x020fe890 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fe958 - ov2.GetRAMAddr(), 0x02004b00);
            ov2.Write32(0x020fea44 - ov2.GetRAMAddr(), 0x02004b00);

            //Add the dynamic library loading and cleanup code
            Program.m_ROM.WriteBlock(0x90864, Properties.Resources.dynamic_library_loader);

            //Add the hooks (by replacing LoadObjBankOverlays())
            Program.m_ROM.WriteBlock(0x2df70, Properties.Resources.static_overlay_loader);
            
            if (!autorw) Program.m_ROM.EndRW();
            ov2.SaveChanges();

            return true;
        }

        public void makeOverlay(uint overlayID)
        {
            FileInfo f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (!f.Exists) return;
            FileStream fs = f.OpenRead();
            FileInfo symFile = new FileInfo(romdir.FullName + "/newcode.sym");
            StreamReader symStr = symFile.OpenText();

            byte[] newdata = new byte[fs.Length];
            fs.Read(newdata, 0, (int)fs.Length);
            fs.Close();


            BinaryWriter newOvl = new BinaryWriter(new MemoryStream());
            BinaryReader newOvlR = new BinaryReader(newOvl.BaseStream);

            try
            {
                newOvl.Write(newdata);
                alignStream(newOvl.BaseStream, 4);

                uint staticInitCount = 0;

                while (!symStr.EndOfStream)
                {
                    string line = symStr.ReadLine();

                    if (line.Contains("_Z4initv")) //gcc name mangling of init()
                    {
                        uint addr = (uint)parseHex(line.Substring(0, 8));
                        newOvl.Write(addr);
                        ++staticInitCount;
                    }
                }

                if (newOvl.BaseStream.Length > 0x4d20)
                    throw new InvalidDataException
                        ("The overlay must have no more than 19776 bytes; this one will have " + newOvl.BaseStream.Length);

                NitroOverlay ovl = new NitroOverlay(Program.m_ROM, overlayID);
                newOvl.BaseStream.Position = 0;
                ovl.SetInitializer(ovl.GetRAMAddr() + (uint)newOvl.BaseStream.Length - 4 * staticInitCount,
                    4 * staticInitCount);
                ovl.SetSize((uint)newOvl.BaseStream.Length);
                ovl.WriteBlock(0, newOvlR.ReadBytes((int)newOvl.BaseStream.Length));
                ovl.SaveChanges();
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Error", ex).ShowDialog();
                return;
            }
            finally
            {
                symStr.Close();
                newOvl.Dispose();
                newOvlR.Close();
            }
        }

        class CodeSectionOffsets
        {
            public uint textOff, textSize, roDataOff, roDataSize,
                dataOff, dataSize, bssOff, bssSize;
        }

        public void listPtrsToFix(StreamReader lst, BinaryReader oldCode, BinaryWriter oldCodeW,
            uint offset, uint size, List<ushort> ptrFixList, out string lastLine)
        {
            uint end = offset + size;
            oldCodeW.BaseStream.Position = oldCode.BaseStream.Position = offset;
            lastLine = "";
            while(oldCode.BaseStream.Position != end)
            {
                string line = lst.ReadLine();
                lastLine = line;
                string[] arr = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                //Only lines in the form:
                //<line number> <offset> <data> <instruction or description>
                //will be considered.
                if (arr.Length < 4 || !new Regex("^(\\d|[A-F]|[a-f]){4}$").IsMatch(arr[1]))
                    continue;
                while (arr.Length >= 4 && (arr[3] == ".ascii" || arr[3] == ".space"))
                {
                    bool isAscii = arr[3] == ".ascii";
                    //can't assume that all bytes of a .space will be written out.
                    oldCode.BaseStream.Position += 
                        isAscii ? arr[2].Length / 2 : int.Parse(arr[4]);
                    string lineNum = arr[0];

                    while(true)
                    {
                        line = lst.ReadLine();
                        lastLine = line;
                        arr = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 0 || line[0] == '\x0c')
                            continue; //Those annoying page breaks...

                        if (arr[0] != lineNum)
                            break;

                        oldCode.BaseStream.Position += isAscii ? arr[1].Length / 2 : 0;
                    }
                }
                //arr[3] != ".ascii"
                if (arr.Length < 4 || !new Regex("^(\\d|[A-F]|[a-f]){4}$").IsMatch(arr[1]))
                    continue;

                int dataSize = arr[2].Length / 2;
                if (dataSize != 4)
                {
                    oldCode.BaseStream.Position += dataSize;
                    continue;
                }

                offset = (uint)oldCode.BaseStream.Position;
                uint word = oldCode.ReadUInt32();
                
                unchecked
                {
                    //Fix branches and branch links to constant offsets.
                    uint condition  = word & 0xf0000000;
                    uint realOpcode = word & 0x0fffffff;
                    if (!arr[3].StartsWith(".") && condition != 0xf0000000 &&
                        realOpcode - 0x0a000000 < 0x02000000) //branch/branch link
                    {
                        uint destAddr = getDestOfBranch((int)word, m_CodeAddr + offset);
                        if(destAddr - m_CodeAddr >= 0x10000) //definitely a constant destination
                        {
                            oldCodeW.BaseStream.Position = offset;
                            oldCodeW.Write(destAddr / 4 | (word & 0xff000000));
                            ptrFixList.Add((ushort)(offset + 0x10));
                        }
                    }

                    //Fix relative pointers.                    v It will be decimal
                    //They look like, for example:
                    // 123 006c 00000000 .word .LOL
                    else if(arr[3] == ".word" && !new Regex("^\\d+$").IsMatch(arr[4]))
                    {
                        if(word - m_CodeAddr < 0x10000)
                        {
                            oldCodeW.BaseStream.Position = offset;
                            oldCodeW.Write(word - m_CodeAddr + 0x10);
                            ptrFixList.Add((ushort)(offset + 0x10));
                        }
                    }
                }
            }
        }

        public void makeDynamicLibrary(string newFile)
        {
            FileInfo f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (f.Exists) f.Delete();

            m_CodeAddr = 0x02400000;
            compilePatch();

            f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (!f.Exists) return;

            FileStream fs = f.Open(FileMode.Open);
            BinaryReader oldCode = new BinaryReader(fs);
            BinaryWriter oldCodeW = new BinaryWriter(fs);

            BinaryWriter fileOut = new BinaryWriter(new FileStream(newFile, FileMode.Create));
            StreamReader asmMap = new StreamReader(
                new FileStream(romdir.FullName + "/build/newcode.map", FileMode.Open));
            StreamReader symbols = new StreamReader(
                new FileStream(romdir.FullName + "/newcode.sym", FileMode.Open));

            try
            {
                uint dataSize = (uint)((fs.Length + 3) / 4 * 4);
                List<ushort> ptrFixList = new List<ushort>();
                uint ptrFixOffset = dataSize + 0x10;
                uint initFuncOffset = 0xffffffff;
                uint cleanFuncOffset = 0xffffffff;

                Dictionary<string, CodeSectionOffsets> codeSections = new Dictionary<string, CodeSectionOffsets>();

                while (!symbols.EndOfStream)
                {
                    string line = symbols.ReadLine();
                    if(line.Contains("_Z4initv"))
                    {
                        initFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber) - m_CodeAddr + 0x10;
                    }
                    else if(line.Contains("_Z7cleanupv"))
                    {
                        cleanFuncOffset = uint.Parse(line.Substring(0, 8),
                            System.Globalization.NumberStyles.HexNumber) - m_CodeAddr + 0x10;
                    }

                    if (initFuncOffset != 0xffffffff && cleanFuncOffset != 0xffffffff)
                        break;
                }

                while (!asmMap.EndOfStream)
                {
                    string line = asmMap.ReadLine();
                    if (line.Length >= 9 &&
                        (line.Substring(0, 7) == " .text " ||
                         line.Substring(0, 9) == " .rodata " ||
                         line.Substring(0, 7) == " .data " ||
                         line.Substring(0, 6) == " .bss "))
                    {
                        string[] arr = line.Substring(1).Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        if (arr[3].Substring(arr[3].Length - 2, 2) != ".o")
                            continue;

                        if (!codeSections.ContainsKey(arr[3]))
                            codeSections[arr[3]] = new CodeSectionOffsets();

                        uint offset = uint.Parse(arr[1].Substring(2),
                            System.Globalization.NumberStyles.HexNumber) - m_CodeAddr;
                        uint size = uint.Parse(arr[2].Substring(2),
                            System.Globalization.NumberStyles.HexNumber);

                        if(arr[0] == ".text")
                        {
                            codeSections[arr[3]].textOff = offset;
                            codeSections[arr[3]].textSize = size;
                        }
                        else if (arr[0] == ".rodata")
                        {
                            codeSections[arr[3]].roDataOff = offset;
                            codeSections[arr[3]].roDataSize = size;
                        }
                        else if(arr[0] == ".data")
                        {
                            codeSections[arr[3]].dataOff = offset;
                            codeSections[arr[3]].dataSize = size;
                        }
                        else if (arr[0] == ".bss")
                        {
                            codeSections[arr[3]].bssOff = offset;
                            codeSections[arr[3]].bssSize = size;
                        }
                    }
                }

                foreach(KeyValuePair<string, CodeSectionOffsets> pair in codeSections)
                    using(StreamReader lst = new StreamReader(
                          new FileStream(romdir.FullName + "/build/" +
                          pair.Key.Substring(0, pair.Key.Length - 1) + "lst", FileMode.Open)))
                    {
                        bool textDone = false, roDataDone = false, dataDone = false, bssDone = false;
                        while(!lst.EndOfStream)
                        {
                            string line = lst.ReadLine();
                            string[] arr = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length < 2)
                                continue;
                            arr[0] = arr[1];
                            Array.Resize(ref arr, 1);

                            do
                            {
                                arr = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                                if (Array.IndexOf(arr, ".text") != -1 && !textDone)
                                {
                                    listPtrsToFix(lst, oldCode, oldCodeW, pair.Value.textOff, pair.Value.textSize, ptrFixList, out line);
                                    textDone = true;
                                }
                                else if (Array.IndexOf(arr, ".rodata") != -1 && !roDataDone)
                                {
                                    listPtrsToFix(lst, oldCode, oldCodeW, pair.Value.roDataOff, pair.Value.roDataSize, ptrFixList, out line);
                                    roDataDone = true;
                                }
                                else if (Array.IndexOf(arr, ".data") != -1 && !dataDone)
                                {
                                    listPtrsToFix(lst, oldCode, oldCodeW, pair.Value.dataOff, pair.Value.dataSize, ptrFixList, out line);
                                    dataDone = true;
                                }
                                else if (Array.IndexOf(arr, ".bss") != -1 && !bssDone)
                                {
                                    listPtrsToFix(lst, oldCode, oldCodeW, pair.Value.bssOff, pair.Value.bssSize, ptrFixList, out line);
                                    bssDone = true;
                                }
                                else
                                    break;
                            } while (!lst.EndOfStream);
                            
                        }
                    }

                oldCode.BaseStream.Position = 0;
                //Header
                fileOut.Write((ushort)ptrFixList.Count);
                fileOut.Write((ushort)ptrFixOffset);
                fileOut.Write((ushort)initFuncOffset);
                fileOut.Write((ushort)cleanFuncOffset);
                alignStream(fileOut.BaseStream, 0x10);

                //Code
                fileOut.Write(oldCode.ReadBytes((int)oldCode.BaseStream.Length));
                alignStream(fileOut.BaseStream, 4);

                //Pointers to fix list
                foreach (ushort data in ptrFixList)
                    fileOut.Write(data);
            }
            catch (Exception ex)
            {
                new ExceptionMessageBox("Error generating patch", ex).ShowDialog();
                return;
            }
            finally
            {
                oldCode.Dispose();
                oldCodeW.Close();
                fileOut.Close();
                asmMap.Close();
                symbols.Close();
            }
        }

        public void generatePatch(string newFile)
        {
            Console.Out.WriteLine(String.Format("New code address: {0:X8}", m_CodeAddr));

            FileInfo f = new FileInfo(romdir.FullName + "/newcode.bin");
            if (!f.Exists) return;
            FileStream fs = f.OpenRead();

            byte[] newdata = new byte[fs.Length];
            fs.Read(newdata, 0, (int)fs.Length);
            fs.Close();


            BinaryWriter extradata = new BinaryWriter(new FileStream(newFile, FileMode.Create));

            try
            {
                extradata.Write(newdata);
                alignStream(extradata.BaseStream, 4);
            }
            catch(Exception ex)
            {
                new ExceptionMessageBox("Error generating patch", ex).ShowDialog();
                return;
            }
            finally
            {
                extradata.Close();
            }
            
            /*int hookAddr = codeAddr + extradata.getPos();


            f = new FileInfo(romdir.FullName + "/newcode.sym");
            StreamReader s = f.OpenText();

            while (!s.EndOfStream)
            {
                string l = s.ReadLine();

                int ind = -1;
                if (l.Contains("nsub_"))
                    ind = l.IndexOf("nsub_");
                if (l.Contains("hook_"))
                    ind = l.IndexOf("hook_");
                if (l.Contains("repl_"))
                    ind = l.IndexOf("repl_");

                if (ind != -1)
                {
                    int destRamAddr= parseHex(l.Substring(0, 8));    //Redirect dest addr
                    int ramAddr = parseHex(l.Substring(ind + 5, 8)); //Patched addr
                    uint val = 0;

                    int ovId = -1;
                    if (l.Contains("_ov_"))
                        ovId = parseHex(l.Substring(l.IndexOf("_ov_") + 4, 2));

                    int patchCategory = 0;

                    string cmd = l.Substring(ind, 4);
                    int thisHookAddr = 0;

                    switch(cmd)
                    {
                        case "nsub":
                            val = makeBranchOpcode(ramAddr, destRamAddr, false);
                            break;
                        case "repl":
                            val = makeBranchOpcode(ramAddr, destRamAddr, true);
                            break;
                        case "hook":
                            //Jump to the hook addr
                            thisHookAddr = hookAddr;
                            val = makeBranchOpcode(ramAddr, hookAddr, false);

                            uint originalOpcode = handler.readFromRamAddr(ramAddr, ovId);
                            
                            //TODO: Parse and fix original opcode in case of BL instructions
                            //so it's possible to hook over them too.
                            extradata.writeUInt(originalOpcode);
                            hookAddr += 4;
                            extradata.writeUInt(0xE92D5FFF); //push {r0-r12, r14}
                            hookAddr += 4;
                            extradata.writeUInt(makeBranchOpcode(hookAddr, destRamAddr, true));
                            hookAddr += 4;
                            extradata.writeUInt(0xE8BD5FFF); //pop {r0-r12, r14}
                            hookAddr += 4;
                            extradata.writeUInt(makeBranchOpcode(hookAddr, ramAddr+4, false));
                            hookAddr += 4;
                            extradata.writeUInt(0x12345678);
                            hookAddr += 4;
                            break;
                        default:
                            continue;
                    }

                    //Console.Out.WriteLine(String.Format("{0:X8}:{1:X8} = {2:X8}", patchCategory, ramAddr, val));
                    Console.Out.WriteLine(String.Format("              {0:X8} {1:X8}", destRamAddr, thisHookAddr));

                    handler.writeToRamAddr(ramAddr, val, ovId);
                }
            }

            s.Close();

            int newArenaOffs = codeAddr + extradata.getPos();
            handler.writeToRamAddr(ArenaLoOffs, (uint)newArenaOffs, -1);

            handler.sections.Add(new Arm9BinSection(extradata.getArray(), codeAddr, 0));
            handler.saveSections();*/
        }

        public static uint getDestOfBranch(int branchOpcode, uint srcAddr)
        {
            unchecked
            {
                return (uint)(((branchOpcode & 0x00ffffff) << 8 >> 6) + 8 + srcAddr);
            }
        }

        public static uint makeBranchOpcode(int srcAddr, int destAddr, bool withLink)
        {
            unchecked
            {
                uint res = (uint)0xEA000000;

                if (withLink)
                    res |= 0x01000000;

                int offs = (destAddr / 4) - (srcAddr / 4) - 2;
                offs &= 0x00FFFFFF;
                res |= (uint)offs;

                return res;
            }
        }


        public static uint parseUHex(string s)
        {
            return uint.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        public static int parseHex(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
