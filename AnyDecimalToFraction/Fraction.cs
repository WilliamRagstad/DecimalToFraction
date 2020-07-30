using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyDecimalToFraction
{
    public class Fraction
    {
        public static Fraction Default = new Fraction(0, 1);
        public Fraction(Fraction fraction) : this(fraction.Numerator, fraction.Denominator) { }
        public Fraction(long numerator, long denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
            }
            Numerator = numerator;
            Denominator = denominator;
        }

        public long Numerator { get; set; }
        public long Denominator { get; set; }
        public static bool TryParse(string value, out Fraction result, bool printInfiniteDecimals = false)
        {
            int sep = value.IndexOf('|');
            int comma = value.IndexOf(',');
            if (sep != -1 && sep > comma)
            {
                string[] p = value.Split('|');
                if (decimal.TryParse(p[0], out decimal head) && decimal.TryParse(p[1], out decimal decimals))
                {
                    Fraction finiteFraction = Parse(head);
                    decimal infinitePart = 0;
                    int digits = p[1].Length;
                    int mag = comma - sep;  // (Left to Right) Index of where the infinite decimal pattern begins in the number relative to the decimal point/entals siffran
                    decimal infinitePattern = decimals * (decimal)Math.Pow(10, -digits + 1);
                    for (int i = 0; i < 50 / digits; i++) infinitePart = infinitePart / (decimal)Math.Pow(10, digits) + infinitePattern;

                    if (printInfiniteDecimals) Console.WriteLine(head + infinitePart * (decimal)Math.Pow(10, mag));

                    long denr = (long)Math.Pow(10, digits) - 1;
                    decimal numr = denr * infinitePart;
                    if (numr % 1 == 0) // If the result is an integer
                    {
                        numr *= (decimal)Math.Pow(10, mag); // Normalize numerator
                        while(numr % 1 != 0)
                        {
                            numr *= 10;
                            denr *= 10;
                        }

                        Fraction infiniteFraction = new Fraction((long)numr, denr);
                        result = (finiteFraction + infiniteFraction).MaxReduce();
                        return true;
                    }
                    else
                    {
                        throw new ArgumentException("Failed to convert infinite decimal pattern to fraction! (" + numr + " is not an integer!)");
                    }
                }

                result = null;
                return false;
            }
            else if (decimal.TryParse(value, out decimal dec))
            {
                result = Parse(dec);
                return result != null;
            }
            result = null;
            return false;
        }
        public static Fraction Parse(decimal value) {
            string str = value.ToString();
            long mlt = (long)Math.Pow(10, str.IndexOf(',') != -1 ? str.Split(',')[1].Length : 0);
            if (mlt == long.MinValue || mlt == long.MaxValue) return null;
            return new Fraction(
                (long)(value * mlt),
                mlt
            ).MaxReduce();
        }

        public Fraction MaxReduce()
        {
            Fraction prev = new Fraction(this);
            Fraction cur = prev.Reduce();
            while (!prev.Equals(cur))
            {
                prev = cur;
                cur = prev.Reduce();
            }
            return cur;
        }

        public Fraction Reduce()
        {
            // Prime factorize both Numerator and Denominator and remove all common factors.
            List<long> nf = PrimeFactorize(Numerator);
            List<long> df = PrimeFactorize(Denominator);

            long amnt = 1;
            for (int i = 0; i < nf.Count; i++)
            {
                for (int j = 0; j < df.Count; j++)
                {
                    if (nf[i] == df[j])
                    {
                        amnt *= nf[i];
                        nf.RemoveAt(i);
                        df.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }
            Fraction result = new Fraction(this);

            result.Numerator /= amnt;
            result.Denominator /= amnt;
            return result;
        }

        private List<long> Factorize(long n)
        {
            List<long> factors = new List<long>();
            void addFactor(long m)
            {
                /*
                if (factors.Contains(m)) factors[factors.IndexOf(m)] = m * m;
                else factors.Add(m);
                */
                factors.Add(m);
            }
            // long times = (long)Math.Ceiling(n / 2.0f);
            for (long i = 2; i < n /* i <= times */; i++)
            {
                if (n % i == 0)
                {
                    n /= i;
                    addFactor(i);
                }
            }
            addFactor(n); // n is prime
            return factors;
        }

        private bool IsPrime(long n)
        {
            for (int i = 2; i <= Math.Sqrt(n); i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        private List<long> PrimeFactorize(long n)
        {
            List<long> factors = Factorize(n);
            sortPrimes:
            for (int i = 0; i < factors.Count; i++)
            {
                long f = factors[i];
                if (!IsPrime(f))
                {
                    factors.RemoveAt(i);
                    factors.AddRange(Factorize(f));
                    goto sortPrimes;
                }
            }

            return factors;
        }

        public Fraction Expand(int factor)
        {
            Fraction result = new Fraction(this);
            // Multiply both Numerator and Denominator with a common factor.

            result.Numerator *= factor;
            if (factor != 0) result.Denominator *= factor;

            return result;
        }

        public override string ToString() => $"{Numerator}/{Denominator}";
        public bool Equals(Fraction frac) => (frac.Numerator == Numerator && frac.Denominator == Denominator);

        public static Fraction operator +(Fraction a, Fraction b)
        {
            long newDenominator = a.Denominator * b.Denominator;
            long newNumerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
            return new Fraction(newNumerator, newDenominator);
        }
        public static Fraction operator -(Fraction a) => new Fraction(-a.Numerator, a.Denominator);
        public static Fraction operator -(Fraction a, Fraction b) => a + (-b);
        public static Fraction operator /(Fraction a, long b) => new Fraction(a.Numerator, a.Denominator * b);
        public static Fraction operator *(Fraction a, long b) => new Fraction(a.Numerator * b, a.Denominator);
    }
}
