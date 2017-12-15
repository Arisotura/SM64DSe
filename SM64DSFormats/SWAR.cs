using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SM64DSe.SM64DSFormats
{
    class SwavStream : WaveStream, ISampleProvider
    {
        public static int[] INDEX_TABLE = { -1, -1, -1, -1, 2, 4, 6, 8 };
        public static int[] ADPCM_TABLE =
        {
            0x0007,0x0008,0x0009,0x000A,0x000B,0x000C,0x000D,0x000E,0x0010,0x0011,0x0013,0x0015,
            0x0017,0x0019,0x001C,0x001F,0x0022,0x0025,0x0029,0x002D,0x0032,0x0037,0x003C,0x0042,
            0x0049,0x0050,0x0058,0x0061,0x006B,0x0076,0x0082,0x008F,0x009D,0x00AD,0x00BE,0x00D1,
            0x00E6,0x00FD,0x0117,0x0133,0x0151,0x0173,0x0198,0x01C1,0x01EE,0x0220,0x0256,0x0292,
            0x02D4,0x031C,0x036C,0x03C3,0x0424,0x048E,0x0502,0x0583,0x0610,0x06AB,0x0756,0x0812,
            0x08E0,0x09C3,0x0ABD,0x0BD0,0x0CFF,0x0E4C,0x0FBA,0x114C,0x1307,0x14EE,0x1706,0x1954,
            0x1BDC,0x1EA5,0x21B6,0x2515,0x28CA,0x2CDF,0x315B,0x364B,0x3BB9,0x41B2,0x4844,0x4F7E,
            0x5771,0x602F,0x69CE,0x7462,0x7FFF
        };

        private WaveFormat m_WaveFormat;
        public SWAR.Wave m_Wave;
        public ISampleProvider m_Resampled { get; private set; }
        private int m_Position;
        public override long Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = (int)value;
                if (value == 0)
                    Reset();
            }
        }

        public override long Length
        {
            get
            {
                return m_Wave.m_WaveType == SWAR.WaveType.ADPCM
                    ? m_Wave.m_TheWave.m_Data.Length * 4 - 16
                    : m_Wave.m_TheWave.m_Data.Length;
            }
        }

        //for IMA ADPCM
        private int m_CurrPcm16Val;
        private int m_CurrIndex;
        private int m_LoopStartPcm16Val;
        private int m_LoopStartIndex;

        public SwavStream(SWAR.Wave wave)
        {
            m_Wave = wave;
            m_WaveFormat = new WaveFormat(wave.m_SampleRate,
                wave.m_WaveType == SWAR.WaveType.PCM8 ? 8 : 16, 1);
            m_Position = 0;

            if(m_Wave.m_WaveType == SWAR.WaveType.ADPCM)
            {
                m_CurrPcm16Val = (short)m_Wave.m_TheWave.Read16(0);
                m_CurrIndex = m_Wave.m_TheWave.Read16(2);
                m_Position = 4;

                if(m_Wave.m_Loop)
                {
                    byte[] buffer = new byte[4 * (m_Wave.m_LoopStart - 4)];
                    Read(buffer, 0, (int)(4 * (m_Wave.m_LoopStart - 4)));
                    m_LoopStartPcm16Val = m_CurrPcm16Val;
                    m_LoopStartIndex = m_CurrIndex;

                    //restore the starting state
                    m_CurrPcm16Val = (short)m_Wave.m_TheWave.Read16(0);
                    m_CurrIndex = m_Wave.m_TheWave.Read16(2);
                    m_Position = 4;
                }
            }

            m_Resampled = new WdlResamplingSampleProvider(this, 44100);
        }

        public void Reset()
        {
            m_Position = m_Wave.m_WaveType == SWAR.WaveType.ADPCM ? 4 : 0;
            if (m_Wave.m_WaveType == SWAR.WaveType.ADPCM)
            {
                m_CurrPcm16Val = (short)m_Wave.m_TheWave.Read16(0);
                m_CurrIndex = m_Wave.m_TheWave.Read16(2);
            }
        }

        public override WaveFormat WaveFormat { get { return m_WaveFormat; } }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_Wave.m_WaveType != SWAR.WaveType.PCM8)
                count = count / 2 * 2;

            int currCount;
            int totalCount = 0;
            do
            {
                currCount = count;
                switch (m_Wave.m_WaveType)
                {
                    case SWAR.WaveType.PCM8:
                    case SWAR.WaveType.PCM16:
                        currCount = Math.Min(currCount, m_Wave.m_TheWave.m_Data.Length - m_Position);
                        Array.Copy(m_Wave.m_TheWave.m_Data, m_Position, buffer, offset, currCount);
                        m_Position += currCount;
                        break;

                    case SWAR.WaveType.ADPCM:
                        currCount = Math.Min(currCount, (m_Wave.m_TheWave.m_Data.Length - m_Position) * 4);
                        int byteData = 0;
                        for (int i = 0; i < currCount / 2; ++i)
                        {
                            if (i % 2 == 0)
                                byteData = m_Wave.m_TheWave.Read8((uint)m_Position);
                            int nybble = byteData >> 4 * (i % 2) & 0xf;

                            int diff = ADPCM_TABLE[m_CurrIndex] / 8;
                            if ((nybble & 1) != 0) diff += ADPCM_TABLE[m_CurrIndex] / 4;
                            if ((nybble & 2) != 0) diff += ADPCM_TABLE[m_CurrIndex] / 2;
                            if ((nybble & 4) != 0) diff += ADPCM_TABLE[m_CurrIndex] / 1;
                            if ((nybble & 8) == 0) m_CurrPcm16Val = Math.Min(m_CurrPcm16Val + diff,  0x7fff);
                            else                   m_CurrPcm16Val = Math.Max(m_CurrPcm16Val - diff, -0x7fff);
                            m_CurrIndex = Math.Min(Math.Max(m_CurrIndex + INDEX_TABLE[nybble & 7], 0), 88);
                            buffer[offset + 2 * i    ] = (byte)(m_CurrPcm16Val >> 0);
                            buffer[offset + 2 * i + 1] = (byte)(m_CurrPcm16Val >> 8);

                            if (i % 2 == 1)
                                ++m_Position;
                        }
                        break;
                }

                if(m_Wave.m_Loop && count > currCount)
                {
                    m_Position = (int)m_Wave.m_LoopStart;
                    m_CurrPcm16Val = m_LoopStartPcm16Val;
                    m_CurrIndex = m_LoopStartIndex;
                }
                count -= currCount;
                totalCount += currCount;
                offset += currCount;
            } while (m_Wave.m_Loop && count > 0);
            
            return totalCount;
        }

        //the offset is into the buffer.
        public int Read(float[] buffer, int offset, int count)
        {
            if (m_Wave.m_WaveType == SWAR.WaveType.ADPCM)
                count = count / 2 * 2;
            int byteCount = count * (m_Wave.m_WaveType == SWAR.WaveType.PCM8 ? 1 : 2);
            INitroROMBlock byteBuffer = new INitroROMBlock(new byte[byteCount]);
            byteCount = Read(byteBuffer.m_Data, 0, byteCount);
            count = byteCount / (m_Wave.m_WaveType == SWAR.WaveType.PCM8 ? 1 : 2);

            if (m_Wave.m_WaveType == SWAR.WaveType.PCM8)
                for (int i = 0; i < count; ++i)
                    buffer[offset + i] = (sbyte)byteBuffer.m_Data[i] / 128.0f;
            else
                for (int i = 0; i < count; ++i)
                    buffer[offset + i] = (short)byteBuffer.Read16((uint)(2 * i)) / 32768.0f;

            return count;
        }
    }

    //http://mark-dot-net.blogspot.co.uk/2014/02/fire-and-forget-audio-playback-with.html
    class AudioPlaybackEngine
    {
        private readonly IWavePlayer m_OutputDevice;

        public AudioPlaybackEngine()
        {
            m_OutputDevice = new WaveOutEvent();
        }

        public void PlaySound(SwavStream input)
        {
            m_OutputDevice.Stop();
            input.Reset();
            m_OutputDevice.Init(input.m_Resampled);
            m_OutputDevice.Play();
        }
        public void PlaySound(SWAR.Wave wave)
        {
            PlaySound(wave.m_TheTrueWave);
        }

        public void StopSound()
        {
            m_OutputDevice.Stop();
        }

        public void Dispose()
        {
            m_OutputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine();
    }

    class SWAR : SDAT.Record
    {
        public enum WaveType
        {
            PCM8 = 0,
            PCM16,
            ADPCM
        }
        public class Wave
        {
            public WaveType m_WaveType;
            public bool m_Loop;
            public int m_SampleRate;
            public int m_Time; //sampleRate * time = 16756991
            public uint m_LoopStart; //measured by byte
            public uint m_LoopLength; //measured by byte
            public INitroROMBlock m_TheWave;
            public SwavStream m_TheTrueWave;

            public void Play()
            {
                AudioPlaybackEngine.Instance.PlaySound(m_TheTrueWave);
            }
        }
        public string m_Name { get; private set; }
        public Wave[] m_Waves { get; private set; }

        public SWAR(string name, INitroROMBlock swar)
        {
            m_Name = name;
            m_Waves = new Wave[swar.Read32(0x38)];
            for(uint i = 0; i < m_Waves.Length; ++i)
            {
                uint offset = swar.Read32(0x3c + 4 * i);
                Wave wave = new Wave();
                wave.m_WaveType = (WaveType)swar.Read8(offset + 0);
                wave.m_Loop = swar.Read8(offset + 1) != 0;
                wave.m_SampleRate = swar.Read16(offset + 2);
                wave.m_Time = swar.Read16(offset + 4);
                wave.m_LoopStart = swar.Read16(offset + 6) * 4u;
                wave.m_LoopLength = swar.Read32(offset + 8) * 4u;
                wave.m_TheWave = new INitroROMBlock(swar.ReadBlock(
                    offset + 12, wave.m_LoopStart + wave.m_LoopLength));
                wave.m_TheTrueWave = new SwavStream(wave);
                m_Waves[i] = wave;

                /*sample.Play();

                System.Threading.Thread.Sleep(2000);*/
            }
        }
    }
}
