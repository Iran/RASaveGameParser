using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;

namespace RASaveGameParser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure we're using en-US culture to prevent weird issues from appearing
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            string filepath = Environment.CurrentDirectory;
            DirectoryInfo d = new DirectoryInfo(filepath);

            Console.WriteLine("Red Alert savegame parser written by Iran.");
            Console.WriteLine();
            Console.WriteLine("{0,3} | {1,10} | {2,3} | {3, 8} | {4,5} | {5,19}  | {6,5} |", "Num", "Name",
                "Lvl", "Side", "Vers", "File Time", "Is MP");
            Console.WriteLine("--------------------------------------------------------------------------");

            foreach (var file in d.GetFiles("savegame.*"))
            {
                var SaveGame = new RASaveGame(file.Name);
                Console.WriteLine("{0,3:D3} | {1,10} | {2,3} | {3, 8} | {4,5} | {5,19} | {6,5} |", SaveGame.Number, SaveGame.Name,
                    SaveGame.ScenarioNumber, SaveGame.Get_Player_House(), SaveGame.Get_Version_String(), 
                    SaveGame.Date, SaveGame.Is_Multiplayer());
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }
    }

    class RASaveGame
    {
        public string Name = null;
        public int? Number = null;
        public int? ScenarioNumber = null;
        public byte? HouseType = null;
        public int? VersionRawID = null;
        public DateTime? Date = null;

        public RASaveGame(string FilePath)
        {
            Parse_Save_Game_Number(FilePath);

    	    using (BinaryReader b = new BinaryReader(File.Open(FilePath, FileMode.Open)))
	        {
                Parse_Save_Game_Name(b);
                this.ScenarioNumber = b.ReadInt32();

                this.HouseType = b.ReadByte();
                this.Date = File.GetLastWriteTime(FilePath);

                this.VersionRawID = b.ReadInt32();

	        }
	    }

        private void Parse_Save_Game_Name(BinaryReader b)
        {
                Byte[] NameBytes = b.ReadBytes(44);
                this.Name = Encoding.ASCII.GetString(NameBytes);

                string Gibberish = Encoding.ASCII.GetString(new Byte[] {0x0D, 0x0A, 0x00, 0x1A });
                int RemoveIndex =this.Name.IndexOf(Gibberish);
                this.Name = this.Name.Remove(RemoveIndex);

        }

        private void Parse_Save_Game_Number(string FilePath)
        {
            string[] SplitFile = FilePath.Split('.');
            string FileExt = SplitFile[SplitFile.Length - 1].ToLowerInvariant();
            if (FileExt == "net")
            {
                this.Number = -1;
            }
            else
            {
                this.Number = Int32.Parse(FileExt);
            }
        }

        public bool Is_Multiplayer()
        {
            return this.ScenarioNumber == 0;
        }

        public string Get_Version_String()
        {
            switch (this.VersionRawID)
            {
                case 0x100618B: return "3.03";
                case 0x1007000: return "3.03p";

                default: return "Invalid/Unknown";
            }
        }

        public string Get_Player_House()
        {
            switch (this.HouseType)
            {
                case 0xDD: return "Skirmish";
                case 0x00: return "Spain";
                case 0x01: return "Greece";
                case 0x02: return "USSR";
                case 0x03: return "England";
                case 0x04: return "Ukraine";
                case 0x05: return "Germany";
                case 0x06: return "France";
                case 0x07: return "Turkey";
                case 0x08: return "GoodGuy";
                case 0x09: return "BadGuy";
                case 0x0A: return "Neutral";
                case 0x0B: return "Special";
                case 0x0C: return "Multi1";
                case 0x0D: return "Multi2";
                case 0x0E: return "Multi3";
                case 0x0F: return "Multi4";
                case 0x10: return "Multi5";
                case 0x11: return "Multi6";
                case 0x12: return "Multi7";
                case 0x13: return "Multi8";

                default: return "Invalid";
            }
        }
    }
}
