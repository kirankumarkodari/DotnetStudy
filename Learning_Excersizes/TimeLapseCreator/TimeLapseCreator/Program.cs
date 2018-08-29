using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.IO;
namespace TimeLapseCreator
{
    class Program
    {
        const string basepath = @"D:\Projects\Work_Monitoring_System\2_Pre_RequirementAnalysis\2_POC\Web_App\Time_lapse(Video)\Timelapse_test_img\";

        static void Main(string[] args)
        {
            var Images_count = 39;
            using (var videowriter = new VideoFileWriter())
            {
                try
                {
                    videowriter.Open(basepath + "myfirstvedio.avi", 2048, 1536, 5, VideoCodec.MPEG4, 1000000); // 2048 --> Width  1536--> Height  15-->Framerate VideoEncoding-->MPEG4  Birate-->1000000
                    for (int imageframe = 0; imageframe < Images_count; imageframe++)
                    {
                        var img_path = string.Format("{0}{1}.jpg", basepath, imageframe);
                        /* using (Bitmap image = new Bitmap.FromFile(img_path) as Bitmap)
                         {

                         }*/
                        using (FileStream fs = new FileStream(img_path, FileMode.Open, FileAccess.Read))
                        {
                            using (Image original = Image.FromStream(fs))
                            {
                                videowriter.WriteVideoFrame(original as Bitmap);
                            }
                        }
                    }
                    videowriter.Close();

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
              
            }
        }
    }
}
