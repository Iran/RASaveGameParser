using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RASaveGameParser
{
    class Program
    {
        static void Main(string[] args)
        {


            var SaveGame = new RASaveGame("Savegame.834");
            Console.ReadLine();
        }
    }

    class RASaveGame
    {
        string Name = null;
        int? Number = null;
        int? ScenarioNumber = null;
        byte? HouseType = null;
        int? VersionRawID = null;
        DateTime? FileTime = null;

        public RASaveGame(string FilePath)
        {
            string[] SplitFile = FilePath.Split('.');
            this.Number = Int32.Parse(SplitFile[SplitFile.Length - 1]);

            Console.WriteLine(Number);

    	    using (BinaryReader b = new BinaryReader(File.Open(FilePath, FileMode.Open)))
	        {
                Parse_Save_Game_Name(b);
                this.ScenarioNumber = b.ReadInt32();
                Console.WriteLine(ScenarioNumber);

                this.HouseType = b.ReadByte();
                this.FileTime = File.GetLastWriteTime(FilePath);

                this.VersionRawID = b.ReadInt32();
                Console.WriteLine("{0:X}",VersionRawID);
                Console.WriteLine(this.FileTime);

	        }
	    }

        private void Parse_Save_Game_Name(BinaryReader b)
        {
                Byte[] NameBytes = b.ReadBytes(44);
                this.Name = Encoding.ASCII.GetString(NameBytes);

                string Gibberish = Encoding.ASCII.GetString(new Byte[] {0x0D, 0x0A, 0x00, 0x1A });
                this.Name = this.Name.Replace(Gibberish, "");
        }
    }
}
