using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TestingFunctions
{
    internal class Program
    {
        static void Main()
        {
            long current = DateTime.Now.Ticks;
            string filePath = @"C:\Users\harry\Desktop\srumutil.xml";
            int chunkSize = 100; // Set your desired chunk size

            // Load the XML document
            XDocument xmlDoc = XDocument.Load(filePath);

            // Get the root element of the XML document
            XElement rootElement = xmlDoc.Root;

            // Split the XML elements into chunks
            var chunks = SplitIntoChunks(rootElement.Elements(), chunkSize);

            // Process chunks in parallel
            Parallel.ForEach(chunks, chunk =>
            {
                Console.WriteLine("a");
                ProcessChunk(chunk);
            });

            Console.WriteLine("Processing complete.");
            long finish = DateTime.Now.Ticks;
            Console.WriteLine(finish - current);
        }

        static void diff()
        {
            string filePath = @"C:\Users\harry\Desktop\srumutil.xml";
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                    {
                }
            }
        }

        static void ProcessChunk(IEnumerable<XElement> chunk)
        {
            foreach (var element in chunk)
            {
                // Process each XML element in the chunk
            }
        }

        static IEnumerable<IEnumerable<T>> SplitIntoChunks<T>(IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
    }
}
