using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SM64DSe
{
    public class Level
    {
        public static readonly int[] k_LevelObjTypeSizes = { 16, 16, 6, 6, 14, 8, 8, 8, 8, 12, 14, 2, 2, 0, 4 };
        public static readonly LevelObject.Type[] k_MiscLevelObjTypeOrder = new LevelObject.Type[] { 
            LevelObject.Type.PATH_NODE, 
            LevelObject.Type.PATH, 
            LevelObject.Type.VIEW, 
            LevelObject.Type.MINIMAP_TILE_ID, 
            LevelObject.Type.ENTRANCE, 
            LevelObject.Type.TELEPORT_SOURCE, 
            LevelObject.Type.TELEPORT_DESTINATION, 
            LevelObject.Type.FOG, 
            LevelObject.Type.DOOR, 
            LevelObject.Type.MINIMAP_SCALE, 
            LevelObject.Type.UNKNOWN_14, 
            LevelObject.Type.EXIT
        };
        public static readonly LevelObject.Type[] k_StandardLevelObjTypeOrder = new LevelObject.Type[] {
            LevelObject.Type.SIMPLE, 
            LevelObject.Type.STANDARD
        };
        public static readonly int k_MaxNumAreas = 8;
        public const byte k_LevelFormatVersion = 0x01;

        public NitroOverlay m_Overlay;
        public int m_LevelID;

        public LevelSettings m_LevelSettings;
        public int m_NumAreas;
        public Dictionary<uint, LevelObject> m_LevelObjects;
        public List<LevelTexAnim> m_TexAnims;
        public List<ushort> m_DynLibIDs;
        public CLPS m_CLPS;

        public ushort[] m_MinimapFileIDs;
        public byte[] m_MinimapIndices;

        private int m_EntranceID = 0;
        private int m_ViewID = 0;
        private int m_PathID = 0;
        private int m_PathNodeID = 0;
        private int m_MinimapTileIDNum = 0;

        public Level() { }

        public Level(int levelID)
            : this(levelID, new NitroOverlay(Program.m_ROM, Program.m_ROM.GetLevelOverlayID(levelID))) { }

        public Level(int levelID, NitroOverlay overlay)
        {
            m_LevelID = levelID;
            m_Overlay = overlay;

            m_LevelSettings = new LevelSettings(m_Overlay);
            if (m_LevelSettings.LevelFormatVersion > k_LevelFormatVersion)
            {
                throw new InvalidDataException("This level was added by a later version of SM64DSe and cannot be read");
            }

            LoadCLPS(m_Overlay);

            // read object lists

            m_NumAreas = m_Overlay.Read8(0x74);
            uint objlistptr = m_Overlay.ReadPointer(0x70);

            m_LevelObjects = new Dictionary<uint, LevelObject>();
            m_TexAnims = new List<LevelTexAnim>(8);
            for (int i = 0; i < 8; ++i)
                m_TexAnims.Add(new LevelTexAnim(m_Overlay, i, m_NumAreas, m_LevelSettings.LevelFormatVersion));

            m_DynLibIDs = new List<ushort>();
            if (DoesLevelUseDynamicLibs())
            {
                uint dlDataOffset = m_Overlay.Read32(0x30);
                uint numDLs = m_Overlay.Read16(dlDataOffset);
                m_DynLibIDs = new List<ushort>((int)numDLs);
                for (uint i = 0; i < numDLs; ++i)
                    m_DynLibIDs.Add(m_Overlay.Read16(dlDataOffset + 2 * i + 2));
            }

            ReadObjectTable(m_Overlay, m_Overlay.ReadPointer(0x64), 0);
            m_MinimapIndices = new byte[k_MaxNumAreas];
            for (byte a = 0; a < m_NumAreas; a++)
            {
                // read object tables
                uint addr = (uint)(objlistptr + (a * 12));
                if (m_Overlay.Read32(addr) != 0)
                {
                    ReadObjectTable(m_Overlay, m_Overlay.ReadPointer(addr), a);
                }

                // texture animations, have already been read
                addr += 4;

                addr += 4;
                m_MinimapIndices[a] = m_Overlay.Read8(addr);
            }

            m_ObjAvailable = new Dictionary<ushort, bool>();
            DetermineAvailableObjects();
        }

        private bool DoesLevelUseDynamicLibs()
        {
            switch (Program.m_ROM.m_Version)
            {
                default:
                case NitroROM.Version.EUR:
                    return m_LevelSettings.OverlayInitialiserVersion > 0;
                case NitroROM.Version.USA_v1:
                case NitroROM.Version.USA_v2:
                case NitroROM.Version.JAP:
                    return false;
            }
        }

        private bool DoesLevelSupportDynamicLibs()
        {
            switch (Program.m_ROM.m_Version)
            {
                default:
                case NitroROM.Version.EUR:
                    return true;
                case NitroROM.Version.USA_v1:
                case NitroROM.Version.USA_v2:
                case NitroROM.Version.JAP:
                    return false;
            }
        }

        private void LoadCLPS(NitroOverlay ovl)
        {
            uint clpsAddr = ovl.ReadPointer(0x60);
            int numCLPSes = ovl.Read16(clpsAddr + 0x06);
            m_CLPS = new CLPS();

            clpsAddr += 8;
            for (int i = 0; i < numCLPSes; ++i)
            {
                CLPS.Entry clps = new CLPS.Entry();
                clps.flags = ovl.Read32(clpsAddr);
                clps.flags |= (ulong)ovl.Read32(clpsAddr + 4) << 32;
                m_CLPS.Add(clps);
                clpsAddr += 8;
            }
        }

        private void ReadObjectTable(NitroOverlay ovl, uint offset, int area)
        {
            uint subtbl_num = ovl.Read32(offset);
            uint subtbl_offset = ovl.ReadPointer(offset + 0x4);
            for (uint st = 0; st < subtbl_num; st++)
            {
                uint curoffset = subtbl_offset + (st * 8);

                byte flags = ovl.Read8(curoffset);
                byte entries_num = ovl.Read8(curoffset + 0x1);
                uint entries_offset = ovl.ReadPointer(curoffset + 0x4);

                byte type = (byte)(flags & 0x1F);
                byte layer = (byte)(flags >> 5);

                if (type == 11)
                    m_MinimapFileIDs = new ushort[entries_num];

                for (byte e = 0; e < entries_num; ++e)
                {
                    LevelObject obj;
                    INitroROMBlock objData = new INitroROMBlock();
                    objData.m_Data = ovl.ReadBlock((uint)(entries_offset + e * k_LevelObjTypeSizes[type]), (uint)k_LevelObjTypeSizes[type]);
                    switch (type)
                    {
                        case 0:
                            obj = new StandardObject(objData, m_LevelObjects.Count, layer, area);
                            break;

                        case 1:
                            obj = new EntranceObject(objData, m_LevelObjects.Count, layer, m_EntranceID++);
                            break;

                        case 2: // Path Node
                            obj = new PathPointObject(objData, m_LevelObjects.Count, m_PathNodeID++);
                            break;

                        case 3: // Path
                            obj = new PathObject(objData, m_LevelObjects.Count, (ushort)m_PathID++);
                            break;

                        case 4:
                            obj = new ViewObject(objData, m_LevelObjects.Count, m_ViewID++);
                            break;

                        case 5:
                            obj = new SimpleObject(objData, m_LevelObjects.Count, layer, area);
                            break;

                        case 6:
                            obj = new TpSrcObject(objData, m_LevelObjects.Count, layer);
                            break;

                        case 7:
                            obj = new TpDstObject(objData, m_LevelObjects.Count, layer);
                            break;

                        case 8:
                            // Fog
                            obj = new FogObject(objData, m_LevelObjects.Count, layer, area);
                            break;

                        case 9:
                            obj = new DoorObject(objData, m_LevelObjects.Count, layer);
                            break;

                        case 10:
                            obj = new ExitObject(objData, m_LevelObjects.Count, layer);
                            break;

                        case 11:
                            obj = new MinimapTileIDObject(objData, m_LevelObjects.Count, layer, m_MinimapTileIDNum++);
                            // This is still used by Minimap Editor
                            m_MinimapFileIDs[e] = ovl.Read16((uint)(entries_offset + (e * 2)));
                            break;

                        case 12:
                            // per-area minimap scale factors
                            obj = new MinimapScaleObject(objData, m_LevelObjects.Count, layer, area);
                            break;

                        case 14:
                            // ??? Unknown
                            obj = new Type14Object(objData, m_LevelObjects.Count, layer, area);
                            break;
                        default:
                            throw new InvalidDataException("Bad object type: " + type);
                    }

                    m_LevelObjects.Add(obj.m_UniqueID, obj);
                }
            }
        }

        public Dictionary<ushort, bool> m_ObjAvailable;
        public void DetermineAvailableObjects()
        {
            m_ObjAvailable.Clear();

            for (int i = 0; i < LevelObject.NUM_OBJ_TYPES; i++)
            {
                ObjectDatabase.ObjectInfo objinfo = ObjectDatabase.m_ObjectInfo[i];

                bool available = true;
                if (objinfo.m_BankRequirement == 1)
                {
                    if (m_LevelSettings.ObjectBanks[objinfo.m_NumBank] != objinfo.m_BankSetting)
                        available = false;
                }
                else if (objinfo.m_BankRequirement == 2)
                {
                    available = false;
                }

                m_ObjAvailable.Add((ushort)i, available);
            }

            m_ObjAvailable.Add(511, true);
        }

        private INitroROMBlock InitialiseDataForObject(LevelObject.Type type)
        {
            INitroROMBlock data = new INitroROMBlock();
            data.m_Data = new byte[k_LevelObjTypeSizes[(int)type]]; //of zeros
            return data;
        }

        private INitroROMBlock InitialiseDataForObject(LevelObject.Type type, ushort id)
        {
            INitroROMBlock data = InitialiseDataForObject(type);
            data.Write16(0, id);
            return data;
        }

        private uint GetNextUniqueID()
        {
            uint uniqueid = m_LevelObjects.Keys.DefaultIfEmpty((uint)m_LevelObjects.Count).First(uid => m_LevelObjects.Keys.Count(uid2 => (uid2 & 0x0FFFFFFF) == ((uid & 0x0FFFFFFF) + 1)) == 0);
            uniqueid = (uniqueid & 0x0FFFFFFF) + 1;
            return uniqueid;
        }

        public StandardObject AddStandardObject(ushort id, int layer, int area)
        {
            StandardObject obj = new StandardObject(
                InitialiseDataForObject(LevelObject.Type.STANDARD, id), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public EntranceObject AddEntranceObject(int layer)
        {
            IEnumerable<LevelObject> entrances = GetAllObjectsByType(LevelObject.Type.ENTRANCE);
            int maxid = (entrances.Count() < 1) ? -1 : entrances.Max(obj2 => ((EntranceObject)obj2).m_EntranceID);
            EntranceObject obj = new EntranceObject(
                InitialiseDataForObject(LevelObject.Type.ENTRANCE), (int)GetNextUniqueID(), layer, maxid + 1);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public PathPointObject AddPathPointObject(int pathID, int index = -1)
        {
            // Calculate the Node ID using the parent path's start offset and length

            Console.WriteLine("PathID for adding: " + pathID);

            List<LevelObject> paths = this.GetAllObjectsByType(LevelObject.Type.PATH).ToList();
            List<LevelObject> pathNodes = this.GetAllObjectsByType(LevelObject.Type.PATH_NODE)
                .OrderBy(obj1 => ((PathPointObject)obj1).m_NodeID).ToList();

            PathObject path = (PathObject)paths[pathID];

            int indexInPath;
            if (index < 0)
            {
                indexInPath = path.Parameters[1];
            }
            else
            {
                indexInPath = index;
            }
            int nodeID = path.Parameters[0] + indexInPath;

            Console.WriteLine("NodeIndex: " + nodeID);

            // Update Node ID's of following path nodes
            if (pathNodes.Count>0)
            {
                for (int i = pathNodes.Count - 1; i >= nodeID; i--)
                {
                    PathPointObject node = (PathPointObject)pathNodes[i];
                    node.m_NodeID++;
                }
            }
            // If possible, create object after last node in path
            PathPointObject obj = new PathPointObject(
                InitialiseDataForObject(LevelObject.Type.PATH_NODE), (int)GetNextUniqueID(), nodeID);

            obj.m_IndexInPath = (byte)indexInPath;
            obj.ParentPath = (ushort)pathID;
            if (indexInPath > 0)
            {
                obj.Position = pathNodes[nodeID - 1].Position;
            }

            obj.GenerateProperties();
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            
            // Update start indices and lengths of paths
            for (int i = pathID; i < paths.Count; i++)
            {
                if (i == pathID)
                {
                    // Increase length of parent path
                    paths[i].Parameters[1] += 1;
                }
                else if (i > pathID)
                {
                    // Increase start node index for all following paths
                    paths[i].Parameters[0] += 1;
                }
                paths[i].GenerateProperties();
            }
            return obj;
        }

        public PathObject AddPathObject()
        {
            IEnumerable<LevelObject> paths = GetAllObjectsByType(LevelObject.Type.PATH);
            IEnumerable<LevelObject> pathNodes = GetAllObjectsByType(LevelObject.Type.PATH_NODE);
            int maxid = (paths.Count() < 1) ? -1 : paths.Max(obj2 => ((PathObject)obj2).m_PathID);
            int maxNode = (pathNodes.Count() < 1) ? -1 : pathNodes.Max(obj2 => ((PathPointObject)obj2).m_NodeID);
            PathObject obj = new PathObject(
                InitialiseDataForObject(LevelObject.Type.PATH), (int)GetNextUniqueID(), (ushort)(maxid + 1));
            obj.Parameters[0] = (ushort)(maxNode+1);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public ViewObject AddViewObject()
        {
            IEnumerable<LevelObject> views = GetAllObjectsByType(LevelObject.Type.VIEW);
            int maxid = (views.Count() < 1) ? -1 : views.Max(obj2 => ((ViewObject)obj2).m_ViewID);
            ViewObject obj = new ViewObject(
                InitialiseDataForObject(LevelObject.Type.VIEW), (int)GetNextUniqueID(), maxid+1);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public SimpleObject AddSimpleObject(ushort id, int layer, int area)
        {
            SimpleObject obj = new SimpleObject(
                InitialiseDataForObject(LevelObject.Type.SIMPLE, id), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public TpSrcObject AddTpSrcObject(int layer)
        {
            TpSrcObject obj = new TpSrcObject(
                InitialiseDataForObject(LevelObject.Type.TELEPORT_SOURCE), (int)GetNextUniqueID(), layer);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public TpDstObject AddTpDstObject(int layer)
        {
            TpDstObject obj = new TpDstObject(
                InitialiseDataForObject(LevelObject.Type.TELEPORT_DESTINATION), (int)GetNextUniqueID(), layer);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public FogObject AddFogObject(int layer, int area)
        {
            FogObject obj = new FogObject(
                InitialiseDataForObject(LevelObject.Type.FOG), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public DoorObject AddDoorObject(int layer)
        {
            DoorObject obj = new DoorObject(
                InitialiseDataForObject(LevelObject.Type.DOOR), (int)GetNextUniqueID(), layer);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public ExitObject AddExitObject(int layer)
        {
            ExitObject obj = new ExitObject(
                InitialiseDataForObject(LevelObject.Type.EXIT), (int)GetNextUniqueID(), layer);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public MinimapTileIDObject AddMinimapTileIDObject(int layer, int area)
        {
            MinimapTileIDObject obj = new MinimapTileIDObject(
                InitialiseDataForObject(LevelObject.Type.MINIMAP_TILE_ID), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public MinimapScaleObject AddMinimapScaleObject(int layer, int area)
        {
            MinimapScaleObject obj = new MinimapScaleObject(
                InitialiseDataForObject(LevelObject.Type.MINIMAP_SCALE), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public Type14Object AddType14Object(int layer, int area)
        {
            Type14Object obj = new Type14Object(
                InitialiseDataForObject(LevelObject.Type.UNKNOWN_14), (int)GetNextUniqueID(), layer, area);
            m_LevelObjects.Add(obj.m_UniqueID, obj);
            return obj;
        }

        public IEnumerable<LevelObject> GetAllObjectsByType(LevelObject.Type type)
        {
            return m_LevelObjects.Values.Where(obj => obj.m_Type == type);
        }

        public bool RemoveObject(LevelObject obj)
        {
            if (obj == null) return false;
            if (m_LevelObjects.Remove(obj.m_UniqueID))
            {
                obj.Release();

                if (obj.m_Type == LevelObject.Type.ENTRANCE)
                {
                    IEnumerable<LevelObject> toUpdate = GetAllObjectsByType(LevelObject.Type.ENTRANCE)
                        .Where(obj2 => ((EntranceObject)obj2).m_EntranceID >= ((EntranceObject)obj).m_EntranceID);
                    foreach (LevelObject entrance in toUpdate)
                    {
                        ((EntranceObject)entrance).m_EntranceID--;
                    }
                }
                else if (obj.m_Type == LevelObject.Type.VIEW)
                {
                    IEnumerable<LevelObject> toUpdate = GetAllObjectsByType(LevelObject.Type.VIEW)
                        .Where(obj2 => ((ViewObject)obj2).m_ViewID >= ((ViewObject)obj).m_ViewID);
                    foreach (LevelObject view in toUpdate)
                    {
                        ((ViewObject)view).m_ViewID--;
                    }
                }
                else if (obj.m_Type == LevelObject.Type.PATH_NODE)
                {
                    UpdatePathsNodeRemoved((PathPointObject)obj);
                }
                else if (obj.m_Type == LevelObject.Type.PATH)
                {
                    if (obj.Parameters[1] < 1)
                        return true;
                    //remove the paths nodes
                    List<LevelObject> toRemove = GetAllObjectsByType(LevelObject.Type.PATH_NODE)
                        .Where(obj2 => ((PathPointObject)obj2).ParentPath == ((PathObject)obj).m_PathID).ToList();
                    if (toRemove.Count > 0)
                    {
                        for (int i = toRemove.Count - 1; i >= 0; i--)
                        {
                            Console.WriteLine("PathID: "+((PathPointObject)toRemove[i]).ParentPath);
                            toRemove[i].Release();
                            m_LevelObjects.Remove(toRemove[i].m_UniqueID);
                        }
                    }

                    //shift the following paths
                    IEnumerable<LevelObject> toUpdate = GetAllObjectsByType(LevelObject.Type.PATH)
                        .Where(obj2 => ((PathObject)obj2).m_PathID >= ((PathObject)obj).m_PathID);
                    int maxNodeId = obj.Parameters[0];
                    foreach (PathObject path in toUpdate)
                    {
                        path.m_PathID--;
                        path.Parameters[0] = (ushort)maxNodeId;
                        path.GenerateProperties();
                        maxNodeId += path.Parameters[1];
                    }

                    // Update Node ID's and parentPaths of following path nodes
                    int gapSize = obj.Parameters[1];
                    IEnumerable<LevelObject> followingPathNodes = GetAllObjectsByType(LevelObject.Type.PATH_NODE).
                        Where(obj0 => ((PathPointObject)obj0).m_NodeID >= obj.Parameters[0]+gapSize);
                    foreach (PathPointObject node in followingPathNodes)
                    {
                        node.m_NodeID -= gapSize;
                        node.ParentPath--;
                    }
                }

                return true;
            }
            return false;
        }

        public void RemoveAllObjects()
        {
            foreach (LevelObject obj in m_LevelObjects.Values)
            {
                obj.Release();
            }
            m_LevelObjects.Clear();
        }

        public bool RemoveAllObjectsByType(LevelObject.Type type)
        {
            List<LevelObject> objects = GetAllObjectsByType(type).ToList();
            if (objects.Count() < 1) return false;
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                RemoveObject(objects[i]);
            }
            return true;
        }

        private int GetPathNodeParentIDFromNodeID(int nodeID)
        {
            int pos = -1;

            IEnumerable<LevelObject> paths = GetAllObjectsByType(LevelObject.Type.PATH);
            IEnumerable<LevelObject> pathNodes = GetAllObjectsByType(LevelObject.Type.PATH_NODE);
            for (int i = 0; i < paths.Count(); i++)
            {
                if (nodeID >= paths.ElementAt(i).Parameters[0] &&
                    nodeID < paths.ElementAt(i).Parameters[0] + paths.ElementAt(i).Parameters[1])
                {
                    pos = i;
                    break;
                }
            }

            return pos;
        }

        private void UpdatePathsNodeRemoved(PathPointObject removedNode)
        {
            int pathNum = GetPathNodeParentIDFromNodeID(removedNode.m_NodeID);
            if (pathNum == -1) return;

            // Decrease length of current path
            IEnumerable<LevelObject> paths = GetAllObjectsByType(LevelObject.Type.PATH);
            paths.ElementAt(pathNum).Parameters[1] -= 1;
            paths.ElementAt(pathNum).GenerateProperties();
            // Decrease starting indices of following paths
            for (int i = pathNum + 1; i < paths.Count(); i++)
            {
                paths.ElementAt(i).Parameters[0] -= 1;
                paths.ElementAt(i).GenerateProperties();
            }
            IEnumerable<LevelObject> pathNodes = GetAllObjectsByType(LevelObject.Type.PATH_NODE);
            // Update Node ID's of following path nodes
            for (int i = removedNode.m_NodeID + 1; i < pathNodes.Count(); i++)
            {
                PathPointObject node = (PathPointObject)pathNodes.Where(obj0 => ((PathPointObject)obj0).m_NodeID == i).ElementAt(0);
                node.m_NodeID--;
            }
        }

        public bool ContainsObjectsIncompatibleWithBankSettings()
        {
            bool bankwarning = false;
            IEnumerable<LevelObject> objs = m_LevelObjects.Values.Where(obj => (obj.m_UniqueID >> 28) == 1);
            foreach (LevelObject obj in objs)
            {
                if (obj.ID < LevelObject.NUM_OBJ_TYPES && !m_ObjAvailable[obj.ID])
                {
                    bankwarning = true;
                    break;
                }
            }
            return bankwarning;
        }

        public void SaveChanges()
        {
            //using a MemoryStream instead of an INitroROMBlock for dynamic size
            MemoryStream stream = new MemoryStream();
            BinaryWriter binWriter = new BinaryWriter(stream);

            byte overlayInitialiserVersion = 0;
            switch (Program.m_ROM.m_Version)
            {
                case NitroROM.Version.EUR:
                    binWriter.Write(Properties.Resources.level_ovl_init_EUR_001);
                    overlayInitialiserVersion = 1;
                    break;
                case NitroROM.Version.USA_v1:
                    binWriter.Write(Properties.Resources.level_ovl_init_USAv1);
                    break;
                case NitroROM.Version.USA_v2:
                    binWriter.Write(Properties.Resources.level_ovl_init_USAv2);
                    break;
                case NitroROM.Version.JAP:
                    binWriter.Write(Properties.Resources.level_ovl_init_JAP);
                    break;
                default:
                    throw new InvalidDataException("This ROM is an unknown version.");
            }
            m_LevelSettings.LevelFormatVersion = k_LevelFormatVersion;
            m_LevelSettings.OverlayInitialiserVersion = overlayInitialiserVersion;

            uint areaTableOffset;

            m_LevelSettings.SaveChanges(binWriter);
            SaveCLPS(binWriter);
            SaveMiscObjs(binWriter);
            SaveRegularObjs(binWriter, out areaTableOffset);
            LevelTexAnim.SaveAll(binWriter, m_TexAnims, areaTableOffset, (uint)m_NumAreas);
            if (DoesLevelSupportDynamicLibs())
            {
                Helper.WritePosAndRestore(binWriter, 0x30, 0);
                binWriter.Write((ushort)m_DynLibIDs.Count);
                m_DynLibIDs.ForEach(x => binWriter.Write((ushort)x));
                Helper.AlignWriter(binWriter, 4);
            }

            Array.Clear(m_Overlay.m_Data, 0, m_Overlay.m_Data.Length);
            Array.Resize(ref m_Overlay.m_Data, (int)stream.Length);
            stream.Position = 0;
            new BinaryReader(stream).Read(m_Overlay.m_Data, 0, (int)stream.Length);
            m_Overlay.SaveChanges();
        }

        private void SaveCLPS(BinaryWriter binWriter)
        {
            Helper.WritePosAndRestore(binWriter, 0x60, Program.m_ROM.LevelOvlOffset);
            m_CLPS.SaveChanges(binWriter);
        }

        private void SaveObjList(BinaryWriter binWriter, IEnumerable<LevelObject> objList, LevelObject.Type[] typeOrder)
        {
            IEnumerable<IGrouping<int, LevelObject>> objGroups = objList
                .Where(x => Array.Exists(typeOrder, y => y == x.m_Type))
                .GroupBy(x => (int)x.m_Type | (int)x.m_Layer << 5);

            binWriter.Write(objGroups.Count());
            binWriter.Write((uint)(binWriter.BaseStream.Position + 4 + Program.m_ROM.LevelOvlOffset));

            uint basePos = (uint)binWriter.BaseStream.Position;
            binWriter.Write(new byte[8 * objGroups.Count()]);

            uint i = 0;
            //This below is a very important order.
            //Not following it could result in objects being initialized in the wrong order.
            //For example, if 1 came before 4, the camera wouldn't be initialized properly.
            foreach (int type in typeOrder)
            {
                foreach (IGrouping<int, LevelObject> group in objGroups.Where(x => (x.Key & 0x1f) == type))
                {
                    uint oldPos = (uint)binWriter.BaseStream.Position;
                    binWriter.BaseStream.Position = basePos + 8 * i;
                    binWriter.Write((byte)group.Key);
                    binWriter.Write((byte)group.Count());
                    binWriter.Write((ushort)0);
                    binWriter.Write(oldPos + Program.m_ROM.LevelOvlOffset);
                    binWriter.BaseStream.Position = oldPos;
                    if (type == (int)LevelObject.Type.PATH_NODE)
                    {
                        group.OrderBy(obj => ((PathPointObject)obj).m_NodeID).ToList().ForEach(x => x.SaveChanges(binWriter));
                    } else
                    {
                        group.ToList().ForEach(x => x.SaveChanges(binWriter));
                    }
                    Helper.AlignWriter(binWriter, 4);

                    ++i;
                }
            }
        }

        private void SaveMiscObjs(BinaryWriter binWriter)
        {
            Helper.WritePosAndRestore(binWriter, 0x64, Program.m_ROM.LevelOvlOffset);
            SaveObjList(binWriter, m_LevelObjects.Values, k_MiscLevelObjTypeOrder);
        }

        public int CalculateNumberOfAreas()
        {
            int numAreas = m_LevelObjects.Max(x => x.Value.m_Area);
            numAreas = Math.Max(numAreas, m_LevelObjects.Values.Where(x => x is EntranceObject)
                .Select(x => x.Parameters[3] & 7).Max() + 1); //include entrance areas
            numAreas = Math.Max(numAreas, m_TexAnims.FindLastIndex(x => x.m_Defs.Count != 0) + 1);
            return numAreas;
        }

        private void SaveRegularObjs(BinaryWriter binWriter, out uint areaTableOffset)
        {
            List<List<LevelObject>> objsByArea = new List<List<LevelObject>>();
            for (int i = 0; i < 8; ++i)
                objsByArea.Add(m_LevelObjects.Values.Where(x => x.m_Area == i).ToList());
            m_NumAreas = Math.Max(m_NumAreas, CalculateNumberOfAreas());

            if (m_NumAreas > k_MaxNumAreas)
                throw new InvalidDataException("The game can support only " + k_MaxNumAreas + " areas. You have " + m_NumAreas + ".");

            uint oldPos = (uint)binWriter.BaseStream.Position;
            binWriter.BaseStream.Position = 0x70;
            binWriter.Write(oldPos + Program.m_ROM.LevelOvlOffset);
            binWriter.Write((byte)m_NumAreas);
            binWriter.BaseStream.Position = oldPos;

            areaTableOffset = (uint)binWriter.BaseStream.Position;
            for (int i = 0; i < m_NumAreas; ++i)
                binWriter.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, m_MinimapIndices[i], 0, 0, 0 });

            for (int i = 0; i < m_NumAreas; ++i)
            {
                Helper.WritePosAndRestore(binWriter, (uint)(areaTableOffset + 12 * i), Program.m_ROM.LevelOvlOffset);
                SaveObjList(binWriter, objsByArea[i], k_StandardLevelObjTypeOrder); //Order shouldn't matter here
            }
        }
    }

    public class LevelSettings
    {
        public LevelSettings()
        {
            ObjectBanks = new uint[8];
            MusicBytes = new byte[3];
        }

        public LevelSettings(NitroOverlay ovl)
        {
            QuestionMarks = (byte)(ovl.Read8(0x78) & 0xf);
            Background = (byte)(ovl.Read8(0x78) >> 4);
            ObjectBanks = new uint[8];
            ObjectBanks[0] = ovl.Read8(0x54);
            ObjectBanks[1] = ovl.Read8(0x55);
            ObjectBanks[2] = ovl.Read8(0x56);
            ObjectBanks[3] = ovl.Read8(0x57);
            ObjectBanks[4] = ovl.Read8(0x58);
            ObjectBanks[5] = ovl.Read8(0x59);
            ObjectBanks[6] = ovl.Read8(0x5A);
            ObjectBanks[7] = ovl.Read32(0x5C);

            BMDFileID = ovl.Read16(0x68);
            KCLFileID = ovl.Read16(0x6A);
            MinimapTsetFileID = ovl.Read16(0x6C);
            MinimapPalFileID = ovl.Read16(0x6E);

            MusicBytes = new byte[3];
            MusicBytes[0] = ovl.Read8(0x7C);
            MusicBytes[1] = ovl.Read8(0x7D);
            MusicBytes[2] = ovl.Read8(0x7E);

            MinimapCoordinateScale = ovl.Read16(0x76);
            CameraStartZoomedOut = ovl.Read8(0x75);

            LevelFormatVersion = (byte)(ovl.Read8(0x7F) & 0x0F);
            OverlayInitialiserVersion = (byte)(ovl.Read8(0x7F) >> 4);
        }

        public void SaveChanges(System.IO.BinaryWriter binWriter)
        {
            for (int i = 0; i < 7; ++i)
                binWriter.Write((byte)ObjectBanks[i]);
            binWriter.Write((byte)0);
            binWriter.Write(ObjectBanks[7]); //0x5c
            binWriter.Write((long)0); //0x60-0x68

            binWriter.Write(BMDFileID); //0x68
            binWriter.Write(KCLFileID); //0x6a
            binWriter.Write(MinimapTsetFileID); //0x6c
            binWriter.Write(MinimapPalFileID); //0x6e

            binWriter.Write(new byte[5]); //0x70-0x75
            binWriter.Write(CameraStartZoomedOut); //0x75
            binWriter.Write(MinimapCoordinateScale); //0x76
            binWriter.Write((byte)(QuestionMarks | Background << 4)); //0x78
            binWriter.Write(new byte[3]); //0x79-0x7c, maybe (and hopefully!) part of Background
            binWriter.Write(MusicBytes); //0x7c-0x7f
            binWriter.Write((byte)((OverlayInitialiserVersion << 4) | LevelFormatVersion)); //0x7f
            binWriter.Write(new byte[20]); //0x80-0x94
        }

        public byte QuestionMarks;
        public byte Background;
        public uint[] ObjectBanks;
        public ushort BMDFileID, KCLFileID, MinimapTsetFileID, MinimapPalFileID;
        public byte[] MusicBytes;
        public ushort MinimapCoordinateScale;
        public byte CameraStartZoomedOut;
        public byte LevelFormatVersion;
        public byte OverlayInitialiserVersion;
        public byte ActSelectorID;// NOT stored in the overlay - not possible
        public ushort[] DynLibIDs;
    }

    public class CLPS : IEnumerable<CLPS.Entry>
    {
        public static readonly char[] HEADER_START = "CLPS".ToCharArray();

        public List<CLPS.Entry> m_Entries;

        public CLPS(List<CLPS.Entry> entries)
        {
            m_Entries = entries;
        }

        public CLPS()
            : this(new List<CLPS.Entry>()) { }

        public int Count { get { return m_Entries.Count; } }

        public Entry this[int index]
        {
            get
            {
                return m_Entries[index];
            }

            set
            {
                m_Entries[index] = value;
            }
        }

        public void Add(Entry entry)
        {
            m_Entries.Add(entry);
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            foreach (Entry entry in m_Entries)
            {
                yield return entry;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void SaveChanges(BinaryWriter binWriter)
        {
            binWriter.Write(HEADER_START);
            binWriter.Write((ushort)0x0008);
            binWriter.Write((ushort)Count);
            foreach (Entry entry in m_Entries)
                binWriter.Write(entry.flags);
        }

        public struct Entry
        {
            public ulong flags;
            public Entry(ulong texture, ulong water, ulong viewID, ulong traction, ulong camBehav, ulong behav,
                ulong camThrough, ulong toxic, ulong unk26, ulong windID)
            {
                flags = (texture & 0x1fuL) << 0 |
                        (water & 0x1uL) << 5 |
                        (viewID & 0x3fuL) << 6 |
                        (traction & 0x7uL) << 12 |
                        (camBehav & 0xfuL) << 15 |
                        (behav & 0x1fuL) << 19 |
                        (camThrough & 0x1uL) << 24 |
                        (toxic & 0x1uL) << 25 |
                        (unk26 & 0x1uL) << 26 |
                        (windID & 0xffuL) << 32;
            }

            public ulong m_Texture
            {
                get { return flags & 0x1fuL; }
                set { flags = flags & ~0x1fuL | value & 0x1fuL; }
            }
            public ulong m_Water
            {
                get { return flags >> 5 & 0x1uL; }
                set { flags = flags & ~(0x1uL << 5) | (value & 0x1uL) << 5; }
            }
            public ulong m_ViewID
            {
                get { return flags >> 6 & 0x3fuL; }
                set { flags = flags & ~(0x3fuL << 6) | (value & 0x3fuL) << 6; }
            }
            public ulong m_Traction
            {
                get { return flags >> 12 & 0x7uL; }
                set { flags = flags & ~(0x7uL << 12) | (value & 0x7uL) << 12; }
            }
            public ulong m_CamBehav
            {
                get { return flags >> 15 & 0xfuL; }
                set { flags = flags & ~(0xfuL << 15) | (value & 0xfuL) << 15; }
            }
            public ulong m_Behav
            {
                get { return flags >> 19 & 0x1fuL; }
                set { flags = flags & ~(0x1fuL << 19) | (value & 0x1fuL) << 19; }
            }
            public ulong m_CamThrough
            {
                get { return flags >> 24 & 0x1uL; }
                set { flags = flags & ~(0x1uL << 24) | (value & 0x1uL) << 24; }
            }
            public ulong m_Toxic
            {
                get { return flags >> 25 & 0x1uL; }
                set { flags = flags & ~(0x1uL << 25) | (value & 0x1uL) << 25; }
            }
            public ulong m_Unk26
            {
                get { return flags >> 26 & 0x1uL; }
                set { flags = flags & ~(0x1uL << 26) | (value & 0x1uL) << 26; }
            }
            public ulong m_Pad1
            {
                get { return flags >> 27 & 0x1fuL; }
                set { flags = flags & ~(0x1fuL << 27) | (value & 0x1fuL) << 27; }
            }
            public ulong m_WindID
            {
                get { return flags >> 32 & 0xffuL; }
                set { flags = flags & ~(0xffuL << 32) | (value & 0xffuL) << 32; }
            }
            public ulong m_Pad2
            {
                get { return flags >> 40 & 0xffffffuL; }
                set { flags = flags & ~(0xffffffuL << 40) | (value & 0xffffffuL) << 40; }
            }

            public byte[] FlagsBytes
            {
                get
                {
                    byte[] flagsBytes = new byte[8];
                    Array.Copy(DataHelper.GetBytes32((uint)(flags & 0xFFFFFFFF)), 0, flagsBytes, 0, 4);
                    Array.Copy(DataHelper.GetBytes32((uint)(flags >> 32)), 0, flagsBytes, 4, 4);
                    return flagsBytes;
                }
            }
        }

        class CLPSEqualityComparer : IEqualityComparer<CLPS.Entry>
        {
            public bool Equals(CLPS.Entry c1, CLPS.Entry c2) { return c1.flags == c2.flags; }
            public int GetHashCode(CLPS.Entry c) { return (int)(c.flags ^ c.m_WindID << 27); }
        }
    }
}
