using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

using ProphetLamb.Tools.Core;
using ProphetLamb.Tools.JsonResources;
using System;

namespace ProphetLamb.Tools.UnitTest
{
    public class JsonResourceTest
    {
        private const string resDir = ".\\resource_test";
        private static readonly CultureInfo german = CultureInfo.GetCultureInfo("de-de"),
                                            english = CultureInfo.GetCultureInfo("en-us");
        private ResourceManager resourceManager;

        private IFoo foo;

        [SetUp]
        public void SetUp()
        {
            // Clear directory
            if (Directory.Exists(resDir))
                Directory.Delete(resDir, true);
            Directory.CreateDirectory(resDir);

            foo = new Foo("Schneider", 0.3146d);
        }

        [Test]
        public void  FunctionalityTest()
        {
            resourceManager = new ResourceManager("CommonResource", resDir);
            using (var writer = new ResourceWriter(resourceManager, german))
            {
                writer.AddResource("first", "Hallo Welt!");
                writer.AddResource("0to100", foo);
            }
            using (var writer = new ResourceWriter(resourceManager, english))
            {
                writer.AddResource("first", "Hello World!");
                writer.AddResource("0to100", foo);
            }
            Assert.AreEqual(2, resourceManager.Cultures.Count());
            // Clear resources
            resourceManager.Dispose();
            resourceManager = new ResourceManager("CommonResource", resDir);
            using (var reader = new ResourceReader(resourceManager, german))
                reader.ReadToEnd();
            using (var reader = new ResourceReader(resourceManager, english))
                reader.ReadToEnd();
            resourceManager.Culture = german;
            Assert.AreEqual("Hallo Welt!", resourceManager.GetString("first"));
            var fooRes = resourceManager.GetObject("0to100") as Foo;
            Assert.AreEqual(foo, fooRes);
            Assert.AreEqual("Hello World!", resourceManager.GetString("first", english));

            Assert.Pass();
        }

        [Test]
        public void PerformanceTest()
        {
            resourceManager = new ResourceManager("PerfResource", resDir);
            using var rwGerman = new ResourceWriter(resourceManager, german);
            using var rwEnglish = new ResourceWriter(resourceManager, english);
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < 2048; i++)
            {
                rwEnglish.AddResource("ResNr_" + i, RandomString(4196));
            }
            rwEnglish.Close();
            for (int i = 0; i < 2048; i++)
            {
                rwGerman.AddResource("ResNr_" + i, RandomString(4196));
            }
            rwGerman.Close();
            sw.Stop();
            Console.WriteLine("Write: " + sw.Elapsed);
            resourceManager.Dispose();
            resourceManager = new ResourceManager("PerfResource", resDir);
            sw.Reset();
            using var rrGerman = new ResourceReader(resourceManager, german);
            using var rrEnglish = new ResourceReader(resourceManager, english);
            rrGerman.ReadToEnd();
            rrGerman.Close();
            rrEnglish.ReadToEnd();
            rrEnglish.Close();
            sw.Stop();
            Console.WriteLine("Read: " + sw.Elapsed);
            Assert.Pass();
        }

        private unsafe string RandomString(int length)
        {
            Random rng = new Random();
            string str = StringHelper.FastAllocateString(length);
            fixed (char* outStr = str)
            {
                for (int i = 0; i != length; i++)
                {
                    outStr[i] = (char)rng.Next(65, 89); // Uppercase ASCII letters
                }
            }
            return str;
        }
    }
}