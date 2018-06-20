using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FE2PDF
{
    public static class Int2of5
    {
        private static string Encode(string Data)
        {
            try
            {
                //0 = thin
                //1 = thick
                IDictionary<int, string> NumbersMapping = new Dictionary<int, string>();
                NumbersMapping.Add(0, "00110");
                NumbersMapping.Add(1, "10001");
                NumbersMapping.Add(2, "01001");
                NumbersMapping.Add(3, "11000");
                NumbersMapping.Add(4, "00101");
                NumbersMapping.Add(5, "10100");
                NumbersMapping.Add(6, "01100");
                NumbersMapping.Add(7, "00011");
                NumbersMapping.Add(8, "10010");
                NumbersMapping.Add(9, "01010");

                if (String.IsNullOrEmpty(Data)) throw new Exception("No data received");
                if (!Data.All(Char.IsDigit)) throw new Exception("Only numbers are accepted");
                if (Data.Length % 2 != 0) throw new Exception("Number os digits have to be even");

                IList<KeyValuePair<int, string>> Digits = (from t in Data let key = Convert.ToInt32(t.ToString()) let value = NumbersMapping[Convert.ToInt32(t.ToString())] select new KeyValuePair<int, string>(Convert.ToInt32(t.ToString()), NumbersMapping[Convert.ToInt32(t.ToString())])).ToList();

                var Result = String.Empty;
                for (var i = 0; i < Digits.Count; i += 2)
                {
                    var Pair1 = Digits[i].Value;
                    var Pair2 = Digits[i + 1].Value;

                    //Pair 1 e 2 will get interleaved
                    //Pair 1 = will be bars
                    //Pair 2 = will be spaces
                    //Pseudo-codes:
                    //A = thin space
                    //B = thick space
                    //X = thin bar
                    //Y = thick bar
                    for (var j = 0; j < 5; j++)
                        Result += (Pair1[j].ToString() == "0" ? "X" : "Y") +
                                  (Pair2[j].ToString() == "0" ? "A" : "B");
                }

                //Append start and ending
                return "XAXA" + Result + "YAX";
            }
            catch (Exception ex)
            {
                return "#" + ex.Message;
            }
        }

        public static Image GenerateBarCode(string Data, int Width, int Height, int ScaleFactor)
        {
            var EncodedData = Encode(Data);
            
            if (string.IsNullOrEmpty(EncodedData))
                throw new Exception("Encoding process returned empty");
            
            if (EncodedData[0].ToString() == "#") 
                throw new Exception(EncodedData);

            int   Position = 20, ThinWidth = 1 * ScaleFactor, ThickWidth = 3 * ScaleFactor;
            Image img      = new Bitmap(Width, Height);

            using (var gr = Graphics.FromImage(img))
            {
                //Initial white color filling
                gr.FillRectangle(Brushes.Transparent, 0, 0, Width, Height);

                foreach (var t in EncodedData)
                {
                    switch (t.ToString())
                    {
                        case "A":
                            gr.FillRectangle(Brushes.Transparent, Position, 0, ThinWidth, Height);
                            Position += ThinWidth;
                            break;
                        case "B":
                            gr.FillRectangle(Brushes.Transparent, Position, 0, ThickWidth, Height);
                            Position += ThickWidth;
                            break;
                        case "X":
                            gr.FillRectangle(Brushes.Black, Position, 0, ThinWidth, Height);
                            Position += ThinWidth;
                            break;
                        case "Y":
                            gr.FillRectangle(Brushes.Black, Position, 0, ThickWidth, Height);
                            Position += ThickWidth;
                            break;
                    }
                }

                return img;
            }
        }

        public static string ToBase64(this Image img)
        {
            var imgFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            img.Save(imgFile);

            var base64String = string.Empty;

            using (var image = Image.FromFile(imgFile))
            {
                using (var m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    var imageBytes = m.ToArray();

                    base64String = Convert.ToBase64String(imageBytes);
                }
            }

            if (File.Exists(imgFile))
                File.Delete(imgFile);

            return base64String;
        }
    }
}