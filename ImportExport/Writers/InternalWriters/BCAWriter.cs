/* BCAWriter
 * 
 * Given a ModelBase object with animations, produces a BCA file.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe.ImportExport.Writers.InternalWriters
{
    public class BCAWriter : AbstractModelWriter
    {
        public NitroFile m_ModelFile;

        private BMDImporter.BCAImportationOptions m_BCAImportationOptions;

        public BCAWriter(ModelBase model, ref NitroFile modelFile, BMDImporter.BCAImportationOptions bcaImportationOptions) :
            base(model, modelFile.m_Name)
        {
            m_ModelFile = modelFile;
            m_BCAImportationOptions = bcaImportationOptions;
        }

        public override void WriteModel(bool save = true)
        {
            ModelBase.BoneDefRoot boneTree = m_Model.m_BoneTree;
            Dictionary<string, ModelBase.AnimationDef> boneAnimationsMap = new Dictionary<string, ModelBase.AnimationDef>();
            foreach (ModelBase.AnimationDef anim in m_Model.m_Animations.Values)
                boneAnimationsMap.Add(anim.m_BoneID, anim);

            if (boneAnimationsMap.Count == 0) return;

            NitroFile bca = m_ModelFile;
            bca.Clear();

            uint dataoffset = 0x00;
            uint headersize = 0x18;
            int numAnimations = boneTree.Count;
            int numFrames = boneAnimationsMap.ElementAt(0).Value.m_NumFrames;

            int numScale = 0;
            foreach (ModelBase.BoneDef boneDef in m_Model.m_BoneTree)
            {
                if (boneAnimationsMap.ContainsKey(boneDef.m_ID))
                {
                    numScale += boneAnimationsMap[boneDef.m_ID].GetScaleValuesCount();
                }
                else
                {
                    numScale += 3;// Original X, Y, Z
                }
            }
            int numRotation = 0;
            foreach (ModelBase.BoneDef boneDef in m_Model.m_BoneTree)
            {
                if (boneAnimationsMap.ContainsKey(boneDef.m_ID))
                {
                    numRotation += boneAnimationsMap[boneDef.m_ID].GetRotateValuesCount();
                }
                else
                {
                    numRotation += 3;// Original X, Y, Z
                }
            }
            int numTranslation = 0;
            foreach (ModelBase.BoneDef boneDef in m_Model.m_BoneTree)
            {
                if (boneAnimationsMap.ContainsKey(boneDef.m_ID))
                {
                    numTranslation += boneAnimationsMap[boneDef.m_ID].GetTranslateValuesCount();
                }
                else
                {
                    numTranslation += 3;// Original X, Y, Z
                }
            }
            uint scaleValuesOffset = headersize;
            uint rotationValuesOffset = scaleValuesOffset + (uint)(numScale * 4);
            uint translationValuesOffset = (uint)(((rotationValuesOffset + (uint)(numRotation * 2)) + 3) & ~3);
            uint animationDataOffset = translationValuesOffset + (uint)(numTranslation * 4);

            bca.Write16(0x00, (ushort)numAnimations);// Number of bones to be handled (should match the number of bones in the BMD)
            bca.Write16(0x02, (ushort)numFrames);// Number of animation frames
            bca.Write32(0x04, 1);// Whether the animation loops, 0 - false, 1 - true
            bca.Write32(0x08, scaleValuesOffset);// Offset to scale values section
            bca.Write32(0x0C, rotationValuesOffset);// Offset to rotation values section
            bca.Write32(0x10, translationValuesOffset);// Offset to translation values section
            bca.Write32(0x14, animationDataOffset);// Offset to animation section

            dataoffset = scaleValuesOffset;

            int boneScaleValuesOffset = 0;
            Dictionary<string, int> boneScaleValuesOffsets = new Dictionary<string, int>();
            int boneRotateValuesOffset = 0;
            Dictionary<string, int> boneRotateValuesOffsets = new Dictionary<string, int>();
            int boneTranslateValuesOffset = 0;
            Dictionary<string, int> boneTranslateValuesOffsets = new Dictionary<string, int>();

            foreach (ModelBase.BoneDef bone in boneTree)
            {
                boneScaleValuesOffsets.Add(bone.m_ID, boneScaleValuesOffset);

                if (boneAnimationsMap.ContainsKey(bone.m_ID))
                {
                    ModelBase.AnimationDef anim = boneAnimationsMap[bone.m_ID];

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetNumValues() * 4);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleY].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleY].GetNumValues() * 4);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleZ].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleZ].GetNumValues() * 4);

                    boneScaleValuesOffset += anim.GetScaleValuesCount();
                }
                else
                {
                    // Write bone's scale values
                    bca.Write32(dataoffset, bone.m_20_12Scale[0]);
                    dataoffset += 4;
                    bca.Write32(dataoffset, bone.m_20_12Scale[1]);
                    dataoffset += 4;
                    bca.Write32(dataoffset, bone.m_20_12Scale[2]);
                    dataoffset += 4;

                    boneScaleValuesOffset += 3;
                }
            }

            dataoffset = rotationValuesOffset;

            // Rotation is in Radians
            foreach (ModelBase.BoneDef bone in boneTree)
            {
                boneRotateValuesOffsets.Add(bone.m_ID, boneRotateValuesOffset);

                if (boneAnimationsMap.ContainsKey(bone.m_ID))
                {
                    ModelBase.AnimationDef anim = boneAnimationsMap[bone.m_ID];

                    bca.WriteBlock(dataoffset, 
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetNumValues() * 2);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateY].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateY].GetNumValues() * 2);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateZ].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateZ].GetNumValues() * 2);

                    boneRotateValuesOffset += anim.GetRotateValuesCount();
                }
                else
                {
                    // Write bone's rotation values
                    bca.Write16(dataoffset, bone.m_4_12Rotation[0]);
                    dataoffset += 2;
                    bca.Write16(dataoffset, bone.m_4_12Rotation[1]);
                    dataoffset += 2;
                    bca.Write16(dataoffset, bone.m_4_12Rotation[2]);
                    dataoffset += 2;

                    boneRotateValuesOffset += 3;
                }
            }

            dataoffset = translationValuesOffset;

            foreach (ModelBase.BoneDef bone in boneTree)
            {
                boneTranslateValuesOffsets.Add(bone.m_ID, boneTranslateValuesOffset);

                if (boneAnimationsMap.ContainsKey(bone.m_ID))
                {
                    ModelBase.AnimationDef anim = boneAnimationsMap[bone.m_ID];

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetNumValues() * 4);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateY].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateY].GetNumValues() * 4);

                    bca.WriteBlock(dataoffset,
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateZ].GetFixedPointValues());
                    dataoffset += (uint)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateZ].GetNumValues() * 4);

                    boneTranslateValuesOffset += anim.GetTranslateValuesCount();
                }
                else
                {
                    // Write bone's translation values
                    bca.Write32(dataoffset, bone.m_20_12Translation[0]);
                    dataoffset += 4;
                    bca.Write32(dataoffset, bone.m_20_12Translation[1]);
                    dataoffset += 4;
                    bca.Write32(dataoffset, bone.m_20_12Translation[2]);
                    dataoffset += 4;

                    boneTranslateValuesOffset += 3;
                }
            }

            dataoffset = animationDataOffset;

            // For each bone, write the animation descriptor for each transformation component
            foreach (ModelBase.BoneDef bone in boneTree)
            {
                if (boneAnimationsMap.ContainsKey(bone.m_ID))
                {
                    ModelBase.AnimationDef anim = boneAnimationsMap[bone.m_ID];

                    // Scale X
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetIsConstant(),
                        boneScaleValuesOffsets[bone.m_ID]);
                    dataoffset += 4;

                    // Scale Y
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleY].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleY].GetIsConstant(),
                        (boneScaleValuesOffsets[bone.m_ID] + 
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetNumValues()));
                    dataoffset += 4;

                    // Scale Z
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleZ].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleZ].GetIsConstant(),
                        (boneScaleValuesOffsets[bone.m_ID] + 
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleX].GetNumValues() + 
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.ScaleY].GetNumValues()));
                    dataoffset += 4;

                    // Rotate X
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetIsConstant(),
                        boneRotateValuesOffsets[bone.m_ID]);
                    dataoffset += 4;

                    // Rotate Y
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateY].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateY].GetIsConstant(),
                        (boneRotateValuesOffsets[bone.m_ID] +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetNumValues()));
                    dataoffset += 4;

                    // Rotate Z
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateZ].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateZ].GetIsConstant(),
                        (boneRotateValuesOffsets[bone.m_ID] +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateX].GetNumValues() +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.RotateY].GetNumValues()));
                    dataoffset += 4;

                    // Translate X
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetIsConstant(),
                        boneTranslateValuesOffsets[bone.m_ID]);
                    dataoffset += 4;

                    // Translate Y
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateY].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateY].GetIsConstant(),
                        (boneTranslateValuesOffsets[bone.m_ID] +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetNumValues()));
                    dataoffset += 4;

                    // Translate Z
                    WriteBCAAnimationDescriptor(bca, dataoffset,
                        (byte)(anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateZ].GetFrameStep() >> 1),
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateZ].GetIsConstant(),
                        (boneTranslateValuesOffsets[bone.m_ID] +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateX].GetNumValues() +
                        anim.m_AnimationComponents[ModelBase.AnimationComponentType.TranslateY].GetNumValues()));
                    dataoffset += 4;
                }
                else
                {
                    // Set to use constant values (the bone's transformation as there's no animation)

                    // Scale X
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneScaleValuesOffsets[bone.m_ID] + 0));
                    dataoffset += 4;

                    // Scale Y
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneScaleValuesOffsets[bone.m_ID] + 1));
                    dataoffset += 4;

                    // Scale Z
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneScaleValuesOffsets[bone.m_ID] + 2));
                    dataoffset += 4;

                    // Rotation X
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneRotateValuesOffsets[bone.m_ID] + 0));
                    dataoffset += 4;

                    // Rotation Y
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneRotateValuesOffsets[bone.m_ID] + 1));
                    dataoffset += 4;

                    // Rotation Z
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneRotateValuesOffsets[bone.m_ID] + 2));
                    dataoffset += 4;

                    // Translation X
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneTranslateValuesOffsets[bone.m_ID] + 0));
                    dataoffset += 4;

                    // Translation Y
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneTranslateValuesOffsets[bone.m_ID] + 1));
                    dataoffset += 4;

                    // Translation Z
                    WriteBCAAnimationDescriptor(bca, dataoffset, 0, true, (boneTranslateValuesOffsets[bone.m_ID] + 2));
                    dataoffset += 4;
                }
            }

            if (m_BCAImportationOptions.m_Optimise) Optimise(bca);
            if (save) bca.SaveChanges();
        }

        public struct BCAOffsetData
        {
            public uint scaleOffset;
            public uint rotOffset;
            public uint posOffset;
            public uint animOffset;

            public void GetDataOffsetAndSize(uint trTypeIndex, out uint offset, out uint size)
            {
                switch (trTypeIndex)
                {
                    case 0: offset = scaleOffset; size = 4; break;
                    case 1: offset = rotOffset  ; size = 2; break;
                    case 2: offset = posOffset  ; size = 4; break;
                    default: throw new Exception("This wasn't supposed to happen!");
                }
            }
        }

        public void BackSpace(NitroFile bca, uint addr, uint len, ref BCAOffsetData offs, bool forAlignment = false)
        {
            bca.AddSpace(addr, (uint)-len);
            uint oldRotOffset = offs.rotOffset;
            uint oldPosOffset = offs.posOffset;
            uint oldAnimOffset = offs.animOffset;

            if (addr <= offs.scaleOffset)
                offs.scaleOffset -= len;
            if (addr <= offs.rotOffset)
                offs.rotOffset -= len;
            if (addr <= offs.posOffset)
                offs.posOffset -= len;
            if (addr <= offs.animOffset)
                offs.animOffset -= len;

            bca.Write32(0x08, offs.scaleOffset);
            bca.Write32(0x0c, offs.rotOffset);
            bca.Write32(0x10, offs.posOffset);
            bca.Write32(0x14, offs.animOffset);

            if (forAlignment)
                return;

            uint trTypeIndex;
            if (addr > offs.scaleOffset && addr <= oldRotOffset)
                trTypeIndex = 0;
            else if (addr > offs.rotOffset && addr <= oldPosOffset)
                trTypeIndex = 1;
            else if (addr > offs.posOffset && addr <= oldAnimOffset)
                trTypeIndex = 2;
            else
                return;

            uint dataOffset, unitSize;
            offs.GetDataOffsetAndSize(trTypeIndex, out dataOffset, out unitSize);

            uint startSubAt = (addr - dataOffset) / unitSize;
            uint valToSub = len / unitSize;

            uint numBones = bca.Read16(0x00);
            for(uint i = 0; i < numBones; ++i)
                for(uint j = 0; j < 3; ++j)
                {
                    uint offset = offs.animOffset + 0x24 * i + 4 * (3 * trTypeIndex + j) + 2;
                    uint temp = bca.Read16(offset);
                    if (temp >= startSubAt)
                        bca.Write16(offset, (ushort)(temp - valToSub));
                }
        }

        private struct BCAEntryToSort
        {
            public uint boneID;
            public uint axisID;
            public uint[] data;
        }

        private static bool UintArraysEqual(uint[] a1, uint[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for(int i = 0; i < a1.Length; ++i)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }

        class UintArrEqualityComparer : IEqualityComparer<uint[]>
        {
            public bool Equals(uint[] a1, uint[] a2) { return UintArraysEqual(a1, a2); }
            public int GetHashCode(uint[] arr) { return (int)(arr.Length + (arr.First() << 10) + (arr.Last() << 20)); }
        }

        public void Optimise(NitroFile bca)
        {
            uint numBones = bca.Read16(0x00);
            uint numFrames = bca.Read16(0x02);

            BCAOffsetData offs = new BCAOffsetData
            {
                scaleOffset = bca.Read32(0x08),
                rotOffset   = bca.Read32(0x0c),
                posOffset   = bca.Read32(0x10),
                animOffset  = bca.Read32(0x14)
            };

            //Pass 1: The constant value pass
            if (numFrames > 1)
                for (uint i = 0; i < numBones; ++i)
                {
                    for(uint j = 0; j < 9; ++j)
                    {
                        if (bca.Read8(offs.animOffset + 0x24 * i + 4 * j + 1) == 0)
                            continue;

                        uint dataOffset, unitSize;
                        offs.GetDataOffsetAndSize(j / 3, out dataOffset, out unitSize);

                        bool interped = bca.Read8(offs.animOffset + 0x24 * i + 4 * j + 0) != 0;
                        uint firstValID = bca.Read16(offs.animOffset + 0x24 * i + 4 * j + 2);
                        uint numVals = interped ?
                            numFrames / 2 + 1 :
                            numFrames;

                        uint[] vals = new uint[numVals];
                        for (uint k = 0; k < numVals; ++k)
                            vals[k] = bca.ReadVar(dataOffset + (firstValID + k) * unitSize, unitSize);

                        if(!Array.Exists(vals, x => x != vals[0])) //They're all the same.
                        {
                            uint addr = dataOffset + (firstValID + numVals) * unitSize;
                            BackSpace(bca, addr, (numVals - 1) * unitSize, ref offs);
                            bca.Write16(offs.animOffset + 0x24 * i + 4 * j + 0, 0x0000);
                            continue;
                        }

                        if (interped)
                            continue;

                        //Pass 2: The interpolation pass
                        bool shouldInterp = true;
                        int[] valInts;
                        if (unitSize == 2)
                            valInts = Array.ConvertAll(vals, x => x >= 0x8000 ? (int)(x | 0xffff0000) : (int)x);
                        else
                            valInts = Array.ConvertAll(vals, x => (int)x);

                        for (int k = 0; k < numVals - 2; k += 2)
                            if (Math.Abs(valInts[k] - 2 * valInts[k + 1] + valInts[k + 2]) > 1)
                                shouldInterp = false;

                        if (!shouldInterp)
                            continue;

                        for (uint k = 0; k < numVals; k += 2)
                            bca.WriteVar(dataOffset + (firstValID + k / 2) * unitSize, unitSize,
                                bca.ReadVar(dataOffset + (firstValID + k) * unitSize, unitSize));
                        if (numVals % 2 == 0) //can't interpolate on last frame even if that frame is odd
                            bca.WriteVar(dataOffset + (firstValID + numVals / 2) * unitSize, unitSize,
                                bca.ReadVar(dataOffset + (firstValID + numVals - 1) * unitSize, unitSize));

                        bca.Write8(offs.animOffset + 0x24 * i + 4 * j + 0, 1);
                        BackSpace(bca, dataOffset + (firstValID + numVals) * unitSize,
                            (numVals - 1) / 2 * unitSize, ref offs);
                    }
                }

            //Pass 3: The share equal data pass.
            for (uint trID = 0; trID < 3; ++trID)
            {
                uint dataOffset, unitSize;
                offs.GetDataOffsetAndSize(trID, out dataOffset, out unitSize);

                BCAEntryToSort[] bcaEntries = new BCAEntryToSort[3 * numBones];
                for (uint i = 0; i < numBones; ++i)
                    for (uint j = 0; j < 3; ++j)
                    {
                        bool constant = bca.Read8(offs.animOffset + 0x24 * i + 4 * (3 * trID + j) + 1) == 0;
                        bool interped = bca.Read8(offs.animOffset + 0x24 * i + 4 * (3 * trID + j) + 0) != 0;
                        uint firstValID = bca.Read16(offs.animOffset + 0x24 * i + 4 * (3 * trID + j) + 2);
                        uint numVals = constant ? 1 : interped ?
                            numFrames / 2 + 1 :
                            numFrames;

                        bcaEntries[3 * i + j].boneID = i;
                        bcaEntries[3 * i + j].axisID = j;
                        bcaEntries[3 * i + j].data = new uint[numVals];
                        for (uint k = 0; k < numVals; ++k)
                            bcaEntries[3 * i + j].data[k] =
                                bca.ReadVar(dataOffset + (firstValID + k) * unitSize, unitSize);
                    }

                BCAEntryToSort[][] sortedEnts = bcaEntries.GroupBy(x => x.data, new UintArrEqualityComparer()).
                                                Select(g => g.ToArray()).
                                                ToArray();

                foreach(BCAEntryToSort[] entArr in sortedEnts)
                {
                    uint boneID = entArr[0].boneID;
                    uint axisID = entArr[0].axisID;

                    for(int i = 1; i < entArr.Length; ++i) //Using the 1st for reference; skip it
                    {
                        bool constant = bca.Read8(offs.animOffset + 0x24 * entArr[i].boneID +
                            4 * (3 * trID + entArr[i].axisID) + 1) == 0;
                        bool interped = bca.Read8(offs.animOffset + 0x24 * entArr[i].boneID +
                            4 * (3 * trID + entArr[i].axisID) + 0) != 0;
                        uint firstValID = bca.Read16(offs.animOffset + 0x24 * entArr[i].boneID +
                            4 * (3 * trID + entArr[i].axisID) + 2);
                        uint numVals = constant ? 1 : interped ?
                            numFrames / 2 + 1 :
                            numFrames;

                        uint sameValID = bca.Read16(offs.animOffset + 0x24 * boneID +
                            4 * (3 * trID + axisID) + 2);
                        if (firstValID != sameValID) //Avoid deleting entries when they are already shared
                        {
                            bca.Write16(offs.animOffset + 0x24 * entArr[i].boneID +
                                4 * (3 * trID + entArr[i].axisID) + 2, (ushort)sameValID);
                            BackSpace(bca, dataOffset + (firstValID + numVals) * unitSize,
                                numVals * unitSize, ref offs);
                        }
                    }
                }
            }

            if ((offs.posOffset - offs.rotOffset) % 4 != 0)
                BackSpace(bca, offs.posOffset, 0xfffffffe, ref offs, true);
        }

        private static void WriteBCAAnimationDescriptor(NitroFile bca, uint offset, byte interpolation, bool isConstant, int startIndex)
        {
            bca.Write8(offset + 0x00, interpolation);// Interpolation
            bca.Write8(offset + 0x01, (isConstant == true) ? (byte)0 : (byte)1);// Index increments with each frame
            bca.Write16(offset + 0x02, (ushort)startIndex);// Starting index
        }
    }
}
