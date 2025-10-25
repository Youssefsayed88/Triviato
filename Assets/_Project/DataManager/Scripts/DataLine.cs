using System;

namespace Shacleau
{
    [Serializable]
    public class DataLine
    {
        public string s0 = "-";
        public string s1 = "-";
        public string s2 = "-";
        public string s3 = "-";
        public string s4 = "-";
        public string s5 = "-";
        public string s6 = "-";
        public string s7 = "-";
        public string s8 = "-";
        public string s9 = "-";
        public string s10 = "-";
        public string s11 = "-";
        public string s12 = "-";
        public string s13 = "-";
        public string s14 = "-";
        public string s15 = "-";
        public string s16 = "-";
        public string s17 = "-";
        public string s18 = "-";
        public string s19 = "-";
        public string s20 = "-";
        public string s21 = "-";
        public string s22 = "-";
        public string s23 = "-";
        public string s24 = "-";
        public string s25 = "-";

        public void SetData(int ID, string data)
        {
            switch (ID)
            {
                case 0:
                    s0 = data;
                    break;
                case 1:
                    s1 = data;
                    break;
                case 2:
                    s2 = data;
                    break;
                case 3:
                    s3 = data;
                    break;
                case 4:
                    s4 = data;
                    break;
                case 5:
                    s5 = data;
                    break;
                case 6:
                    s6 = data;
                    break;
                case 7:
                    s7 = data;
                    break;
                case 8:
                    s8 = data;
                    break;
                case 9:
                    s9 = data;
                    break;
                case 10:
                    s10 = data;
                    break;
                case 11:
                    s11 = data;
                    break;
                case 12:
                    s12 = data;
                    break;
                case 13:
                    s13 = data;
                    break;
                case 14:
                    s14 = data;
                    break;
                case 15:
                    s15 = data;
                    break;
                case 16:
                    s16 = data;
                    break;
                case 17:
                    s17 = data;
                    break;
                case 18:
                    s18 = data;
                    break;
                case 19:
                    s19 = data;
                    break;
                case 20:
                    s20 = data;
                    break;
                case 21:
                    s21 = data;
                    break;
                case 22:
                    s22 = data;
                    break;
                case 23:
                    s23 = data;
                    break;
                case 24:
                    s24 = data;
                    break;
                case 25:
                    s25 = data;
                    break;
                default:
                    // Handle the case when ID is out of range
                    throw new ArgumentOutOfRangeException(nameof(ID), "Invalid ID value");
            }
        }
        
        


    }
}