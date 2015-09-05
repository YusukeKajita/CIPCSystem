using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CIPC_CS_Unity.CODER
{
    public class MHCODER
    {
        public MHCODER()
        {
            
        }

        public static int[] MHENCODER(bool[] data)
        {
            List<int> lst_int;
            lst_int= new List<int>();
            bool oldstate=false;
            int count = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if(data[i] != oldstate)
                {
                    lst_int.Add(count);
                    count = 0;
                }

                count++;
                oldstate = data[i];
            }
            lst_int.Add(count);

            return lst_int.ToArray();
        }

        public static bool[] MHDECODER(int[] intdata)
        {
            List<bool> lst_bool;
            lst_bool = new List<bool>();

            for (int i = 0; i < intdata.Length; i++ )
            {
                for (int k = 0; k < intdata[i]; k++)
                {
                    if(i%2==0)
                    {
                        lst_bool.Add(false);
                    }
                    else
                    {
                        lst_bool.Add(true);
                    }
                }
            }
            return lst_bool.ToArray();
        }
    }
}
