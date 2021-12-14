using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenShotGrabber
{
    class MixedRadix
    {
        List<List<char>> digits;
        int maxNum;
        public int currentNum = 0;
        private string countPath;

        public MixedRadix(List<List<char>> digits, string countPath)
        {
            this.digits = digits;
            this.countPath = countPath;
            maxNum = 1;
            for (int i = 0; i < digits.Count; i++)
            {
                maxNum *= digits[i].Count;
            }
        }

        public string Next(int next = 0)
        {
            File.WriteAllText(countPath, currentNum.ToString());
            next = next == 0 ? 1 : next;
            currentNum += next;
            if (!InBounds(currentNum)) { return null; }
            return GetRadix(currentNum);
        }

        public string GetRadix(int num)
        {
            if (!InBounds(num)){ return null; }

            string s = "";
            for (int i = 0; i < digits.Count; i++)
            {
                int divider = GetDivider(i);
                int digit = num / divider;
                s += digits[i][digit];
                num -= divider * digit;
            }
            return s;
        }

        private bool InBounds(int num)
        {
            if (num < 0 || num >= maxNum)
            {
                Console.WriteLine($"ERROR: the number {num} is out of bounds");
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetDivider(int digitIndex)
        {
            int divider = 1;
            for (int i = digits.Count - 1; i >= digitIndex; i--)
            {
                divider *= digits[i].Count;
            }
            divider /= digits[digitIndex].Count;
            
            return divider;
        }

    }
}
