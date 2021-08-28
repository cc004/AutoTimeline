using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace PCRAutoTimeline
{
    public class AobscanHelper
    {
        /// <summary>
        /// 搜索Byte数组
        /// </summary>
        /// <param name="a">源数组</param>
        /// <param name="alen">长度</param>
        /// <param name="b">被搜索的数组</param>
        /// <param name="blen">被搜数组的长度</param>
        /// <returns>失败返回-1</returns>
        private static long Memmem(byte[] a, long alen, byte[] b, int blen, Func<long, bool> matchValidator)
        {
            long i, j, diff = alen - blen;
            for (i = 0; i < diff; i += 4) /* 4 bytes alignment */
            {
                j = 0;
                while (j < blen)
                {
                    if (a[i + j] != b[j])
                        goto next;
                    ++j;
                }
                if (matchValidator(i))
                    return i;
                next: ;
            }
            return -1;
        }



        public delegate List<(long, long)> CompDele(byte[] a, long alen, int b_low, int b_high, Func<long, bool> matchValidator);
        public static List<(long, long)> MemmemBossComp(byte[] a, long alen, int b_low, int b_high, Func<long, bool> matchValidator)
        {
            long i, diff = alen - 4;
            var res_list = new List<(long, long)>();
            int int_place;
            for (i = 0; i < diff; i += 4) /* 4 bytes alignment */
            {   
                int_place = BitConverter.ToInt32(a, (int)i);
                if (int_place >= b_low && int_place <= b_high && matchValidator(i))
                { res_list.Add((i, int_place)); }
            }
            return res_list;
        }

        public static List<(long, long)> MemmemUnitComp(byte[] a, long alen, int b_low, int b_high, Func<long, bool> matchValidator)//因为做了concat，比较起来有些麻烦
        {
            long i, diff = alen - 8;
            int int_place;
            var res_list = new List<(long, long)>();//实验结果发现在一个BLOCK里可能有多个角色，所以还是得用List存，否则要大改循环


            for (i = 0; i < diff; i += 4) /* 4 bytes alignment */
            {
                if (a[i] == a[i + 4] && a[i + 1] == a[i + 5] && a[i + 2] == a[i + 6] && a[i + 3] == a[i + 7]) //如果前后对称
                {
                    int_place = BitConverter.ToInt32(a, (int)i);

                    if (int_place >= b_low && int_place <= b_high && matchValidator(i))
                    {
                        Console.WriteLine(int_place);
                        res_list.Add((i, int_place));
                    }
                }
            }
            return res_list;
        }

        /// <summary>
        /// 失败返回-1
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="aob"></param>
        /// <returns></returns>
        public static (long, long) Aobscan(long handle, byte[] aob, Func<long, bool> matchValidator, long blockToStart = 0, Action<string> callback = null)
        {
            long i = blockToStart;
            while (i < long.MaxValue)
            {
                int flag = NativeFunctions.VirtualQueryEx(handle, i, out NativeFunctions.MEMORY_BASIC_INFORMATION mbi, NativeFunctions.MEMORY_BASIC_INFORMATION_SIZE);
                if (flag != NativeFunctions.MEMORY_BASIC_INFORMATION_SIZE)
                    break;
                if (mbi.RegionSize <= 0)
                    break;
                if (mbi.State != (int)NativeFunctions.AllocationType.Commit)
                {
                    i = mbi.BaseAddress + mbi.RegionSize;
                    continue;
                }
                if (callback != null) callback($"scanning {mbi.BaseAddress:x}...");
                else Console.Write($"\rscanning {mbi.BaseAddress:x}...");
                byte[] va = new byte[mbi.RegionSize];
                NativeFunctions.ReadProcessMemory(handle, mbi.BaseAddress, va, mbi.RegionSize, 0);
                long r = Memmem(va, mbi.RegionSize, aob, aob.Length, r => matchValidator(mbi.BaseAddress + r));
                //long r = KMP.IndexOf(va, aob);  还好最后没用KMP，否则比较匹配匹配不了了，得重新写
                if (r >= 0)
                {
                    return (mbi.BaseAddress + r, i);
                }
                i = mbi.BaseAddress + mbi.RegionSize;
            }
            return (-1, -1);
        }


        public static List<(long, long)> Compscan(long handle, int aob_low, int aob_high, Func<long, bool> matchValidator, CompDele memComp, long blockToStart = 0, Action<string> callback = null)
        {
            long i = blockToStart;
            var res_list = new List<(long, long)>();
            while (i < long.MaxValue)
            {
                int flag = NativeFunctions.VirtualQueryEx(handle, i, out NativeFunctions.MEMORY_BASIC_INFORMATION mbi, NativeFunctions.MEMORY_BASIC_INFORMATION_SIZE);
                if (flag != NativeFunctions.MEMORY_BASIC_INFORMATION_SIZE)
                    break;
                if (mbi.RegionSize <= 0)
                    break;
                if (mbi.State != (int)NativeFunctions.AllocationType.Commit)
                {
                    i = mbi.BaseAddress + mbi.RegionSize;
                    continue;
                }
                if (callback != null) callback($"scanning {mbi.BaseAddress:x}...");
                else Console.Write($"\rscanning {mbi.BaseAddress:x}...");
                byte[] va = new byte[mbi.RegionSize];
                NativeFunctions.ReadProcessMemory(handle, mbi.BaseAddress, va, mbi.RegionSize, 0);
                var res = memComp(va, mbi.RegionSize, aob_low,aob_high, r => matchValidator(mbi.BaseAddress + r));
                //long r = KMP.IndexOf(va, aob);
                if (res.Count > 0)
                {
                    foreach (var res_list_data in res) 
                    {
                        res_list.Add((mbi.BaseAddress + res_list_data.Item1, res_list_data.Item2));
                    }
                    
                }
                i = mbi.BaseAddress + mbi.RegionSize;
            }
            return res_list;
        }

    }
}
