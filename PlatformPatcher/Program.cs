using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using AssetsTools;

namespace PlatformPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            /*
            foreach (var a in client.GetStringAsync("http://l1-prod-patch-gzlj.bilibiligame.net/client_ob_24/Manifest/AssetBundles/Android/202103011331/manifest/manifest_assetmanifest")
                .Result.Split('\n').Select(a => a.Split(',').First())
                .SelectMany(a => client.GetStringAsync($"http://l1-prod-patch-gzlj.bilibiligame.net/client_ob_24/Manifest/AssetBundles/Android/202103011331/{a}").Result
                .Split('\n')).Select(a => a.Split(','))
                .Where(a => a[0].Contains("1701") || a[0].Contains("1702")))
            {
                Console.WriteLine(string.Join(",", a));
                File.WriteAllBytes(a[0], client.GetByteArrayAsync($"http://l1-prod-patch-gzlj.bilibiligame.net/client_ob_24/pool/AssetBundles/Android/{a[1].Substring(0, 2)}/{a[1]}").Result
                    );
            }
            */
            
            Console.WriteLine(Convert.ToBase64String("6B 20 E2 AB 6C 31 13 30 F7 61 D7 37 CE 3F 30 25 75 08 50 66 5E EA 58 B6 37 2F 8D 2F 57 50 1E B3 6D 5C 30 BA 27 C9 38 F5 BC A9 D1 D9 90 64 98 19 51 DB CA 43"
                .Split(' ').Select(i => (byte)int.Parse(i, System.Globalization.NumberStyles.HexNumber)).ToArray()));
            
            foreach (var file in Directory.GetFiles(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner/Assets/a_"))
            {
                var fn = Path.GetFileName(file);
                var ab = AssetBundleFile.LoadFromFile(file);
                foreach (var handle in ab.Files)
                {
                    if (handle.Name.EndsWith(".resS")) continue;
                    var asset = handle.ToAssetsFile();
                    asset.MetadataHeader.TargetPlatform = 19;
                    //asset.MetadataHeader.UnityVersion = "2018.4.21f1";
                    handle.LoadAssetsFile(asset);
                }
                //ab.Header.versionEngine = "2018.4.21f1";
                ab.SaveToFile(@$"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner/Assets/a/{fn}");
            }
            /*
            var ab = AssetBundleFile.LoadFromFile(@"D:\PrincessConnectReDive\a\64669037b7eb150c28347b8b0189c54188105906");
            Console.WriteLine(ab.Files[0].ToAssetsFile().MetadataHeader.TargetPlatform);
            var ab1 = AssetBundleFile.LoadFromFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101_.unity3d");

            var ab2 = AssetBundleFile.LoadFromFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101.unity3d");
            var a1 = ab1.Files[0].ToAssetsFile();

            var a2 = ab2.Files[0].ToAssetsFile();

            a2.MetadataHeader.UnityVersion = "2018.4.21f1";
            a2.MetadataHeader.TargetPlatform = 19;
            ab2.Files[0].LoadAssetsFile(a2);
            ab2.Header.versionEngine = "2018.4.21f1";
            ab2.SaveToFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101.unity3d");

            ab2 = AssetBundleFile.LoadFromFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101.unity3d");
            a2 = ab2.Files[0].ToAssetsFile();
            ab1.SaveToFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101_.unity3d");
            ab1 = AssetBundleFile.LoadFromFile(@"C:\Users\Administrator\Documents\Tencent Files\1176321897\FileRecv\priconner\Assets\a\all_battleunitprefab_100101_.unity3d");

            void Compare(byte[] a, byte[] b)
            {
                for (int i = 0; i < a.Length; ++i)
                    if (a[i] != b[i]) Console.WriteLine($"{i:x4} {a[i]:x2} {b[i]:x2}");
            }
            Compare(a1.Objects[0].Data, a2.Objects[0].Data);
            Compare(a1.Objects[1].Data, a2.Objects[1].Data);

            Compare(ab1.Files[1].Data, ab2.Files[1].Data);*/

        }
    }
}
