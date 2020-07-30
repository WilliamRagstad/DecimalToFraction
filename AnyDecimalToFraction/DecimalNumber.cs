using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyDecimalToFraction
{
    public class DecimalNumber
    {
        /// <summary>
        ///  This class will parse numbers formatted as following: xxx.yyy[zzz or xxx,yyy[zzz
        ///  As xxx as the integer value, yyy as non reccurent decimals and zzz as recurrent.
        /// </summary>
        public double IntegerValue { get; }
        public string DecimalValue { get; }
        public string RecurrentDecimalPattern { get; } // Continues endlessly after the DecimalValue

        public DecimalNumber(double integer, string decimals, string recurrent)
        {
            IntegerValue = integer;
            DecimalValue = decimals;
            RecurrentDecimalPattern = recurrent;
        }
        public DecimalNumber(string value)
        {
            bool isDecimals = false;
            bool isReccurentDecimals = false;

            string integerVal = "";
            string decimalsVal = "";
            string reccurentDecimals = "";

            for (int i = 0; i < value.Length; i++)
            {
                char digit = value[i];
                if (!isDecimals && (digit == '.' || digit == ',')) { isDecimals = true; continue; }
                if (!isReccurentDecimals && digit == '[') { isReccurentDecimals = true; continue; }

                if (!char.IsDigit(digit) && digit != '-') throw new ArgumentException($"{digit} is not a valid digit!");

                if (isDecimals)
                {
                    if (digit == '-') throw new ArgumentException($"{digit} is not a valid digit!");

                    if (isReccurentDecimals)
                    {
                        reccurentDecimals += digit;
                    }
                    else
                    {
                        decimalsVal += digit;
                    }
                }
                else if (isReccurentDecimals)
                {
                    reccurentDecimals += digit;
                }
                else
                {
                    integerVal += digit;
                }
            }

            integerVal = integerVal != "" ? integerVal : "0";
            reccurentDecimals = reccurentDecimals != "" ? reccurentDecimals : "0";

            IntegerValue = double.Parse(integerVal);
            DecimalValue = decimalsVal;
            RecurrentDecimalPattern = reccurentDecimals;
        }
        public DecimalNumber(double value) : this(value.ToString("G")) { }
        public DecimalNumber(int value) : this(value.ToString("G")) { }
        public DecimalNumber(float value) : this(value.ToString("G")) { }
        public DecimalNumber(decimal value) : this(value.ToString("G")) { }
        public DecimalNumber(short value) : this(value.ToString("G")) { }
        public DecimalNumber(long value) : this(value.ToString("G")) { }

        public static explicit operator DecimalNumber(string value) => new DecimalNumber(value);
        public override string ToString()
        {
            if (DecimalValue == "0" && RecurrentDecimalPattern == "0") return $"{IntegerValue}";
            return $"{IntegerValue}.{DecimalValue}{RecurrentDecimalPattern}{RecurrentDecimalPattern}{RecurrentDecimalPattern}...";
        }


        public static DecimalNumber operator +(DecimalNumber a, int b) => new DecimalNumber(a.IntegerValue + b, a.DecimalValue, a.RecurrentDecimalPattern);
        public static DecimalNumber operator -(DecimalNumber a, int b) => new DecimalNumber(a.IntegerValue - b, a.DecimalValue, a.RecurrentDecimalPattern);
        public static DecimalNumber operator *(DecimalNumber a, int b) => new DecimalNumber(a.IntegerValue * b, a.DecimalValue, a.RecurrentDecimalPattern);

    }
}
