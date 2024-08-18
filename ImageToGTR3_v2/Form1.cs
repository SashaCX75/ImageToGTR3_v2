using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeppOS_Converter_VB_;

namespace ImageToZeppOS
{
    public partial class Form1 : Form
    {
        private byte[] _streamBuffer;
        List<Color> colorMapList = new List<Color>();
        int ImageWidth;

        public Form1()
        {
            InitializeComponent();

            label_version.Text = "v " +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
        }

        private void button_TgaToPng_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png, *.tga) | *.png; *.tga";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            //openFileDialog.Title = Properties.FormStrings.Dialog_Title_Pack;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> FileNames = openFileDialog.FileNames.ToList();
                progressBar1.Value = 0;
                progressBar1.Maximum = FileNames.Count;
                string path = "";
                int fix_color = 1;
                if (radioButton_type2.Checked) fix_color = 2;
                if (radioButton_type3.Checked) fix_color = 3;
                progressBar1.Visible = true;
                foreach (String file in FileNames)
                {
                    progressBar1.Value++;
                    path = ImageAutoDetectReadFormat(file, fix_color);
                }
                progressBar1.Visible = false;
                if (path.Length > 5 && Directory.Exists(path))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", path));
                }
            }
        }

        private string ImageAutoDetectReadFormat(string fileNameFull, int fix_color, bool newFolder = true)
        {
            string path = "";
            if (File.Exists(fileNameFull))
            {
                if (newFolder)
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                        string targetFolder = Path.Combine(Path.GetDirectoryName(fileNameFull), "Png");
                        if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
                        string targetFileName = Path.Combine(targetFolder, fileName + ".png");
                        using (var fileStream = File.OpenRead(fileNameFull))
                        {
                            _streamBuffer = new byte[fileStream.Length];
                            fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                            Header header = new Header(_streamBuffer, fileNameFull, targetFileName);
                            if (checkBox_externalConverter.Checked && fix_color == 1)
                            {
                                ZeppOSConverter_VB myLibrary = new ZeppOSConverter_VB();
                                bool result = myLibrary.MyMethod(fileNameFull, targetFileName);
                                if (result) path = Path.GetDirectoryName(targetFileName);
                            }
                            else
                            {
                                if (header.GetExistsColorMap() == 1 && header.GetImageType() == 1) path = TgaToPng(fileNameFull, targetFileName, fix_color);
                                if (header.GetExistsColorMap() == 0 && header.GetImageType() == 2)
                                {
                                    if (header.GetBitsPerPixel() != 32 && fix_color == 1)
                                    {
                                        ZeppOSConverter_VB myLibrary = new ZeppOSConverter_VB();
                                        bool result = myLibrary.MyMethod(fileNameFull, targetFileName);
                                        if (result) path = Path.GetDirectoryName(targetFileName);
                                    }
                                    else
                                    {
                                        path = TgaARGBToPng(fileNameFull, targetFileName, fix_color);
                                    }
                                }
                                if (header.GetExistsColorMap() == 1 && header.GetImageType() == 9) path = TgaToPng(fileNameFull, targetFileName, fix_color); 
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("Не верный формат исходного файла" + Environment.NewLine +
                            exp);
                    } 
                }
                else
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                        using (var fileStream = File.OpenRead(fileNameFull))
                        {
                            _streamBuffer = new byte[fileStream.Length];
                            fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                            Header header = new Header(_streamBuffer, Path.GetFileName(fileNameFull));
                            if (checkBox_externalConverter.Checked && fix_color == 1)
                            {
                                ZeppOSConverter_VB myLibrary = new ZeppOSConverter_VB();
                                bool result = myLibrary.MyMethod(fileNameFull, fileNameFull + "_temp");
                                if (result) path = Path.GetDirectoryName(fileNameFull + "_temp");
                            }
                            else
                            {
                                if (header.GetExistsColorMap() == 1 && header.GetImageType() == 1) path = TgaToPng(fileNameFull, fileNameFull + "_temp", fix_color);
                                if (header.GetExistsColorMap() == 0 && header.GetImageType() == 2)
                                {
                                    if (header.GetBitsPerPixel() != 32 && fix_color == 1)
                                    {
                                        ZeppOSConverter_VB myLibrary = new ZeppOSConverter_VB();
                                        bool result = myLibrary.MyMethod(fileNameFull, fileNameFull + "_temp");
                                        if (result) path = Path.GetDirectoryName(fileNameFull + "_temp");
                                    }
                                    else
                                    {
                                        path = TgaARGBToPng(fileNameFull, fileNameFull + "_temp", fix_color);
                                    }
                                }
                                if (header.GetExistsColorMap() == 1 && header.GetImageType() == 9) path = TgaToPng(fileNameFull, fileNameFull + "_temp", fix_color); 
                            }
                        }
                        if (File.Exists(fileNameFull + "_temp"))
                        {
                            if (File.Exists(fileNameFull)) File.Delete(fileNameFull);
                            File.Move(fileNameFull + "_temp", fileNameFull);
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("Не верный формат исходного файла" + Environment.NewLine +
                            exp);
                    }
                }
            }
            return path;
        }

        /// <summary>Преобразуем ARGB Tga в Png</summary>
        private string TgaARGBToPng(string file, string targetFile, int fix_color)
        {
            string path = "";
            if (File.Exists(file))
            {
                try
                {
                    //string fileNameFull = openFileDialog.FileName;
                    ImageMagick.MagickImage image;
                    string fileNameFull = file;
                    string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                    path = Path.GetDirectoryName(fileNameFull);
                    //fileName = Path.Combine(path, fileName);
                    int RealWidth = -1;
                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        byte[] _streamBuffer;
                        _streamBuffer = new byte[fileStream.Length];
                        fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                        Header header = new Header(_streamBuffer, Path.GetFileName(fileNameFull));
                        ImageDescription imageDescription = new ImageDescription(_streamBuffer, header.GetImageIDLength());
                        RealWidth = imageDescription.GetRealWidth();
                    }

                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        image = new ImageMagick.MagickImage(fileStream, ImageMagick.MagickFormat.Tga);
                    }

                    image.Format = ImageMagick.MagickFormat.Png32;
                    if (RealWidth > 0 && RealWidth != image.Width)
                    {
                        int height = image.Height;
                        image = (ImageMagick.MagickImage)image.Clone(RealWidth, height);
                    }

                    //ImageMagick.IMagickImage Blue = image.Separate(ImageMagick.Channels.Blue).First();
                    //ImageMagick.IMagickImage Red = image.Separate(ImageMagick.Channels.Red).First();
                    //ImageMagick.IMagickImage Alpha = image.Separate(ImageMagick.Channels.Red).First();
                    //if (fix_color == 1)
                    //{
                    //    image.Composite(Red, ImageMagick.CompositeOperator.Replace, ImageMagick.Channels.Blue);
                    //    image.Composite(Blue, ImageMagick.CompositeOperator.Replace, ImageMagick.Channels.Red);
                    //}
                    image.Write(targetFile);
                }
                catch (Exception exp)
                {
                    if (exp.Message != "Отсутствует карта цветов")
                    {
                        MessageBox.Show("Не верный формат исходного файла." + Environment.NewLine +
                                        "Файл \"" + Path.GetFileName(targetFile) + "\" не был сохранен." +
                                        Environment.NewLine + exp);
                    }
                }
            }
            path = Path.GetDirectoryName(targetFile);
            return path;
        }

        /// <summary>Преобразуем Tga в Png</summary>
        private string TgaToPng(string file, string targetFile, int fix_color)
        {
            string path = "";
            if (File.Exists(file))
            {
                try
                {
                    //string fileNameFull = openFileDialog.FileName;
                    ImageMagick.MagickImage image;
                    string fileNameFull = file;
                    string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                    path = Path.GetDirectoryName(fileNameFull);
                    //fileName = Path.Combine(path, fileName);
                    int RealWidth = -1;
                    bool colored = true;
                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        byte[] _streamBuffer;
                        _streamBuffer = new byte[fileStream.Length];
                        fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                        Header header = new Header(_streamBuffer, Path.GetFileName(fileNameFull));
                        if (header.GetColorMapCount() == 0) colored = false;
                        ImageDescription imageDescription = new ImageDescription(_streamBuffer, header.GetImageIDLength());
                        RealWidth = imageDescription.GetRealWidth();
                    }

                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        image = new ImageMagick.MagickImage(fileStream, ImageMagick.MagickFormat.Tga);
                    }

                    image.Format = ImageMagick.MagickFormat.Png32;
                    if (RealWidth > 0 && RealWidth != image.Width)
                    {
                        int height = image.Height;
                        image = (ImageMagick.MagickImage)image.Clone(RealWidth, height);
                    }

                    ImageMagick.IMagickImage Blue = image.Separate(ImageMagick.Channels.Blue).First();
                    ImageMagick.IMagickImage Red = image.Separate(ImageMagick.Channels.Red).First();
                    ImageMagick.IMagickImage Alpha = image.Separate(ImageMagick.Channels.Red).First();
                    if (fix_color == 1)
                    {
                        image.Composite(Red, ImageMagick.CompositeOperator.Replace, ImageMagick.Channels.Blue);
                        image.Composite(Blue, ImageMagick.CompositeOperator.Replace, ImageMagick.Channels.Red);
                        if (!colored)
                        {
                            image.Composite(Alpha, ImageMagick.CompositeOperator.CopyAlpha, ImageMagick.Channels.Alpha);
                        }
                        image.Write(targetFile, MagickFormat.Png);
                    }
                    if (fix_color == 2)
                    {
                        if (!colored)
                        {
                            image.Composite(Alpha, ImageMagick.CompositeOperator.CopyAlpha, ImageMagick.Channels.Alpha);
                        }
                        image.Write(targetFile, MagickFormat.Png);
                    }
                    if (fix_color == 3)
                    {
                        if (image.ColormapSize == 256)
                        {
                            MagickColor[,] colorMap = new MagickColor[16, 16];
                            int index = 0;
                            for (int x = 0; x < 16; x++)
                            {
                                for (int y = 0; y < 16; y++)
                                {
                                    colorMap[x, y] = image.GetColormap(index);
                                    index++;
                                }
                            }
                            index = 0;
                            for (int x = 0; x < 16; x++)
                            {
                                for (int y = 0; y < 16; y++)
                                {
                                    image.SetColormap(index, colorMap[y, x]);
                                    index++;
                                }
                            }
                            index = 0;
                            for (int x = 0; x < 16; x++)
                            {
                                for (int y = 0; y < 16; y++)
                                {
                                    colorMap[x, y] = image.GetColormap(index);
                                    index++;
                                }
                            }
                        }
                        if (!colored)
                        {
                            image.Composite(Alpha, ImageMagick.CompositeOperator.CopyAlpha, ImageMagick.Channels.Alpha);
                        }

                        image.Format = MagickFormat.Png32;
                        image.Write(targetFile + "_temp", MagickFormat.Png);

                        using (var fileStream = File.OpenRead(targetFile + "_temp"))
                        {
                            image = new ImageMagick.MagickImage(fileStream, ImageMagick.MagickFormat.Png32);
                            image.Write(targetFile, ImageMagick.MagickFormat.Png32);
                        }
                        File.Delete(targetFile + "_temp");
                    }
                }
                catch (Exception exp)
                {
                    if (exp.Message != "Отсутствует карта цветов")
                    {
                        MessageBox.Show("Не верный формат исходного файла." + Environment.NewLine +
                                        "Файл \"" + Path.GetFileName(targetFile) + "\" не был сохранен." +
                                        Environment.NewLine + exp);
                    }
                }
            }
            path = Path.GetDirectoryName(targetFile);
            return path;
        }

        private void button_PngToTga_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png) | *.png";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            //openFileDialog.Title = Properties.FormStrings.Dialog_Title_Pack;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> FileNames = openFileDialog.FileNames.ToList();
                progressBar1.Value = 0;
                progressBar1.Maximum = FileNames.Count;
                progressBar1.Visible = true;
                string fileNameFull = "";
                int fix_color = 1;
                if (radioButton_type2.Checked) fix_color = 2;
                if (radioButton_type3.Checked) fix_color = 3;
                bool fix_size = radioButton_type1.Checked;
                foreach (String file in FileNames)
                {
                    progressBar1.Value++;
                    //ImageAutoDetectBestFormat(file);
                    fileNameFull = ImageAutoDetectBestFormat(file, fix_size, fix_color);
                    //if (fileNameFull != null) ImageFix(fileNameFull);
                }
                progressBar1.Visible = false;
                if (Directory.Exists(Path.GetDirectoryName(fileNameFull)))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", Path.GetDirectoryName(fileNameFull)));
                }
            }
        }

        private string ImageAutoDetectBestFormat(string fileNameFull, bool fix_size, int fix_color, bool newFolder = true)
        {
            if (!File.Exists(fileNameFull)) return null;
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                string path = Path.GetDirectoryName(fileNameFull);
                ImageMagick.MagickImage image;

                using (var fileStream = File.OpenRead(fileNameFull))
                {
                    image = new ImageMagick.MagickImage(fileStream);
                }
                ImageWidth = image.Width;
                int imageWidth = ImageWidth;
                int newWidth = ImageWidth;
                int newHeight = image.Height;

                if (fix_size)
                {
                    while (newWidth % 16 != 0)
                    {
                        newWidth++;
                    }

                    if (ImageWidth != newWidth)
                    {
                        Bitmap bitmap = image.ToBitmap();
                        Bitmap bitmapNew = new Bitmap(newWidth, newHeight);
                        Graphics gfx = Graphics.FromImage(bitmapNew);
                        gfx.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                        image = new ImageMagick.MagickImage(bitmapNew);
                    }
                }

                string targetFolder = Path.Combine(path, "Fix");
                if (!newFolder) targetFolder = path;
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                if ((image.TotalColors >= numericUpDown_colorCount.Value || checkBox_newAlgorithm.Checked) && !checkBox_oldAlgorithm.Checked)
                {
                    fileNameFull = PngToTgaARGB(fileNameFull, targetFolder, image, fix_color);
                    ImageFix_ARGB(fileNameFull, imageWidth);
                    return fileNameFull;
                }
                else
                {
                    fileNameFull = PngToTga(fileNameFull, targetFolder, image, fix_color);
                    if (fileNameFull != null) ImageFix(fileNameFull, fix_color);
                    return fileNameFull;
                }

            }
            catch (Exception exp)
            {
                MessageBox.Show("Не верный формат исходного файла" + Environment.NewLine + exp);
            }
            return null;
        }

        private string PngToTgaARGB(string fileNameFull, string targetFolder, ImageMagick.MagickImage image, int fix_color)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                //string path = Path.GetDirectoryName(fileNameFull);
                //ImageMagick.MagickImage image_temp = new ImageMagick.MagickImage(image);

                if (image.ChannelCount != 4) 
                { 
                    image.Format = MagickFormat.Png32;
                    image.Alpha(AlphaOption.On);
                }
                if (image.ColorSpace != ImageMagick.ColorSpace.sRGB || image.ChannelCount != 4)
                {
                    MessageBox.Show("Изображение не должно быть монохромным и должно быть в формате 32bit" +
                        Environment.NewLine + fileNameFull);
                    return null;
                }
               
                if (fix_color == 1)
                {
                    IPixelCollection pixels = image.GetPixels();
                    for (int w = 0; w < image.Width; w++)
                    {
                        for (int h = 0; h < image.Height; h++)
                        {
                            ushort red = pixels[w, h].GetChannel(0);
                            ushort green = pixels[w, h].GetChannel(1);
                            ushort blue = pixels[w, h].GetChannel(2);
                            ushort alpha = pixels[w, h].GetChannel(3);
                            float scale = (float)alpha / ushort.MaxValue;

                            red = (ushort)(red * scale);
                            green = (ushort)(green * scale);
                            blue = (ushort)(blue * scale);

                            pixels[w, h].SetChannel(0, red);
                            pixels[w, h].SetChannel(1, green);
                            pixels[w, h].SetChannel(2, blue);
                            pixels[w, h].SetChannel(3, alpha);
                        }
                    }
                }


                //path = Path.Combine(path, "Fix");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                string newFileName = Path.Combine(targetFolder, fileName + ".tga");
                image.Write(newFileName, ImageMagick.MagickFormat.Tga);
                return newFileName;

            }
            catch (Exception exp)
            {
                MessageBox.Show("Не верный формат исходного файла" + Environment.NewLine + exp);
            }
            return null;
        }

        private void ImageFix_ARGB(string fileNameFull, int imageWidth)
        {
            if (File.Exists(fileNameFull))
            {
                try
                {
                    byte[] _streamBuffer;
                    string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                    string path = Path.GetDirectoryName(fileNameFull);

                    // читаем картинку в массив
                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        _streamBuffer = new byte[fileStream.Length];
                        fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                        Header header = new Header(_streamBuffer);
                        ImageDescription imageDescription = new ImageDescription(_streamBuffer, header.GetImageIDLength());

                        byte ImageIDLength = header.GetImageIDLength(); // длина описания
                        Image_data imageData = new Image_data(_streamBuffer, 18 + ImageIDLength);

                        Footer footer = new Footer();

                        #region fix
                        header.SetImageIDLength(46);
                        imageDescription.SetSize(46, imageWidth);

                        header.SetColorMapCount(0);
                        header.SetColorMapEntrySize(0);
                        #endregion

                        int newLength = 18 + header.GetImageIDLength() + imageData._imageData.Length;
                        //if (checkBox_Footer.Checked) newLength = newLength + footer._footer.Length;
                        byte[] newTGA = new byte[newLength];

                        header._header.CopyTo(newTGA, 0);
                        int offset = header._header.Length;

                        imageDescription._imageDescription.CopyTo(newTGA, offset);
                        offset = offset + imageDescription._imageDescription.Length;

                        imageData._imageData.CopyTo(newTGA, offset);
                        offset = offset + imageData._imageData.Length;

                        //if (checkBox_Footer.Checked) footer._footer.CopyTo(newTGA, offset);

                        if (newTGA != null && newTGA.Length > 0)
                        {
                            string newFileName = Path.Combine(path, fileName + ".png");

                            using (var fileStreamTGA = File.OpenWrite(newFileName))
                            {
                                fileStreamTGA.Write(newTGA, 0, newTGA.Length);
                                fileStreamTGA.Flush();
                            }
                        }
                    }

                    try
                    {
                        File.Delete(fileNameFull);
                    }
                    catch (Exception)
                    {
                    }

                }
                catch (Exception exp)
                {
                    MessageBox.Show(Properties.FormStrings.Message_ImageFix_Error + Environment.NewLine + exp,
                        Properties.FormStrings.Message_Warning_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private string PngToTga(string fileNameFull, string targetFolder, ImageMagick.MagickImage image, int fix_color)
        {
            if (File.Exists(fileNameFull))
            {
                colorMapList.Clear();
                try
                {
                    //string fileNameFull = openFileDialog.FileName;
                    string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                    string path = Path.GetDirectoryName(fileNameFull);
                    //fileName = Path.Combine(path, fileName);
                    //ImageMagick.MagickImage image;
                    ImageMagick.MagickImage image_temp = new MagickImage(image);

                    ImageMagick.Pixel pixel = image.GetPixels().GetPixel(0, 0);
                    bool transparent = false;

                    image.ColorType = ImageMagick.ColorType.Palette;
                    if (image.ColorSpace != ImageMagick.ColorSpace.sRGB)
                    {
                        image = image_temp;
                        ushort[] p;
                        if (pixel[2] > 256)
                        {
                            if (pixel.Channels == 4) p = new ushort[] { pixel[0], pixel[1], (ushort)(pixel[2] - 256), pixel[3] };
                            else p = new ushort[] { pixel[0], pixel[1], (ushort)(pixel[2] - 256) };
                        }
                        else
                        {
                            if (pixel.Channels == 4) p = new ushort[] { pixel[0], pixel[1], (ushort)(pixel[2] + 256), pixel[3] };
                            else
                            {
                                p = new ushort[] { pixel[0], pixel[1], (ushort)(pixel[2] + 256) };
                                transparent = true;
                            }
                        }
                        image.GetPixels().SetPixel(0, 0, p);
                        pixel = image.GetPixels().GetPixel(0, 0);
                        image.ColorType = ImageMagick.ColorType.Palette;
                        pixel = image.GetPixels().GetPixel(0, 0);
                        if (image.ColorSpace != ImageMagick.ColorSpace.sRGB)
                        {
                            MessageBox.Show("Изображение не должно быть монохромным и должно быть в формате 32bit" +
                                Environment.NewLine + fileNameFull);
                            return null;
                        }
                    }


                    for (int i = 0; i < image.ColormapSize; i++)
                    {
                        colorMapList.Add(image.GetColormap(i));
                    }
                    while (fix_color == 3 && colorMapList.Count < 256)
                    {
                        colorMapList.Add(Color.FromArgb(0, 0, 0, 0));
                    }
                    if (transparent && colorMapList.Count == 2)
                    {
                        colorMapList[0] = Color.FromArgb(0, colorMapList[0].R, colorMapList[0].G, colorMapList[0].B);
                        colorMapList[1] = Color.FromArgb(0, colorMapList[1].R, colorMapList[1].G, colorMapList[1].B);
                    }
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    string newFileName = Path.Combine(targetFolder, fileName + ".tga");
                    image.Write(newFileName, ImageMagick.MagickFormat.Tga);
                    return newFileName;

                }
                catch (Exception exp)
                {
                    MessageBox.Show("Не верный формат исходного файла" + Environment.NewLine +
                            exp);
                }
            }
            return null;
        }

        private void ImageFix(string fileNameFull, int fix_color)
        {
            if (File.Exists(fileNameFull))
            {
                try
                {
                    byte[] _streamBuffer;
                    string fileName = Path.GetFileNameWithoutExtension(fileNameFull);
                    string path = Path.GetDirectoryName(fileNameFull);
                    //fileName = Path.Combine(path, fileName);

                    //ImageMagick.MagickImage image = new ImageMagick.MagickImage(fileNameFull, ImageMagick.MagickFormat.Tga);

                    // читаем картинку в массив
                    using (var fileStream = File.OpenRead(fileNameFull))
                    {
                        _streamBuffer = new byte[fileStream.Length];
                        fileStream.Read(_streamBuffer, 0, (int)fileStream.Length);

                        Header header = new Header(_streamBuffer);
                        ImageDescription imageDescription = new ImageDescription(_streamBuffer, header.GetImageIDLength());

                        int ColorMapCount = header.GetColorMapCount(); // количество цветов в карте
                        byte ColorMapEntrySize = header.GetColorMapEntrySize(); // битность цвета
                        byte ImageIDLength = header.GetImageIDLength(); // длина описания
                        ColorMap ColorMap = new ColorMap(_streamBuffer, ColorMapCount, ColorMapEntrySize, 18 + ImageIDLength);

                        int ColorMapLength = ColorMap._colorMap.Length;
                        Image_data imageData = new Image_data(_streamBuffer, 18 + ImageIDLength + ColorMapLength);

                        Footer footer = new Footer();

                        #region fix
                        header.SetImageIDLength(46);
                        imageDescription.SetSize(46, ImageWidth);
                        //imageDescription.SetSize(46, header.Width);

                        int colorMapCount = ColorMap.ColorMapCount;
                        bool argb_brga = true;
                        colorMapCount = 256;
                        header.SetColorMapCount(colorMapCount);
                        byte colorMapEntrySize = 32;

                        ColorMap.RestoreColor(colorMapList);
                        ColorMap.ColorsFix(argb_brga, colorMapCount, colorMapEntrySize, fix_color);
                        header.SetColorMapEntrySize(32);
                        #endregion

                        int newLength = 18 + header.GetImageIDLength() + ColorMap._colorMap.Length + imageData._imageData.Length;
                        //if (checkBox_Footer.Checked) newLength = newLength + footer._footer.Length;
                        byte[] newTGA = new byte[newLength];

                        header._header.CopyTo(newTGA, 0);
                        int offset = header._header.Length;

                        imageDescription._imageDescription.CopyTo(newTGA, offset);
                        offset = offset + imageDescription._imageDescription.Length;

                        ColorMap._colorMap.CopyTo(newTGA, offset);
                        offset = offset + ColorMap._colorMap.Length;

                        imageData._imageData.CopyTo(newTGA, offset);
                        offset = offset + imageData._imageData.Length;

                        if (newTGA != null && newTGA.Length > 0)
                        {
                            string newFileName = Path.Combine(path, fileName + ".png");

                            using (var fileStreamTGA = File.OpenWrite(newFileName))
                            {
                                fileStreamTGA.Write(newTGA, 0, newTGA.Length);
                                fileStreamTGA.Flush();
                            }
                        }
                    }

                    try
                    {
                        File.Delete(fileNameFull);
                    }
                    catch (Exception)
                    {
                    }

                }
                catch (Exception exp)
                {
                    MessageBox.Show(Properties.FormStrings.Message_ImageFix_Error + Environment.NewLine + exp,
                        Properties.FormStrings.Message_Warning_Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void checkBox_Algorithm_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_newAlgorithm.Checked || checkBox_oldAlgorithm.Checked) numericUpDown_colorCount.Enabled = false;
            else numericUpDown_colorCount.Enabled = true;
            checkBox_newAlgorithm.Enabled = !checkBox_oldAlgorithm.Checked;
            checkBox_oldAlgorithm.Enabled = !checkBox_newAlgorithm.Checked;
        }

        private void button_Batch_TgaToPng_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            dialog.RestoreDirectory = true;
            dialog.EnsureValidNames = false;
            //dialog.Title = "Выбрать папку с !ut файлами";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dirName = dialog.FileName;

                List<string> allTempsFiles = GetRecursFiles(dirName, "*.png", 5, dirName);
                progressBar1.Value = 0;
                progressBar1.Maximum = allTempsFiles.Count;
                progressBar1.Visible = true;

                string path = "";
                int fix_color = 1;
                if (radioButton_type2.Checked) fix_color = 2;
                if (radioButton_type3.Checked) fix_color = 3;

                foreach (string file in allTempsFiles)
                {
                    progressBar1.Value++;
                    string fullFileName = dirName + file;
                    if (File.Exists(fullFileName))
                    {
                        path = ImageAutoDetectReadFormat(fullFileName, fix_color, false);
                    }
                }

                progressBar1.Visible = false;
                if (path.Length > 5 && Directory.Exists(dirName))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", dirName));
                }
            }
        }

        private void button_Batch_PngToTga_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            dialog.RestoreDirectory = true;
            dialog.EnsureValidNames = false;
            //dialog.Title = "Выбрать папку с !ut файлами";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string dirName = dialog.FileName;

                List<string> allTempsFiles = GetRecursFiles(dirName, "*.png", 5, dirName);
                progressBar1.Value = 0;
                progressBar1.Maximum = allTempsFiles.Count;
                progressBar1.Visible = true;

                string path = "";
                int fix_color = 1;
                if (radioButton_type2.Checked) fix_color = 2;
                if (radioButton_type3.Checked) fix_color = 3;
                bool fix_size = radioButton_type1.Checked;

                foreach (string file in allTempsFiles)
                {
                    progressBar1.Value++;
                    string fullFileName = dirName + file;
                    if (File.Exists(fullFileName))
                    {
                        path = ImageAutoDetectBestFormat(fullFileName, fix_size, fix_color, false);
                    }
                }

                progressBar1.Visible = false;
                if (path != null && path.Length > 5 && Directory.Exists(dirName))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", dirName));
                }
            }
        }

        /// <summary>Получаем список файлов в папке</summary>
        /// <param name="start_path">Начальная папка для просмотра</param>
        /// <param name="mask">Маска для поиска файлов</param>
        /// <param name="depth">Глубина просмотра подкаталогов</param>
        /// <param name="relative_path">Начальная папка? относительно которой будут возвращатся пути файлов</param>
        private List<string> GetRecursFiles(string start_path, string mask, int depth, string relative_path)
        {
            List<string> listFiles = new List<string>();
            if (depth < 0) return listFiles;
            depth--;
            try
            {
                string[] folders = Directory.GetDirectories(start_path);
                foreach (string folder in folders)
                {
                    //ls.Add("Папка: " + folder);
                    listFiles.AddRange(GetRecursFiles(folder, mask, depth, relative_path));
                }
                string[] files = Directory.GetFiles(start_path, mask);
                foreach (string fileName in files)
                {
                    if (relative_path.Length > 3) listFiles.Add(fileName.Replace(relative_path, ""));
                    else listFiles.Add(fileName);
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return listFiles;
        }
    }
}
