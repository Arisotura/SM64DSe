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

        public BCAWriter(ModelBase model, ref NitroFile modelFile) :
            base(model, modelFile.m_Name)
        {
            m_ModelFile = modelFile;
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

            if (save)
                bca.SaveChanges();
        }

        private static void WriteBCAAnimationDescriptor(NitroFile bca, uint offset, byte interpolation, bool isConstant, int startIndex)
        {
            bca.Write8(offset + 0x00, interpolation);// Interpolation
            bca.Write8(offset + 0x01, (isConstant == true) ? (byte)0 : (byte)1);// Index increments with each frame
            bca.Write16(offset + 0x02, (ushort)startIndex);// Starting index
        }
    }
}
