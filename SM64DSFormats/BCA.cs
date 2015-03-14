/* BCA
 * 
 * Animation format for BMD models.
 * 
 * Based on Mega-Mario's specification and Gericom's MKDS Course Modifier implementation with additional information added to 
 * specification.
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SM64DSe
{
    public class BCA
    {
        public ushort m_NumBones;
        public ushort m_NumFrames;
        public uint m_Looped;
        public uint m_ScaleValuesOffset;
        public uint m_RotationValuesOffset;
        public uint m_TranslationValuesOffset;
        public uint m_AnimationOffset;

        public AnimationData[] m_AnimationData;

        public NitroFile m_File;
        public string m_FileName;

        public BCA(NitroFile file)
        {
            m_File = file;
            m_FileName = m_File.m_Name;

            ReadHeader();

            ReadAnimationData();
        }

        private void ReadHeader()
        {
            m_NumBones = m_File.Read16(0x00);
            m_NumFrames = m_File.Read16(0x02);
            m_Looped = m_File.Read32(0x04);
            m_ScaleValuesOffset = m_File.Read32(0x08);
            m_RotationValuesOffset = m_File.Read32(0x0C);
            m_TranslationValuesOffset = m_File.Read32(0x10);
            m_AnimationOffset = m_File.Read32(0x14);
        }

        private void ReadAnimationData()
        {
            m_AnimationData = new AnimationData[m_NumBones];

            for (int i = 0; i < m_NumBones; i++)
            {
                m_AnimationData[i] = new AnimationData(this, m_AnimationOffset + (uint)(i * 36), i);
            }
        }

        public Matrix4[] GetAllMatricesForFrame(BMD.ModelChunk[] chunks, int frame)
        {
            Matrix4[] matrices = new Matrix4[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                if (i >= m_AnimationData.Length) break;
                matrices[i] = m_AnimationData[i].GetMatrix(chunks, i, matrices, frame);
            }
            return matrices;
        }

        public Matrix4[] GetAllLocalMatricesForFrame(BMD.ModelChunk[] chunks, int frame)
        {
            Matrix4[] matrices = new Matrix4[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                if (i >= m_AnimationData.Length) break;
                matrices[i] = m_AnimationData[i].GetLocalMatrix(frame);
            }
            return matrices;
        }

        public SRTContainer[] GetAllLocalSRTValuesForFrame(BMD.ModelChunk[] chunks, int frame)
        {
            return GetAllLocalSRTValuesForFrame(chunks.Length, frame);
        }

        public SRTContainer[] GetAllLocalSRTValuesForFrame(int numChunks, int frame)
        {
            SRTContainer[] localSRTValues = new SRTContainer[numChunks];
            for (int i = 0; i < numChunks; i++)
            {
                if (i >= m_AnimationData.Length) break;
                localSRTValues[i] = m_AnimationData[i].GetScaleRotationTranslation(frame);
            }
            return localSRTValues;
        }

        public SRTContainer[] GetAllLocalSRTValuesForBone(int boneIndex)
        {
            SRTContainer[] localSRTValues = new SRTContainer[m_NumFrames];
            for (int i = 0; i < m_NumFrames; i++)
            {
                localSRTValues[i] = m_AnimationData[boneIndex].GetScaleRotationTranslation(i);
            }
            return localSRTValues;
        }

        public class AnimationDescriptor
        {
            public byte m_Interpolate;
            public bool m_ConstantValue;
            public ushort m_StartOffset;

            public AnimationDescriptor(NitroFile file, uint offset)
            {
                m_Interpolate = file.Read8(offset + 0x00);
                m_ConstantValue = (file.Read8(offset + 0x01) != 1);
                m_StartOffset = file.Read16(offset + 0x02);
            }
        }

        public class AnimationData
        {
            public BCA m_BCA;

            public AnimationDescriptor m_ScaleX;
            public AnimationDescriptor m_ScaleY;
            public AnimationDescriptor m_ScaleZ;
            public AnimationDescriptor m_RotationX;
            public AnimationDescriptor m_RotationY;
            public AnimationDescriptor m_RotationZ;
            public AnimationDescriptor m_TranslationX;
            public AnimationDescriptor m_TranslationY;
            public AnimationDescriptor m_TranslationZ;

            public float[] m_ScaleXValues;
            public float[] m_ScaleYValues;
            public float[] m_ScaleZValues;
            public float[] m_RotationXValues;
            public float[] m_RotationYValues;
            public float[] m_RotationZValues;
            public float[] m_TranslationXValues;
            public float[] m_TranslationYValues;
            public float[] m_TranslationZValues;

            public AnimationData(BCA bca, uint offset, int boneID)
            {
                m_BCA = bca;

                m_ScaleX = new AnimationDescriptor(m_BCA.m_File, offset + 0x00);
                m_ScaleY = new AnimationDescriptor(m_BCA.m_File, offset + 0x04);
                m_ScaleZ = new AnimationDescriptor(m_BCA.m_File, offset + 0x08);
                m_RotationX = new AnimationDescriptor(m_BCA.m_File, offset + 0x0C);
                m_RotationY = new AnimationDescriptor(m_BCA.m_File, offset + 0x10);
                m_RotationZ = new AnimationDescriptor(m_BCA.m_File, offset + 0x14);
                m_TranslationX = new AnimationDescriptor(m_BCA.m_File, offset + 0x18);
                m_TranslationY = new AnimationDescriptor(m_BCA.m_File, offset + 0x1C);
                m_TranslationZ = new AnimationDescriptor(m_BCA.m_File, offset + 0x20);

                m_ScaleXValues = ReadValuesForDescriptor20_12Float(m_BCA.m_ScaleValuesOffset, m_ScaleX);
                m_ScaleYValues = ReadValuesForDescriptor20_12Float(m_BCA.m_ScaleValuesOffset, m_ScaleY);
                m_ScaleZValues = ReadValuesForDescriptor20_12Float(m_BCA.m_ScaleValuesOffset, m_ScaleZ);
                m_RotationXValues = ReadValuesForDescriptor4_12Rotation(m_BCA.m_RotationValuesOffset, m_RotationX);
                m_RotationYValues = ReadValuesForDescriptor4_12Rotation(m_BCA.m_RotationValuesOffset, m_RotationY);
                m_RotationZValues = ReadValuesForDescriptor4_12Rotation(m_BCA.m_RotationValuesOffset, m_RotationZ);
                m_TranslationXValues = ReadValuesForDescriptor20_12Float(m_BCA.m_TranslationValuesOffset, m_TranslationX);
                m_TranslationYValues = ReadValuesForDescriptor20_12Float(m_BCA.m_TranslationValuesOffset, m_TranslationY);
                m_TranslationZValues = ReadValuesForDescriptor20_12Float(m_BCA.m_TranslationValuesOffset, m_TranslationZ);
            }

            /*
             * Returns the SRT matrix for the specified bone for the current frame having been multiplied by all parent transformations
             */ 
            public Matrix4 GetMatrix(BMD.ModelChunk[] chunks, int boneID, Matrix4[] otherMatrices, int frame)
            {
                SRTContainer srt = GetScaleRotationTranslation(frame);

                Matrix4 ret = srt.m_Matrix;

                if (chunks[boneID].m_ParentOffset < 0)
                {
                    Matrix4.Mult(ref ret, ref otherMatrices[boneID + chunks[boneID].m_ParentOffset], out ret);
                }

                return ret;
            }

            /*
             * Returns the SRT matrix for the specified bone for the current frame NOT multiplied by all parent transformations
             */ 
            public Matrix4 GetLocalMatrix(int frame)
            {
                SRTContainer srt = GetScaleRotationTranslation(frame);

                return srt.m_Matrix;
            }

            public SRTContainer GetScaleRotationTranslation(int frame)
            {
                Vector3 scale = new Vector3(
                    GetFrameValueForDescriptor(m_ScaleX, m_ScaleXValues, frame),
                    GetFrameValueForDescriptor(m_ScaleY, m_ScaleYValues, frame),
                    GetFrameValueForDescriptor(m_ScaleZ, m_ScaleZValues, frame));
                Vector3 rotation = new Vector3(
                    GetFrameValueForDescriptor(m_RotationX, m_RotationXValues, frame, true),
                    GetFrameValueForDescriptor(m_RotationY, m_RotationYValues, frame, true),
                    GetFrameValueForDescriptor(m_RotationZ, m_RotationZValues, frame, true));
                Vector3 translation = new Vector3(
                    GetFrameValueForDescriptor(m_TranslationX, m_TranslationXValues, frame),
                    GetFrameValueForDescriptor(m_TranslationY, m_TranslationYValues, frame),
                    GetFrameValueForDescriptor(m_TranslationZ, m_TranslationZValues, frame));

                return new SRTContainer(scale, rotation, translation);
            }

            private float GetFrameValueForDescriptor(AnimationDescriptor descriptor, float[] values, int frameNum, bool isRotation = false)
            {
                float val;

                if (descriptor.m_ConstantValue)
                {
                    val = values[0];
                }
                else
                {
                    if (descriptor.m_Interpolate == 1)
                    {
                        // Odd frames
                        if ((frameNum & 1) != 0)
                        {
                            
                            if ((frameNum >> 1) + 1 > values.Length - 1)
                            {
                                // if floor(frameNum / 2) + 1 > number of values, use floor(frameNum / 2)
                                val = values[frameNum >> 1];
                            }
                            else if (frameNum == (m_BCA.m_NumFrames - 1))
                            {
                                // else if it's the last frame, don't interpolate
                                val = values[(frameNum >> 1) + 1];
                            }
                            else // else interpolate between current and next values
                            {
                                /* The below code checks for and corrects the following scenario:
                                * Example:
                                * The rotation of a bone is set to use interpolation and has the values -170 followed by 170.
                                * Here, during interpolation the mid-point will be calculated as 0 instead of 180 as (-170 + 170) / 2 equals 0.
                                * What we want is for the second value to be -190 so that during interpolation the midpoint is calculated as 
                                * (-170 + -190) / 2 equals -180
                                * 
                                * To correct this, the code checks:
                                * 1)
                                * eg. -170, 170: change to -170, -190
                                * eg. -5, 5: don't change
                                * if (val1 < 0 && val2 > 0)
                                *      if ( abs(val2 - (val1 + 360)) < abs(val2 - val1)) then val2 -= 360  
                                *      // If the difference between values 1 and 2 is smaller when both are less than zero, make both less than zero
                                * 2)
                                * eg. 170, -170: change to 170, 190
                                * eg. 5, -5: don't change
                                * if (val1 > 0 && val2 < 0)
                                *      if (abs(val1 - (val2 + 360)) < abs(val1 - val2)) then val2 += 360
                                *      // If the difference between values 1 and 2 is smaller when both are greater then zero, math both greater than zero
                                *      
                                * (Degrees used instead of radians to aid understanding)
                                */
                                float val1 = values[frameNum >> 1];
                                float val2 = values[(frameNum >> 1) + 1];
                                if (isRotation)
                                {
                                    if (val1 < 0f && val2 > 0f)
                                    {
                                        if (Math.Abs(val2 - (val1 + (Math.PI * 2f))) < Math.Abs(val2 - val1))
                                        {
                                            val2 -= (float)(Math.PI * 2f);
                                        }
                                    }
                                    else if (val1 > 0f && val2 < 0f)
                                    {
                                        if (Math.Abs(val1 - (val2 + (Math.PI * 2f))) < Math.Abs(val1 - val2))
                                        {
                                            val2 += (float)(Math.PI * 2f);
                                        }
                                    }
                                }
                                val = (val1 + val2) / 2f;
                            }
                        }
                        else
                        {
                            // Even frames
                            val = values[frameNum >> 1];
                        }
                    }
                    else
                    {
                        val = values[frameNum];
                    }
                }

                return val;
            }

            private float[] ReadValuesForDescriptor20_12Float(uint offset, AnimationDescriptor descriptor)
            {
                return (descriptor.m_ConstantValue) ?
                    Read20_12Floats((uint)(offset + (descriptor.m_StartOffset * 4)), 1) :
                    Read20_12Floats((uint)(offset + (descriptor.m_StartOffset * 4)), 
                        (int)(m_BCA.m_NumFrames / ((descriptor.m_Interpolate == 1) ? 2u : 1u) + (uint)descriptor.m_Interpolate));
            }

            private float[] ReadValuesForDescriptor4_12Rotation(uint offset, AnimationDescriptor descriptor)
            {
                return (descriptor.m_ConstantValue) ?
                    Read4_12Rotation((uint)(offset + (descriptor.m_StartOffset * 2)), 1) :
                    Read4_12Rotation((uint)(offset + (descriptor.m_StartOffset * 2)),
                        (int)(m_BCA.m_NumFrames / ((descriptor.m_Interpolate == 1) ? 2u : 1u) + (uint)descriptor.m_Interpolate));
            }

            private float[] Read20_12Floats(uint offset, int count)
            {
                float[] values = new float[count];

                for (int i = 0; i < count; i++)
                {
                    values[i] = (float)(int)m_BCA.m_File.Read32(offset + (uint)(i * 4)) / 4096.0f;
                }
                
                return values;
            }

            private float[] Read4_12Rotation(uint offset, int count)
            {
                float[] values = new float[count];

                for (int i = 0; i < count; i++)
                {
                    values[i] = ((float)((short)m_BCA.m_File.Read16(offset + (uint)(i * 2))) * (float)Math.PI) / 2048.0f;
                }

                return values;
            }

        }

        public class SRTContainer
        {
            public Vector3 m_Scale;
            public Vector3 m_Rotation;
            public Vector3 m_Translation;
            public Matrix4 m_Matrix;

            public SRTContainer(Vector3 scale, Vector3 rotation, Vector3 translation)
            {
                m_Scale = scale;
                m_Rotation = rotation;
                m_Translation = translation;
                m_Matrix = Helper.SRTToMatrix(m_Scale, m_Rotation, m_Translation);
            }

            public Vector3 GetRotationInDegrees()
            {
                Vector3 rotDeg = new Vector3(m_Rotation.X * Helper.Rad2Deg, m_Rotation.Y * Helper.Rad2Deg, m_Rotation.Z * Helper.Rad2Deg);

                return rotDeg;
            }
        }
    }
}
