using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace TextureGeneration
{
    public static class TextureGeneration
    {
        public static void GetBuildingTexture(string path)
        {
            const int IMAGE_SIZE = 512;
            const int NUM_WINDOWS = 4096;
            const int WHITE_BLACK_RATE = 6;
            Bitmap bm = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);

            // Subdivide bitmap into windows
            for (int i = 0; i < IMAGE_SIZE; ++i) {
                for (int j = 0; j < IMAGE_SIZE; ++j) {
                    if (i % 8 == 0 || i % 8 == 7 || j % 8 == 0 || j % 8 == 7) {
                        bm.SetPixel(i, j, Color.Black);
                    } else {
                        bm.SetPixel(i, j, Color.White);
                    }
                }
            }

            // Modify each window

            Random rnd = new Random();
            for (int i = 0; i < NUM_WINDOWS; ++i) {
                int row = i / 64;
                int col = i % 64;
                double rand = rnd.NextDouble();
                rand = Math.Pow(rand, WHITE_BLACK_RATE);
                int color = (int) (rand * 255);
                if (color < 32 && rnd.NextDouble() < .3) {
                    color += 20;
                }
                for (int j = 1; j < 7; ++j) {
                    for (int k = 1; k < 7; ++k) {
                        bm.SetPixel(row * 8 + j, col * 8 + k, Color.FromArgb(color, color, color));
                    }
                }

                // Add random color noise to bottom half of image if window is not too dark
                if (color > 32) {
                    for (int j = 1; j < 7; ++j) {
                        for (int k = 5; k < 7; ++k) {
                            bm.SetPixel(row * 8 + j, col * 8 + k, Color.FromArgb(rnd.Next(255) / 2, rnd.Next(255) / 2, rnd.Next(255) / 2));
                        }
                    }
                }

            }

            bm.Save(path);
        }
    }
}
