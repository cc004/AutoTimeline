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




        private static (long,int) Memmem_boss_comp(byte[] a, long alen, byte[] b_low, int blen, byte[] b_high, Func<long, bool> matchValidator)
        {
            long i, j, diff = alen - blen;
            for (i = 0; i < diff; i += 4) /* 4 bytes alignment */
            {
                j = blen-1;
                while (j >= 0)//从最高位byte开始匹配
                {
                    if ((a[i + j] > b_low[j]) && (a[i + j] < b_high[j]))//如果比下限高且比下限低，说明肯定属于这个范围
                    { break; }
                    else if ((a[i + j] == b_low[j]) || ((a[i + j] == b_high[j]))) { --j; }//如果和下限或者上限的某一位相同，则进一步比较
                    else { goto next; } //剩下的说明该位低于下限或者高于上限
                    
                }
                if (matchValidator(i))
                    return (i, BitConverter.ToInt32(new byte[] { a[i],a[i+1],a[i+2],a[i+3]}, 0));
                next:;
            }
            return (-1,-1);
        }

        private static (long, int) Memmem_unit_comp(byte[] a, long alen, byte[] b_low, int blen, byte[] b_high, Func<long, bool> matchValidator)//因为做了concat，比较起来有些麻烦
        {
            long i, j, diff = alen - blen;
            var true_b_low = new byte[] { b_low[0], b_low[1], b_low[2], b_low[3] };
            var true_b_high = new byte[] { b_high[0], b_high[1], b_high[2], b_high[3] };
            for (i = 0; i < diff; i += 4) /* 4 bytes alignment */
            {
                if (a[i] == a[i + 4] && a[i + 1] == a[i + 5] && a[i + 2] == a[i + 6] && a[i + 3] == a[i + 7]) //如果前后对称
                {
                    j = blen / 2 - 1;
                    while (j >= 0)//从最高位byte开始匹配
                    {
                        if ((a[i + j] > true_b_low[j]) && (a[i + j] < true_b_high[j]))//如果比下限高且比下限低，说明肯定属于这个范围
                        { break; }
                        else if ((a[i + j] == true_b_low[j]) || ((a[i + j] == true_b_high[j]))) { --j; }//如果和下限或者上限的某一位相同，则进一步比较
                        else { goto next; } //剩下的说明该位低于下限或者高于上限

                    }
                    if (matchValidator(i))
                        return (i, BitConverter.ToInt32(new byte[] { a[i], a[i + 1], a[i + 2], a[i + 3] }, 0));
                    next:;
                }
            }
            return (-1, -1);
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


        public static List<(long, int)> Bosscompscan(long handle, byte[] aob_low, byte[] aob_high, Func<long, bool> matchValidator, long blockToStart = 0, Action<string> callback = null)
        {
            long i = blockToStart;
            var res_list = new List<(long, int)>();
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
                var res= Memmem_boss_comp(va, mbi.RegionSize, aob_low, aob_low.Length, aob_high, r => matchValidator(mbi.BaseAddress + r));
                //long r = KMP.IndexOf(va, aob);
                if (res.Item1 >= 0)
                {
                    res_list.Add((mbi.BaseAddress + res.Item1, res.Item2));
                }
                i = mbi.BaseAddress + mbi.RegionSize;
            }
            return res_list;
        }

        public static List<(long, int)> Unitcompscan(long handle, byte[] aob_low, byte[] aob_high, Func<long, bool> matchValidator, long blockToStart = 0, Action<string> callback = null)
        {
            long i = blockToStart;
            var res_list = new List<(long, int)>();
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
                var res = Memmem_unit_comp(va, mbi.RegionSize, aob_low, aob_low.Length, aob_high, r => matchValidator(mbi.BaseAddress + r));
                //long r = KMP.IndexOf(va, aob);
                if (res.Item1 >= 0)
                {
                    res_list.Add((mbi.BaseAddress + res.Item1, res.Item2));
                }
                i = mbi.BaseAddress + mbi.RegionSize;
            }
            return res_list;
        }

    }
}
