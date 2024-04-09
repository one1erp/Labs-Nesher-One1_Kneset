using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Write2HMI
{
    public class ReorderCls
    {


        [DllImport("gdi32.dll", EntryPoint = "GetCharacterPlacementW")]
        static extern uint GetCharacterPlacementW(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string lpString,
           int nCount, int nMaxExtent, ref GCP_RESULTS lpResults, uint dwFlags);

        //[DllImport("user32.dll", SetLastError = false)]
        //static extern IntPtr GetDesktopWindow();

        //[DllImport("user32.dll")]//For .NET Compact Framework, change user32.dll to coredll.dll
        //static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SetLayout")]
        public static extern IntPtr SetLayout(IntPtr hdc, UInt32 dwLayout);

        IntPtr hdc;

        public ReorderCls()
        {
            hdc = CreateCompatibleDC(new IntPtr(0));
            SetLayout(hdc, 0x1); // 0x1 - right to left layout
        }

        ~ReorderCls()
        {
            DeleteDC(hdc);
        }

        public string ReorderStr(string src)
        {

            if (string.IsNullOrEmpty(src))
            {
                return "";
            }


            GCP_RESULTS results;


            long ret;




            src = src.Replace("(", "*@*@*");
            src = src.Replace(")", "(");
            src = src.Replace("*@*@*", ")");

            int n = src.Length;
            //טיפול במקרה שהמחרוזת איזוגית אז נוסיף עוד תו רווח אחרת לא יוצג התו האחרון
            if ((n % 2) != 0) { src += " "; n++; }


            results = new GCP_RESULTS();

            results.StructSize = Marshal.SizeOf(typeof(GCP_RESULTS)); ;

            results.OutString = new string(' ', n);

            results.Order = new IntPtr(0);

            results.Dx = new IntPtr(0);

            results.CaretPos = new IntPtr(0);

            results.Class = new String(' ', n);
            // new string(n, null);//todo:

            results.GlyphCount = n;



            //            hwnd = GetDesktopWindow();

            //            hdc = GetWindowDC(hwnd);

            //      (IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string lpString,
            //    int nCount, int nMaxExtent, ref GCP_RESULTS lpResults, uint dwFlags);
            int lGCP_MAXEXTENT = (int)GCPFlags.GCP_MAXEXTENT;
            uint lGCP_REORDER = (uint)GCPFlags.GCP_REORDER;

            ret = GetCharacterPlacementW(hdc, src, n, lGCP_MAXEXTENT, ref results, lGCP_REORDER);

            var res = results.OutString;
            var charArr = res.Reverse();
            string newSrc = string.Concat(charArr);


            return newSrc;
        }
        public String ReorderByLineLength(String str, int LineLength)
        {
            int pos;
            String tmp = str;
            int nl;

            if (String.IsNullOrEmpty(str))
          
                
                
                return str;

            nl = tmp.IndexOf("\r");
            while (nl != -1)
            {
                tmp = tmp.Substring(0, nl) + new String(' ', LineLength - nl % LineLength) + tmp.Substring(nl + 1);
                nl = tmp.IndexOf("\r", nl);
            }

            for (int i = 0; tmp.Length - i >= LineLength; i += LineLength)
            {
                if (tmp.Substring(i + 1, 1) == " ")
                {
                    tmp = tmp.Substring(0, i) + tmp.Substring(i + 1);
                }
                pos = tmp.LastIndexOf(" ", i + LineLength, LineLength);
                if (pos > i && pos < i + LineLength - 1)
                {
                    tmp = tmp.Insert(pos, new String(' ', i + LineLength - pos));
                }
            }
            return tmp;
        }

        //public string ReorderByLineLengthOld(string src, int LineLength)
        //{
        //    for (int i = LineLength; i < src.Length; i += LineLength)
        //    {
        //        if (src[i] != ' ')
        //        {
        //            string txtSpaces = src.Substring(0, i);
        //            int startindex = txtSpaces.LastIndexOf(' ');
        //            if (!(startindex > (-1)))//מקרה שאין רווח באותה שורה
        //            {
        //                startindex = i;
        //            }
        //                int numSpaces = i - startindex;
        //                txtSpaces = src.Substring(0, startindex);
        //                string nextTxt = src.Substring(startindex);
        //                src = txtSpaces + new String(' ', numSpaces - 1) + nextTxt;
        //        }
        //    }

        //    return src;
        //}

    }


    [StructLayout(LayoutKind.Sequential)]
    public struct GCP_RESULTS
    {
        public int StructSize;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string OutString;
        public IntPtr Order;
        public IntPtr Dx;
        public IntPtr CaretPos;
        //   public IntPtr Class;
        public string Class;
        public IntPtr Glyphs;
        public int GlyphCount;
        public int MaxFit;
    }


    [Flags]
    public enum GCPFlags : uint
    {
        GCP_DBCS = 0x0001,
        GCP_REORDER = 0x0002,
        GCP_USEKERNING = 0x0008,
        GCP_GLYPHSHAPE = 0x0010,
        GCP_LIGATE = 0x0020,
        GCP_DIACRITIC = 0x0100,
        GCP_KASHIDA = 0x0400,
        GCP_ERROR = 0x8000,
        GCP_JUSTIFY = 0x00010000,
        GCP_CLASSIN = 0x00080000,
        GCP_MAXEXTENT = 0x00100000,
        GCP_JUSTIFYIN = 0x00200000,
        GCP_DISPLAYZWG = 0x00400000,
        GCP_SYMSWAPOFF = 0x00800000,
        GCP_NUMERICOVERRIDE = 0x01000000,
        GCP_NEUTRALOVERRIDE = 0x02000000,
        GCP_NUMERICSLATIN = 0x04000000,
        GCP_NUMERICSLOCAL = 0x08000000,
    }
}

