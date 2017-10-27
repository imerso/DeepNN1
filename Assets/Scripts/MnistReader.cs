//
// Written by Vander Roberto Nunes Dias, a.k.a. "imerso" / imersiva.com
//
// Reads MNIST set of labels and images
// each entry in the labels file tells which digit the next image is (0..9)
// each entry in the images file is one grayscale digit image
//

using System.Collections;
using System.Collections.Generic;
using System.IO;

// label format
// [offset] [type]          [value]          [description] 
// 0000     32 bit integer  0x00000801(2049) magic number (MSB first) 
// 0004     32 bit integer  60000            number of items 
// 0008     unsigned byte   ??               label 
// 0009     unsigned byte   ??               label 
// ........ 
// xxxx     unsigned byte   ??               label
// The labels values are 0 to 9.

// image format
// [offset] [type]          [value]          [description] 
// 0000     32 bit integer  0x00000803(2051) magic number 
// 0004     32 bit integer  60000            number of images 
// 0008     32 bit integer  28               number of rows 
// 0012     32 bit integer  28               number of columns 
// 0016     unsigned byte   ??               pixel 
// 0017     unsigned byte   ??               pixel 
// ........ 
// 	xxxx     unsigned byte   ??               pixel
// Pixels are organized row-wise. Pixel values are 0 to 255. 0 means background (white), 255 means foreground (black).

public class MnistReader
{
	FileStream labelsFs;
	FileStream imagesFs;
	int labelsCount;
	int imagesCount, imagesWidth, imagesHeight;
	int current;
	byte[] image;
	byte[] cb = new byte[4];

	public int Current { get { return current; } }
	public int ImageCount { get { return imagesCount; } }
	public int ImageLength { get { return imagesWidth * imagesHeight; } }
	public int ImageWidth { get { return imagesWidth; } }
	public int ImageHeight { get { return imagesHeight; } }

	// simple constructor
	public MnistReader()
	{
	}

	// open a set with a sequence of labels and images
	public bool OpenSet(string labelsFileName, string imagesFileName)
	{
		current = imagesCount = imagesWidth = imagesHeight = 0;

		if (!File.Exists(labelsFileName) || !File.Exists(imagesFileName))
		{
			return false;
		}

		// open the labels file
		labelsFs = File.Open(labelsFileName, FileMode.Open);

		// check if there is at least a complete header in the file (8 bytes)
		if (labelsFs.Length < 8)
		{
			CloseSet();
			return false;
		}

		// read the header to determine how many labels
		int labelsMagic = ReadInt(labelsFs);

		// check the magic signature
		if (labelsMagic != 0x00000801)
		{
			CloseSet();
			return false;
		}

		// read the labels count
		labelsCount = ReadInt(labelsFs);

		// ---

		// open the images file
		imagesFs = File.Open(imagesFileName, FileMode.Open);

		// check if there is at least a complete header in the file (16 bytes)
		if (imagesFs.Length < 16)
		{
			CloseSet();
			return false;
		}

		// read the header to determine how many images,
		// and what resolution
		int imagesMagic = ReadInt(imagesFs);

		// check the magic signature
		if (imagesMagic != 0x00000803)
		{
			CloseSet();
			return false;
		}

		// read image count
		imagesCount = ReadInt(imagesFs);

		// must match the number of labels count
		if (imagesCount != labelsCount)
		{
			CloseSet();
			return false;
		}

		// read image size
		imagesWidth = ReadInt(imagesFs);
		imagesHeight = ReadInt(imagesFs);

		// allocate space for reading next image
		image = new byte[imagesWidth * imagesHeight];

		return true;
	}

	// read next label (will return 0..9 or 255 if end of sequence reached)
	public byte ReadNextLabel()
	{
		if (imagesFs == null || current == imagesCount) return 255;
		return (byte)labelsFs.ReadByte();
	}

	// read next image (will return a byte array with the grayscale pixels in the 0..255 range, or null if end of sequence reached)
	public byte[] ReadNextImage()
	{
		if (imagesFs == null || current == imagesCount) return null;

		imagesFs.Read(image, 0, imagesWidth * imagesHeight);
		current++;

		return image;
	}

	// restart files reading
	public void Restart()
	{
		labelsFs.Seek(8, SeekOrigin.Begin);
		imagesFs.Seek(16, SeekOrigin.Begin);
		current = 0;
	}

	// close the set of labels/images
	public void CloseSet()
	{
		if (labelsFs != null)
		{
			labelsFs.Close();
			labelsFs = null;
		}

		if (imagesFs != null)
		{
			imagesFs.Close();
			imagesFs = null;
		}
	}

	// read 32 bits integer from stream in MSB first (high endian, non-intel format)
	int ReadInt(FileStream fs)
	{
		// read the individual bytes
		for (int i = 0; i < 4; i++) cb[i] = (byte)fs.ReadByte();

		// mount the integer
		int value = 0;
		for (int i = 0; i < 4; i++) value |= (cb[3-i]<<(i*8));

		// return it
		return value;
	}
}
