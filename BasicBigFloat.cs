using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SM64DSe
{
    public struct BasicBigFloat : IComparable<BasicBigFloat>
    {
        // 12345.67890:
        // m_Whole = { 5, 4, 3, 2, 1, 0, 0, ..., 0 }
        // m_Exponent = -5 
        // m_Fraction = { 6, 7, 8, 9, 0, 0, ..., 0 }

        private static readonly int NUM_WHOLE_DIGITS = 38;
        private static readonly int NUM_FRACTIONAL_DIGITS = 8;

        private byte[] m_Whole;
        private byte[] m_Fraction;
        private int m_Exponent;
        private bool m_IsNegative;

        public BasicBigFloat(string number)
        {
            m_Whole = new byte[NUM_WHOLE_DIGITS];
            m_Fraction = new byte[NUM_FRACTIONAL_DIGITS];
            string trimmed = number.Trim().Replace(",", ".");
            m_IsNegative = trimmed.StartsWith("-");
            string positive = trimmed.Replace("-", "");
            int decPointInd = positive.IndexOf(".");
            string fraction = positive.Substring(Math.Max(0, decPointInd + 1),
                ((decPointInd > -1) ? Math.Min(NUM_FRACTIONAL_DIGITS, positive.Length - (decPointInd + 1)) : 0));
            string whole = positive.Substring(0, (decPointInd > -1) ?
                Math.Min(NUM_WHOLE_DIGITS, decPointInd) : Math.Min(NUM_WHOLE_DIGITS, positive.Length));
            string result = whole + "." + fraction;

            m_Exponent = (decPointInd > -1) ? (0 - (result.Length - (decPointInd + 1))) : 0;

            char[] wholeChars = whole.ToCharArray();
            for (int i = 0; i < whole.Length; i++)
            {
                m_Whole[whole.Length - (i + 1)] = Byte.Parse(wholeChars[i].ToString());
            }
            char[] fractionChars = fraction.ToCharArray();
            for (int i = 0; i < fraction.Length; i++)
            {
                m_Fraction[i] = Byte.Parse(fractionChars[i].ToString());
            }
        }

        public BasicBigFloat(BasicBigFloat other)
        {
            m_Whole = new byte[NUM_WHOLE_DIGITS];
            m_Fraction = new byte[NUM_FRACTIONAL_DIGITS];

            Array.Copy(other.m_Whole, m_Whole, NUM_WHOLE_DIGITS);
            Array.Copy(other.m_Fraction, m_Fraction, NUM_FRACTIONAL_DIGITS);

            m_Exponent = other.m_Exponent;
            m_IsNegative = other.m_IsNegative;
        }

        private void SetPositive()
        {
            m_IsNegative = false;
        }

        private void SetNegative()
        {
            m_IsNegative = true;
        }

        private static string TrimToLimits(string number)
        {
            string result = number.Trim().Replace(",", ".");
            int decPointInd = result.IndexOf(".");
            string fraction = (decPointInd > -1) ? result.Substring(decPointInd, NUM_FRACTIONAL_DIGITS) : "";
            string whole = result.Substring(0, Math.Min(NUM_WHOLE_DIGITS, decPointInd));
            return whole + "." + fraction;
        }

        public BasicBigFloat ToPositive()
        {
            BasicBigFloat positive = new BasicBigFloat(this);
            positive.SetPositive();
            return positive;
        }

        public BasicBigFloat ToNegative()
        {
            BasicBigFloat negative = new BasicBigFloat(this);
            negative.SetNegative();
            return negative;
        }

        public BasicBigFloat ShiftExponent(int shift)
        {
            BasicBigFloat result = new BasicBigFloat(this);
            if (shift > 0)
            {
                for (int i = 0; i < shift; i++)
                {
                    ShiftRight(result.m_Whole);
                    result.m_Whole[0] = result.m_Fraction[0];
                    ShiftLeft(result.m_Fraction);
                }
            }
            else if (shift < 0)
            {
                for (int i = shift; i <= 0; i++)
                {
                    ShiftRight(result.m_Fraction);
                    result.m_Fraction[0] = result.m_Whole[0];
                    ShiftLeft(result.m_Whole);
                }
            }
            return result;
        }

        static void ShiftLeft<T>(T[] source)
        {
            Array.Copy(source, 1, source, 0, source.Length - 1);
        }

        static void ShiftRight<T>(T[] source)
        {
            Array.Copy(source, 0, source, 1, source.Length - 1);
        }

        public static BasicBigFloat Add(string first, string second)
        {
            return Add(new BasicBigFloat(first), new BasicBigFloat(second));
        }

        public static BasicBigFloat Add(BasicBigFloat first, BasicBigFloat second)
        {
            if (first.m_IsNegative)
            {
                if (second.m_IsNegative)
                {
                    return Add(first.ToPositive(), second.ToPositive()).ToNegative();
                }
                else
                {
                    return Subtract(second, first.ToPositive());
                }
            }
            else
            {
                if (second.m_IsNegative)
                {
                    return Subtract(first, second.ToPositive());
                }
                else
                {
                    BasicBigFloat result = new BasicBigFloat(first);
                    byte carryTen = 0;
                    for (int i = NUM_FRACTIONAL_DIGITS - 1; i >= 0; i--)
                    {
                        byte sum = (byte)(result.m_Fraction[i] + second.m_Fraction[i] + carryTen);
                        result.m_Fraction[i] = (byte)(sum % 10);
                        carryTen = (byte)(sum / 10);
                    }
                    for (int i = 0; i < NUM_WHOLE_DIGITS; i++)
                    {
                        byte sum = (byte)(result.m_Whole[i] + second.m_Whole[i] + carryTen);
                        result.m_Whole[i] = (byte)(sum % 10);
                        carryTen = (byte)(sum / 10);
                    }
                    result.m_Exponent = Math.Min(result.m_Exponent, second.m_Exponent);
                    return result;
                }
            }
        }

        public static BasicBigFloat Subtract(string first, string second)
        {
            return Subtract(new BasicBigFloat(first), new BasicBigFloat(second));
        }

        public static BasicBigFloat Subtract(BasicBigFloat first, BasicBigFloat second)
        {
            if (first.m_IsNegative)
            {
                if (second.m_IsNegative)
                {
                    return Add(first, second.ToPositive());
                }
                else
                {
                    return Add(first.ToPositive(), second).ToNegative();
                }
            }
            else
            {
                if (second.m_IsNegative)
                {
                    return Add(first, second.ToPositive());
                }
                else
                {
                    if (first < second)
                    {
                        return Subtract(second, first).ToNegative();
                    }
                    else 
                    {
                        BasicBigFloat result = new BasicBigFloat(first);
                        byte carryTen = 0;
                        byte borrowTen = 0;
                        for (int i = NUM_FRACTIONAL_DIGITS - 1; i >= 0; i--)
                        {
                            borrowTen = (byte)((result.m_Fraction[i] < (second.m_Fraction[i] + carryTen)) ? 1 : 0);
                            byte subtract = (byte)((result.m_Fraction[i] + ((borrowTen > 0) ? (10 * borrowTen) : 0)) -
                                (second.m_Fraction[i] + carryTen));
                            result.m_Fraction[i] = subtract;
                            carryTen = borrowTen;
                        }
                        for (int i = 0; i < NUM_WHOLE_DIGITS; i++)
                        {
                            borrowTen = (byte)((result.m_Whole[i] < second.m_Whole[i]) ? 1 : 0);
                            byte subtract = (byte)((result.m_Whole[i] + ((borrowTen > 0) ? (10 * borrowTen) : 0)) -
                                (second.m_Whole[i] + carryTen));
                            result.m_Whole[i] = subtract;
                            carryTen = borrowTen;
                        }
                        result.m_Exponent = Math.Min(result.m_Exponent, second.m_Exponent);
                        return result;
                    }
                }
            }
        }

        public static BasicBigFloat IntMultiply(BasicBigFloat first, int mult)
        {
            BasicBigFloat result = new BasicBigFloat("0");
            // maximum power of 10 is 9 for an int (2147483647)
            for (int i = 9; i >= 0; i--)
            {
                int powerOfTen = (int)Math.Pow(10, i);
                int nPowerOfTen = mult / powerOfTen;
                if (nPowerOfTen > 0)
                {
                    BasicBigFloat shifted = first.ShiftExponent(i);
                    for (int j = 0; j < nPowerOfTen; j++)
                    {
                        result = Add(result, shifted);
                    }
                    mult -= (powerOfTen * nPowerOfTen);
                }
            }
            return result;
        }

        public int CompareTo(BasicBigFloat other)
        {
            if (other.m_Exponent < m_Exponent)
            {
                return -1;
            }
            else if (other.m_Exponent > m_Exponent)
            {
                return 1;
            }
            else
            {
                for (int i = NUM_WHOLE_DIGITS - 1; i >= 0; i--)
                {
                    if (m_Whole[i] > other.m_Whole[i])
                    {
                        return 1;
                    }
                    else if (m_Whole[i] < other.m_Whole[i])
                    {
                        return -1;
                    }
                }
                for (int i = 0; i < NUM_FRACTIONAL_DIGITS; i++)
                {
                    if (m_Fraction[i] > other.m_Fraction[i])
                    {
                        return 1;
                    }
                    else if (m_Fraction[i] < other.m_Fraction[i])
                    {
                        return -1;
                    }
                }
                return 0;
            }
        }

        public static bool operator <(BasicBigFloat first, BasicBigFloat second)
        {
            return first.CompareTo(second) < 0;
        }

        public static bool operator >(BasicBigFloat first, BasicBigFloat second)
        {
            return first.CompareTo(second) > 0;
        }

        public override bool Equals(object obj)
        {
            return (obj is BasicBigFloat) ? (this.CompareTo((BasicBigFloat) obj) == 0) : false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = hash * 7 + ((m_Whole != null) ? m_Whole.GetHashCode() : -1);
                hash = hash * 7 + ((m_Fraction != null) ? m_Fraction.GetHashCode() : -1);
                hash = hash * 7 + m_Exponent.GetHashCode();
                hash = hash * 7 + m_IsNegative.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (m_IsNegative)
            {
                sb.Append("-");
            }

            bool hitNonZero = false;
            for (int i = NUM_WHOLE_DIGITS - 1; i >= 0; i--)
            {
                if (!hitNonZero && m_Whole[i] != 0)
                {
                    hitNonZero = true;
                }
                if (hitNonZero)
                {
                    sb.Append(m_Whole[i]);
                }
            }

            if (sb.Length == (m_IsNegative ? 1 : 0))
            {
                sb.Append("0");
            }

            int nNonZeroFractionDigits = 0;
            for (int i = NUM_FRACTIONAL_DIGITS - 1; i >= 0; i--)
            {
                if (m_Fraction[i] != 0)
                {
                    nNonZeroFractionDigits = i + 1;
                    break;
                }
            }
            if (nNonZeroFractionDigits > 0)
            {
                sb.Append(".");
                for (int i = 0; i < nNonZeroFractionDigits; i++)
                {
                    sb.Append(m_Fraction[i]);
                }
            }

            return sb.ToString();
        }

        public float ToFloat()
        {
            return Helper.ParseFloat(ToString());
        }
    }
}